CREATE PROCEDURE [dbo].[WfForm_Select]
	@PageNumber int = 1,
	@PageSize int = 10,
	@SortOrder nvarchar(max) = 'ASC',
	@SortColumn nvarchar(max) = 'WfFormId',
	@WfFormId nvarchar(max) = null,
	@FormName nvarchar(max) = null,
	@Initial nvarchar(max) = null
AS
BEGIN
    SET NOCOUNT ON;
		
    SELECT
        a.[WfFormId],
        a.[FormName],
        a.[Initial]
    INTO #FilteredData
	FROM [WfForm] a
	WHERE (@WfFormId IS NULL OR CAST(a.WfFormId AS nvarchar) LIKE '%' + @WfFormId + '%')
      AND (@FormName IS NULL OR a.FormName LIKE '%' + @FormName + '%')
      AND (@Initial IS NULL OR a.Initial LIKE '%' + @Initial + '%')

    SELECT
        [WfFormId],
        [FormName],
        [Initial]
    FROM #FilteredData
    ORDER BY
        CASE WHEN @SortColumn = 'WfFormId' AND @SortOrder = 'ASC' THEN [WfFormId] END ASC,
        CASE WHEN @SortColumn = 'WfFormId' AND @SortOrder = 'DESC' THEN [WfFormId] END DESC,
        CASE WHEN @SortColumn = 'FormName' AND @SortOrder = 'ASC' THEN [FormName] END ASC,
        CASE WHEN @SortColumn = 'FormName' AND @SortOrder = 'DESC' THEN [FormName] END DESC,
        CASE WHEN @SortColumn = 'Initial' AND @SortOrder = 'ASC' THEN [Initial] END ASC,
        CASE WHEN @SortColumn = 'Initial' AND @SortOrder = 'DESC' THEN [Initial] END DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
        FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(*) AS TotalRecords
    FROM #FilteredData;
END
GO

