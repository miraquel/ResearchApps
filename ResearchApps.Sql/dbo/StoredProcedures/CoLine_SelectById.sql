CREATE PROCEDURE [dbo].[CoLine_SelectById]
@CoLineId int =1
AS
BEGIN
SELECT p.RecId
     ,a.[CoLineId]
     ,a.[ItemId]
     ,i.[ItemName]
     ,a.[WantedDeliveryDate]
     ,a.[Qty]
     ,a.[UnitId]
     ,u.[UnitName]
     ,a.[Price]
     ,a.[Ppn]
     ,a.[Notes]
     ,a.[CreatedDate]
     ,a.[CreatedBy]
     ,a.[ModifiedDate]
     ,a.[ModifiedBy]
FROM [CoLine] a
    JOIN [Item] i ON i.ItemId = a.ItemId
    JOIN [Unit] u ON u.UnitId = i.UnitId
    JOIN [Co] p ON p.CoId = a.CoId
WHERE a.CoLineId = @CoLineId
END

GO

