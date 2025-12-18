-- Inicio do Arquivo dbo.DATA_TB_RELATORIO_SOLICITACAO

use Program
go

--------------------------------------------------------------------------------
print '*** dbo.DATA_TB_RELATORIO_SOLICITACAO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.DATA_TB_RELATORIO_SOLICITACAO
 	@p_COD_RELATORIO_SOLICITACAO bigint = 0 output
,	@p_COD_RELATORIO_MODELO int = Null
,	@p_COD_USUARIO int
,	@p_DS_PARAMETROS nvarchar (MAX) = Null
,	@p_COD_STATUS_PROCESSAMENTO int
,	@p_DT_CRIACAO datetime2 = Null
,	@p_DT_INICIO_PROCESSAMENTO datetime2 = Null
,	@p_DT_FIM_PROCESSAMENTO datetime2 = Null
,	@p_URL_ARQUIVO varchar (500) = Null
,	@p_NM_ARQUIVO varchar (255) = Null
,	@p_TP_ARQUIVO varchar (10) = Null
,	@p_MSG_ERRO nvarchar (MAX) = Null
,	@p_QT_TENTATIVAS int = Null
,	@p_FL_NOTIFICADO bit = Null
,	@p_COD_JOB_EXTERNO varchar (100) = Null
,	@p_QT_REGISTROS_GERADOS int = Null
,	@p_QT_TAMANHO_ARQUIVO_KB int = Null
,	@p_NM_PROCEDURE_AD_HOC varchar (100) = Null
,	@p_TP_ARQUIVO_SOLICITADO varchar (10) = Null
,	@p_DS_LAYOUT_SAIDA_JSON nvarchar (MAX) = Null
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
			from	Report.DBO.TB_RELATORIO_SOLICITACAO	with (nolock)
			where	COD_RELATORIO_SOLICITACAO = @p_COD_RELATORIO_SOLICITACAO

		) begin

			update	Report.DBO.TB_RELATORIO_SOLICITACAO
			set	COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO
			,	COD_USUARIO = @p_COD_USUARIO
			,	DS_PARAMETROS = upper(@p_DS_PARAMETROS)
			,	COD_STATUS_PROCESSAMENTO = @p_COD_STATUS_PROCESSAMENTO
			,	DT_INICIO_PROCESSAMENTO = @p_DT_INICIO_PROCESSAMENTO
			,	DT_FIM_PROCESSAMENTO = @p_DT_FIM_PROCESSAMENTO
			,	URL_ARQUIVO = upper(@p_URL_ARQUIVO)
			,	NM_ARQUIVO = upper(@p_NM_ARQUIVO)
			,	TP_ARQUIVO = upper(@p_TP_ARQUIVO)
			,	MSG_ERRO = upper(@p_MSG_ERRO)
			,	QT_TENTATIVAS = @p_QT_TENTATIVAS
			,	FL_NOTIFICADO = @p_FL_NOTIFICADO
			,	COD_JOB_EXTERNO = upper(@p_COD_JOB_EXTERNO)
			,	QT_REGISTROS_GERADOS = @p_QT_REGISTROS_GERADOS
			,	QT_TAMANHO_ARQUIVO_KB = @p_QT_TAMANHO_ARQUIVO_KB
			,	NM_PROCEDURE_AD_HOC = upper(@p_NM_PROCEDURE_AD_HOC)
			,	TP_ARQUIVO_SOLICITADO = upper(@p_TP_ARQUIVO_SOLICITADO)
			,	DS_LAYOUT_SAIDA_JSON = upper(@p_DS_LAYOUT_SAIDA_JSON)
			where	COD_RELATORIO_SOLICITACAO = @p_COD_RELATORIO_SOLICITACAO

			select @RETORNO = @@ERROR

		end else begin

			insert	Report.dbo.TB_RELATORIO_SOLICITACAO(
				COD_RELATORIO_MODELO
			,	COD_USUARIO
			,	DS_PARAMETROS
			,	COD_STATUS_PROCESSAMENTO
			,	DT_CRIACAO
			,	DT_INICIO_PROCESSAMENTO
			,	DT_FIM_PROCESSAMENTO
			,	URL_ARQUIVO
			,	NM_ARQUIVO
			,	TP_ARQUIVO
			,	MSG_ERRO
			,	QT_TENTATIVAS
			,	FL_NOTIFICADO
			,	COD_JOB_EXTERNO
			,	QT_REGISTROS_GERADOS
			,	QT_TAMANHO_ARQUIVO_KB
			,	NM_PROCEDURE_AD_HOC
			,	TP_ARQUIVO_SOLICITADO
			,	DS_LAYOUT_SAIDA_JSON
			)values(
				@p_COD_RELATORIO_MODELO
			,	@p_COD_USUARIO
			,	upper(@p_DS_PARAMETROS)
			,	@p_COD_STATUS_PROCESSAMENTO
			,	GETDATE()
			,	@p_DT_INICIO_PROCESSAMENTO
			,	@p_DT_FIM_PROCESSAMENTO
			,	upper(@p_URL_ARQUIVO)
			,	upper(@p_NM_ARQUIVO)
			,	upper(@p_TP_ARQUIVO)
			,	upper(@p_MSG_ERRO)
			,	@p_QT_TENTATIVAS
			,	@p_FL_NOTIFICADO
			,	upper(@p_COD_JOB_EXTERNO)
			,	@p_QT_REGISTROS_GERADOS
			,	@p_QT_TAMANHO_ARQUIVO_KB
			,	upper(@p_NM_PROCEDURE_AD_HOC)
			,	upper(@p_TP_ARQUIVO_SOLICITADO)
			,	upper(@p_DS_LAYOUT_SAIDA_JSON)
			)

			set	@RETORNO=@@ERROR
			set	@p_COD_RELATORIO_SOLICITACAO = SCOPE_IDENTITY()

		end
	end else begin
		-- Operação de exclusão (D)
		-- Verificar se ID é obrigatório para operação D
		IF @p_COD_RELATORIO_SOLICITACAO IS NULL OR @p_COD_RELATORIO_SOLICITACAO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operação de exclusão'
			ROLLBACK TRANSACTION
			RETURN @RETORNO
		END

		if exists (

			select	1
			from	Report.DBO.TB_RELATORIO_SOLICITACAO	with (nolock)
			where	COD_RELATORIO_SOLICITACAO = @p_COD_RELATORIO_SOLICITACAO

		) begin

			delete	Report.DBO.TB_RELATORIO_SOLICITACAO
			where	COD_RELATORIO_SOLICITACAO = @p_COD_RELATORIO_SOLICITACAO

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

-- Fim do Arquivo dbo.DATA_TB_RELATORIO_SOLICITACAO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.RULE_TB_RELATORIO_SOLICITACAO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.RULE_TB_RELATORIO_SOLICITACAO
 	@p_COD_RELATORIO_SOLICITACAO bigint = NULL output
,	@p_COD_RELATORIO_MODELO int = Null
,	@p_COD_USUARIO int = Null
,	@p_DS_PARAMETROS nvarchar (MAX) = Null
,	@p_COD_STATUS_PROCESSAMENTO int = Null
,	@p_DT_INICIO_PROCESSAMENTO datetime2 = Null
,	@p_DT_FIM_PROCESSAMENTO datetime2 = Null
,	@p_URL_ARQUIVO varchar (500) = Null
,	@p_NM_ARQUIVO varchar (255) = Null
,	@p_TP_ARQUIVO varchar (10) = Null
,	@p_MSG_ERRO nvarchar (MAX) = Null
,	@p_QT_TENTATIVAS int = Null
,	@p_FL_NOTIFICADO bit = Null
,	@p_COD_JOB_EXTERNO varchar (100) = Null
,	@p_QT_REGISTROS_GERADOS int = Null
,	@p_QT_TAMANHO_ARQUIVO_KB int = Null
,	@p_NM_PROCEDURE_AD_HOC varchar (100) = Null
,	@p_TP_ARQUIVO_SOLICITADO varchar (10) = Null
,	@p_DS_LAYOUT_SAIDA_JSON nvarchar (MAX) = Null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare 	@RETORNO	int	 = 0
,	@MSG_ERR	varchar(8000)	 = ''

if (@p_COD_RELATORIO_SOLICITACAO IS NULL OR @p_COD_RELATORIO_SOLICITACAO <= 0
) and @p_OPERACAO in ('U','D')
begin
	SELECT	@MSG_ERR	 = 'Chave primária não informada'
	,	@p_MSG_RETORNO	 = @MSG_ERR
	,	@RETORNO	 = 1
end

if (@p_OPERACAO <> 'D') begin
	if (@p_COD_USUARIO IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo COD_USUARIO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_COD_STATUS_PROCESSAMENTO IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo COD_STATUS_PROCESSAMENTO é obrigatório.' + CHAR(10) + CHAR(13)
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
-- Fim do Arquivo dbo.RULE_TB_RELATORIO_SOLICITACAO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.APP_TB_RELATORIO_SOLICITACAO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.APP_TB_RELATORIO_SOLICITACAO
 	@p_COD_RELATORIO_SOLICITACAO bigint = NULL output
,	@p_COD_RELATORIO_MODELO int = Null
,	@p_COD_USUARIO int = Null
,	@p_DS_PARAMETROS nvarchar (MAX) = Null
,	@p_COD_STATUS_PROCESSAMENTO int = Null
,	@p_DT_INICIO_PROCESSAMENTO datetime2 = Null
,	@p_DT_FIM_PROCESSAMENTO datetime2 = Null
,	@p_URL_ARQUIVO varchar (500) = Null
,	@p_NM_ARQUIVO varchar (255) = Null
,	@p_TP_ARQUIVO varchar (10) = Null
,	@p_MSG_ERRO nvarchar (MAX) = Null
,	@p_QT_TENTATIVAS int = Null
,	@p_FL_NOTIFICADO bit = Null
,	@p_COD_JOB_EXTERNO varchar (100) = Null
,	@p_QT_REGISTROS_GERADOS int = Null
,	@p_QT_TAMANHO_ARQUIVO_KB int = Null
,	@p_NM_PROCEDURE_AD_HOC varchar (100) = Null
,	@p_TP_ARQUIVO_SOLICITADO varchar (10) = Null
,	@p_DS_LAYOUT_SAIDA_JSON nvarchar (MAX) = Null
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
		IF @p_COD_RELATORIO_SOLICITACAO IS NULL OR @p_COD_RELATORIO_SOLICITACAO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operações de atualização e exclusão'
			RETURN @RETORNO
		END
	END

	-- Chama a procedure de validação (RULE)
	execute @RETORNO = dbo.RULE_TB_RELATORIO_SOLICITACAO
	  @p_COD_RELATORIO_SOLICITACAO = @p_COD_RELATORIO_SOLICITACAO output
	, @p_COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO
	, @p_COD_USUARIO = @p_COD_USUARIO
	, @p_DS_PARAMETROS = @p_DS_PARAMETROS
	, @p_COD_STATUS_PROCESSAMENTO = @p_COD_STATUS_PROCESSAMENTO
	, @p_DT_INICIO_PROCESSAMENTO = @p_DT_INICIO_PROCESSAMENTO
	, @p_DT_FIM_PROCESSAMENTO = @p_DT_FIM_PROCESSAMENTO
	, @p_URL_ARQUIVO = @p_URL_ARQUIVO
	, @p_NM_ARQUIVO = @p_NM_ARQUIVO
	, @p_TP_ARQUIVO = @p_TP_ARQUIVO
	, @p_MSG_ERRO = @p_MSG_ERRO
	, @p_QT_TENTATIVAS = @p_QT_TENTATIVAS
	, @p_FL_NOTIFICADO = @p_FL_NOTIFICADO
	, @p_COD_JOB_EXTERNO = @p_COD_JOB_EXTERNO
	, @p_QT_REGISTROS_GERADOS = @p_QT_REGISTROS_GERADOS
	, @p_QT_TAMANHO_ARQUIVO_KB = @p_QT_TAMANHO_ARQUIVO_KB
	, @p_NM_PROCEDURE_AD_HOC = @p_NM_PROCEDURE_AD_HOC
	, @p_TP_ARQUIVO_SOLICITADO = @p_TP_ARQUIVO_SOLICITADO
	, @p_DS_LAYOUT_SAIDA_JSON = @p_DS_LAYOUT_SAIDA_JSON
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

	IF (@RETORNO != 0) BEGIN
		RETURN @RETORNO
	END

	-- Chama a procedure de dados (DATA)
	execute @RETORNO = dbo.DATA_TB_RELATORIO_SOLICITACAO
	  @p_COD_RELATORIO_SOLICITACAO = @p_COD_RELATORIO_SOLICITACAO output
	, @p_COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO
	, @p_COD_USUARIO = @p_COD_USUARIO
	, @p_DS_PARAMETROS = @p_DS_PARAMETROS
	, @p_COD_STATUS_PROCESSAMENTO = @p_COD_STATUS_PROCESSAMENTO
	, @p_DT_INICIO_PROCESSAMENTO = @p_DT_INICIO_PROCESSAMENTO
	, @p_DT_FIM_PROCESSAMENTO = @p_DT_FIM_PROCESSAMENTO
	, @p_URL_ARQUIVO = @p_URL_ARQUIVO
	, @p_NM_ARQUIVO = @p_NM_ARQUIVO
	, @p_TP_ARQUIVO = @p_TP_ARQUIVO
	, @p_MSG_ERRO = @p_MSG_ERRO
	, @p_QT_TENTATIVAS = @p_QT_TENTATIVAS
	, @p_FL_NOTIFICADO = @p_FL_NOTIFICADO
	, @p_COD_JOB_EXTERNO = @p_COD_JOB_EXTERNO
	, @p_QT_REGISTROS_GERADOS = @p_QT_REGISTROS_GERADOS
	, @p_QT_TAMANHO_ARQUIVO_KB = @p_QT_TAMANHO_ARQUIVO_KB
	, @p_NM_PROCEDURE_AD_HOC = @p_NM_PROCEDURE_AD_HOC
	, @p_TP_ARQUIVO_SOLICITADO = @p_TP_ARQUIVO_SOLICITADO
	, @p_DS_LAYOUT_SAIDA_JSON = @p_DS_LAYOUT_SAIDA_JSON
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
-- Fim do Arquivo dbo.APP_TB_RELATORIO_SOLICITACAO



