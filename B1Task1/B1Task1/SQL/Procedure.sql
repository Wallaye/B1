CREATE PROCEDURE dbo.MyProc
AS 
BEGIN
DECLARE @sumInt BIGINT;
DECLARE @doubleMean FLOAT;
DECLARE @count BIGINT;
SELECT @count = COUNT_BIG(*) FROM dbo.Tables;
SELECT @sumInt =  SUM(CAST(IntValue AS BIGINT)) FROM dbo.Tables;
SELECT @doubleMean = AVG(CAST(doubleValue AS FLOAT)) FROM dbo.Tables;
SELECT @sumInt AS SumInt, @doubleMean AS MeanDouble;
END;