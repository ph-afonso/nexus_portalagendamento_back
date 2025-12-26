-- Inicio do Arquivo dbo.DATA_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO

use Program
go

--------------------------------------------------------------------------------
print '*** dbo.DATA_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.DATA_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO
 	@p_ID bigint = 0 output
,	@p_IDENTIFICADOR_CLIENTES uniqueidentifier = Null
,	@p_DT_SUGESTAO_AGENDA DATETIME = NULL
,	@p_COD_INCLUSAO_USUARIOS smallint = NULL
,	@p_ID_CONHECIMENTOS BIGINT = null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare @RETORNO int = 0
DECLARE @ErrorMessage NVARCHAR(4000)
DECLARE @ErrorSeverity INT
DECLARE @ErrorState INT


	BEGIN TRY
	BEGIN TRAN
	
				update	NewSitex.DBO.TB_CONHECIMENTOS
				SET		DT_SUGESTAO_AGENDAMENTO = @p_DT_SUGESTAO_AGENDA   
						,COD_USUARIO_SUGESTAO_AGENDAMENTO = @p_COD_INCLUSAO_USUARIOS   
						,DT_ALTERACAO_SUGESTAO_AGENDAMENTO = GETDATE()  
						,COD_ATUALIZACAO_USUARIOS = @p_COD_INCLUSAO_USUARIOS   
						,DT_ATUALIZACAO_CONHECIMENTOS = GETDATE()  
				where	ID = @p_ID_CONHECIMENTOS
			
				select @RETORNO = @@ERROR

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

-- Fim do Arquivo dbo.DATA_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.RULE_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.RULE_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO
 	@p_ID bigint = NULL output
,	@p_IDENTIFICADOR_CLIENTES uniqueidentifier = Null
,	@p_DT_SUGESTAO_AGENDA DATETIME = NULL
,   @p_ID_CONHECIMENTOS BIGINT = null
,	@p_OPERACAO char (1) = Null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare 	@RETORNO	int	 = 0
,	@MSG_ERR	varchar(8000)	 = ''

if (@p_ID IS NULL OR @p_ID <= 0
) and @p_OPERACAO in ('D')
begin
	SELECT	@MSG_ERR	 = 'Chave primária não informada'
	,	@p_MSG_RETORNO	 = @MSG_ERR
	,	@RETORNO	 = 1
end

if (@p_OPERACAO <> 'D') begin
	if (@p_IDENTIFICADOR_CLIENTES IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo IDENTIFICADOR_CLIENTES é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	end
	if (@p_DT_SUGESTAO_AGENDA IS NULL) begin
		select @MSG_ERR = @MSG_ERR + '^O campo DT_SUGESTAO_AGENDA é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	END
	if (@p_DT_SUGESTAO_AGENDA IS NOT NULL AND @p_IDENTIFICADOR_CLIENTES IS NOT NULL) begin
		if EXISTS (SELECT 1 FROM NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA WHERE DT_SUGESTAO_AGENDAMENTO > @p_DT_SUGESTAO_AGENDA AND IDENTIFICADOR_CLIENTES = @p_IDENTIFICADOR_CLIENTES)
		BEGIN
			select @MSG_ERR = @MSG_ERR + '^A data selecionada está antecipada a previsão acordada, por favor, selecionar outra data' + CHAR(10) + CHAR(13)
			, @RETORNO = @RETORNO + 1
		END

	END
		if (@p_ID_CONHECIMENTOS IS NULL OR @p_ID_CONHECIMENTOS <= 0) begin
		select @MSG_ERR = @MSG_ERR + '^O campo ID_CONHECIMENTOS é obrigatório.' + CHAR(10) + CHAR(13)
		, @RETORNO = @RETORNO + 1
	END


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
-- Fim do Arquivo dbo.RULE_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO



use Program
go

--------------------------------------------------------------------------------
print '*** dbo.APP_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO - Data de Execução: ' + FORMAT(GETDATE(),'dd/MM/yyyy HH:mm:ss')
--------------------------------------------------------------------------------
go

CREATE OR ALTER PROCEDURE dbo.APP_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO
 	@p_ID bigint = NULL output
,	@p_IDENTIFICADOR_CLIENTES uniqueidentifier = Null
,	@p_DT_SUGESTAO_AGENDA DATETIME = null
,	@p_OPERACAO char (1) = NULL
,	@p_COD_INCLUSAO_USUARIOS smallint = null
,	@p_MSG_RETORNO varchar(8000) = '' output
as
Set nocount on
Declare	@RETORNO	int		 = 0
DECLARE @ErrorMessage NVARCHAR(4000)
DECLARE @ErrorSeverity INT
DECLARE @ErrorState INT

BEGIN TRY

	DECLARE @ID_CONHECIMENTOS BIGINT = NULL
	
	
	SELECT @ID_CONHECIMENTOS = ID_CONHECIMENTOS
	FROM NewSitex.dbo.TB_PORTAL_AGENDAMENTO_DPA AS TPAD
	WHERE IDENTIFICADOR_CLIENTES = @p_IDENTIFICADOR_CLIENTES
	

	-- Chama a procedure de validação (RULE)
	execute @RETORNO = dbo.RULE_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO
	  @p_ID = @p_ID output
	, @p_IDENTIFICADOR_CLIENTES = @p_IDENTIFICADOR_CLIENTES
	, @p_DT_SUGESTAO_AGENDA  = @p_DT_SUGESTAO_AGENDA
	, @p_ID_CONHECIMENTOS = @ID_CONHECIMENTOS
	, @p_OPERACAO = @p_OPERACAO
	, @p_MSG_RETORNO = @p_MSG_RETORNO output

	IF (@RETORNO != 0) BEGIN
		RETURN @RETORNO
	END

	-- Chama a procedure de dados (DATA)
	execute @RETORNO = dbo.DATA_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO
	  @p_ID = @p_ID output
	, @p_IDENTIFICADOR_CLIENTES = @p_IDENTIFICADOR_CLIENTES
	, @p_DT_SUGESTAO_AGENDA = @p_DT_SUGESTAO_AGENDA
	, @p_ID_CONHECIMENTOS = @ID_CONHECIMENTOS
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
-- Fim do Arquivo dbo.APP_DT_SUGESTAO_AGENDA_PORTAL_AGENDAMENTO



