CREATE PROCEDURE dbo.MyProc
AS 
BEGIN
DECLARE @sumInt BIGINT;
DECLARE @doubleMean FLOAT;
SELECT @sumInt = SUM(IntValue) FROM dbo.Tables;
SELECT @doubleMean = AVG(CAST(doubleValue AS FLOAT)) FROM dbo.Tables;
SELECT @sumInt AS SumInt, @doubleMean AS MeanDouble;
END;