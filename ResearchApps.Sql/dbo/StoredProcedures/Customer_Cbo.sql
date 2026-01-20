CREATE PROCEDURE [dbo].[Customer_Cbo]
    @Id INT = NULL,
    @Term NVARCHAR(MAX) = NULL
AS
BEGIN
    SELECT a.[CustomerId]
         ,a.[CustomerName] + ' '+ a.[Address] as [CustomerName]
         ,a.City
    FROM [Customer] a
    WHERE a.StatusId = 1
      AND (@Id IS NULL OR a.CustomerId = @Id)
      AND (@Term IS NULL OR a.CustomerName LIKE '%' + @Term + '%' OR a.Address LIKE '%' + @Term + '%')
    ORDER BY a.[CustomerName] + ' '+ a.[Address]
END

GO

