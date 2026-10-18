---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/parser.rs"
ref: HEAD
---

use crate::ast::*;
use crate::lexer::Lexer;
use crate::token::{Token, TokenKind};

#[derive(Debug, Clone, PartialEq)]
pub struct ParseError {
    pub message: String,
    pub line: usize,
    pub col: usize,
    /// Exclusive end column of the offending token, for consumers (e.g. an
    /// editor) that want to underline the exact span rather than just a
    /// single point. Lexer-level errors (which don't track a token's own
    /// end) fall back to `col + 1`.
    pub end_col: usize,
}

pub struct Parser {
    tokens: Vec<Token>,
    pos: usize,
}

impl Parser {
    pub fn new(tokens: Vec<Token>) -> Self {
        Parser { tokens, pos: 0 }
    }

    pub fn parse_str(src: &str) -> Result<Program, ParseError> {
        let tokens = Lexer::tokenize(src).map_err(|e| ParseError {
            message: e.message,
            line: e.line,
            col: e.col,
            end_col: e.col + 1,
        })?;
        Parser::new(tokens).program()
    }

    fn peek(&self) -> &Token {
        &self.tokens[self.pos]
    }

    fn peek_kind(&self) -> &TokenKind {
        &self.tokens[self.pos].kind
    }

    fn advance(&mut self) -> Token {
        let t = self.tokens[self.pos].clone();
        if self.pos + 1 < self.tokens.len() {
            self.pos += 1;
        }
        t
    }

    fn check(&self, kind: &TokenKind) -> bool {
        self.peek_kind() == kind
    }

    fn error(&self, message: impl Into<String>) -> ParseError {
        let t = self.peek();
        ParseError {
            message: message.into(),
            line: t.line,
            col: t.col,
            end_col: t.end_col,
        }
    }

    fn expect(&mut self, kind: TokenKind) -> Result<Token, ParseError> {
        if self.peek_kind() == &kind {
            Ok(self.advance())
        } else {
            Err(self.error(format!("expected {:?}, found {:?}", kind, self.peek_kind())))
        }
    }

    fn expect_identifier(&mut self) -> Result<String, ParseError> {
        match self.peek_kind().clone() {
            TokenKind::Identifier(name) => {
                self.advance();
                Ok(name)
            }
            other => Err(self.error(format!("expected identifier, found {other:?}"))),
        }
    }

    fn expect_type_name(&mut self) -> Result<String, ParseError> {
        self.expect_identifier()
    }

    pub fn program(&mut self) -> Result<Program, ParseError> {
        let mut items = Vec::new();
        while !self.check(&TokenKind::Eof) {
            items.push(self.top_level()?);
        }
        Ok(Program { items })
    }

    fn top_level(&mut self) -> Result<TopLevel, ParseError> {
        match self.peek_kind().clone() {
            TokenKind::Globals => self.globals_block(),
            TokenKind::Type => self.type_def(),
            TokenKind::Native => {
                let line = self.peek().line;
                self.advance();
                let sig = self.function_sig(false, line)?;
                Ok(TopLevel::Native(sig))
            }
            TokenKind::Constant => {
                let line = self.peek().line;
                self.advance();
                self.expect(TokenKind::Native)?;
                let sig = self.function_sig(true, line)?;
                Ok(TopLevel::Native(sig))
            }
            TokenKind::Function => self.function_decl(),
            other => Err(self.error(format!("unexpected top-level token {other:?}"))),
        }
    }

    fn type_def(&mut self) -> Result<TopLevel, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::Type)?;
        let name = self.expect_identifier()?;
        self.expect(TokenKind::Extends)?;
        let extends = self.expect_type_name()?;
        Ok(TopLevel::TypeDef {
            name,
            extends,
            line,
        })
    }

    fn globals_block(&mut self) -> Result<TopLevel, ParseError> {
        self.expect(TokenKind::Globals)?;
        let mut decls = Vec::new();
        while !self.check(&TokenKind::EndGlobals) {
            if self.check(&TokenKind::Eof) {
                return Err(self.error("unexpected end of file inside globals block"));
            }
            decls.push(self.global_decl()?);
        }
        self.expect(TokenKind::EndGlobals)?;
        Ok(TopLevel::Globals(decls))
    }

    fn global_decl(&mut self) -> Result<GlobalDecl, ParseError> {
        let line = self.peek().line;
        let is_constant = if self.check(&TokenKind::Constant) {
            self.advance();
            true
        } else {
            false
        };
        let type_name = self.expect_type_name()?;
        let is_array = if self.check(&TokenKind::Array) {
            self.advance();
            true
        } else {
            false
        };
        let name = self.expect_identifier()?;
        let initializer = if self.check(&TokenKind::Assign) {
            self.advance();
            Some(self.expr()?)
        } else {
            None
        };
        Ok(GlobalDecl {
            is_constant,
            type_name,
            is_array,
            name,
            initializer,
            line,
        })
    }

    fn function_sig(&mut self, is_constant: bool, line: usize) -> Result<FunctionSig, ParseError> {
        let name = self.expect_identifier()?;
        self.expect(TokenKind::Takes)?;
        let params = if self.check(&TokenKind::Nothing) {
            self.advance();
            Vec::new()
        } else {
            let mut params = vec![self.param()?];
            while self.check(&TokenKind::Comma) {
                self.advance();
                params.push(self.param()?);
            }
            params
        };
        self.expect(TokenKind::Returns)?;
        let returns = if self.check(&TokenKind::Nothing) {
            self.advance();
            None
        } else {
            Some(self.expect_type_name()?)
        };
        Ok(FunctionSig {
            name,
            params,
            returns,
            is_constant,
            line,
        })
    }

    fn param(&mut self) -> Result<Param, ParseError> {
        let type_name = self.expect_type_name()?;
        let name = self.expect_identifier()?;
        Ok(Param { type_name, name })
    }

    fn function_decl(&mut self) -> Result<TopLevel, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::Function)?;
        let sig = self.function_sig(false, line)?;
        let body = self.stmt_block(&[TokenKind::EndFunction])?;
        self.expect(TokenKind::EndFunction)?;
        Ok(TopLevel::Function(FunctionDecl { sig, body }))
    }

    fn stmt_block(&mut self, stop: &[TokenKind]) -> Result<Vec<Stmt>, ParseError> {
        let mut stmts = Vec::new();
        while !stop.contains(self.peek_kind()) {
            if self.check(&TokenKind::Eof) {
                return Err(self.error("unexpected end of file"));
            }
            stmts.push(self.stmt()?);
        }
        Ok(stmts)
    }

    fn stmt(&mut self) -> Result<Stmt, ParseError> {
        match self.peek_kind().clone() {
            TokenKind::Local => self.local_stmt(),
            TokenKind::Set => self.set_stmt(),
            TokenKind::Call => self.call_stmt(),
            TokenKind::If => self.if_stmt(),
            TokenKind::Loop => self.loop_stmt(),
            TokenKind::ExitWhen => self.exitwhen_stmt(),
            TokenKind::Return => self.return_stmt(),
            other => Err(self.error(format!("unexpected token in statement position: {other:?}"))),
        }
    }

    fn local_stmt(&mut self) -> Result<Stmt, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::Local)?;
        let type_name = self.expect_type_name()?;
        let is_array = if self.check(&TokenKind::Array) {
            self.advance();
            true
        } else {
            false
        };
        let name = self.expect_identifier()?;
        let initializer = if self.check(&TokenKind::Assign) {
            self.advance();
            Some(self.expr()?)
        } else {
            None
        };
        Ok(Stmt::Local {
            type_name,
            is_array,
            name,
            initializer,
            line,
        })
    }

    fn set_stmt(&mut self) -> Result<Stmt, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::Set)?;
        let name = self.expect_identifier()?;
        let index = if self.check(&TokenKind::LBracket) {
            self.advance();
            let idx = self.expr()?;
            self.expect(TokenKind::RBracket)?;
            Some(idx)
        } else {
            None
        };
        self.expect(TokenKind::Assign)?;
        let value = self.expr()?;
        Ok(Stmt::Set {
            name,
            index,
            value,
            line,
        })
    }

    fn call_stmt(&mut self) -> Result<Stmt, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::Call)?;
        let name = self.expect_identifier()?;
        self.expect(TokenKind::LParen)?;
        let args = self.expr_list(&TokenKind::RParen)?;
        self.expect(TokenKind::RParen)?;
        Ok(Stmt::Call { name, args, line })
    }

    fn if_stmt(&mut self) -> Result<Stmt, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::If)?;
        let mut branches = Vec::new();

        let cond = self.expr()?;
        self.expect(TokenKind::Then)?;
        let body = self.stmt_block(&[TokenKind::ElseIf, TokenKind::Else, TokenKind::EndIf])?;
        branches.push((cond, body));

        let mut else_branch = None;
        loop {
            match self.peek_kind() {
                TokenKind::ElseIf => {
                    self.advance();
                    let cond = self.expr()?;
                    self.expect(TokenKind::Then)?;
                    let body =
                        self.stmt_block(&[TokenKind::ElseIf, TokenKind::Else, TokenKind::EndIf])?;
                    branches.push((cond, body));
                }
                TokenKind::Else => {
                    self.advance();
                    let body = self.stmt_block(&[TokenKind::EndIf])?;
                    else_branch = Some(body);
                    break;
                }
                TokenKind::EndIf => break,
                _ => return Err(self.error("expected 'elseif', 'else', or 'endif'")),
            }
        }
        self.expect(TokenKind::EndIf)?;
        Ok(Stmt::If {
            branches,
            else_branch,
            line,
        })
    }

    fn loop_stmt(&mut self) -> Result<Stmt, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::Loop)?;
        let body = self.stmt_block(&[TokenKind::EndLoop])?;
        self.expect(TokenKind::EndLoop)?;
        Ok(Stmt::Loop { body, line })
    }

    fn exitwhen_stmt(&mut self) -> Result<Stmt, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::ExitWhen)?;
        let cond = self.expr()?;
        Ok(Stmt::ExitWhen { cond, line })
    }

    fn return_stmt(&mut self) -> Result<Stmt, ParseError> {
        let line = self.peek().line;
        self.expect(TokenKind::Return)?;
        let value = if self.starts_expr() {
            Some(self.expr()?)
        } else {
            None
        };
        Ok(Stmt::Return { value, line })
    }

    fn starts_expr(&self) -> bool {
        matches!(
            self.peek_kind(),
            TokenKind::Identifier(_)
                | TokenKind::IntLiteral(_)
                | TokenKind::RealLiteral(_)
                | TokenKind::StringLiteral(_)
                | TokenKind::True
                | TokenKind::False
                | TokenKind::Null
                | TokenKind::LParen
                | TokenKind::Minus
                | TokenKind::Not
                | TokenKind::Function
        )
    }

    fn expr_list(&mut self, end: &TokenKind) -> Result<Vec<Expr>, ParseError> {
        let mut args = Vec::new();
        if self.peek_kind() == end {
            return Ok(args);
        }
        args.push(self.expr()?);
        while self.check(&TokenKind::Comma) {
            self.advance();
            args.push(self.expr()?);
        }
        Ok(args)
    }

    pub fn expr(&mut self) -> Result<Expr, ParseError> {
        self.or_expr()
    }

    fn or_expr(&mut self) -> Result<Expr, ParseError> {
        let mut left = self.and_expr()?;
        while self.check(&TokenKind::Or) {
            self.advance();
            let right = self.and_expr()?;
            let span = combine_spans(left.span, right.span);
            left = Expr::new(
                ExprKind::Binary(Box::new(left), BinOp::Or, Box::new(right)),
                span,
            );
        }
        Ok(left)
    }

    fn and_expr(&mut self) -> Result<Expr, ParseError> {
        let mut left = self.equality_expr()?;
        while self.check(&TokenKind::And) {
            self.advance();
            let right = self.equality_expr()?;
            let span = combine_spans(left.span, right.span);
            left = Expr::new(
                ExprKind::Binary(Box::new(left), BinOp::And, Box::new(right)),
                span,
            );
        }
        Ok(left)
    }

    fn equality_expr(&mut self) -> Result<Expr, ParseError> {
        let mut left = self.relational_expr()?;
        loop {
            let op = match self.peek_kind() {
                TokenKind::EqEq => BinOp::Eq,
                TokenKind::NotEq => BinOp::NotEq,
                _ => break,
            };
            self.advance();
            let right = self.relational_expr()?;
            let span = combine_spans(left.span, right.span);
            left = Expr::new(ExprKind::Binary(Box::new(left), op, Box::new(right)), span);
        }
        Ok(left)
    }

    fn relational_expr(&mut self) -> Result<Expr, ParseError> {
        let mut left = self.additive_expr()?;
        loop {
            let op = match self.peek_kind() {
                TokenKind::Gt => BinOp::Gt,
                TokenKind::Lt => BinOp::Lt,
                TokenKind::GtEq => BinOp::GtEq,
                TokenKind::LtEq => BinOp::LtEq,
                _ => break,
            };
            self.advance();
            let right = self.additive_expr()?;
            let span = combine_spans(left.span, right.span);
            left = Expr::new(ExprKind::Binary(Box::new(left), op, Box::new(right)), span);
        }
        Ok(left)
    }

    fn additive_expr(&mut self) -> Result<Expr, ParseError> {
        let mut left = self.multiplicative_expr()?;
        loop {
            let op = match self.peek_kind() {
                TokenKind::Plus => BinOp::Add,
                TokenKind::Minus => BinOp::Sub,
                _ => break,
            };
            self.advance();
            let right = self.multiplicative_expr()?;
            let span = combine_spans(left.span, right.span);
            left = Expr::new(ExprKind::Binary(Box::new(left), op, Box::new(right)), span);
        }
        Ok(left)
    }

    fn multiplicative_expr(&mut self) -> Result<Expr, ParseError> {
        let mut left = self.unary_expr()?;
        loop {
            let op = match self.peek_kind() {
                TokenKind::Star => BinOp::Mul,
                TokenKind::Slash => BinOp::Div,
                _ => break,
            };
            self.advance();
            let right = self.unary_expr()?;
            let span = combine_spans(left.span, right.span);
            left = Expr::new(ExprKind::Binary(Box::new(left), op, Box::new(right)), span);
        }
        Ok(left)
    }

    fn unary_expr(&mut self) -> Result<Expr, ParseError> {
        match self.peek_kind() {
            TokenKind::Minus => {
                let op_tok = self.advance();
                let e = self.unary_expr()?;
                let span = Span::new(op_tok.line, op_tok.col, e.span.end_line, e.span.end_col);
                Ok(Expr::new(ExprKind::Unary(UnaryOp::Neg, Box::new(e)), span))
            }
            TokenKind::Not => {
                let op_tok = self.advance();
                let e = self.unary_expr()?;
                let span = Span::new(op_tok.line, op_tok.col, e.span.end_line, e.span.end_col);
                Ok(Expr::new(ExprKind::Unary(UnaryOp::Not, Box::new(e)), span))
            }
            _ => self.primary_expr(),
        }
    }

    fn primary_expr(&mut self) -> Result<Expr, ParseError> {
        match self.peek_kind().clone() {
            TokenKind::IntLiteral(v) => {
                let tok = self.advance();
                Ok(Expr::new(ExprKind::IntLiteral(v), tok_span(&tok)))
            }
            TokenKind::RealLiteral(v) => {
                let tok = self.advance();
                Ok(Expr::new(ExprKind::RealLiteral(v), tok_span(&tok)))
            }
            TokenKind::StringLiteral(v) => {
                let tok = self.advance();
                Ok(Expr::new(ExprKind::StringLiteral(v), tok_span(&tok)))
            }
            TokenKind::True => {
                let tok = self.advance();
                Ok(Expr::new(ExprKind::BoolLiteral(true), tok_span(&tok)))
            }
            TokenKind::False => {
                let tok = self.advance();
                Ok(Expr::new(ExprKind::BoolLiteral(false), tok_span(&tok)))
            }
            TokenKind::Null => {
                let tok = self.advance();
                Ok(Expr::new(ExprKind::Null, tok_span(&tok)))
            }
            TokenKind::LParen => {
                let open = self.advance();
                let e = self.expr()?;
                let close = self.expect(TokenKind::RParen)?;
                let span = Span::new(open.line, open.col, close.line, close.end_col);
                Ok(Expr::new(e.kind, span))
            }
            TokenKind::Function => {
                let start = self.advance();
                let ident_tok = self.peek().clone();
                let name = self.expect_identifier()?;
                let span = Span::new(start.line, start.col, ident_tok.line, ident_tok.end_col);
                Ok(Expr::new(ExprKind::FuncRef(name), span))
            }
            TokenKind::Identifier(name) => {
                let tok = self.advance();
                if self.check(&TokenKind::LParen) {
                    self.advance();
                    let args = self.expr_list(&TokenKind::RParen)?;
                    let close = self.expect(TokenKind::RParen)?;
                    let span = Span::new(tok.line, tok.col, close.line, close.end_col);
                    Ok(Expr::new(ExprKind::Call(name, args), span))
                } else if self.check(&TokenKind::LBracket) {
                    self.advance();
                    let idx = self.expr()?;
                    let close = self.expect(TokenKind::RBracket)?;
                    let span = Span::new(tok.line, tok.col, close.line, close.end_col);
                    Ok(Expr::new(ExprKind::ArrayAccess(name, Box::new(idx)), span))
                } else {
                    Ok(Expr::new(ExprKind::Var(name), tok_span(&tok)))
                }
            }
            other => Err(self.error(format!("unexpected token in expression: {other:?}"))),
        }
    }
}

fn tok_span(tok: &Token) -> Span {
    Span::new(tok.line, tok.col, tok.line, tok.end_col)
}

fn combine_spans(a: Span, b: Span) -> Span {
    Span::new(a.start_line, a.start_col, b.end_line, b.end_col)
}

#[cfg(test)]
#[path = "parser.test.rs"]
mod tests;
