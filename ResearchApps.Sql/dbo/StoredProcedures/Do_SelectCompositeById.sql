CREATE PROCEDURE [dbo].[Do_SelectCompositeById]
    @RecId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @DoId NVARCHAR(20);
    DECLARE @CustomerId INT;
    
    -- Get DoId and CustomerId for later queries
    SELECT @DoId = DoId, @CustomerId = CustomerId 
    FROM Do 
    WHERE RecId = @RecId;
    
    -- Result Set 1: Header
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
    WHERE a.RecId = @RecId;
    
    -- Result Set 2: Lines
    SELECT a.[DoLineId]
         ,a.[DoId]
         ,a.[CoId]
         ,a.[CoLineId]
         ,i.[ItemId]
         ,i.[ItemName]
         ,a.[WhId]
         ,w.[WhName]
         ,a.[Qty]
         ,u.[UnitName]
         ,a.[Price]
         ,a.CustomerId
         ,c.[CustomerName]
         ,a.[Notes]
         ,h.DoStatusId
    FROM [DoLine] a
        JOIN [Do] h on h.DoId = a.DoId
        JOIN [Item] i ON i.ItemId = a.ItemId
        JOIN [Wh] w ON w.WhId = a.WhId
        JOIN [Unit] u ON u.UnitId = i.UnitId
        JOIN Customer c ON c.CustomerId = a.CustomerId
    WHERE a.DoId = @DoId
    ORDER BY a.DoLineId;
    
    -- Result Set 3: Outstanding
    SELECT  a.RecId as DoRecId
         , b.DoLineId
         , b.DoId
         , a.CustomerId
         , s.CustomerName
         , a.CoId
         , a.RefId as PoCustomer
         , b.ItemId
         , i.ItemName
         , i.UnitId
         , u.UnitName
         , b.Qty as QtyDo
         , ISNULL(c.QtySi,0) as QtySi
         , b.Qty - ISNULL(c.QtySi,0) as QtyOs
    FROM Do a
             JOIN DoLine b ON b.DoId = a.DoId
             JOIN Item i ON i.ItemId = b.ItemId
             JOIN Customer s ON s.CustomerId = a.CustomerId
             JOIN Unit u ON u.UnitId = i.UnitId
             LEFT JOIN
         (
             SELECT b.DoLineId, SUM(Qty) as QtySi
             FROM Si a
                      JOIN SiLine b ON b.SiId = a.SiId
             WHERE a.SiStatusId <> 3
             GROUP BY b.DoLineId
         ) c ON c.DoLineId = b.DoLineId
    WHERE a.DoStatusId = 1
      AND a.CustomerId = @CustomerId
      AND b.Qty - ISNULL(c.QtySi,0) > 0
    ORDER BY a.DoDate desc, b.DoId desc;
END

GO

