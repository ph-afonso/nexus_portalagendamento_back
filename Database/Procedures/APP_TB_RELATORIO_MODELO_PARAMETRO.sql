-- Inicio do Arquivo dbo.DATA_TB_RELATORIO_MODELO_PARAMETRO

use Program
go

--------------------------------------------------------------------------------
print '*** dbo.DATA_TB_RELATORIO_MODELO_PARAMETRO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.DATA_TB_RELATORIO_MODELO_PARAMETRO
 	@p_COD_PARAMETRO int = 0 output
,	@p_COD_RELATORIO_MODELO int
,	@p_NM_PARAMETRO varchar (100)
,	@p_DS_PARAMETRO varchar (200)
,	@p_COD_TIPO_PARAMETRO int
,	@p_NR_ORDEM int
,	@p_FL_OBRIGATORIO bit = Null
,	@p_VL_PADRAO varchar (500) = Null
,	@p_DS_HELP text = Null
,	@p_QT_MIN_CARACTERES int = Null
,	@p_QT_MAX_CARACTERES int = Null
,	@p_VL_MINIMO decimal (18,4) = Null
,	@p_VL_MAXIMO decimal (18,4) = Null
,	@p_FL_VISIVEL bit = Null
,	@p_FL_ATIVO bit = Null
,	@p_DT_CRIACAO datetime2 = Null
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
			from	Report.DBO.TB_RELATORIO_MODELO_PARAMETRO	with (nolock)
			where	COD_PARAMETRO = @p_COD_PARAMETRO

		) begin

			update	Report.DBO.TB_RELATORIO_MODELO_PARAMETRO
			set	COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO
			,	NM_PARAMETRO = upper(@p_NM_PARAMETRO)
			,	DS_PARAMETRO = upper(@p_DS_PARAMETRO)
			,	COD_TIPO_PARAMETRO = @p_COD_TIPO_PARAMETRO
			,	NR_ORDEM = @p_NR_ORDEM
			,	FL_OBRIGATORIO = @p_FL_OBRIGATORIO
			,	VL_PADRAO = upper(@p_VL_PADRAO)
			,	DS_HELP = @p_DS_HELP
			,	QT_MIN_CARACTERES = @p_QT_MIN_CARACTERES
			,	QT_MAX_CARACTERES = @p_QT_MAX_CARACTERES
			,	VL_MINIMO = @p_VL_MINIMO
			,	VL_MAXIMO = @p_VL_MAXIMO
			,	FL_VISIVEL = @p_FL_VISIVEL
			,	FL_ATIVO = @p_FL_ATIVO
			where	COD_PARAMETRO = @p_COD_PARAMETRO

			select @RETORNO = @@ERROR

		end else begin

			insert	Report.dbo.TB_RELATORIO_MODELO_PARAMETRO(
				COD_RELATORIO_MODELO
			,	NM_PARAMETRO
			,	DS_PARAMETRO
			,	COD_TIPO_PARAMETRO
			,	NR_ORDEM
			,	FL_OBRIGATORIO
			,	VL_PADRAO
			,	DS_HELP
			,	QT_MIN_CARACTERES
			,	QT_MAX_CARACTERES
			,	VL_MINIMO
			,	VL_MAXIMO
			,	FL_VISIVEL
			,	FL_ATIVO
			,	DT_CRIACAO
			)values(
				@p_COD_RELATORIO_MODELO
			,	upper(@p_NM_PARAMETRO)
			,	upper(@p_DS_PARAMETRO)
			,	@p_COD_TIPO_PARAMETRO
			,	@p_NR_ORDEM
			,	@p_FL_OBRIGATORIO
			,	upper(@p_VL_PADRAO)
			,	@p_DS_HELP
			,	@p_QT_MIN_CARACTERES
			,	@p_QT_MAX_CARACTERES
			,	@p_VL_MINIMO
			,	@p_VL_MAXIMO
			,	@p_FL_VISIVEL
			,	@p_FL_ATIVO
			,	GETDATE()
			)

			set	@RETORNO=@@ERROR
			set	@p_COD_PARAMETRO = SCOPE_IDENTITY()

		end
	end else begin
		-- Operação de exclusão (D)
		-- Verificar se ID é obrigatório para operação D
		IF @p_COD_PARAMETRO IS NULL OR @p_COD_PARAMETRO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operação de exclusão'
			ROLLBACK TRANSACTION
			RETURN @RETORNO
		END

		if exists (

			select	1
			from	Report.DBO.TB_RELATORIO_MODELO_PARAMETRO	with (nolock)
			where	COD_PARAMETRO = @p_COD_PARAMETRO

		) begin

			delete	Report.DBO.TB_RELATORIO_MODELO_PARAMETRO
			where	COD_PARAMETRO = @p_COD_PARAMETRO

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

-- Fim do Arquivo dbo.DATA_TB_RELATORIO_MODELO_PARAMETRO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.RULE_TB_RELATORIO_MODELO_PARAMETRO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.RULE_TB_RELATORIO_MODELO_PARAMETRO
 	@p_COD_PARAMETRO int = 0 output
,	@p_COD_RELATORIO_MODELO int = Null
,	@p_NM_PARAMETRO varchar (100) = Null
,	@p_DS_PARAMETRO varchar (200) = Null
,	@p_COD_TIPO_PARAMETRO int = Null
,	@p_NR_ORDEM int = Null
,	@p_FL_OBRIGATORIO bit = Null
,	@p_VL_PADRAO varchar (500) = Null
,	@p_DS_HELP text = Null
,	@p_QT_MIN_CARACTERES int = Null
,	@p_QT_MAX_CARACTERES int = Null
,	@p_VL_MINIMO decimal (18,4) = Null
,	@p_VL_MAXIMO decimal (18,4) = Null
,	@p_FL_VISIVEL bit = Null
,	@p_FL_ATIVO bit = Null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare 	@RETORNO	int	 = 0
,	@MSG_ERR	varchar(8000)	 = ''

if (@p_COD_PARAMETRO IS NULL OR @p_COD_PARAMETRO <= 0
) and @p_OPERACAO in ('U','D')
begin
	SELECT	@MSG_ERR	 = 'Chave primária não informada'
	,	@p_MSG_RETORNO	 = @MSG_ERR
	,	@RETORNO	 = 1
end

if (@p_OPERACAO <> 'D') begin
	if (@p_COD_RELATORIO_MODELO IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo COD_RELATORIO_MODELO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_NM_PARAMETRO IS NULL OR LTRIM(RTRIM(@p_NM_PARAMETRO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo NM_PARAMETRO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_DS_PARAMETRO IS NULL OR LTRIM(RTRIM(@p_DS_PARAMETRO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo DS_PARAMETRO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_COD_TIPO_PARAMETRO IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo COD_TIPO_PARAMETRO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_NR_ORDEM IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo NR_ORDEM é obrigatório.' + CHAR(10) + CHAR(13)
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
-- Fim do Arquivo dbo.RULE_TB_RELATORIO_MODELO_PARAMETRO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.APP_TB_RELATORIO_MODELO_PARAMETRO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.APP_TB_RELATORIO_MODELO_PARAMETRO
 	@p_COD_PARAMETRO int = 0 output
,	@p_COD_RELATORIO_MODELO int = Null
,	@p_NM_PARAMETRO varchar (100) = Null
,	@p_DS_PARAMETRO varchar (200) = Null
,	@p_COD_TIPO_PARAMETRO int = Null
,	@p_NR_ORDEM int = Null
,	@p_FL_OBRIGATORIO bit = Null
,	@p_VL_PADRAO varchar (500) = Null
,	@p_DS_HELP text = Null
,	@p_QT_MIN_CARACTERES int = Null
,	@p_QT_MAX_CARACTERES int = Null
,	@p_VL_MINIMO decimal (18,4) = Null
,	@p_VL_MAXIMO decimal (18,4) = Null
,	@p_FL_VISIVEL bit = Null
,	@p_FL_ATIVO bit = Null
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
		IF @p_COD_PARAMETRO IS NULL OR @p_COD_PARAMETRO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operações de atualização e exclusão'
			RETURN @RETORNO
		END
	END

	-- Chama a procedure de validação (RULE)
	execute @RETORNO = dbo.RULE_TB_RELATORIO_MODELO_PARAMETRO
	  @p_COD_PARAMETRO = @p_COD_PARAMETRO output
	, @p_COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO
	, @p_NM_PARAMETRO = @p_NM_PARAMETRO
	, @p_DS_PARAMETRO = @p_DS_PARAMETRO
	, @p_COD_TIPO_PARAMETRO = @p_COD_TIPO_PARAMETRO
	, @p_NR_ORDEM = @p_NR_ORDEM
	, @p_FL_OBRIGATORIO = @p_FL_OBRIGATORIO
	, @p_VL_PADRAO = @p_VL_PADRAO
	, @p_DS_HELP = @p_DS_HELP
	, @p_QT_MIN_CARACTERES = @p_QT_MIN_CARACTERES
	, @p_QT_MAX_CARACTERES = @p_QT_MAX_CARACTERES
	, @p_VL_MINIMO = @p_VL_MINIMO
	, @p_VL_MAXIMO = @p_VL_MAXIMO
	, @p_FL_VISIVEL = @p_FL_VISIVEL
	, @p_FL_ATIVO = @p_FL_ATIVO
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

	IF (@RETORNO != 0) BEGIN
		RETURN @RETORNO
	END

	-- Chama a procedure de dados (DATA)
	execute @RETORNO = dbo.DATA_TB_RELATORIO_MODELO_PARAMETRO
	  @p_COD_PARAMETRO = @p_COD_PARAMETRO output
	, @p_COD_RELATORIO_MODELO = @p_COD_RELATORIO_MODELO
	, @p_NM_PARAMETRO = @p_NM_PARAMETRO
	, @p_DS_PARAMETRO = @p_DS_PARAMETRO
	, @p_COD_TIPO_PARAMETRO = @p_COD_TIPO_PARAMETRO
	, @p_NR_ORDEM = @p_NR_ORDEM
	, @p_FL_OBRIGATORIO = @p_FL_OBRIGATORIO
	, @p_VL_PADRAO = @p_VL_PADRAO
	, @p_DS_HELP = @p_DS_HELP
	, @p_QT_MIN_CARACTERES = @p_QT_MIN_CARACTERES
	, @p_QT_MAX_CARACTERES = @p_QT_MAX_CARACTERES
	, @p_VL_MINIMO = @p_VL_MINIMO
	, @p_VL_MAXIMO = @p_VL_MAXIMO
	, @p_FL_VISIVEL = @p_FL_VISIVEL
	, @p_FL_ATIVO = @p_FL_ATIVO
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
-- Fim do Arquivo dbo.APP_TB_RELATORIO_MODELO_PARAMETRO



