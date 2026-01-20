CREATE PROCEDURE [dbo].[Supplier_Update]
@SupplierId int,
@SupplierName nvarchar(200),
@Address nvarchar(200),
@City nvarchar(50),
@Telp nvarchar(100), 
@Fax nvarchar(100) = '',
@Email nvarchar(100) = '',
@TopId int, 
@IsPpn bit,
@Npwp nvarchar(20) = '',  
@Notes nvarchar(100) = '',  
@StatusId int,
@ModifiedBy  nvarchar(20)
AS
BEGIN

	--* Cek ada nama yg sama *--
	IF EXISTS ( 
			SELECT SupplierId FROM [Supplier]
			WHERE [SupplierName] = @SupplierName AND SupplierId <> @SupplierId
		)
	BEGIN
		SELECT -1 as SupplierId
		RETURN
	END
		
	--* Proses Update *--
	UPDATE [Supplier]
	SET SupplierName = @SupplierName
		, [Address] = @Address
		, [City] = @City
		, [Telp] = @Telp
		, [Fax] = @Fax
		, [Email] = @Email
		, [TopId] = @TopId		
		, [IsPpn] = @IsPpn
		, [Npwp] = @Npwp
		, [Notes] = @Notes
		, StatusId = @StatusId
		, ModifiedBy = @ModifiedBy
		, ModifiedDate = GETDATE()
	WHERE [SupplierId] = @SupplierId
	
	SELECT @SupplierId as SupplierId
END

GO

