USE Program
GO

CREATE OR ALTER PROCEDURE dbo.LST_TB_RELATORIO_PARAMETRO_OPCAO
	@p_FILTER		   VARCHAR(1024) = NULL
	, @p_PAGE_NUMBER	   INT			 = 1
	, @p_PAGE_SIZE	   INT			 = 50
	, @p_TOTAL_REGISTERS INT			 = 0 OUTPUT
	, @p_MSG_RETORNO	   VARCHAR(MAX)	 = '' OUTPUT
	AS

BEGIN TRY
		-- set transaction isolation level serializable;

		DECLARE @Retorno INT = 0



		DECLARE @sql			NVARCHAR(MAX)
			  , @sqlFromWhere   NVARCHAR(MAX)
			  , @selectColumns  VARCHAR(1024)
			  , @ParmDefinition NVARCHAR(500)
			  , @filtro			VARCHAR(1300) = ISNULL(Program.dbo.FNC_BUILD_PARAMS_FILTER(@p_Filter), '')

		SET @selectColumns = 'select a.COD_OPCAO, a.COD_PARAMETRO, a.VL_OPCAO, a.DS_OPCAO, a.NR_ORDEM, a.FL_ATIVO '

		SET @sqlFromWhere = '  
	                    from Report.dbo.TB_RELATORIO_PARAMETRO_OPCAO a
	                    WHERE 1=1 
	    ' + @filtro

		-- 2. VERIFICA SE A FUNÇÃO RETORNOU ERRO (NULL)
		IF @filtro IS NULL BEGIN
			-- Agora SIM, podemos chamar RAISERROR a partir do procedimento!
			RAISERROR ('Parâmetros de filtro inválidos ou contêm ameaça de SQL Injection.', 16, 1);
			RETURN; -- Encerra a execução do procedimento
END

		SET @sql = 'select @qtd=count(1)' + @sqlFromWhere

		SET @ParmDefinition = N'@qtd varchar(30) output'

		EXECUTE sp_executesql
		@sql
	  , @ParmDefinition
	  , @qtd = @p_TOTAL_REGISTERS OUTPUT

		SET @sql = @selectColumns + @sqlFromWhere + Program.dbo.FNC_BUILD_OFFSET_PAGE('a.COD_OPCAO', @p_PAGE_NUMBER, @p_PAGE_SIZE)

		PRINT @sql

		EXECUTE (@sql)


END TRY

BEGIN CATCH
SELECT @p_MSG_RETORNO = 'Erro: [' + CAST(ERROR_NUMBER() AS VARCHAR) + '] - ' + ERROR_MESSAGE()
     , @Retorno			  = 77701

END CATCH


GO


 --EXEC LST_TB_RELATORIO_PARAMETRO_OPCAO @p_Filter = 'COD_OPCAO:Maior que:0', @p_PageNumber = 1
