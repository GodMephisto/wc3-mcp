---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/html.rs"
ref: HEAD
---

use crate::ast::Program;
use crate::printer::{build_tree, Node};
use crate::token;

/// Renders a self-contained HTML page that visualizes a parsed program's
/// AST: a source panel with syntax highlighting on the left, a collapsible,
/// color-coded tree on the right. Clicking (or selecting) a token in the
/// source highlights the smallest AST node covering it, and vice versa —
/// both directions are wired through exact (line, column) spans, not just
/// whole-line matching. Everything (CSS, JS, data) is inlined, so the file
/// works offline from `file://` — no server, no browser plugin, no IDE
/// integration, no editor library (Monaco/CodeMirror) required. Open it by
/// double-clicking, or point a browser at it directly.
pub fn render_html(program: &Program, source: &str, title: &str) -> String {
    let tree = build_tree(program);
    let tree_json = node_to_json(&tree);
    let source_html = highlight_source(source);

    TEMPLATE
        .replace("__TITLE__", &html_escape(title))
        .replace("__SOURCE_HTML__", &source_html)
        .replace("__TREE_JSON__", &tree_json)
}

fn node_to_json(node: &Node) -> String {
    let mut s = String::new();
    write_node_json(node, &mut s);
    s
}

fn write_node_json(node: &Node, out: &mut String) {
    out.push('{');
    out.push_str("\"label\":\"");
    out.push_str(&json_escape(&node.label));
    out.push_str("\",\"kind\":\"");
    out.push_str(node.kind);
    out.push('"');
    match node.span {
        Some(span) => {
            out.push_str(&format!(",\"line\":{}", span.start_line));
            out.push_str(&format!(",\"eline\":{}", span.end_line));
            if span.start_col == 0 {
                // A "whole line" sentinel (see `Span::whole_line`): statements
                // only track their starting line, not columns.
                out.push_str(",\"scol\":null,\"ecol\":null");
            } else {
                out.push_str(&format!(",\"scol\":{}", span.start_col));
                out.push_str(&format!(",\"ecol\":{}", span.end_col));
            }
        }
        None => out.push_str(",\"line\":null,\"eline\":null,\"scol\":null,\"ecol\":null"),
    }
    out.push_str(",\"children\":[");
    for (i, child) in node.children.iter().enumerate() {
        if i > 0 {
            out.push(',');
        }
        write_node_json(child, out);
    }
    out.push_str("]}");
}

/// JSON-escapes a string, additionally escaping `/` as `\/` so that a
/// JASS string literal containing `</script>` can't prematurely close the
/// `<script>` tag this JSON is embedded in.
fn json_escape(s: &str) -> String {
    let mut out = String::with_capacity(s.len() + 2);
    for c in s.chars() {
        match c {
            '"' => out.push_str("\\\""),
            '\\' => out.push_str("\\\\"),
            '/' => out.push_str("\\/"),
            '\n' => out.push_str("\\n"),
            '\r' => out.push_str("\\r"),
            '\t' => out.push_str("\\t"),
            c if (c as u32) < 0x20 => out.push_str(&format!("\\u{:04x}", c as u32)),
            c => out.push(c),
        }
    }
    out
}

fn html_escape(s: &str) -> String {
    s.replace('&', "&amp;")
        .replace('<', "&lt;")
        .replace('>', "&gt;")
}

fn highlight_source(src: &str) -> String {
    let mut out = String::new();
    for (i, line) in src.lines().enumerate() {
        let n = i + 1;
        out.push_str(&format!("<div class=\"line\" id=\"L{n}\">"));
        out.push_str(&format!("<span class=\"ln\">{n}</span>"));
        out.push_str("<span class=\"code\">");
        out.push_str(&highlight_line(line));
        out.push_str("</span></div>\n");
    }
    out
}

/// Tokenizes a single source line for display, wrapping every non-space
/// lexeme (identifiers, keywords, literals, comments, and individual
/// punctuation/operator characters) in a `<span class="tok ...">` tagged
/// with `data-scol`/`data-ecol`. Column bookkeeping here deliberately
/// mirrors [`crate::lexer::Lexer`]'s (1-indexed, one column per character)
/// so that these spans line up exactly with the [`crate::ast::Span`]s the
/// parser records — that alignment is what lets the viewer's JS map a
/// clicked source token straight to the AST node whose span contains it,
/// and back again, without needing a real text-editor component.
fn highlight_line(line: &str) -> String {
    let chars: Vec<char> = line.chars().collect();
    let mut i = 0usize;
    let mut col = 1usize;
    let mut out = String::new();

    while i < chars.len() {
        let c = chars[i];

        if c == '/' && chars.get(i + 1) == Some(&'/') {
            let text: String = chars[i..].iter().collect();
            let start_col = col;
            let end_col = col + (chars.len() - i);
            emit_tok(&mut out, "tok-com", start_col, end_col, &text);
            break;
        }

        if c == '"' {
            let start_i = i;
            let start_col = col;
            i += 1;
            col += 1;
            while i < chars.len() && chars[i] != '"' {
                if chars[i] == '\\' && i + 1 < chars.len() {
                    i += 1;
                    col += 1;
                }
                i += 1;
                col += 1;
            }
            if i < chars.len() {
                i += 1;
                col += 1;
            }
            let text: String = chars[start_i..i].iter().collect();
            emit_tok(&mut out, "tok-str", start_col, col, &text);
            continue;
        }

        if c == '\'' {
            let start_i = i;
            let start_col = col;
            i += 1;
            col += 1;
            while i < chars.len() && chars[i] != '\'' {
                i += 1;
                col += 1;
            }
            if i < chars.len() {
                i += 1;
                col += 1;
            }
            let text: String = chars[start_i..i].iter().collect();
            emit_tok(&mut out, "tok-str", start_col, col, &text);
            continue;
        }

        if c.is_ascii_digit() {
            let start_i = i;
            let start_col = col;
            while i < chars.len() && (chars[i].is_ascii_alphanumeric() || chars[i] == '.') {
                i += 1;
                col += 1;
            }
            let text: String = chars[start_i..i].iter().collect();
            emit_tok(&mut out, "tok-num", start_col, col, &text);
            continue;
        }

        if c == '_' || c.is_alphabetic() {
            let start_i = i;
            let start_col = col;
            while i < chars.len() && (chars[i] == '_' || chars[i].is_alphanumeric()) {
                i += 1;
                col += 1;
            }
            let text: String = chars[start_i..i].iter().collect();
            let class = if token::keyword(&text).is_some() {
                "tok-kw"
            } else {
                "tok-ident"
            };
            emit_tok(&mut out, class, start_col, col, &text);
            continue;
        }

        if c.is_whitespace() {
            out.push(c);
            i += 1;
            col += 1;
            continue;
        }

        let start_col = col;
        emit_tok(
            &mut out,
            "tok-punct",
            start_col,
            start_col + 1,
            &c.to_string(),
        );
        i += 1;
        col += 1;
    }

    out
}

fn emit_tok(out: &mut String, class: &str, start_col: usize, end_col: usize, text: &str) {
    out.push_str(&format!(
        "<span class=\"tok {class}\" data-scol=\"{start_col}\" data-ecol=\"{end_col}\">"
    ));
    out.push_str(&html_escape(text));
    out.push_str("</span>");
}

const TEMPLATE: &str = include_str!("html_template.html");

#[cfg(test)]
#[path = "html.test.rs"]
mod tests;
