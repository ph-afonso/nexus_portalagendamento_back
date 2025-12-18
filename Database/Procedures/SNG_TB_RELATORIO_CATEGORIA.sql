use Program
go

--------------------------------------------------------------------------------
print '*** dbo.SNG_TB_RELATORIO_CATEGORIA - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE [dbo.SNG_TB_RELATORIO_CATEGORIA]
	@p_COD_CATEGORIA int,
	@p_MSG_RETORNO VARCHAR(8000) = '' OUTPUT
AS
SET NOCOUNT ON

DECLARE @RETORNO INT = 0

BEGIN TRY

	SELECT *
	FROM Report.dbo.TB_RELATORIO_CATEGORIA a
	WHERE a.COD_CATEGORIA = @p_COD_CATEGORIA

END TRY
BEGIN CATCH
	SET @RETORNO = 50000
	SET @p_MSG_RETORNO = ERROR_MESSAGE()
END CATCH

RETURN @RETORNO
GO
