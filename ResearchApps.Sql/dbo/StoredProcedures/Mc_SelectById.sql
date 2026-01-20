CREATE PROCEDURE [dbo].[Mc_SelectById]
@RecId int = 54
AS
BEGIN
	SELECT a.[McId]
      ,a.[McDate]
      ,CONVERT(VARCHAR(11),a.[McDate],106) as [McDateStr]
      ,a.[CustomerId]
      ,c.[CustomerName]
	  ,a.[SjNo]
      ,a.[RefNo]
      ,a.[Notes]
      ,a.[McStatusId]
      ,s.[McStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Mc] a
  JOIN [McStatus] s ON s.McStatusId = a.McStatusId
  JOIN [Customer] c ON c.CustomerId = a.CustomerId
  WHERE a.RecId = @RecId
END

GO

