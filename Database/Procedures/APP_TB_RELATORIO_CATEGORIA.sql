-- Inicio do Arquivo dbo.DATA_TB_RELATORIO_CATEGORIA

use Program
go

--------------------------------------------------------------------------------
print '*** dbo.DATA_TB_RELATORIO_CATEGORIA - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.DATA_TB_RELATORIO_CATEGORIA
 	@p_COD_CATEGORIA int = 0 output
,	@p_NM_CATEGORIA varchar (100)
,	@p_DS_CATEGORIA varchar (300)
,	@p_FL_ATIVO bit
,	@p_NR_ORDEM int
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare @RETORNO int = 0
DECLARE @ErrorMessage NVARCHAR(4000)
DECLARE @ErrorSeverity INT
DECLARE @ErrorState INT

BEGIN TRY
	BEGIN TRANSACTION

	if (isnull(@p_OPERACAO, 'X') != 'D') begin
		if exists (
			select	1
			from	Report.DBO.TB_RELATORIO_CATEGORIA	with (nolock)
			where	COD_CATEGORIA = @p_COD_CATEGORIA
		) begin
			update	Report.DBO.TB_RELATORIO_CATEGORIA
			set	NM_CATEGORIA = @p_NM_CATEGORIA
			,	DS_CATEGORIA = @p_DS_CATEGORIA
			,	FL_ATIVO = @p_FL_ATIVO
			,	NR_ORDEM = @p_NR_ORDEM
			where	COD_CATEGORIA = @p_COD_CATEGORIA
			select @RETORNO = @@ERROR
		end else begin
			insert	Report.dbo.TB_RELATORIO_CATEGORIA(
				NM_CATEGORIA
			,	DS_CATEGORIA
			,	FL_ATIVO
			,	NR_ORDEM
			)values(
				@p_NM_CATEGORIA
			,	@p_DS_CATEGORIA
			,	@p_FL_ATIVO
			,	@p_NR_ORDEM
			)
			set	@RETORNO=@@ERROR
			set	@p_COD_CATEGORIA = SCOPE_IDENTITY()
		end
	end else begin
		-- Operação de exclusão (D)
		delete	Report.DBO.TB_RELATORIO_CATEGORIA
		where	COD_CATEGORIA = @p_COD_CATEGORIA
		set @RETORNO = @@Error
	end
	IF (@RETORNO = 0)
		COMMIT TRANSACTION
	ELSE
		ROLLBACK TRANSACTION
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION
	SET @RETORNO = 1
	SET @p_MSG_RETORNO = ERROR_MESSAGE()
END CATCH

return @RETORNO
GO


use Program
go

--------------------------------------------------------------------------------
print '*** dbo.RULE_TB_RELATORIO_CATEGORIA - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.RULE_TB_RELATORIO_CATEGORIA
 	@p_COD_CATEGORIA int = 0 output
,	@p_NM_CATEGORIA varchar (100) = Null
,	@p_DS_CATEGORIA varchar (300) = Null
,	@p_FL_ATIVO bit = Null
,	@p_NR_ORDEM int = Null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare	@RETORNO	int	 = 0
,	@MSG_ERR	varchar(8000)	 = ''

-- Validações básicas
-- Adicione suas validações específicas aqui

if (@RETORNO > 0) begin
	set @p_MSG_RETORNO = @MSG_ERR
end

return	@RETORNO
GO


use Program
go

--------------------------------------------------------------------------------
print '*** dbo.APP_TB_RELATORIO_CATEGORIA - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.APP_TB_RELATORIO_CATEGORIA
 	@p_COD_CATEGORIA int = 0 output
,	@p_NM_CATEGORIA varchar (100) = Null
,	@p_DS_CATEGORIA varchar (300) = Null
,	@p_FL_ATIVO bit = Null
,	@p_NR_ORDEM int = Null
,	@p_OPERACAO char (1) = Null
,	@p_COD_USUARIOS int = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare	@RETORNO	int		 = 0
DECLARE @ErrorMessage NVARCHAR(4000)
DECLARE @ErrorSeverity INT
DECLARE @ErrorState INT

BEGIN TRY

	-- Chama a procedure de validação (RULE)
	execute @RETORNO = dbo.RULE_TB_RELATORIO_CATEGORIA
	  @p_COD_CATEGORIA = @p_COD_CATEGORIA output
	, @p_NM_CATEGORIA = @p_NM_CATEGORIA
	, @p_DS_CATEGORIA = @p_DS_CATEGORIA
	, @p_FL_ATIVO = @p_FL_ATIVO
	, @p_NR_ORDEM = @p_NR_ORDEM
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

	IF (@RETORNO != 0) BEGIN
		RETURN @RETORNO
	END

	-- Chama a procedure de dados (DATA)
	execute @RETORNO = dbo.DATA_TB_RELATORIO_CATEGORIA
	  @p_COD_CATEGORIA = @p_COD_CATEGORIA output
	, @p_NM_CATEGORIA = @p_NM_CATEGORIA
	, @p_DS_CATEGORIA = @p_DS_CATEGORIA
	, @p_FL_ATIVO = @p_FL_ATIVO
	, @p_NR_ORDEM = @p_NR_ORDEM
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

END TRY
BEGIN CATCH
	SET @RETORNO = 1
	SET @p_MSG_RETORNO = ERROR_MESSAGE()
END CATCH

IF (@RETORNO = 0)
	RETURN 0
ELSE
	RETURN @RETORNO
go

