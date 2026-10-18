---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/query.rs"
ref: HEAD
---

//! Position-based lookup over a parsed [`Program`] — "which AST node is at
//! (or covers) this location" — the reverse-search operation any consumer
//! of this crate (a CLI, an editor integration, ...) needs regularly, e.g.
//! to answer "what's under the cursor" or "what does this selection cover".
//!
//! Coordinates here are always in [`Span`]'s native representation (line,
//! codepoint-counted column). A consumer working in a different
//! [`ColumnEncoding`] converts its query column with
//! [`ColumnEncoding::to_codepoints`] before calling in, and the returned
//! node's [`AstRef::span`] with [`Span::in_encoding`] before using it.

use crate::ast::*;

/// A reference to whichever kind of AST node a query matched. Borrows from
/// the [`Program`] that was searched, so it can't outlive it.
#[derive(Debug, Clone, Copy)]
pub enum AstRef<'a> {
    TopLevel(&'a TopLevel),
    GlobalDecl(&'a GlobalDecl),
    Stmt(&'a Stmt),
    Expr(&'a Expr),
}

impl<'a> AstRef<'a> {
    /// The node's location. Statements, declarations, and top-level items
    /// only carry a starting line (see [`Span::whole_line`]); expressions
    /// carry a precise column range.
    pub fn span(&self) -> Span {
        match self {
            AstRef::TopLevel(t) => Span::whole_line(top_level_line(t)),
            AstRef::GlobalDecl(d) => Span::whole_line(d.line),
            AstRef::Stmt(s) => Span::whole_line(stmt_line(s)),
            AstRef::Expr(e) => e.span,
        }
    }
}

fn top_level_line(item: &TopLevel) -> usize {
    match item {
        // Never itself returned by a search (only its children are), so
        // this placeholder is never actually observed.
        TopLevel::Globals(_) => 0,
        TopLevel::TypeDef { line, .. } => *line,
        TopLevel::Native(sig) => sig.line,
        TopLevel::Function(f) => f.sig.line,
    }
}

fn stmt_line(s: &Stmt) -> usize {
    match s {
        Stmt::Local { line, .. }
        | Stmt::Set { line, .. }
        | Stmt::Call { line, .. }
        | Stmt::If { line, .. }
        | Stmt::Loop { line, .. }
        | Stmt::ExitWhen { line, .. }
        | Stmt::Return { line, .. } => *line,
    }
}

/// Finds the most specific (deepest) AST node whose span contains the
/// single-line range `[start_col, end_col)` on `line` — a point query if
/// `start_col == end_col`. Columns are codepoint-counted (see the module
/// docs for working in another [`ColumnEncoding`]).
pub fn find_node_in(
    program: &Program,
    line: usize,
    start_col: usize,
    end_col: usize,
) -> Option<AstRef<'_>> {
    program
        .items
        .iter()
        .find_map(|item| find_in_top_level(item, line, start_col, end_col))
}

/// Finds the most specific AST node at a single position — shorthand for
/// [`find_node_in`] with a zero-width range.
pub fn find_node_at(program: &Program, line: usize, col: usize) -> Option<AstRef<'_>> {
    find_node_in(program, line, col, col)
}

fn contains(span: Span, line: usize, start_col: usize, end_col: usize) -> bool {
    if span.start_col == 0 {
        // Whole-line sentinel: matches any column query on its line range.
        line >= span.start_line && line <= span.end_line
    } else {
        span.start_line == line
            && span.end_line == line
            && span.start_col <= start_col
            && end_col <= span.end_col
    }
}

fn self_or_none(
    node: AstRef<'_>,
    span: Span,
    line: usize,
    sc: usize,
    ec: usize,
) -> Option<AstRef<'_>> {
    contains(span, line, sc, ec).then_some(node)
}

fn find_in_top_level<'a>(
    item: &'a TopLevel,
    line: usize,
    sc: usize,
    ec: usize,
) -> Option<AstRef<'a>> {
    match item {
        TopLevel::Globals(decls) => decls.iter().find_map(|d| {
            find_in_expr_opt(d.initializer.as_ref(), line, sc, ec).or_else(|| {
                self_or_none(
                    AstRef::GlobalDecl(d),
                    Span::whole_line(d.line),
                    line,
                    sc,
                    ec,
                )
            })
        }),
        TopLevel::TypeDef { line: l, .. } => {
            self_or_none(AstRef::TopLevel(item), Span::whole_line(*l), line, sc, ec)
        }
        TopLevel::Native(sig) => self_or_none(
            AstRef::TopLevel(item),
            Span::whole_line(sig.line),
            line,
            sc,
            ec,
        ),
        TopLevel::Function(f) => f
            .body
            .iter()
            .find_map(|s| find_in_stmt(s, line, sc, ec))
            .or_else(|| {
                self_or_none(
                    AstRef::TopLevel(item),
                    Span::whole_line(f.sig.line),
                    line,
                    sc,
                    ec,
                )
            }),
    }
}

fn find_in_stmt<'a>(s: &'a Stmt, line: usize, sc: usize, ec: usize) -> Option<AstRef<'a>> {
    let own = Span::whole_line(stmt_line(s));
    let child_hit = match s {
        Stmt::Local { initializer, .. } => find_in_expr_opt(initializer.as_ref(), line, sc, ec),
        Stmt::Set { index, value, .. } => find_in_expr_opt(index.as_ref(), line, sc, ec)
            .or_else(|| find_in_expr(value, line, sc, ec)),
        Stmt::Call { args, .. } => args.iter().find_map(|a| find_in_expr(a, line, sc, ec)),
        Stmt::If {
            branches,
            else_branch,
            ..
        } => branches
            .iter()
            .find_map(|(cond, body)| {
                find_in_expr(cond, line, sc, ec)
                    .or_else(|| body.iter().find_map(|st| find_in_stmt(st, line, sc, ec)))
            })
            .or_else(|| {
                else_branch
                    .as_ref()
                    .and_then(|body| body.iter().find_map(|st| find_in_stmt(st, line, sc, ec)))
            }),
        Stmt::Loop { body, .. } => body.iter().find_map(|st| find_in_stmt(st, line, sc, ec)),
        Stmt::ExitWhen { cond, .. } => find_in_expr(cond, line, sc, ec),
        Stmt::Return { value, .. } => find_in_expr_opt(value.as_ref(), line, sc, ec),
    };
    child_hit.or_else(|| self_or_none(AstRef::Stmt(s), own, line, sc, ec))
}

fn find_in_expr<'a>(e: &'a Expr, line: usize, sc: usize, ec: usize) -> Option<AstRef<'a>> {
    let child_hit = match &e.kind {
        ExprKind::ArrayAccess(_, idx) => find_in_expr(idx, line, sc, ec),
        ExprKind::Call(_, args) => args.iter().find_map(|a| find_in_expr(a, line, sc, ec)),
        ExprKind::Unary(_, inner) => find_in_expr(inner, line, sc, ec),
        ExprKind::Binary(l, _, r) => {
            find_in_expr(l, line, sc, ec).or_else(|| find_in_expr(r, line, sc, ec))
        }
        ExprKind::IntLiteral(_)
        | ExprKind::RealLiteral(_)
        | ExprKind::StringLiteral(_)
        | ExprKind::BoolLiteral(_)
        | ExprKind::Null
        | ExprKind::Var(_)
        | ExprKind::FuncRef(_) => None,
    };
    child_hit.or_else(|| self_or_none(AstRef::Expr(e), e.span, line, sc, ec))
}

fn find_in_expr_opt<'a>(
    e: Option<&'a Expr>,
    line: usize,
    sc: usize,
    ec: usize,
) -> Option<AstRef<'a>> {
    e.and_then(|e| find_in_expr(e, line, sc, ec))
}

#[cfg(test)]
#[path = "query.test.rs"]
mod tests;
