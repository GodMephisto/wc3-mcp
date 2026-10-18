---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/printer.rs"
ref: HEAD
---

use crate::ast::*;

/// A generic labelled tree used as an intermediate representation between
/// the AST and its rendered forms (indented text, Graphviz DOT, the HTML
/// viewer, ...). `kind` is a coarse category used for coloring in the HTML
/// viewer; `span` (when known) lets that viewer highlight the matching
/// source range on click. Statements only track a starting line (see
/// [`Span::whole_line`]), while expressions carry a precise column range.
#[derive(Debug, Clone, PartialEq)]
pub struct Node {
    pub label: String,
    pub kind: &'static str,
    pub span: Option<Span>,
    pub children: Vec<Node>,
}

impl Node {
    fn leaf(label: impl Into<String>) -> Self {
        Node {
            label: label.into(),
            kind: "node",
            span: None,
            children: Vec::new(),
        }
    }

    fn with_children(label: impl Into<String>, children: Vec<Node>) -> Self {
        Node {
            label: label.into(),
            kind: "node",
            span: None,
            children,
        }
    }

    fn tag(mut self, kind: &'static str, span: Option<Span>) -> Self {
        self.kind = kind;
        self.span = span;
        self
    }
}

/// Builds a visualizable tree out of a parsed program.
pub fn build_tree(program: &Program) -> Node {
    Node::with_children(
        "Program",
        program.items.iter().map(top_level_node).collect(),
    )
    .tag("root", None)
}

/// Renders the program's AST as an indented tree, e.g.:
///
/// ```text
/// Program
/// └─ Function Add(integer a, integer b) -> integer (line 2)
///    ├─ Local integer sum (line 3)
///    │  └─ Binary Add
///    │     ├─ Var a
///    │     └─ Var b
///    └─ Return (line 4)
///       └─ Var sum
/// ```
pub fn render_tree(program: &Program) -> String {
    let root = build_tree(program);
    let mut out = String::new();
    out.push_str(&root.label);
    out.push('\n');
    render_children(&root.children, "", &mut out);
    out
}

fn render_children(children: &[Node], prefix: &str, out: &mut String) {
    for (i, child) in children.iter().enumerate() {
        let is_last = i == children.len() - 1;
        let connector = if is_last { "└─ " } else { "├─ " };
        out.push_str(prefix);
        out.push_str(connector);
        out.push_str(&child.label);
        out.push('\n');
        let child_prefix = format!("{prefix}{}", if is_last { "   " } else { "│  " });
        render_children(&child.children, &child_prefix, out);
    }
}

/// Renders the program's AST as a Graphviz DOT graph. Feed the output to
/// `dot -Tpng` (or an online viewer) to get an actual picture of the tree.
pub fn render_dot(program: &Program) -> String {
    let root = build_tree(program);
    let mut out = String::new();
    out.push_str("digraph AST {\n");
    out.push_str("    node [shape=box, fontname=\"monospace\"];\n");
    let mut counter = 0usize;
    write_dot_node(&root, &mut counter, None, &mut out);
    out.push_str("}\n");
    out
}

fn write_dot_node(node: &Node, counter: &mut usize, parent: Option<usize>, out: &mut String) {
    let id = *counter;
    *counter += 1;
    let escaped = node.label.replace('\\', "\\\\").replace('"', "\\\"");
    out.push_str(&format!("    n{id} [label=\"{escaped}\"];\n"));
    if let Some(p) = parent {
        out.push_str(&format!("    n{p} -> n{id};\n"));
    }
    for child in &node.children {
        write_dot_node(child, counter, Some(id), out);
    }
}

fn top_level_node(item: &TopLevel) -> Node {
    match item {
        TopLevel::Globals(decls) => {
            Node::with_children("Globals", decls.iter().map(global_decl_node).collect())
                .tag("container", None)
        }
        TopLevel::TypeDef {
            name,
            extends,
            line,
        } => Node::leaf(format!("Type {name} extends {extends} (line {line})"))
            .tag("decl", Some(Span::whole_line(*line))),
        TopLevel::Native(sig) => Node::leaf(format!("Native {}", sig_label(sig)))
            .tag("sig", Some(Span::whole_line(sig.line))),
        TopLevel::Function(f) => Node::with_children(
            format!("Function {}", sig_label(&f.sig)),
            f.body.iter().map(stmt_node).collect(),
        )
        .tag("sig", Some(Span::whole_line(f.sig.line))),
    }
}

fn sig_label(sig: &FunctionSig) -> String {
    let params = sig
        .params
        .iter()
        .map(|p| format!("{} {}", p.type_name, p.name))
        .collect::<Vec<_>>()
        .join(", ");
    let params = if params.is_empty() {
        "nothing".to_string()
    } else {
        params
    };
    let returns = sig.returns.clone().unwrap_or_else(|| "nothing".to_string());
    let constant = if sig.is_constant { "constant " } else { "" };
    format!(
        "{constant}{}({params}) -> {returns} (line {})",
        sig.name, sig.line
    )
}

fn global_decl_node(d: &GlobalDecl) -> Node {
    let label = format!(
        "{}{}{} {} (line {})",
        if d.is_constant { "constant " } else { "" },
        d.type_name,
        if d.is_array { " array" } else { "" },
        d.name,
        d.line
    );
    let children = d.initializer.iter().map(expr_node).collect();
    Node::with_children(label, children).tag("decl", Some(Span::whole_line(d.line)))
}

fn stmt_node(s: &Stmt) -> Node {
    match s {
        Stmt::Local {
            type_name,
            is_array,
            name,
            initializer,
            line,
        } => {
            let label = format!(
                "Local {type_name}{} {name} (line {line})",
                if *is_array { " array" } else { "" }
            );
            Node::with_children(label, initializer.iter().map(expr_node).collect())
                .tag("stmt", Some(Span::whole_line(*line)))
        }
        Stmt::Set {
            name,
            index,
            value,
            line,
        } => {
            let mut children = Vec::new();
            if let Some(idx) = index {
                children.push(
                    Node::with_children("Index", vec![expr_node(idx)])
                        .tag("container", Some(Span::whole_line(*line))),
                );
            }
            children.push(
                Node::with_children("Value", vec![expr_node(value)])
                    .tag("container", Some(Span::whole_line(*line))),
            );
            Node::with_children(format!("Set {name} (line {line})"), children)
                .tag("stmt", Some(Span::whole_line(*line)))
        }
        Stmt::Call { name, args, line } => Node::with_children(
            format!("Call {name} (line {line})"),
            args.iter().map(expr_node).collect(),
        )
        .tag("stmt", Some(Span::whole_line(*line))),
        Stmt::If {
            branches,
            else_branch,
            line,
        } => {
            let mut children = Vec::new();
            for (i, (cond, body)) in branches.iter().enumerate() {
                let keyword = if i == 0 { "If" } else { "ElseIf" };
                children.push(
                    Node::with_children(format!("{keyword} condition"), vec![expr_node(cond)])
                        .tag("container", Some(Span::whole_line(*line))),
                );
                children.push(
                    Node::with_children("Then", body.iter().map(stmt_node).collect())
                        .tag("container", Some(Span::whole_line(*line))),
                );
            }
            if let Some(body) = else_branch {
                children.push(
                    Node::with_children("Else", body.iter().map(stmt_node).collect())
                        .tag("container", Some(Span::whole_line(*line))),
                );
            }
            Node::with_children(format!("If (line {line})"), children)
                .tag("stmt", Some(Span::whole_line(*line)))
        }
        Stmt::Loop { body, line } => Node::with_children(
            format!("Loop (line {line})"),
            body.iter().map(stmt_node).collect(),
        )
        .tag("stmt", Some(Span::whole_line(*line))),
        Stmt::ExitWhen { cond, line } => {
            Node::with_children(format!("ExitWhen (line {line})"), vec![expr_node(cond)])
                .tag("stmt", Some(Span::whole_line(*line)))
        }
        Stmt::Return { value, line } => Node::with_children(
            format!("Return (line {line})"),
            value.iter().map(expr_node).collect(),
        )
        .tag("stmt", Some(Span::whole_line(*line))),
    }
}

fn expr_node(e: &Expr) -> Node {
    let span = Some(e.span);
    match &e.kind {
        ExprKind::IntLiteral(v) => Node::leaf(format!("Int {v}")).tag("lit", span),
        ExprKind::RealLiteral(v) => Node::leaf(format!("Real {v}")).tag("lit", span),
        ExprKind::StringLiteral(v) => Node::leaf(format!("String {v:?}")).tag("lit", span),
        ExprKind::BoolLiteral(v) => Node::leaf(format!("Bool {v}")).tag("lit", span),
        ExprKind::Null => Node::leaf("Null").tag("lit", span),
        ExprKind::Var(name) => Node::leaf(format!("Var {name}")).tag("expr", span),
        ExprKind::ArrayAccess(name, idx) => {
            Node::with_children(format!("ArrayAccess {name}"), vec![expr_node(idx)])
                .tag("expr", span)
        }
        ExprKind::Call(name, args) => {
            Node::with_children(format!("Call {name}"), args.iter().map(expr_node).collect())
                .tag("expr", span)
        }
        ExprKind::FuncRef(name) => Node::leaf(format!("FuncRef {name}")).tag("expr", span),
        ExprKind::Unary(op, inner) => {
            Node::with_children(format!("Unary {op:?}"), vec![expr_node(inner)]).tag("expr", span)
        }
        ExprKind::Binary(l, op, r) => {
            Node::with_children(format!("Binary {op:?}"), vec![expr_node(l), expr_node(r)])
                .tag("expr", span)
        }
    }
}

#[cfg(test)]
#[path = "printer.test.rs"]
mod tests;
