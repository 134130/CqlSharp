parser grammar CqlParser;

options {  
    tokenVocab=CQLLexer;
}

query:
    EOF |
    (queryStatement (SEMICOLON_SYMBOL EOF? | EOF))+;

queryStatement:
    selectStatement;

selectStatement:
    selectExpression |
    selectExpressionWithParens;
    
selectExpression:
    SELECT_SYMBOL selectItemList fromClause? whereClause? orderClause? limitClause?;
    
selectExpressionWithParens:
    OPEN_PAR_SYMBOL (selectExpression | selectExpressionWithParens) CLOSE_PAR_SYMBOL;
    
selectItemList:
    selectItem (COMMA_SYMBOL selectItem)*;
    
selectItem:
    expression (AS_SYMBOL? (identifier | textStringLiteral))? #singleItemSelect | 
    identifier DOT_SYMBOL MULT_OPERATOR #multItemSelect |
    MULT_OPERATOR #multItemSelect;

// ORDER
orderClause:
    ORDER_SYMBOL BY_SYMBOL orderList;
orderList:
    orderExpression (COMMA_SYMBOL orderExpression)*;
orderExpression:
    identifier direction?;
direction:
    ASC_SYMBOL |
    DESC_SYMBOL;

// LIMIT
limitClause:
    LIMIT_SYMBOL DIGITS (OFFSET_SYMBOL DIGITS)?;

// FROM
fromClause:
    FROM_SYMBOL tableReference; // (COMMA_SYMBOL tableReference)*;

tableReference:
    singleTable |
    singleTableWithParens |
    selectExpressionWithParens (AS_SYMBOL? identifier)?; // # subquery

singleTable:
    identifier (AS_SYMBOL? identifier)? |
    csvFilePath (AS_SYMBOL? identifier)?;
    
csvFilePath:
    DOUBLE_QUOTED_TEXT;
    
singleTableWithParens:
    OPEN_PAR_SYMBOL (singleTable | singleTableWithParens) CLOSE_PAR_SYMBOL;

// WHERE 
whereClause:
    WHERE_SYMBOL expression;

// Expression
expression:
    boolPrimitive #expressionDefault |
    boolPrimitive IS_SYMBOL NOT_SYMBOL? type = (TRUE_SYMBOL | FALSE_SYMBOL) #expressionIs |
    NOT_SYMBOL expression #expressionNot |  // Single Expression Node
    expression AND_SYMBOL expression #expressionAnd |
    expression OR_SYMBOL expression #expressionOr;
    
boolPrimitive: 
    predicate |
    boolPrimitive compareOperator predicate; // TODO: Binary Expression node 

predicate:
    simpleExpression | 
    simpleExpression NOT_SYMBOL? predicateOperation; // Predicate expression
    
simpleExpression:
    literal #simpleExpressionLiteral |
    identifier (DOT_SYMBOL identifier)? #simpleExpressionColumnReference;
    // TODO: COUNT_SYMBOL OPEN_PAR_SYMBOL MULT_OPERATOR CLOSE_PAR_SYMBOL #simpleExpressionCountExpression;
  
    // TODO: bitExpression operator=PLUS_OPERATOR bitExpression #bitBinaryExpression;  // For string concat
    // operator = PLUS_OPERATOR simpleExpression #simpleExpression| // | MINUS_OPERATOR | BITWISE_NOT_OPERATOR

predicateOperation:
    LIKE_SYMBOL textStringLiteral #predicateOperationLike |
    IN_SYMBOL (OPEN_PAR_SYMBOL textStringLiteral (COMMA_SYMBOL textStringLiteral)* CLOSE_PAR_SYMBOL) #predicateOperationIn | // TODO: subquery?
    BETWEEN_SYMBOL textStringLiteral AND_SYMBOL textStringLiteral #predicateOperationBetween |
    REGEXP_SYMBOL textStringLiteral #predicateOperationRegexp ;

compareOperator:
    EQUAL_OPERATOR |
    NOT_EQUAL_OPERATOR |
    GREATER_OR_EQUAL_OPERATOR |
    GREATER_THAN_OPERATOR |
    LESS_OR_EQUAL_OPERATOR |
    LESS_THAN_OPERATOR;

// Literals
literal:
    textLiteral |
    numLiteral |
    boolLiteral;

textLiteral:
    textStringLiteral+;
    
textStringLiteral:
    value = SINGLE_QUOTED_TEXT;
    
numLiteral:
    DIGITS |
    DIGITS? DOT_SYMBOL DIGITS;
    
boolLiteral:
    TRUE_SYMBOL |
    FALSE_SYMBOL;
    
identifier:
    IDENTIFIER;