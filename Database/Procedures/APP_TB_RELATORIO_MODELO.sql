-- Inicio do Arquivo dbo.DATA_TB_RELATORIO_MODELO

use Program
go

--------------------------------------------------------------------------------
print '*** dbo.DATA_TB_RELATORIO_MODELO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.DATA_TB_RELATORIO_MODELO
 	@p_COD_RELATORIO_MODELO int = 0 output
,	@p_NM_MODELO varchar (100)
,	@p_DS_MODELO varchar (500)
,	@p_NM_PROCEDURE varchar (100)
,	@p_TP_ARQUIVO_PADRAO varchar (10)
,	@p_COD_CATEGORIA int = Null
,	@p_FL_ATIVO bit = Null
,	@p_QT_MAX_TENTATIVAS int = Null
,	@p_QT_TIMEOUT_MINUTOS int = Null
,	@p_DT_CRIACAO datetime2 = Null
,	@p_COD_USUARIO_CRIACAO int
,	@p_DT_ALTERACAO datetime2 = Null
,	@p_COD_USUARIO_ALTERACAO int = Null
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
			from	Report.DBO.TB_RELATORIO_MODELO	with (nolock)
			where	COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO

		) begin

			update	Report.DBO.TB_RELATORIO_MODELO
			set	NM_MODELO = upper(@p_NM_MODELO)
			,	DS_MODELO = upper(@p_DS_MODELO)
			,	NM_PROCEDURE = upper(@p_NM_PROCEDURE)
			,	TP_ARQUIVO_PADRAO = upper(@p_TP_ARQUIVO_PADRAO)
			,	COD_CATEGORIA = @p_COD_CATEGORIA
			,	FL_ATIVO = @p_FL_ATIVO
			,	QT_MAX_TENTATIVAS = @p_QT_MAX_TENTATIVAS
			,	QT_TIMEOUT_MINUTOS = @p_QT_TIMEOUT_MINUTOS
			,	COD_USUARIO_CRIACAO = @p_COD_USUARIO_CRIACAO
			,	DT_ALTERACAO = @p_DT_ALTERACAO
			,	COD_USUARIO_ALTERACAO = @p_COD_USUARIO_ALTERACAO
			where	COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO

			select @RETORNO = @@ERROR

		end else begin

			insert	Report.dbo.TB_RELATORIO_MODELO(
				NM_MODELO
			,	DS_MODELO
			,	NM_PROCEDURE
			,	TP_ARQUIVO_PADRAO
			,	COD_CATEGORIA
			,	FL_ATIVO
			,	QT_MAX_TENTATIVAS
			,	QT_TIMEOUT_MINUTOS
			,	DT_CRIACAO
			,	COD_USUARIO_CRIACAO
			,	DT_ALTERACAO
			,	COD_USUARIO_ALTERACAO
			)values(
				upper(@p_NM_MODELO)
			,	upper(@p_DS_MODELO)
			,	upper(@p_NM_PROCEDURE)
			,	upper(@p_TP_ARQUIVO_PADRAO)
			,	@p_COD_CATEGORIA
			,	@p_FL_ATIVO
			,	@p_QT_MAX_TENTATIVAS
			,	@p_QT_TIMEOUT_MINUTOS
			,	GETDATE()
			,	@p_COD_USUARIO_CRIACAO
			,	@p_DT_ALTERACAO
			,	@p_COD_USUARIO_ALTERACAO
			)

			set	@RETORNO=@@ERROR
			set	@p_COD_RELATORIO_MODELO = SCOPE_IDENTITY()

		end
	end else begin
		-- Operação de exclusão (D)
		-- Verificar se ID é obrigatório para operação D
		IF @p_COD_RELATORIO_MODELO IS NULL OR @p_COD_RELATORIO_MODELO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operação de exclusão'
			ROLLBACK TRANSACTION
			RETURN @RETORNO
		END

		if exists (

			select	1
			from	Report.DBO.TB_RELATORIO_MODELO	with (nolock)
			where	COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO

		) begin

			delete	Report.DBO.TB_RELATORIO_MODELO
			where	COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO

			set @RETORNO = @@Error

		end else begin

			set @RETORNO = 1
			SET @p_MSG_RETORNO = 'Registro não encontrado para exclusão'

		end

	end

	IF (@RETORNO = 0)
		COMMIT TRANSACTION
	ELSE
		ROLLBACK TRANSACTION
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION

	SELECT
		@ErrorMessage = ERROR_MESSAGE(),
		@ErrorSeverity = ERROR_SEVERITY(),
		@ErrorState = ERROR_STATE()

	SET @RETORNO = 1
	SET @p_MSG_RETORNO = @ErrorMessage
END CATCH

return @RETORNO
GO

-- Fim do Arquivo dbo.DATA_TB_RELATORIO_MODELO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.RULE_TB_RELATORIO_MODELO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.RULE_TB_RELATORIO_MODELO
 	@p_COD_RELATORIO_MODELO int = 0 output
,	@p_NM_MODELO varchar (100) = Null
,	@p_DS_MODELO varchar (500) = Null
,	@p_NM_PROCEDURE varchar (100) = Null
,	@p_TP_ARQUIVO_PADRAO varchar (10) = Null
,	@p_COD_CATEGORIA int = Null
,	@p_FL_ATIVO bit = Null
,	@p_QT_MAX_TENTATIVAS int = Null
,	@p_QT_TIMEOUT_MINUTOS int = Null
,	@p_COD_USUARIO_CRIACAO int = Null
,	@p_DT_ALTERACAO datetime2 = Null
,	@p_COD_USUARIO_ALTERACAO int = Null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare 	@RETORNO	int	 = 0
,	@MSG_ERR	varchar(8000)	 = ''

if (@p_COD_RELATORIO_MODELO IS NULL OR @p_COD_RELATORIO_MODELO <= 0
) and @p_OPERACAO in ('U','D')
begin
	SELECT	@MSG_ERR	 = 'Chave primária não informada'
	,	@p_MSG_RETORNO	 = @MSG_ERR
	,	@RETORNO	 = 1
end

if (@p_OPERACAO <> 'D') begin
	if (@p_NM_MODELO IS NULL OR LTRIM(RTRIM(@p_NM_MODELO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo NM_MODELO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_DS_MODELO IS NULL OR LTRIM(RTRIM(@p_DS_MODELO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo DS_MODELO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_NM_PROCEDURE IS NULL OR LTRIM(RTRIM(@p_NM_PROCEDURE)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo NM_PROCEDURE é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_TP_ARQUIVO_PADRAO IS NULL OR LTRIM(RTRIM(@p_TP_ARQUIVO_PADRAO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo TP_ARQUIVO_PADRAO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_COD_USUARIO_CRIACAO IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo COD_USUARIO_CRIACAO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
end

if (@RETORNO > 0) begin
	set @p_MSG_RETORNO = @MSG_ERR
end

--validações/regras de negócio adicionais
--Exemplo:
--if (@p_OPERACAO <> 'D' and @p_VlrMensalidade <= 0) begin
--	select @MSG_ERR = @MSG_ERR + '^O valor da mensalidade não pode ser 0 ou negativo.' + CHAR(10) + CHAR(13)
--	, @RETORNO = @RETORNO + 1
--	, @p_MSG_RETORNO = @MSG_ERR
--end

return	@RETORNO
GO
-- Fim do Arquivo dbo.RULE_TB_RELATORIO_MODELO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.APP_TB_RELATORIO_MODELO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.APP_TB_RELATORIO_MODELO
 	@p_COD_RELATORIO_MODELO int = 0 output
,	@p_NM_MODELO varchar (100) = Null
,	@p_DS_MODELO varchar (500) = Null
,	@p_NM_PROCEDURE varchar (100) = Null
,	@p_TP_ARQUIVO_PADRAO varchar (10) = Null
,	@p_COD_CATEGORIA int = Null
,	@p_FL_ATIVO bit = Null
,	@p_QT_MAX_TENTATIVAS int = Null
,	@p_QT_TIMEOUT_MINUTOS int = Null
,	@p_COD_USUARIO_CRIACAO int = Null
,	@p_DT_ALTERACAO datetime2 = Null
,	@p_COD_USUARIO_ALTERACAO int = Null
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

	-- Verificação de chave primária para operações U e D
	IF (@p_OPERACAO IN ('U', 'D'))
	BEGIN
		IF @p_COD_RELATORIO_MODELO IS NULL OR @p_COD_RELATORIO_MODELO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operações de atualização e exclusão'
			RETURN @RETORNO
		END
	END

	-- Chama a procedure de validação (RULE)
	execute @RETORNO = dbo.RULE_TB_RELATORIO_MODELO
	  @p_COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO output
	, @p_NM_MODELO = @p_NM_MODELO
	, @p_DS_MODELO = @p_DS_MODELO
	, @p_NM_PROCEDURE = @p_NM_PROCEDURE
	, @p_TP_ARQUIVO_PADRAO = @p_TP_ARQUIVO_PADRAO
	, @p_COD_CATEGORIA = @p_COD_CATEGORIA
	, @p_FL_ATIVO = @p_FL_ATIVO
	, @p_QT_MAX_TENTATIVAS = @p_QT_MAX_TENTATIVAS
	, @p_QT_TIMEOUT_MINUTOS = @p_QT_TIMEOUT_MINUTOS
	, @p_COD_USUARIO_CRIACAO = @p_COD_USUARIO_CRIACAO
	, @p_DT_ALTERACAO = @p_DT_ALTERACAO
	, @p_COD_USUARIO_ALTERACAO = @p_COD_USUARIO_ALTERACAO
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

	IF (@RETORNO != 0) BEGIN
		RETURN @RETORNO
	END

	-- Chama a procedure de dados (DATA)
	execute @RETORNO = dbo.DATA_TB_RELATORIO_MODELO
	  @p_COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO output
	, @p_NM_MODELO = @p_NM_MODELO
	, @p_DS_MODELO = @p_DS_MODELO
	, @p_NM_PROCEDURE = @p_NM_PROCEDURE
	, @p_TP_ARQUIVO_PADRAO = @p_TP_ARQUIVO_PADRAO
	, @p_COD_CATEGORIA = @p_COD_CATEGORIA
	, @p_FL_ATIVO = @p_FL_ATIVO
	, @p_QT_MAX_TENTATIVAS = @p_QT_MAX_TENTATIVAS
	, @p_QT_TIMEOUT_MINUTOS = @p_QT_TIMEOUT_MINUTOS
	, @p_COD_USUARIO_CRIACAO = @p_COD_USUARIO_CRIACAO
	, @p_DT_ALTERACAO = @p_DT_ALTERACAO
	, @p_COD_USUARIO_ALTERACAO = @p_COD_USUARIO_ALTERACAO
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

END TRY
BEGIN CATCH

	SELECT
		@ErrorMessage = ERROR_MESSAGE(),
		@ErrorSeverity = ERROR_SEVERITY(),
		@ErrorState = ERROR_STATE()

	SET @RETORNO = 1
	SET @p_MSG_RETORNO = @ErrorMessage
END CATCH

-- Se não houver erro, retorna 0
IF (@RETORNO = 0)
	RETURN 0
ELSE
	RETURN @RETORNO
go
-- Fim do Arquivo dbo.APP_TB_RELATORIO_MODELO



