---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/html.test.rs"
ref: HEAD
---

use super::*;
use crate::parser::Parser;

#[test]
fn embeds_title_and_tree_data() {
    let program =
        Parser::parse_str("function Foo takes nothing returns nothing\nendfunction").unwrap();
    let html = render_html(
        &program,
        "function Foo takes nothing returns nothing\nendfunction",
        "demo.j",
    );
    assert!(html.contains("demo.j"));
    assert!(html.contains("\"label\":\"Program\""));
    assert!(html.contains("id=\"ast-data\""));
    assert!(html.contains("id=\"L1\""));
    assert!(html.contains("tok-kw"));
}

#[test]
fn escapes_closing_script_tags_in_string_literals() {
    let src = "function Foo takes nothing returns nothing\n    call P(\"</script>\")\nendfunction";
    let program = Parser::parse_str(src).unwrap();
    let html = render_html(&program, src, "demo.j");
    assert!(!html.contains("</script>\")"));
    assert!(html.contains("<\\/script>"));
}

#[test]
fn highlights_keywords_strings_numbers_and_comments() {
    let line = highlight_line("local integer x = 1 // hi \"there\"");
    assert!(line.contains(r#"<span class="tok tok-kw" data-scol="1" data-ecol="6">local</span>"#));
    assert!(line.contains(r#"<span class="tok tok-num" data-scol="19" data-ecol="20">1</span>"#));
    assert!(line.contains(
        r#"<span class="tok tok-com" data-scol="21" data-ecol="34">// hi "there"</span>"#
    ));
}

#[test]
fn tags_identifiers_and_punctuation_with_column_positions() {
    let line = highlight_line("set x = 1");
    assert!(line.contains(r#"<span class="tok tok-kw" data-scol="1" data-ecol="4">set</span>"#));
    assert!(line.contains(r#"<span class="tok tok-ident" data-scol="5" data-ecol="6">x</span>"#));
    assert!(line.contains(r#"<span class="tok tok-punct" data-scol="7" data-ecol="8">=</span>"#));
}

#[test]
fn expression_node_spans_align_with_source_token_columns() {
    let src =
        "function Add takes integer a, integer b returns integer\n    return a + b\nendfunction";
    let program = Parser::parse_str(src).unwrap();
    let html = render_html(&program, src, "demo.j");
    // The `a` in `return a + b` is a Var node with span (2,12,2,13); the
    // highlighted source token for that same identifier must carry the
    // exact same columns, or clicking either side wouldn't find a match.
    assert!(html.contains(
        "\"label\":\"Var a\",\"kind\":\"expr\",\"line\":2,\"eline\":2,\"scol\":12,\"ecol\":13"
    ));
    assert!(html.contains(r#"<span class="tok tok-ident" data-scol="12" data-ecol="13">a</span>"#));
}
