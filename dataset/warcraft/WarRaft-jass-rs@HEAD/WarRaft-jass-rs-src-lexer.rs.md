---
type: vendor-doc
source_repo: "https://github.com/WarRaft/jass-rs"
source_path: "src/lexer.rs"
ref: HEAD
---

use crate::token::{keyword, Token, TokenKind};

#[derive(Debug, Clone, PartialEq)]
pub struct LexError {
    pub message: String,
    pub line: usize,
    pub col: usize,
}

pub struct Lexer {
    chars: Vec<char>,
    pos: usize,
    line: usize,
    col: usize,
}

impl Lexer {
    pub fn new(src: &str) -> Self {
        Lexer {
            chars: src.chars().collect(),
            pos: 0,
            line: 1,
            col: 1,
        }
    }

    pub fn tokenize(src: &str) -> Result<Vec<Token>, LexError> {
        let mut lexer = Lexer::new(src);
        let mut tokens = Vec::new();
        loop {
            let tok = lexer.next_token()?;
            let is_eof = tok.kind == TokenKind::Eof;
            tokens.push(tok);
            if is_eof {
                break;
            }
        }
        Ok(tokens)
    }

    fn peek(&self) -> Option<char> {
        self.chars.get(self.pos).copied()
    }

    fn peek_at(&self, offset: usize) -> Option<char> {
        self.chars.get(self.pos + offset).copied()
    }

    fn advance(&mut self) -> Option<char> {
        let c = self.peek()?;
        self.pos += 1;
        if c == '\n' {
            self.line += 1;
            self.col = 1;
        } else {
            self.col += 1;
        }
        Some(c)
    }

    fn skip_trivia(&mut self) {
        loop {
            match self.peek() {
                Some(c) if c.is_whitespace() => {
                    self.advance();
                }
                Some('/') if self.peek_at(1) == Some('/') => {
                    while let Some(c) = self.peek() {
                        if c == '\n' {
                            break;
                        }
                        self.advance();
                    }
                }
                _ => break,
            }
        }
    }

    pub fn next_token(&mut self) -> Result<Token, LexError> {
        self.skip_trivia();
        let (line, col) = (self.line, self.col);

        let c = match self.peek() {
            None => return Ok(Token::new(TokenKind::Eof, line, col, col)),
            Some(c) => c,
        };

        if c.is_ascii_digit() {
            return self.lex_number(line, col);
        }

        if c == '_' || c.is_alphabetic() {
            return self.lex_identifier(line, col);
        }

        if c == '"' {
            return self.lex_string(line, col);
        }

        if c == '\'' {
            return self.lex_rawcode(line, col);
        }

        self.advance();
        let kind = match c {
            '+' => TokenKind::Plus,
            '-' => TokenKind::Minus,
            '*' => TokenKind::Star,
            '/' => TokenKind::Slash,
            '(' => TokenKind::LParen,
            ')' => TokenKind::RParen,
            '[' => TokenKind::LBracket,
            ']' => TokenKind::RBracket,
            ',' => TokenKind::Comma,
            '=' => {
                if self.peek() == Some('=') {
                    self.advance();
                    TokenKind::EqEq
                } else {
                    TokenKind::Assign
                }
            }
            '!' => {
                if self.peek() == Some('=') {
                    self.advance();
                    TokenKind::NotEq
                } else {
                    return Err(LexError {
                        message: "unexpected character '!'".to_string(),
                        line,
                        col,
                    });
                }
            }
            '>' => {
                if self.peek() == Some('=') {
                    self.advance();
                    TokenKind::GtEq
                } else {
                    TokenKind::Gt
                }
            }
            '<' => {
                if self.peek() == Some('=') {
                    self.advance();
                    TokenKind::LtEq
                } else {
                    TokenKind::Lt
                }
            }
            other => {
                return Err(LexError {
                    message: format!("unexpected character '{other}'"),
                    line,
                    col,
                });
            }
        };

        Ok(Token::new(kind, line, col, self.col))
    }

    fn lex_identifier(&mut self, line: usize, col: usize) -> Result<Token, LexError> {
        let mut s = String::new();
        while let Some(c) = self.peek() {
            if c == '_' || c.is_alphanumeric() {
                s.push(c);
                self.advance();
            } else {
                break;
            }
        }
        let kind = keyword(&s).unwrap_or(TokenKind::Identifier(s));
        Ok(Token::new(kind, line, col, self.col))
    }

    fn lex_number(&mut self, line: usize, col: usize) -> Result<Token, LexError> {
        // Hex literal: 0x... or $...
        if self.peek() == Some('0') && matches!(self.peek_at(1), Some('x') | Some('X')) {
            self.advance();
            self.advance();
            let mut s = String::new();
            while let Some(c) = self.peek() {
                if c.is_ascii_hexdigit() {
                    s.push(c);
                    self.advance();
                } else {
                    break;
                }
            }
            let value = i64::from_str_radix(&s, 16).map_err(|_| LexError {
                message: format!("invalid hex literal '0x{s}'"),
                line,
                col,
            })?;
            return Ok(Token::new(
                TokenKind::IntLiteral(value),
                line,
                col,
                self.col,
            ));
        }

        let mut s = String::new();
        while let Some(c) = self.peek() {
            if c.is_ascii_digit() {
                s.push(c);
                self.advance();
            } else {
                break;
            }
        }

        if self.peek() == Some('.') && self.peek_at(1).map(|c| c.is_ascii_digit()).unwrap_or(false)
        {
            s.push('.');
            self.advance();
            while let Some(c) = self.peek() {
                if c.is_ascii_digit() {
                    s.push(c);
                    self.advance();
                } else {
                    break;
                }
            }
            let value: f64 = s.parse().map_err(|_| LexError {
                message: format!("invalid real literal '{s}'"),
                line,
                col,
            })?;
            return Ok(Token::new(
                TokenKind::RealLiteral(value),
                line,
                col,
                self.col,
            ));
        }

        let value: i64 = s.parse().map_err(|_| LexError {
            message: format!("invalid integer literal '{s}'"),
            line,
            col,
        })?;
        Ok(Token::new(
            TokenKind::IntLiteral(value),
            line,
            col,
            self.col,
        ))
    }

    fn lex_string(&mut self, line: usize, col: usize) -> Result<Token, LexError> {
        self.advance(); // opening quote
        let mut s = String::new();
        loop {
            match self.peek() {
                None | Some('\n') => {
                    return Err(LexError {
                        message: "unterminated string literal".to_string(),
                        line,
                        col,
                    });
                }
                Some('"') => {
                    self.advance();
                    break;
                }
                Some('\\') => {
                    self.advance();
                    match self.advance() {
                        Some('n') => s.push('\n'),
                        Some('t') => s.push('\t'),
                        Some('"') => s.push('"'),
                        Some('\\') => s.push('\\'),
                        Some(other) => s.push(other),
                        None => {
                            return Err(LexError {
                                message: "unterminated string literal".to_string(),
                                line,
                                col,
                            });
                        }
                    }
                }
                Some(c) => {
                    s.push(c);
                    self.advance();
                }
            }
        }
        Ok(Token::new(TokenKind::StringLiteral(s), line, col, self.col))
    }

    fn lex_rawcode(&mut self, line: usize, col: usize) -> Result<Token, LexError> {
        self.advance(); // opening quote
        let mut s = String::new();
        loop {
            match self.peek() {
                None | Some('\n') => {
                    return Err(LexError {
                        message: "unterminated rawcode literal".to_string(),
                        line,
                        col,
                    });
                }
                Some('\'') => {
                    self.advance();
                    break;
                }
                Some(c) => {
                    s.push(c);
                    self.advance();
                }
            }
        }
        if s.len() != 4 {
            return Err(LexError {
                message: format!("rawcode literal '{s}' must be exactly 4 characters"),
                line,
                col,
            });
        }
        let mut value: i64 = 0;
        for b in s.bytes() {
            value = (value << 8) | b as i64;
        }
        Ok(Token::new(
            TokenKind::IntLiteral(value),
            line,
            col,
            self.col,
        ))
    }
}

#[cfg(test)]
#[path = "lexer.test.rs"]
mod tests;
