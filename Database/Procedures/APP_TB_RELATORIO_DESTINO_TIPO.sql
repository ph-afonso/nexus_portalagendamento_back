-- Inicio do Arquivo dbo.DATA_TB_RELATORIO_DESTINO_TIPO

use Program
go

--------------------------------------------------------------------------------
print '*** dbo.DATA_TB_RELATORIO_DESTINO_TIPO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.DATA_TB_RELATORIO_DESTINO_TIPO
 	@p_COD_DESTINO_TIPO int = 0 output
,	@p_NM_TIPO varchar (30)
,	@p_DS_TIPO varchar (200) = Null
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
			from	Report.DBO.TB_RELATORIO_DESTINO_TIPO	with (nolock)
			where	COD_DESTINO_TIPO = @p_COD_DESTINO_TIPO

		) begin

			update	Report.DBO.TB_RELATORIO_DESTINO_TIPO
			set	NM_TIPO = upper(@p_NM_TIPO)
			,	DS_TIPO = upper(@p_DS_TIPO)
			,	FL_ATIVO = @p_FL_ATIVO
			where	COD_DESTINO_TIPO = @p_COD_DESTINO_TIPO

			select @RETORNO = @@ERROR

		end else begin

			insert	Report.dbo.TB_RELATORIO_DESTINO_TIPO(
				NM_TIPO
			,	DS_TIPO
			,	FL_ATIVO
			)values(
				upper(@p_NM_TIPO)
			,	upper(@p_DS_TIPO)
			,	@p_FL_ATIVO
			)

			set	@RETORNO=@@ERROR
			set	@p_COD_DESTINO_TIPO = SCOPE_IDENTITY()

		end
	end else begin
		-- Operação de exclusão (D)
		-- Verificar se ID é obrigatório para operação D
		IF @p_COD_DESTINO_TIPO IS NULL OR @p_COD_DESTINO_TIPO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operação de exclusão'
			ROLLBACK TRANSACTION
			RETURN @RETORNO
		END

		if exists (

			select	1
			from	Report.DBO.TB_RELATORIO_DESTINO_TIPO	with (nolock)
			where	COD_DESTINO_TIPO = @p_COD_DESTINO_TIPO

		) begin

			delete	Report.DBO.TB_RELATORIO_DESTINO_TIPO
			where	COD_DESTINO_TIPO = @p_COD_DESTINO_TIPO

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

-- Fim do Arquivo dbo.DATA_TB_RELATORIO_DESTINO_TIPO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.RULE_TB_RELATORIO_DESTINO_TIPO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.RULE_TB_RELATORIO_DESTINO_TIPO
 	@p_COD_DESTINO_TIPO int = 0 output
,	@p_NM_TIPO varchar (30) = Null
,	@p_DS_TIPO varchar (200) = Null
,	@p_FL_ATIVO bit = Null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare 	@RETORNO	int	 = 0
,	@MSG_ERR	varchar(8000)	 = ''

if (@p_COD_DESTINO_TIPO IS NULL OR @p_COD_DESTINO_TIPO <= 0
) and @p_OPERACAO in ('U','D')
begin
	SELECT	@MSG_ERR	 = 'Chave primária não informada'
	,	@p_MSG_RETORNO	 = @MSG_ERR
	,	@RETORNO	 = 1
end

if (@p_OPERACAO <> 'D') begin
	if (@p_NM_TIPO IS NULL OR LTRIM(RTRIM(@p_NM_TIPO)) = '') begin
		select @MSG_ERR = @MSG_ERR + '^O campo NM_TIPO é obrigatório.' + CHAR(10) + CHAR(13)
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
-- Fim do Arquivo dbo.RULE_TB_RELATORIO_DESTINO_TIPO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.APP_TB_RELATORIO_DESTINO_TIPO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.APP_TB_RELATORIO_DESTINO_TIPO
 	@p_COD_DESTINO_TIPO int = 0 output
,	@p_NM_TIPO varchar (30) = Null
,	@p_DS_TIPO varchar (200) = Null
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
		IF @p_COD_DESTINO_TIPO IS NULL OR @p_COD_DESTINO_TIPO <= 0
		BEGIN
			SET @RETORNO = 1
			SET @p_MSG_RETORNO = 'A chave primária é obrigatória para operações de atualização e exclusão'
			RETURN @RETORNO
		END
	END

	-- Chama a procedure de validação (RULE)
	execute @RETORNO = dbo.RULE_TB_RELATORIO_DESTINO_TIPO
	  @p_COD_DESTINO_TIPO = @p_COD_DESTINO_TIPO output
	, @p_NM_TIPO = @p_NM_TIPO
	, @p_DS_TIPO = @p_DS_TIPO
	, @p_FL_ATIVO = @p_FL_ATIVO
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

	IF (@RETORNO != 0) BEGIN
		RETURN @RETORNO
	END

	-- Chama a procedure de dados (DATA)
	execute @RETORNO = dbo.DATA_TB_RELATORIO_DESTINO_TIPO
	  @p_COD_DESTINO_TIPO = @p_COD_DESTINO_TIPO output
	, @p_NM_TIPO = @p_NM_TIPO
	, @p_DS_TIPO = @p_DS_TIPO
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
-- Fim do Arquivo dbo.APP_TB_RELATORIO_DESTINO_TIPO



