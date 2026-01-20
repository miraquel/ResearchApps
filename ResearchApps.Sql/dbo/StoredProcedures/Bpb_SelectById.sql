CREATE PROCEDURE [dbo].[Bpb_SelectById]
@RecId int = 15
AS
BEGIN
	SELECT a.[BpbId]
      ,a.[BpbDate]
      ,CONVERT(VARCHAR(11),a.[BpbDate],106) as [BpbDateStr]
      ,a.[Descr]
      ,a.[RefType]
      ,a.[RefId]
      ,a.[Amount]
      ,a.[Notes]
      ,a.[BpbStatusId]
      ,s.[BpbStatusName]
      ,a.[CreatedDate]
      ,a.[CreatedBy]
      ,a.[ModifiedDate]
      ,a.[ModifiedBy]
      ,a.[RecId]
  FROM [Bpb] a
  JOIN [BpbStatus] s ON s.BpbStatusId = a.BpbStatusId
  WHERE a.RecId = @RecId
END

GO

