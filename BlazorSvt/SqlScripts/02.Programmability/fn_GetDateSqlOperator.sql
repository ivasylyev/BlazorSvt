USE [mdm]
GO



CREATE OR ALTER FUNCTION dbo.fn_GetDateSqlOperator (@OperatorName NVARCHAR(50))
RETURNS NVARCHAR(2)
AS
BEGIN
    RETURN CASE @OperatorName
        WHEN '1' THEN '=' -- Equals
        WHEN '2' THEN '<>' -- NotEquals
        WHEN '3' THEN '<' --LessThan
        WHEN '4' THEN '<=' -- LessThanOrEquals
        WHEN '5' THEN '>' --GreaterThan
        WHEN '6' THEN '>=' --GreaterThanOrEquals
        ELSE NULL -- Или можно вернуть пустую строку '', если оператор не найден
    END
END
GO