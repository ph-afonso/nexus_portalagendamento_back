use Program
go

--------------------------------------------------------------------------------
print '*** dbo.LST_TB_RELATORIO_CATEGORIA - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.LST_TB_RELATORIO_CATEGORIA
	@p_FILTER VARCHAR(1024) = NULL,
	@p_PAGE_NUMBER INT = 1,
	@p_PAGE_SIZE INT = 50,
	@p_TOTAL_REGISTERS INT OUTPUT,
	@p_MSG_RETORNO VARCHAR(8000) = '' OUTPUT
AS
SET NOCOUNT ON

DECLARE @RETORNO INT = 0
DECLARE @SQL NVARCHAR(4000)
DECLARE @WHERE_CLAUSE NVARCHAR(2000) = ''
DECLARE @ORDER_CLAUSE NVARCHAR(500) = ''

BEGIN TRY

	-- Construir filtro dinâmico
	IF @p_FILTER IS NOT NULL AND @p_FILTER != ''
	BEGIN
		SET @WHERE_CLAUSE = dbo.FNC_BUILD_PARAMS_FILTER(@p_FILTER)
		IF @WHERE_CLAUSE IS NULL
		BEGIN
			SET @RETORNO = 77701
			SET @p_MSG_RETORNO = 'Parâmetros de filtro inválidos'
			RETURN @RETORNO
		END
	END

	-- Construir ordenação e paginação
	SET @ORDER_CLAUSE = dbo.FNC_BUILD_OFFSET_PAGE('COD_CATEGORIA', @p_PAGE_NUMBER, @p_PAGE_SIZE)

	-- Construir query principal
	SET @SQL = 'SELECT * FROM Report.dbo.TB_RELATORIO_CATEGORIA a'

	IF @WHERE_CLAUSE != ''
		SET @SQL = @SQL + ' WHERE ' + @WHERE_CLAUSE

	SET @SQL = @SQL + @ORDER_CLAUSE

	-- Executar query principal
	EXEC sp_executesql @SQL

	-- Contar total de registros
	SET @SQL = 'SELECT @p_TOTAL_REGISTERS = COUNT(*) FROM Report.dbo.TB_RELATORIO_CATEGORIA a'

	IF @WHERE_CLAUSE != ''
		SET @SQL = @SQL + ' WHERE ' + @WHERE_CLAUSE

	EXEC sp_executesql @SQL, N'@p_TOTAL_REGISTERS INT OUTPUT', @p_TOTAL_REGISTERS OUTPUT

END TRY
BEGIN CATCH
	SET @RETORNO = 77701
	SET @p_MSG_RETORNO = ERROR_MESSAGE()
END CATCH

RETURN @RETORNO
GO
