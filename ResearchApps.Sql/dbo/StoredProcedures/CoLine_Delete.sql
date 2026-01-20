CREATE PROCEDURE [dbo].[CoLine_Delete]
/****** PPN 11% ******/
@CoLineId int,
@ModifiedBy nvarchar(20) = 'system'
AS
BEGIN
	DECLARE @IsPpn bit, @CoId nvarchar(20), @ItemName nvarchar(100), @UnitId int
	DECLARE @SubTotal decimal(18,2), @Ppn decimal(18,2), @Total decimal(18,2)

SELECT @CoId = CoId FROM CoLine WHERE CoLineId = @CoLineId
SELECT @IsPpn = IsPpn FROM Co WHERE CoId = @CoId

--* Co Line *--
DELETE FROM [CoLine]
WHERE CoLineId = @CoLineId

    --* Co Header *--
    IF @IsPpn = 1
BEGIN
UPDATE [CoLine]
SET Ppn = Price * 0.11
WHERE CoId = @CoId

SELECT @SubTotal = SUM(Qty * Price)
     , @Ppn = SUM(Qty * Price * 0.11)
     , @Total = SUM(Qty * Price * 1.11)
FROM CoLine
WHERE CoId = @CoId

UPDATE [Co]
SET SubTotal = isnull(@SubTotal,0)
        ,Ppn = isnull(@Ppn,0)
        ,Total = isnull(@Total,0)
WHERE CoId = @CoId
END
ELSE
BEGIN
UPDATE [CoLine]
SET Ppn = 0
WHERE CoId = @CoId

SELECT @SubTotal = SUM(Qty * Price)
     , @Ppn = 0
     , @Total = SUM(Qty * Price)
FROM CoLine
WHERE CoId = @CoId

UPDATE [Co]
SET SubTotal = @SubTotal
        ,Ppn = @Ppn
        ,Total = @Total
WHERE CoId = @CoId
END

SELECT @CoId
END

GO

