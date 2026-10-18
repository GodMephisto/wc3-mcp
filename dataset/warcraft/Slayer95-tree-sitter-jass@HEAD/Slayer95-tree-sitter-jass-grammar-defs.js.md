---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "grammar-defs.js"
ref: HEAD
---

"use strict";

/**
 * @file Parser for the JASS2 language
 * @author Leonardo Julca <ivojulca@gmail.com>
 * @license MIT
 */

/// <reference types="tree-sitter-cli/dsl" />
// @ts-check

const PRECEDENCES = {
  'and': 1,
	'or': 2,
	'==': 10,
	'!=': 10,
	'<': 15,
	'>': 15,
	'<=': 15,
	'>=': 15,
	'+': 20,
	'-': 20,
	'*': 21,
	'/': 21,
	'%': 21,
	'not': 30,
	'neg': 30,
	'pos': 30,

  Literal: 2,
};

const binaryOperators = [
	'and', 'or', '==', '!=', '<', '>', '<=', '>=', '+', '-', '*', '/', '%',
];

function repeat1WithDelimiter(nodeTypes, arg, delim) {
	return seq(nodeTypes[arg], repeat(seq(delim, nodeTypes[arg])));
}

function makeBinaryExpressionRule(operator) {
	return prec.left(PRECEDENCES[operator], seq(
		field('lhs', this.Expression),
		field('operator', alias(operator, this.Operator)),
		field('rhs', this.Expression),
	));
}

const helpers = {
	loop: false,
	rules: {
		AnyStatementsOrNewLines(nodeTypes) {
			return repeat(choice(
				seq(nodeTypes.RStatement, nodeTypes.NewLine),
				nodeTypes.NewLine,
			));
		},
	},
	setForkWhetherInLoop(baseName, def) {
		const outsideName = `R${baseName}`;
		const insideName = `L${baseName}`;
		const outsideDef = def.bind(helpers);
		const insideDef = def.bind(helpersLoopCtx);
		return {
			[outsideName](nodeTypes) {
				return outsideDef(nodeTypes);
			},
			[insideName](nodeTypes) {
				return insideDef(nodeTypes);
			},
		};
	},
	getForkWhetherInLoop(nodeTypes, baseName) {
		return nodeTypes[`R${baseName}`];
	},
	listForkedNodes(nodeTypes, baseName) {
		return [nodeTypes[`R${baseName}`], nodeTypes[`L${baseName}`]];
	},
};

const helpersLoopCtx = {
	loop: true,
	rules: {
		AnyStatementsOrNewLines(nodeTypes) {
			return repeat(choice(
				seq(nodeTypes.LStatement, nodeTypes.NewLine),
				nodeTypes.NewLine,
			));
		},
	},
	getForkWhetherInLoop(nodeTypes, baseName) {
		return nodeTypes[`L${baseName}`];
	},
};

const rules = {
	NewLine() {
		return /\r?\n|\r/;
	},

	Identifier() {
		return /[a-zA-Z]([a-zA-Z0-9_]*)/;
	},

	AtomicType(nodeTypes) {
		return nodeTypes.Identifier;
	},

	ArrayType(nodeTypes) {
		return seq(nodeTypes.Identifier, 'array');
	},

	ArrayElement(nodeTypes) {
		return seq(
			field('array', alias(nodeTypes.Identifier, nodeTypes.VariableReference)),
			'[',
			field('index', nodeTypes.Expression),
			']',
		);
	},

	BOM(nodeTypes) {
		return token('\xEF\xBB\xBF');
	},

	Comment(nodeTypes) {
		// Rust dot includes every entity, except LF.
		return token(seq('//', /[^\n\x00\xFF]*/));
	},

	Null(nodeTypes) {
		return prec(PRECEDENCES.Literal, 'null');
	},

	True(nodeTypes) {
		return prec(PRECEDENCES.Literal, 'true');
	},

	False(nodeTypes) {
		return prec(PRECEDENCES.Literal, 'false');
	},

	Boolean(nodeTypes) {
		return choice(
			nodeTypes.True,
			nodeTypes.False,
		);
	},

	String(nodeTypes) {
		return token(seq(
			'"',
			repeat(choice(
				/[^"\\\x00\xFF]/,
				/\\(\\|n|r|t|f|b|"|')/,
			)),
			'"',
		));
	},

	DecimalInteger(nodeTypes) {
		return token(seq(
			optional('-'),
			choice(
				'0',
				/[1-9]([0-9]*)/,
			),
		));
	},

	OctalInteger(nodeTypes) {
		return /-?0([0-7]+)/;
	},

	HexInteger(nodeTypes) {
		return token(seq(
			optional('-'),
			choice('0x', '0X', '$'),
			/[0-9a-fA-F]+/
		));
	},

	Integer(nodeTypes) {
		return choice(
			nodeTypes.DecimalInteger,
			nodeTypes.OctalInteger,
			nodeTypes.HexInteger,
		);
	},

	Real(nodeTypes) {
		return token(
			seq(
				optional('-'),
				choice(
					/\.[0-9]+/,
					/[0-9]+\.[0-9]*/,
				),
			),
		);
	},

	SignedChar(nodeTypes) {
		return token(seq(
			"'",
			choice(
				/[^'\\\x00\xFF]/,
				/\\(\\|n|r|t|f|b|"|')/,
			),
			"'",
		));
	},

	FourCC(nodeTypes) {
		return token(seq(
			"'",
			/[^'\\\x00\xFF]/,
			/[^'\\\x00\xFF]/,
			/[^'\\\x00\xFF]/,
			/[^'\\\x00\xFF]/,
			"'",
		));
	},

	Literal(nodeTypes) {
		return choice(
			nodeTypes.Null,
			nodeTypes.Boolean,
			nodeTypes.String,
			nodeTypes.Integer,
			nodeTypes.Real,
			nodeTypes.FourCC,
			nodeTypes.SignedChar,
		);
	},

	TopLevel(nodeTypes) {
		return choice(
			nodeTypes.TypeDeclaration,
			nodeTypes.NativeDeclaration,
			nodeTypes.FunctionDeclaration,
			nodeTypes.GlobalsBlock,
		);
	},

	TypeDeclaration(nodeTypes) {
		return seq(
			'type',
			field('name', nodeTypes.Identifier),
			'extends',
			field('super', alias(nodeTypes.Identifier, nodeTypes.TypeReference)),
		);
	},

	GlobalsBlock(nodeTypes) {
		return seq(
			'globals',
			repeat(choice(
				seq(nodeTypes.GlobalDeclarationStatement, nodeTypes.NewLine),
				nodeTypes.NewLine,
			)),
			'endglobals',
		);
	},

	GlobalDeclarationStatement(nodeTypes) {
		return choice(
			seq(
				optional(nodeTypes.DeclareAttributes),
				field('type', nodeTypes.AtomicType),
				field('name', nodeTypes.Identifier),
				optional(field('value', nodeTypes.Initializer)),
			),
			seq(
				field('type', nodeTypes.ArrayType),
				field('name', nodeTypes.Identifier),
			),
		);
	},

	FunctionSignature(nodeTypes) {
		return seq(
			field('name', nodeTypes.Identifier),
			'takes',
			field('input', choice(
				alias('nothing', nodeTypes.Empty),
				nodeTypes.FunctionParameterList,
			)),
			'returns',
			field('output', choice(
				alias('nothing', nodeTypes.None),
				alias(nodeTypes.Identifier, nodeTypes.TypeReference),
			))
		);
	},

	FunctionParameterList(nodeTypes) {
		return repeat1WithDelimiter(nodeTypes, 'FunctionParameter', ',');
	},

	FunctionParameter(nodeTypes) {
		return seq(
			field('type', alias(nodeTypes.Identifier, nodeTypes.TypeReference)),
			field('name', nodeTypes.Identifier),
		);
	},

	NativeDeclaration(nodeTypes) {
		return seq(
			optional(nodeTypes.NativeAttributes), // inlined to ConstantAttribute
			'native',
			field('signature', nodeTypes.FunctionSignature),
		);
	},

	NativeAttributes(nodeTypes) {
		return nodeTypes.ConstantAttribute;
	},

	FunctionDeclaration(nodeTypes) {
		return seq(
			optional(nodeTypes.FunctionAttributes), // inlined to ConstantAttribute
			'function',
			field('signature', nodeTypes.FunctionSignature),
			nodeTypes.FunctionBody,
			'endfunction',
		);
	},

	FunctionAttributes(nodeTypes) {
		return nodeTypes.ConstantAttribute;
	},

	FunctionBody(nodeTypes) {
		return seq(
			nodeTypes.NewLine,
			repeat(choice(
				seq(nodeTypes.LocalDeclarationStatement, nodeTypes.NewLine),
				nodeTypes.NewLine,
			)),
			helpers.rules.AnyStatementsOrNewLines(nodeTypes),
		);
	},

	LocalDeclarationStatement(nodeTypes) {
		return choice(
			seq(
				'local',
				field('type', nodeTypes.AtomicType),
				field('name', nodeTypes.Identifier),
				optional(field('value', nodeTypes.Initializer)),
			),
			seq(
				'local',
				field('type', nodeTypes.ArrayType),
				field('name', nodeTypes.Identifier),
			),
		);
	},

	DeclareAttributes(nodeTypes) {
		return nodeTypes.ConstantAttribute;
	},

	ConstantAttribute(nodeTypes) {
		return 'constant';
	},

	Initializer(nodeTypes) {
		return seq('=', field('value', nodeTypes.Expression));
	},

	Test(nodeTypes) {
		return nodeTypes.Expression;
	},

	...helpers.setForkWhetherInLoop('Statement', function (nodeTypes) {
		return choice(
			this.getForkWhetherInLoop(nodeTypes, 'IfStatement'),
			nodeTypes.LoopStatement,
			nodeTypes.ReturnStatement,
			nodeTypes.SetStatement,
			nodeTypes.CallStatement,
			...(this.loop ? [nodeTypes.ExitWhenStatement] : []),
		);
	}),

	...helpers.setForkWhetherInLoop('IfStatement', function (nodeTypes) {
		return seq(
			'if',
			field('test', nodeTypes.Test),
			'then',
			this.getForkWhetherInLoop(nodeTypes, 'Consequent'),
			repeat(this.getForkWhetherInLoop(nodeTypes, 'ElseIfStatement')),
			optional(this.getForkWhetherInLoop(nodeTypes, 'ElseStatement')),
			'endif',
		);
	}),

	...helpers.setForkWhetherInLoop('ElseIfStatement', function (nodeTypes) {
		return seq(
			'elseif',
			field('test', nodeTypes.Test),
			'then',
			this.getForkWhetherInLoop(nodeTypes, 'Alternate')
		);
	}),

	...helpers.setForkWhetherInLoop('ElseStatement', function (nodeTypes) {
		return seq(
			'else',
			this.getForkWhetherInLoop(nodeTypes, 'Alternate')
		);
	}),

	...helpers.setForkWhetherInLoop('Consequent', function (nodeTypes) {
		return seq(
			nodeTypes.NewLine,
			this.rules.AnyStatementsOrNewLines(nodeTypes),
		);
	}),

	...helpers.setForkWhetherInLoop('Alternate', function (nodeTypes) {
		return seq(
			nodeTypes.NewLine,
			this.rules.AnyStatementsOrNewLines(nodeTypes),
		);
	}),

	LoopStatement(nodeTypes) {
		return seq(
			'loop',
			nodeTypes.NewLine,
			helpersLoopCtx.rules.AnyStatementsOrNewLines(nodeTypes),
			'endloop',
		);
	},

	ExitWhenStatement(nodeTypes) {
		return seq(
			'exitwhen',
			field('test', nodeTypes.Test),
		);
	},

	ReturnStatement(nodeTypes) {
		return seq(
			'return',
			optional(nodeTypes.Expression),
		);
	},

	SetStatement(nodeTypes) {
		return seq(
			'set',
			field('binding', nodeTypes.Binding),
			field('value', nodeTypes.Initializer),
		);
	},

	Binding(nodeTypes) {
		return choice(
			alias(nodeTypes.Identifier, nodeTypes.VariableReference),
			nodeTypes.ArrayElement,
		);
	},

	CallStatement(nodeTypes) {
		return seq(
			'call',
			nodeTypes.CallExpression,
		);
	},

	CallExpression(nodeTypes) {
		return seq(
			field('callee', alias(nodeTypes.Identifier, nodeTypes.FunctionReference)),
			'(',
			optional(nodeTypes.FunctionArgumentList),
			')',
		);
	},

	FunctionArgumentList(nodeTypes) {
		return repeat1WithDelimiter(nodeTypes, 'FunctionArgument', ',');
	},

	Expression(nodeTypes) {
		return choice(
			nodeTypes.PrimaryExpression,
			nodeTypes.UnaryExpression,
			nodeTypes.BinaryExpression,
		);
	},

	PrimaryExpression(nodeTypes) {
		return choice(
			nodeTypes.Literal,
			alias(nodeTypes.Identifier, nodeTypes.VariableReference),
			nodeTypes.CodeReference,
			nodeTypes.ArrayElement,
			nodeTypes.CallExpression,
			nodeTypes.ParenthesizedExpression,
		);
	},

	UnaryExpression(nodeTypes) {
		return choice(
			nodeTypes.NotExpression,
			nodeTypes.NegativeExpression,
			nodeTypes.PositiveExpression,
		);
	},

	NotExpression(nodeTypes) {
		return prec.right(PRECEDENCES.not, seq('not', nodeTypes.Expression));
	},

	NegativeExpression(nodeTypes) {
		return prec.right(PRECEDENCES.neg, seq('-', nodeTypes.Expression));
	},

	PositiveExpression(nodeTypes) {
		return prec.right(PRECEDENCES.pos, seq('+', nodeTypes.Expression));
	},

	BinaryExpression(nodeTypes) {
		return choice(...binaryOperators.map(makeBinaryExpressionRule, nodeTypes));
	},

	ParenthesizedExpression(nodeTypes) {
		return seq('(', nodeTypes.Expression, ')');
	},

	FunctionArgument(nodeTypes) {
		return nodeTypes.Expression;
	},

	CodeReference(nodeTypes) {
		return seq(
			'function',
			field('funarg', alias(nodeTypes.Identifier, nodeTypes.FunctionReference)),
		);
	},
};

const root = function program(nodeTypes) {
	return seq(
		optional(nodeTypes.BOM),
		repeat(choice(
			seq(nodeTypes.TopLevel, nodeTypes.NewLine),
			nodeTypes.NewLine,
		)),
		optional(nodeTypes.TopLevel),
	);
};

module.exports = {
	root,

	baseDefinitions: {
		name: 'jass',

		extras(nodeTypes) {
			return [
				/ |\t/,
				nodeTypes.Comment,
			];
		},

		word(nodeTypes) {
			return nodeTypes.Identifier;
		},

		inline(nodeTypes) {
			return [
				nodeTypes.TopLevel,
				...helpers.listForkedNodes(nodeTypes, 'Statement'),
				nodeTypes.Binding,
				nodeTypes.Expression,
				nodeTypes.PrimaryExpression,
				nodeTypes.UnaryExpression,
				//nodeTypes.BinaryExpression,
				nodeTypes.Integer,

				nodeTypes.DeclareAttributes, // (global) constant <type>
				nodeTypes.FunctionAttributes, // constant function
				nodeTypes.NativeAttributes, // constant native
			];
		},

		conflicts(nodeTypes) {
			return [
				[nodeTypes.FunctionBody],
			];
		},

		rules,
	},	
};
