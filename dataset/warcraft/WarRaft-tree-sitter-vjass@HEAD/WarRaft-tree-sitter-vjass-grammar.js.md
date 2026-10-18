---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "grammar.js"
ref: HEAD
---

const PREC = {
    COMMA: -1,
    FUNCTION: 1,
    DEFAULT: 1,
    PRIORITY: 2,

    BIT_OR: 6, // => |
    BIT_NOT: 7, // => ~
    BIT_AND: 8, // => &
    SHIFT: 9, // => << >>
    CONCAT: 10, // => ..
    PLUS: 11, // => + -
    MULTI: 12, // => * /             // %
    UNARY: 13, // => not # - ~
    POWER: 14, // => ^

    STATEMENT: 15,
    PROGRAM: 16,
}

module.exports = grammar({
    name: 'vjass',

    externals: $ => [
        $._block_comment_start,
        $._block_comment_content,
        $._block_comment_end,

        $._string_start,
        $._string_content,
        $._string_end,
    ],
    extras: $ => [/\n/, /\s/, $.comment],

    inline: $ => [
        //$.expr,
        //$.field_separator,
        //$.prefix_exp,
        //$.function_impl,
        $.comment,
    ],

    conflicts: $ => [
        [$.id, $.keywords]
    ],

    rules: {
        program: $ => repeat($._block),

        id: _ => token(prec(-1, /[a-zA-Z_][a-zA-Z0-9_]*/)),
        keywords: _ => choice('struct', 'endstruct', 'globals', 'endglobals', 'loop', 'endloop'),

        _block: $ => choice(
            $.globals,
            $.struct,
            $.loop,
            $.expr,
        ),

        struct: $ => seq(
            alias(token('struct'), $.struct_),
            repeat($._block),
            alias(token('endstruct'), $.endstruct_)
        ),

        globals: $ => seq(
            alias(token('globals'), $.globals_),
            repeat($._block),
            alias(token('endglobals'), $.endglobals_)
        ),

        loop: $ => seq(
            alias(token('loop'), $.loop_),
            repeat($._block),
            alias(token('endloop'), $.endloop_)
        ),

        expr: $ => choice(
            $.id,
            $.number,
            $.float,
            $.string,

            // https://learn.microsoft.com/en-us/cpp/cpp/cpp-built-in-operators-precedence-and-associativity?view=msvc-170
            // Group 1 precedence, no associativity
            // Scope resolution	::

            // Group 2 precedence, left to right associativity
            prec.left(2, seq($.expr, '.', $.expr)), // Member selection (object or pointer)	. or ->
            prec.left(2, seq('[', $.expr, ']')), // Array subscript	[]
            prec.left(2, seq('(', repeat($.expr), ')')), // Function call	()
            prec.left(2, seq($.expr, '++')), // Postfix increment	++
            prec.left(2, seq($.expr, '++')), // Postfix decrement	--
            // Type name	typeid
            // Constant type conversion	const_cast
            // Dynamic type conversion	dynamic_cast
            // Reinterpreted type conversion	reinterpret_cast
            // Static type conversion	static_cast

            // Group 3 precedence, right to left associativity
            // Size of object or type	sizeof
            prec.right(3, seq('++', $.expr)), // Prefix increment	++
            prec.right(3, seq('--', $.expr)),  // Prefix decrement	--
            // One's complement	~	compl
            prec.right(3, seq('not', $.expr)), // Logical not	!	not
            prec.right(3, seq('-', $.expr)),  // Unary negation	-
            prec.right(3, seq('+', $.expr)), // Unary plus	+
            // Address-of	&
            // Indirection	*
            // Create object	new
            // Destroy object	delete
            // Cast	()

            // Group 4 precedence, left to right associativity
            // Pointer-to-member (objects or pointers)	.* or ->*

            // Group 5 precedence, left to right associativity
            prec.left(5, seq($.expr, '*', $.expr)), // Multiplication	*
            prec.left(5, seq($.expr, '/', $.expr)), // Division	/
            // Modulus	%

            // Group 6 precedence, left to right associativity
            prec.left(6, seq($.expr, '+', $.expr)), // Addition	+
            prec.left(6, seq($.expr, '-', $.expr)), // Subtraction	-

            // Group 7 precedence, left to right associativity
            // Left shift	<<
            // Right shift	>>

            // Group 8 precedence, left to right associativity
            prec.left(8, seq($.expr, '<', $.expr)), // Less than	<
            prec.left(8, seq($.expr, '>', $.expr)), // Greater than	>
            prec.left(8, seq($.expr, '<=', $.expr)), // Less than or equal to	<=
            prec.left(8, seq($.expr, '>=', $.expr)), // Greater than or equal to	>=

            // Group 9 precedence, left to right associativity
            prec.left(9, seq($.expr, '==', $.expr)), // Equality	==
            prec.left(9, seq($.expr, '!=', $.expr)), // Inequality	!=	not_eq

            // Group 10 precedence left to right associativity
            // Bitwise AND	&	bitand

            // Group 11 precedence, left to right associativity
            // Bitwise exclusive OR	^	xor

            // Group 12 precedence, left to right associativity
            // Bitwise inclusive OR	|	bitor

            // Group 13 precedence, left to right associativity
            prec.left(14 /* in JASS */, seq($.expr, 'and', $.expr)), // Logical AND	&&	and

            // Group 14 precedence, left to right associativity
            prec.left(13 /* in JASS */, seq($.expr, 'or', $.expr)), // Logical OR	||	or

            // Group 15 precedence, right to left associativity
            // Conditional	? :
            prec.right(15, seq($.expr, '=', $.expr)), // Assignment	=
            // Multiplication assignment	*=
            // Division assignment	/=
            // Modulus assignment	%=
            // Addition assignment	+=
            // Subtraction assignment	-=
            // Left-shift assignment	<<=
            // Right-shift assignment	>>=
            // Bitwise AND assignment	&=	and_eq
            // Bitwise inclusive OR assignment	|=	or_eq
            // Bitwise exclusive OR assignment	^=	xor_eq
            // throw expression	throw

            // Group 16 precedence, left to right associativity
            prec.left(16, seq($.expr, ',', $.expr)), // Comma

        ),

        number: _ => {
            const separator = '_'
            const decimal = /[0-9]+/
            const hex = /[0-9a-fA-F]/
            const bin = /[01]/
            const decimalDigits = seq(repeat1(decimal), repeat(seq(separator, repeat1(decimal))))
            const hexDigits = seq(repeat1(hex), repeat(seq(separator, repeat1(hex))))
            const binDigits = seq(repeat1(bin), repeat(seq(separator, repeat1(bin))))

            return token(seq(
                choice(
                    decimalDigits,
                    seq(/0[xX]/, hexDigits),
                    seq(/0[bB]/, binDigits),
                ),
                optional(/([lL]|[uU][lL]?)/),
            ))
        },

        float: _ => {
            const separator = '_'
            const decimal = /[0-9]+/
            const exponent = /[eE][+-]?[0-9]+/
            const decimalDigits = seq(repeat1(decimal), repeat(seq(separator, repeat1(decimal))))

            return token(seq(
                choice(
                    seq(decimalDigits, exponent, optional(/[fF]/)),
                    seq(optional(decimalDigits), '.', repeat1(decimalDigits), optional(exponent), optional(/[fF]/)),
                    seq(decimalDigits, /[fF]/),
                ),
            ))
        },

        var_stmt: $ =>
            prec.right(
                PREC.DEFAULT,
                seq(
                    repeat(
                        choice(
                            alias('local', $.local),
                            alias('constant', $.constant),
                            alias('array', $.array),
                        )
                    ),
                    field('type', $.id),
                    $.var_decl,
                    repeat(seq(',', $.var_decl))
                )
            ),

        var_decl: $ => seq(
            field('name', $.id),
            optional(seq('=', $.expr)
            )
        ),

        _var: $ =>
            prec(
                PREC.PRIORITY,
                choice(
                    $.id,
                    seq($.prefix_exp, '[', $.expr, ']'),
                    seq($.prefix_exp, '.', $.id)
                )
            ),

        var_list: $ => seq($._var, repeat(seq(',', $._var))),

        string: $ =>
            seq(
                field('start', alias($._string_start, 'string_start')),
                field('content', optional(alias($._string_content, 'string_content'))),
                field('end', alias($._string_end, 'string_end'))
            ),

        _statement: $ =>
            prec.right(
                PREC.STATEMENT,
                seq(
                    choice(
                        $.var_stmt,
                        $.function_call,
                        $.do_statement,
                        $.while_statement,
                        $.repeat_statement,
                        $.if_statement,
                        $.function_statement
                        // $.comment
                    ),
                )
            ),

        // Primitives {{{
        function_name: $ => seq($.id),

        function: $ => seq($.function_start, $.function_impl),

        function_impl: $ =>
            seq(
                alias('(', $.function_body_paren),
                optional($.parameter_list),
                alias(')', $.function_body_paren),
                alias(optional($._block), $.function_body),
                alias('end', $.function_end)
            ),

        parameter_list: $ =>
            choice(
                seq(
                    prec.left(PREC.COMMA, seq($.id, repeat(seq(/,\s*/, $.id)))),
                ),
            ),
        // }}}

        unary_operation: $ =>
            prec.left(PREC.UNARY, seq(choice('not', '#', '-', '~'), $.expr)),

        _id_list: $ =>
            prec.right(PREC.COMMA, seq($.id, repeat(seq(/,\s*/, $.id)))),

        return_statement: $ =>
            prec(PREC.PRIORITY, seq('return', optional(seq($.expr, repeat(seq(',', $.expr)))))),

        break_statement: _ => 'break',

        // Blocks {{{
        do_statement: $ =>
            seq(alias('do', $.do_start), optional($._block), alias('end', $.do_end)),

        while_statement: $ =>
            seq(
                alias('while', $.while_start),
                $.expr,
                alias('do', $.while_do),
                optional($._block),
                alias('end', $.while_end)
            ),

        repeat_statement: $ =>
            seq(
                alias('repeat', $.repeat_start),
                optional($._block),
                alias('until', $.repeat_until),
                $.expr
            ),

        if_statement: $ =>
            seq(
                alias('if', $.if_start),
                $.expr,
                alias('then', $.if_then),
                optional($._block),
                repeat(
                    seq(
                        alias('elseif', $.if_elseif),
                        $.expr,
                        alias('then', $.if_then),
                        optional($._block)
                    )
                ),
                optional(seq(alias('else', $.if_else), optional($._block))),
                alias('end', $.if_end)
            ),

        function_start: () => 'function',

        function_statement: $ =>
            prec.right(
                PREC.DEFAULT,
                seq(
                    choice(
                        seq(
                            alias('local', $.local),
                            $.function_start,
                            field('name', $.id)
                        ),
                        seq($.function_start, /\s*/, field('name', $.function_name))
                    ),
                    $.function_impl
                )
            ),

        // }}}

        // Function {{{
        _prefix_exp: $ =>
            choice(
                $._var,
                $.function_call,
                seq('(', $.expr, ')')
            ),

        prefix_exp: $ => $._prefix_exp,

        function_call: $ =>
            prec.right(
                PREC.FUNCTION,
                seq(field('prefix', $.prefix_exp), choice($._args, $._self_call))
            ),

        _args: $ => choice($._parentheses_call, $._string_call),

        _parentheses_call: $ =>
            seq(
                alias('(', $.function_call_paren),
                field('args', optional($.function_arguments)),
                alias(')', $.function_call_paren)
            ),

        _string_call: $ =>
            field(
                'args',
                //  Decide if this is really the name we want to use.
                alias($.string, $.string_argument)
            ),

        _self_call: $ =>
            seq(alias(':', $.self_call_colon), $.id, $._args),

        function_arguments: $ =>
            seq($.expr, optional(repeat(seq(',', $.expr)))),

        // }}}


        // Comments {{{
        // comment: ($) => choice(seq("--", /[^-].*\r?\n/), $._multi_comment),
        comment: $ =>
            choice(
                seq(
                    field('start', alias('//', 'comment_start')),
                    field('content', alias(/[^\r\n]*/, 'comment_content'))
                ),
                seq(
                    field('start', alias($._block_comment_start, 'comment_start')),
                    field(
                        'content',
                        optional(alias($._block_comment_content, 'comment_content'))
                    ),
                    field('end', alias($._block_comment_end, 'comment_end'))
                )
            ),
        // }}}
    },
})