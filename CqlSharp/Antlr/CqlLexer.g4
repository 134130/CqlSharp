lexer grammar CqlLexer;

fragment A: [aA];
fragment B: [bB];
fragment C: [cC];
fragment D: [dD];
fragment E: [eE];
fragment F: [fF];
fragment G: [gG];
fragment H: [hH];
fragment I: [iI];
fragment J: [jJ];
fragment K: [kK];
fragment L: [lL];
fragment M: [mM];
fragment N: [nN];
fragment O: [oO];
fragment P: [pP]; 
fragment Q: [qQ];
fragment R: [rR];
fragment S: [sS];
fragment T: [tT];
fragment U: [uU];
fragment V: [vV];
fragment W: [wW];
fragment X: [xX];
fragment Y: [yY];
fragment Z: [zZ];

fragment DIGIT: [0-9];
DIGITS: DIGIT+;

OPEN_PAR_SYMBOL: '(';
CLOSE_PAR_SYMBOL: ')';
DOT_SYMBOL: '.';
COMMA_SYMBOL: ',';
SEMICOLON_SYMBOL: ';';
AS_SYMBOL: A S;
ASC_SYMBOL: A S C;
DESC_SYMBOL: D E S C;
NOT_SYMBOL: N O T;
AND_SYMBOL: A N D;
XOR_SYMBOL: X O R;
OR_SYMBOL: O R;
NULL_SYMBOL: N U L L;
SELECT_SYMBOL: S E L E C T;
INSERT_SYMBOL: I N S E R T;
INTO_SYMBOL: I N T O;
VALUES_SYMBOL: V A L U E S;
ORDER_SYMBOL: O R D E R;
BY_SYMBOL: B Y;
LIMIT_SYMBOL: L I M I T;
OFFSET_SYMBOL: O F F S E T;
FROM_SYMBOL: F R O M;
WHERE_SYMBOL: W H E R E;
TRUE_SYMBOL : T R U E;
FALSE_SYMBOL : F A L S E;
COUNT_SYMBOL: C O U N T;
LIKE_SYMBOL: L I K E;
IN_SYMBOL: I N;
IS_SYMBOL: I S;
BETWEEN_SYMBOL: B E T W E E N;
REGEXP_SYMBOL: R E G E X P;
MULT_OPERATOR: '*';
PLUS_OPERATOR: '+';
MINUS_OPERATOR: '-';
EQUAL_OPERATOR: '=';
GREATER_OR_EQUAL_OPERATOR: '>='; 
GREATER_THAN_OPERATOR: '>';
LESS_OR_EQUAL_OPERATOR: '<=';
LESS_THAN_OPERATOR: '<';
NOT_EQUAL_OPERATOR: '!=';



fragment LETTER_WHEN_UNQUOTED: DIGIT | LETTER_WHEN_UNQUOTED_NO_DIGIT;
fragment LETTER_WHEN_UNQUOTED_NO_DIGIT: [a-zA-Z_$\u0080-\uffff];

IDENTIFIER: LETTER_WHEN_UNQUOTED_NO_DIGIT LETTER_WHEN_UNQUOTED*;

fragment SINGLE_QUOTE: '\'';
fragment DOUBLE_QUOTE: '"';
SINGLE_QUOTED_TEXT: (SINGLE_QUOTE .*? SINGLE_QUOTE)+;
DOUBLE_QUOTED_TEXT: (DOUBLE_QUOTE .*? DOUBLE_QUOTE)+;

WHITESPACE: [ \t\f\r\n] -> channel(HIDDEN);
SLASH: [/] -> channel(HIDDEN);