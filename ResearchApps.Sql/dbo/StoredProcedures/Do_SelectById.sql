CREATE PROCEDURE [dbo].[Do_SelectById]
@RecId int = 42046
AS
BEGIN
	SELECT a.[DoId]
      ,a.[DoDate]
      ,CONVERT(VARCHAR(11),a.[DoDate],106) as [DoDateStr]
      ,a.[Descr]
	  ,a.[CoId]
	  ,ISNULL(co.[RecId],0) as [CoRecId]
	  ,a.[RefId]
	  ,a.[Dn]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[DoStatusId]
      ,s.[DoStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
	  ,a.CustomerId
	  ,c.CustomerName
      ,c.[Address] + ' ' + c.[City] as [Address]
      ,c.[Telp]
  FROM [Do] a
  JOIN [DoStatus] s ON s.DoStatusId = a.DoStatusId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  LEFT JOIN [Co] co ON co.CoId = a.CoId
  WHERE a.RecId = @RecId
END

GO

