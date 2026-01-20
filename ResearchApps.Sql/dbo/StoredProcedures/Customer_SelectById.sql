CREATE PROCEDURE [dbo].[Customer_SelectById]
@CustomerId int = 1
AS
BEGIN
	SELECT a.[CustomerId]
      ,a.[CustomerName]
      ,a.[Address]
      ,a.[City]
      ,a.[Telp]
      ,a.[Fax]
      ,a.[Email]
      ,a.[TopId]
      ,t.[TopName]
	  ,a.[IsPpn]
      ,a.[Npwp]
      ,a.[Notes]
      ,a.[StatusId]
      ,s.[StatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
  FROM [Customer] a
  JOIN [Top] t ON t.TopId = a.TopId
  JOIN [Status] s ON s.StatusId = a.StatusId
  WHERE a.CustomerId = @CustomerId
END

GO

