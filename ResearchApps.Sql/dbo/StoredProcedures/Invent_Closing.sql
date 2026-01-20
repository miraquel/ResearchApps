CREATE PROCEDURE [dbo].[Invent_Closing]
@Year int = 2022,
@Month int = 9,
@CreatedBy nvarchar(20) = ''
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @ItemId int, @CostPrice decimal(18,2)	
	DECLARE @YearNext int, @MonthNext int

	DECLARE @StockTrans table (
		ItemId int
		, TransDate datetime
		, RefType nvarchar(20)
		, RefNo nvarchar(20)
		, Qty decimal(18,2)
		, Price decimal(18,2)
		, CostPrice decimal(18,2)
		, Value decimal(18,2)
	)

	declare @numerator decimal(16,2)
	declare @denumerator decimal(16,2)
	--DELETE FROM ItemTemp

	DECLARE db_cursor CURSOR FOR  
	SELECT DISTINCT ItemId
		FROM InventTrans
		WHERE YEAR(TransDate) = @Year
			AND MONTH(TransDate) = @Month
			--AND ItemId = 12486
		UNION
		SELECT DISTINCT ItemId
		FROM ItemBegin
		WHERE [Year] = @Year
			AND [Month] = @Month
			--AND ItemId = 12486

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @ItemId

	WHILE @@FETCH_STATUS = 0  
	BEGIN  

		--* Saldo Awal  *--		
		INSERT INTO @StockTrans
		(ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value)
		SELECT ItemId, cast(cast([Year] as varchar) + '-' + cast([Month] as varchar) + '-01' as date), 'Saldo Awal', 'Saldo Awal'
			, QtyBegin, CostPrice, 0, ValueBegin
		FROM ItemBegin
		WHERE [Year] = @Year
			AND [Month] = @Month
			AND ItemId = @ItemId
			AND QtyBegin <> 0
		
		--* PS  *--
		INSERT INTO @StockTrans
		(ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value)
		SELECT b.ItemId, a.PsDate, 'Penyesuaian Stock', a.PsId, b.Qty, i.CostPrice, 0,(b.Qty * i.CostPrice)
		FROM Ps a
		JOIN PsLine b ON b.PsId = a.PsId
		JOIN Item i ON i.ItemId = b.ItemId
		WHERE YEAR(a.PsDate) = @Year
			AND MONTH(a.PsDate) = @Month
			AND b.ItemId = @ItemId
			AND b.Qty > 0

		--* GR  *--
		INSERT INTO @StockTrans
		(ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value)
		SELECT b.ItemId, a.GrDate, 'Goods Receipt', a.GrId, b.Qty, b.Price, 0, (b.Qty * b.Price)
		FROM Gr a
		JOIN GrLine b ON b.GrId = a.GrId
		WHERE YEAR(a.GrDate) = @Year
			AND MONTH(a.GrDate) = @Month
			AND b.ItemId = @ItemId

		--** GR yg harganya nol *--
		UPDATE @StockTrans
		SET Price = i.CostPrice,
			Value = Qty * i.CostPrice
		FROM @StockTrans a
			JOIN Item i ON i.ItemId = a.ItemId
		WHERE RefType = 'Goods Receipt' AND Price = 0

		--* Harga Rata Tertimbang *--
		SET @CostPrice = 0

		SELECT @numerator = SUM(Qty*Price) 
		FROM @StockTrans
		WHERE ItemId = @ItemId

		SELECT @denumerator = SUM(Qty)  
		FROM @StockTrans
		WHERE ItemId = @ItemId

		--INSERT INTO ItemTemp (ItemId,numerator,denumerator)  SELECT @ItemId,@numerator,@denumerator;

		IF @denumerator<>0
			SELECT @CostPrice = isnull( ( SUM(Qty*Price) / SUM(Qty) ) ,0)
			FROM @StockTrans
			WHERE ItemId = @ItemId

		IF @CostPrice = 0
			SELECT @CostPrice = CostPrice
			FROM Item
			WHERE ItemId = @ItemId

		--* Update InventTrans *--
		UPDATE InventTrans
		SET CostPrice = @CostPrice, Value = Qty * @CostPrice
		WHERE YEAR(TransDate) = @Year
			AND MONTH(TransDate) = @Month
			AND ItemId = @ItemId
				
		--* Update InventSum *--
		UPDATE InventSum
		SET CostPrice = @CostPrice, Value = Qty * @CostPrice
		WHERE ItemId = @ItemId

		--* @StockTrans *--
		UPDATE @StockTrans
		SET CostPrice = @CostPrice, Value = Qty * @CostPrice
		WHERE YEAR(TransDate) = @Year
			AND MONTH(TransDate) = @Month
			AND ItemId = @ItemId

		--* Bpb  *--
		INSERT INTO @StockTrans
		(ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value)
		SELECT b.ItemId, a.BpbDate, 'Pengambilan Barang', a.BpbId, -1*b.Qty, 0, @CostPrice, (-1*b.Qty * @CostPrice)
		FROM Bpb a
		JOIN BpbLine b ON b.BpbId = a.BpbId
		WHERE YEAR(a.BpbDate) = @Year
			AND MONTH(a.BpbDate) = @Month
			AND b.ItemId = @ItemId
		
		--* Ps minus  *--
		INSERT INTO @StockTrans
		(ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value)
		SELECT b.ItemId, a.PsDate, 'Penyesuaian Stock', a.PsId, b.Qty, 0, @CostPrice, (b.Qty * @CostPrice)
		FROM Ps a
		JOIN PsLine b ON b.PsId = a.PsId
		WHERE YEAR(a.PsDate) = @Year
			AND MONTH(a.PsDate) = @Month
			AND b.ItemId = @ItemId
			AND b.Qty < 0

		--* MC  *--
		INSERT INTO @StockTrans
		(ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value)
		SELECT b.ItemId, a.McDate, 'Material Customer', a.McId, b.Qty, 0, @CostPrice, (b.Qty * @CostPrice)
		FROM Mc a
		JOIN McLine b ON b.McId = a.McId
		WHERE YEAR(a.McDate) = @Year
			AND MONTH(a.McDate) = @Month
			AND b.ItemId = @ItemId

		--* PHP  *--
		INSERT INTO @StockTrans
		(ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value)
		SELECT b.ItemId, a.PhpDate, 'Hasil Produksi', a.PhpId, b.Qty, 0, @CostPrice, (b.Qty * @CostPrice)
		FROM Php a
		JOIN PhpLine b ON b.PhpId = a.PhpId
		WHERE YEAR(a.PhpDate) = @Year
			AND MONTH(a.PhpDate) = @Month
			AND b.ItemId = @ItemId

		FETCH NEXT FROM db_cursor INTO @ItemId
	END  

	CLOSE db_cursor  
	DEALLOCATE db_cursor 

	--select * from @StockTrans 
	--return
	
	--* Simpan menjadi saldo awal bulan berikutnya *--
	IF @Month =  12
	BEGIN
		SET @YearNext = @Year + 1
		SET @MonthNext = 1
	END
	ELSE 
	BEGIN
		SET @YearNext = @Year
		SET @MonthNext = @Month + 1
	END

	DELETE FROM ItemBegin
	WHERE [Year] = @YearNext AND [Month] = @MonthNext

	INSERT INTO ItemBegin ([Year],[Month],[ItemId],[QtyBegin],[CostPrice],[ValueBegin])
	SELECT @YearNext, @MonthNext, ItemId, SUM(Qty), max(CostPrice), SUM(Value)
	FROM @StockTrans 
	GROUP BY ItemId
		
	--* Simpan ke ItemTrans *--
	DELETE FROM ItemTrans
	WHERE YEAR(TransDate) = @Year AND MONTH(TransDate) = @Month

	INSERT INTO ItemTrans ([Year],[Month],[ItemId],[TransDate],[RefType],[RefNo],[Qty],[Price],[CostPrice],[Value],[CreatedDate],[CreatedBy])
	SELECT @Year, @Month, ItemId, TransDate, RefType, RefNo, Qty, Price, CostPrice, Value, GETDATE(), @CreatedBy
	FROM @StockTrans 
	ORDER BY ItemId

	SELECT '1:::'

	--SELECT * FROM @StockTrans WHERE ItemId = 2194 ORDER BY ItemId
	--SELECT * FROM ItemBegin WHERE ItemId = 2194
	--SELECT * FROM InventTrans WHERE ItemId = 2194
	--SELECT * FROM InventSum WHERE ItemId = 2194
	--SELECT * FROM ItemTrans WHERE ItemId = 2194
END

GO

