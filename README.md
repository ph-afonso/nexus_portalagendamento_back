# 📅 Nexus.PortalAgendamento

![Build Status](https://img.shields.io/badge/Build-Passing-success?style=for-the-badge&logo=appveyor)
![.NET](https://img.shields.io/badge/.NET-8.0%2F9.0-512BD4?style=for-the-badge&logo=dotnet)
![Status](https://img.shields.io/badge/Status-Development-orange?style=for-the-badge)

Backend robusto e modular responsável pelas regras de negócio e integrações do Portal de Agendamento. Construído sobre o **Nexus Framework** com arquitetura de Minimal APIs.

---

## 🏗️ Arquitetura e Framework

O projeto segue uma arquitetura em camadas (N-Tier) modernizada, utilizando o conceito de **Vertical Slices** (Fatias Verticais) na camada de apresentação.

### Estrutura da Solução
* **`Nexus.PortalAgendamento.MinimalApi`**: Camada de entrada (Presentation). Contém a configuração da aplicação, injeção de dependência e os *Endpoints*.
* **`Nexus.PortalAgendamento.Library`**: O coração do sistema. Contém os *Services* (Regra de Negócio), *Repositories* (Acesso a Dados) e *Models*.

### O Nexus Framework
O sistema utiliza um framework proprietário (`Nexus.Framework.*`) que padroniza o desenvolvimento:

* **⚡ Data Access (Procedure Repository):**
    Otimizado para **Stored Procedures**, garantindo alta performance no acesso ao SQL Server.
    * Prefixos padrão: `SNG_` (Single/GetById), `LST_` (List/Search), `APP_` (Application/Save).
    * Exemplo: `_serviceBase.FindByNumericIdAsync` ou `_serviceBase.ExecutePaginatedQueryAsync`.

* **🛡️ NexusResult Pattern:**
    Todas as respostas da API são encapsuladas no objeto `NexusResult<T>`, garantindo um contrato único para o Frontend:
    * `IsSuccess`: Status da operação.
    * `ResultData`: O dado retornado.
    * `Messages`: Lista de validações ou erros.

* **📝 Observabilidade:**
    Integração nativa com **Serilog** e **Seq**. Os logs são automaticamente enriquecidos com `MachineName`, `ThreadId`, `CorrelationId` e `Environment`.

---

## 🚀 Como Rodar o Projeto

### Pré-requisitos
* [.NET SDK 8.0 ou 9.0](https://dotnet.microsoft.com/download)
* SQL Server (com as procedures instaladas)
* Seq (Opcional, para visualização de logs)

### Configuração (appsettings.json)
Certifique-se de configurar a string de conexão no arquivo `appsettings.Development.json` dentro da pasta da API:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVER;Database=SEU_BANCO;User Id=user;Password=pass;"
  },
  "Logging": {
    "Seq": { "ServerUrl": "http://localhost:5341", "ApiKey": "" }
  }
}

```

### Executando a API

No terminal, navegue até a raiz da solução e execute:

```bash
# Restaurar dependências
dotnet restore

# Compilar
dotnet build

# Rodar a Minimal API
dotnet run --project Nexus.PortalAgendamento.MinimalApi

```

A API iniciará (geralmente em `http://localhost:5272` ou `https://localhost:7144`).

---

## 📚 Documentação (OpenAPI/Scalar)

O projeto utiliza **Scalar** (interface moderna para OpenAPI) para documentação interativa.

> **Acesso:** Com a API rodando em modo *Development*, acesse:
> 👉 **[https://localhost:7144/scalar/v1](https://www.google.com/search?q=https://localhost:7144/scalar/v1)**

Lá você pode visualizar os esquemas, testar requisições e ver exemplos de retorno.

---

## 🔌 Rotas Principais

A API organiza as rotas utilizando o prefixo global `/apis.portalagendamento/v1`.

| Grupo | Verbo | Rota (Resumida) | Descrição |
| --- | --- | --- | --- |
| **Health** | `GET` | `/` | Verifica se a API está online. |
| **Voucher** | `POST` | `/portal-agendamento/voucher/{idCliente}` | Gera tratativa de ocorrência e anexa arquivo. |
| **Cliente** | `GET` | `/portal-agendamento/cliente/{idCliente}` | Consulta dados cadastrais do cliente. |
| **Agendamento** | `GET` | `/portal-agendamento/data-agendamento-confirmacao/...` | Retorna dados de confirmação. |
| **Agendamento** | `PUT` | `/portal-agendamento/salvar/...` | Atualiza a data de agendamento. |
| **PDF** | `POST` | `/portal-agendamento/data-agendamento-pdf` | Extrai dados de agendamento via upload de PDF. |
| **Notas** | `GET` | `/portal-agendamento/notas/{idCliente}` | Lista notas fiscais vinculadas. |
| **Token** | `GET` | `/portal-agendamento/validade/{idCliente}` | Verifica validade do token de acesso. |
| **Email** | `POST` | `/portal-agendamento/email/{idCliente}` | Envia e-mail com anexo. |

---

## 📂 Estrutura de Pastas

```bash
Nexus.PortalAgendamento
├── 📂 Nexus.PortalAgendamento.MinimalApi  # Projeto Web API (Entry Point)
│   ├── 📂 Common                          # Interfaces (IEndpoint)
│   ├── 📂 Endpoints                       # Definição das Rotas (Vertical Slices)
│   │   └── 📂 PortalAgendamento           # Endpoints de Domínio
│   └── 📄 Program.cs                      # Configuração, Middleware e DI
│
└── 📂 Nexus.PortalAgendamento.Library     # Biblioteca de Classes (Core)
    ├── 📂 Infrastructure
    │   ├── 📂 Constants                   # Nomes de Procedures
    │   ├── 📂 Domain                      # Modelos (Input/Output/Entity)
    │   ├── 📂 Helper                      # Utilitários (PDF, Email)
    │   ├── 📂 Repository                  # Acesso a Dados
    │   └── 📂 Services                    # Regras de Negócio
    └── 📄 ServiceCollectionExtensions.cs  # Injeção de Dependência da Library

```
