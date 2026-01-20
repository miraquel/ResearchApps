CREATE PROCEDURE [dbo].[Ps_SelectById]
@RecId int = 1
AS
BEGIN
	SELECT a.[PsId]
      ,a.[PsDate]
      ,a.[Descr]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[PsStatusId]
      ,s.[PsStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Ps] a
  JOIN [PsStatus] s ON s.PsStatusId = a.PsStatusId
  WHERE a.RecId = @RecId
END

GO

