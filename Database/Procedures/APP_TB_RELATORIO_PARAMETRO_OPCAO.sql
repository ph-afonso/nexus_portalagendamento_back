-- Inicio do Arquivo dbo.DATA_TB_RELATORIO_PARAMETRO_OPCAO

use Program
go

--------------------------------------------------------------------------------
print '*** dbo.DATA_TB_RELATORIO_PARAMETRO_OPCAO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.DATA_TB_RELATORIO_PARAMETRO_OPCAO
 	@p_COD_OPCAO int = 0 output
,	@p_COD_PARAMETRO int
,	@p_VL_OPCAO varchar (100)
,	@p_DS_OPCAO varchar (200)
,	@p_NR_ORDEM int = Null
,	@p_FL_ATIVO bit = Null
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
			from	Report.DBO.TB_RELATORIO_PARAMETRO_OPCAO	with (nolock)
			where	COD_OPCAO = @p_COD_OPCAO

		) begin

			update	Report.DBO.TB_RELATORIO_PARAMETRO_OPCAO
			set	COD_PARAMETRO = @p_COD_PARAMETRO
			,	VL_OPCAO = upper(@p_VL_OPCAO)
			,	DS_OPCAO = upper(@p_DS_OPCAO)
			,	NR_ORDEM = @p_NR_ORDEM
			,	FL_ATIVO = @p_FL_ATIVO
			where	COD_OPCAO = @p_COD_OPCAO

			select @RETORNO = @@ERROR

		end else begin

			insert	Report.dbo.TB_RELATORIO_PARAMETRO_OPCAO(
				COD_PARAMETRO
			,	VL_OPCAO
			,	DS_OPCAO
			,	NR_ORDEM
			,	FL_ATIVO
			)values(
				@p_COD_PARAMETRO
			,	upper(@p_VL_OPCAO)
			,	upper(@p_DS_OPCAO)
			,	@p_NR_ORDEM
			,	@p_FL_ATIVO
			)

			set	@RETORNO=@@ERROR
			set	@p_COD_OPCAO = SCOPE_IDENTITY()

		end
	end else begin
		-- Operação de exclusão (D)
		-- Verificar se ID é obrigatório para operação D
		IF @p_COD_OPCAO IS NULL OR @p_COD_OPCAO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operação de exclusão'
			ROLLBACK TRANSACTION
			RETURN @RETORNO
		END

		if exists (

			select	1
			from	Report.DBO.TB_RELATORIO_PARAMETRO_OPCAO	with (nolock)
			where	COD_OPCAO = @p_COD_OPCAO

		) begin

			delete	Report.DBO.TB_RELATORIO_PARAMETRO_OPCAO
			where	COD_OPCAO = @p_COD_OPCAO

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

-- Fim do Arquivo dbo.DATA_TB_RELATORIO_PARAMETRO_OPCAO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.RULE_TB_RELATORIO_PARAMETRO_OPCAO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.RULE_TB_RELATORIO_PARAMETRO_OPCAO
 	@p_COD_OPCAO int = 0 output
,	@p_COD_PARAMETRO int = Null
,	@p_VL_OPCAO varchar (100) = Null
,	@p_DS_OPCAO varchar (200) = Null
,	@p_NR_ORDEM int = Null
,	@p_FL_ATIVO bit = Null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare 	@RETORNO	int	 = 0
,	@MSG_ERR	varchar(8000)	 = ''

if (@p_COD_OPCAO IS NULL OR @p_COD_OPCAO <= 0
) and @p_OPERACAO in ('U','D')
begin
	SELECT	@MSG_ERR	 = 'Chave primária não informada'
	,	@p_MSG_RETORNO	 = @MSG_ERR
	,	@RETORNO	 = 1
end

if (@p_OPERACAO <> 'D') begin
	if (@p_COD_PARAMETRO IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo COD_PARAMETRO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_VL_OPCAO IS NULL OR LTRIM(RTRIM(@p_VL_OPCAO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo VL_OPCAO é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_DS_OPCAO IS NULL OR LTRIM(RTRIM(@p_DS_OPCAO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo DS_OPCAO é obrigatório.' + CHAR(10) + CHAR(13)
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
-- Fim do Arquivo dbo.RULE_TB_RELATORIO_PARAMETRO_OPCAO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.APP_TB_RELATORIO_PARAMETRO_OPCAO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.APP_TB_RELATORIO_PARAMETRO_OPCAO
 	@p_COD_OPCAO int = 0 output
,	@p_COD_PARAMETRO int = Null
,	@p_VL_OPCAO varchar (100) = Null
,	@p_DS_OPCAO varchar (200) = Null
,	@p_NR_ORDEM int = Null
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
		IF @p_COD_OPCAO IS NULL OR @p_COD_OPCAO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operações de atualização e exclusão'
			RETURN @RETORNO
		END
	END

	-- Chama a procedure de validação (RULE)
	execute @RETORNO = dbo.RULE_TB_RELATORIO_PARAMETRO_OPCAO
	  @p_COD_OPCAO = @p_COD_OPCAO output
	, @p_COD_PARAMETRO = @p_COD_PARAMETRO
	, @p_VL_OPCAO = @p_VL_OPCAO
	, @p_DS_OPCAO = @p_DS_OPCAO
	, @p_NR_ORDEM = @p_NR_ORDEM
	, @p_FL_ATIVO = @p_FL_ATIVO
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

	IF (@RETORNO != 0) BEGIN
		RETURN @RETORNO
	END

	-- Chama a procedure de dados (DATA)
	execute @RETORNO = dbo.DATA_TB_RELATORIO_PARAMETRO_OPCAO
	  @p_COD_OPCAO = @p_COD_OPCAO output
	, @p_COD_PARAMETRO = @p_COD_PARAMETRO
	, @p_VL_OPCAO = @p_VL_OPCAO
	, @p_DS_OPCAO = @p_DS_OPCAO
	, @p_NR_ORDEM = @p_NR_ORDEM
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
-- Fim do Arquivo dbo.APP_TB_RELATORIO_PARAMETRO_OPCAO



