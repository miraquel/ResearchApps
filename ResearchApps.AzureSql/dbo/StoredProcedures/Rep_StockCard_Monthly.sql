CREATE PROCEDURE [dbo].[Rep_StockCard_Monthly]
@ItemId int = 1001,
@Year int = 2026,
@Month int = 1
AS
BEGIN
	DECLARE @Result table (
		[No] int, ItemId int, ItemName nvarchar(100), UnitName nvarchar(20)
		,RefType nvarchar(20), RefNo nvarchar(20), Descr nvarchar(100)
		,TransDate datetime
		,QtyStockIn numeric(32,16), QtyStockOut numeric(32,16), QtySaldo numeric(32,16)
	)

	DECLARE @No int, @ItemName nvarchar(100), @UnitName nvarchar(100), @OnHand numeric(32,16), @TransDate datetime 
		, @Qty numeric(32,16), @CostPrice numeric(32,16), @RefType nvarchar(20), @RefNo nvarchar(20)
		, @QtyStockIn numeric(32,16), @QtyStockOut numeric(32,16), @QtySaldo numeric(32,16)
		, @QtyStockInBefore numeric(32,16), @QtyStockOutBefore numeric(32,16), @QtySaldoBefore numeric(32,16)
		, @Descr nvarchar(100)

	SET @No = 1

	DECLARE db_cursor CURSOR FOR 
	SELECT a.ItemId
		, a.ItemName
		, i.UnitName
		, ISNULL(s.Qty,0) as OnHand
		, t.TransDate
		, t.Qty
		, t.CostPrice
		, t.RefType
		, t.RefNo
		, ISNULL(ps.Descr,'') + ISNULL(bp.Descr,'') + ISNULL(gr.Descr,'') as Descr
	FROM Item a
	JOIN Unit i ON i.UnitId = a.UnitId
	LEFT JOIN InventSum s ON s.ItemId = a.ItemId AND s.WhId = a.WhId
	LEFT JOIN InventTrans t ON t.ItemId = a.ItemId AND t.WhId = a.WhId
	LEFT JOIN 
	(
		SELECT a.BpbLineId, b.ItemId, c.ItemName + ' ' + a.ProdId as Descr
		FROM BpbLine a
		JOIN Prod b ON b.ProdId = a.ProdId
		JOIN Item c ON c.ItemId = b.ItemId
	) bp ON bp.BpbLineId = t.RefId AND t.RefType = 'Pengambilan Barang'
	LEFT JOIN 
	(		
		SELECT a.PsLineId, b.Descr
		FROM PsLine a
		JOIN Ps b ON b.PsId = a.PsId
	) ps ON ps.PsLineId = t.RefId AND t.RefType = 'Penyesuaian Stock'
	LEFT JOIN 
	(	
		SELECT a.GrLineId, c.SupplierName as Descr
		FROM GrLine a
		JOIN Po b ON b.PoId = a.PoId
		JOIN Supplier c ON c.SupplierId = b.SupplierId
	) gr ON gr.GrLineId = t.RefId AND t.RefType = 'Goods Receipt'
	WHERE a.ItemId = @ItemId AND YEAR(t.TransDate) >= @Year AND MONTH(t.TransDate) >= @Month
	ORDER BY t.TransDate desc, t.RecId desc

	OPEN db_cursor 
	FETCH NEXT FROM db_cursor INTO @ItemId, @ItemName, @UnitName, @OnHand, @TransDate, @Qty, @CostPrice, @RefType, @RefNo, @Descr

	WHILE @@FETCH_STATUS = 0 
	BEGIN 
		--* Initialisasi *--
		IF @RefType = 'Goods Receipt' OR @RefType = 'Hasil Produksi' OR @RefType = 'Penyesuaian Stock' OR @RefType = 'Material Customer' 
		BEGIN
			IF @RefNo = 'PS180001' -- QtySaldo awal pada saat golive
			BEGIN
				SET @QtyStockIn = 0
				SET @QtyStockOut = 0
				SET @RefNo = 'Saldo Awal'
			END
			ELSE
			BEGIN
				SET @QtyStockIn = @Qty
				SET @QtyStockOut = 0
			END
		END 
		ELSE
		BEGIN
			SET @QtyStockIn = 0 
			SET @QtyStockOut = -1*isnull(@Qty,0)
		END

		IF @No=1
			SET @QtySaldo = @OnHand
		ELSE
			SET @QtySaldo = @QtySaldoBefore + @QtyStockOutBefore - @QtyStockInBefore

		--* Hasil *--
		INSERT INTO @Result
		([No], ItemId, ItemName, UnitName
			,RefType, RefNo
			,TransDate, QtyStockIn, QtyStockOut, QtySaldo)
		SELECT @No, @ItemId, @ItemName, @UnitName
		, @RefType, @RefNo
		, @TransDate, @QtyStockIn, @QtyStockOut, @QtySaldo

		SET @No += 1

		--* Before, utk kperluan perhitungan QtySaldo *--		
		SET @QtyStockInBefore = @QtyStockIn
		SET @QtyStockOutBefore = @QtyStockOut
		SET @QtySaldoBefore = @QtySaldo

		FETCH NEXT FROM db_cursor INTO @ItemId, @ItemName, @UnitName, @OnHand, @TransDate, @Qty, @CostPrice, @RefType, @RefNo, @Descr
	END 

	CLOSE db_cursor 
	DEALLOCATE db_cursor 
	
	-- Untuk transaksi bulan kedua setelah golive	
	IF @RefNo <> 'Saldo Awal'
	BEGIN
		SET @QtySaldo = @QtySaldoBefore + @QtyStockOutBefore - @QtyStockInBefore

		INSERT INTO @Result
		([No], ItemId, ItemName, UnitName
			, RefType, RefNo
			, TransDate, QtyStockIn, QtyStockOut, QtySaldo)
		SELECT @No, @ItemId, @ItemName, @UnitName
			, 'Saldo Awal', 'Saldo Awal'
			, CAST(CAST(@Year as varchar) + '-' + CAST(@Month as varchar) + '-01' as datetime)
			, 0, 0, @QtySaldo
	END	

	SELECT [No], ItemId, ItemName, UnitName	
		, RefType, RefNo
		, TransDate
		, CONVERT(varchar,TransDate,106) as TransDateStr
		, QtyStockIn
		, QtyStockOut
		, QtySaldo
	FROM @Result 
	WHERE YEAR(TransDate) = @Year AND MONTH(TransDate) = @Month
	ORDER BY [No] desc

END
GO

