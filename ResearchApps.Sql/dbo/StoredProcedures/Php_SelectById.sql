CREATE PROCEDURE [dbo].[Php_SelectById]
@RecId int = 1
AS
BEGIN
	SELECT a.[PhpId]
      ,a.[PhpDate]
      ,CONVERT(VARCHAR(11),a.[PhpDate],106) as [PhpDateStr]
      ,a.[Descr]
	  ,a.[RefId]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[PhpStatusId]
      ,s.[PhpStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Php] a
  JOIN [PhpStatus] s ON s.PhpStatusId = a.PhpStatusId
  WHERE a.RecId = @RecId
END

GO

