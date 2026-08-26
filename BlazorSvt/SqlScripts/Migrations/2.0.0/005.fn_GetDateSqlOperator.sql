USE [mdm]
GO



CREATE OR ALTER FUNCTION v2.fn_GetDateSqlOperator (@OperatorName NVARCHAR(50))
RETURNS NVARCHAR(2)
AS
BEGIN
    RETURN CASE UPPER(@OperatorName)
        WHEN 'EQUALS' THEN '='
        WHEN '1' THEN '='
        WHEN 'NOTEQUALS' THEN '<>'
        WHEN '2' THEN '<>'
        WHEN 'LESSTHAN' THEN '<'
        WHEN '3' THEN '<'
        WHEN 'LESSTHANOREQUALS' THEN '<='
        WHEN '4' THEN '<='
        WHEN 'GREATERTHAN' THEN '>'
        WHEN '5' THEN '>'
        WHEN 'GREATERTHANOREQUALS' THEN '>='
        WHEN '6' THEN '>='
        ELSE NULL
    END
END
GO