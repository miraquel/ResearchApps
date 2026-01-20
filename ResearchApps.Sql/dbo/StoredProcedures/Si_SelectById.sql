CREATE PROCEDURE [dbo].[Si_SelectById]
@RecId int = 1
AS
BEGIN
	SELECT a.[SiId]
      ,a.[SiDate]
      ,CONVERT(VARCHAR(11),a.[SiDate],106) as [SiDateStr]
	  ,a.CustomerId
	  ,c.CustomerName
      ,c.[Address] + ' ' + c.[City] as [Address]
      ,c.[Telp]
      ,a.[PoNo]
	  ,a.[TaxNo]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[SiStatusId]
      ,s.[SiStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Si] a
  JOIN [SiStatus] s ON s.SiStatusId = a.SiStatusId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  WHERE a.RecId = @RecId
END

GO

