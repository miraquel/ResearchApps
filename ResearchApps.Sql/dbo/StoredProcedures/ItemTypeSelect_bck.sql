CREATE PROCEDURE [dbo].[ItemTypeSelect_bck]
@PageNumber int = 1,
@PageSize int = 10,
@SortOrder nvarchar(max) = 'DESC',
@SortColumn nvarchar(max) = 'ItemTypeId',
@ItemTypeId nvarchar(max) = null,
@ItemTypeName nvarchar(max) = null,
@StatusId int = null,
@StatusName nvarchar(max) = null,
@CreatedDate datetime = null,
@CreatedBy nvarchar(max) = null,
@ModifiedDate datetime = null,
@ModifiedBy nvarchar(max) = null
AS
BEGIN
	SELECT a.[ItemTypeId]
		,a.[ItemTypeName]
		,a.[StatusId]
		,[StatusName] = CASE   
			WHEN a.[StatusId] = 0 THEN CONCAT('<span class="badge bg-warning">',s.[StatusName],'</span>')  
			WHEN a.[StatusId] = 1 THEN CONCAT('<span class="badge bg-success">',s.[StatusName],'</span>')  
			WHEN a.[StatusId] = 2 THEN CONCAT('<span class="badge bg-danger">',s.[StatusName],'</span>')  
			WHEN a.[StatusId] = 3 THEN CONCAT('<span class="badge bg-info">',s.[StatusName],'</span>')  
		END   
		,a.[CreatedDate]
		,a.[CreatedBy]
		,a.[ModifiedDate]
		,a.[ModifiedBy]
	FROM [ItemType] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE	1 = CASE 
				WHEN @ItemTypeId IS NULL THEN 1
				WHEN a.ItemTypeId = @ItemTypeId THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @ItemTypeName IS NULL THEN 1
				WHEN a.ItemTypeName = @ItemTypeName THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @StatusId IS NULL THEN 1
				WHEN a.StatusId = @StatusId THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @StatusName IS NULL THEN 1
				WHEN StatusName = @StatusName THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @CreatedDate IS NULL THEN 1
				WHEN CreatedDate = @CreatedDate THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @CreatedBy IS NULL THEN 1
				WHEN CreatedBy = @CreatedBy THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @ModifiedDate IS NULL THEN 1
				WHEN ModifiedDate = @ModifiedDate THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @ModifiedBy IS NULL THEN 1
				WHEN ModifiedBy = @ModifiedBy THEN 1
				ELSE 0
			END
	ORDER BY ItemTypeId DESC
	OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;

	SELECT COUNT(*)
	FROM [ItemType] a
	JOIN [Status] s ON s.StatusId = a.StatusId
	WHERE	1 = CASE 
				WHEN @ItemTypeId IS NULL THEN 1
				WHEN a.ItemTypeId = @ItemTypeId THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @ItemTypeName IS NULL THEN 1
				WHEN a.ItemTypeName = @ItemTypeName THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @StatusId IS NULL THEN 1
				WHEN a.StatusId = @StatusId THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @StatusName IS NULL THEN 1
				WHEN StatusName = @StatusName THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @CreatedDate IS NULL THEN 1
				WHEN CreatedDate = @CreatedDate THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @CreatedBy IS NULL THEN 1
				WHEN CreatedBy = @CreatedBy THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @ModifiedDate IS NULL THEN 1
				WHEN ModifiedDate = @ModifiedDate THEN 1
				ELSE 0
			END
			AND 1 = CASE 
				WHEN @ModifiedBy IS NULL THEN 1
				WHEN ModifiedBy = @ModifiedBy THEN 1
				ELSE 0
			END;
END
GO

