
# 📅 Nexus.PortalAgendamento

![Build Status](https://img.shields.io/badge/Build-Passing-success?style=for-the-badge&logo=appveyor)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=for-the-badge&logo=docker)
![Status](https://img.shields.io/badge/Status-Development-orange?style=for-the-badge)

Backend robusto e modular responsável pelas regras de negócio, processamento de arquivos e integrações do Portal de Agendamento. Construído sobre o **Nexus Framework** utilizando arquitetura de **Minimal APIs** de alta performance.

---

## 🏗️ Arquitetura e Tecnologias

O projeto segue uma arquitetura moderna, focada em performance e baixa latência:

* **Runtime:** .NET 9.0
* **API:** ASP.NET Core Minimal APIs (baixo overhead).
* **Documentação:** Scalar (OpenAPI/Swagger moderno).
* **Logs:** Serilog + Seq (Rastreabilidade distribuída).
* **OCR/PDF:** iText7 + Tesseract (Extração inteligente de dados em boletos/comprovantes).

### Estrutura da Solução
* **`Nexus.PortalAgendamento.MinimalApi`**: Camada de Apresentação. Contém a configuração do host, injeção de dependência e os *Endpoints* organizados por funcionalidade.
* **`Nexus.PortalAgendamento.Library`**: O coração do sistema (Core). Contém os *Services*, *Repositories*, *Models* e a lógica de integração com o legado.

### O Nexus Framework
O sistema utiliza o ecossistema `Nexus.Framework.*` para padronização:

* **⚡ Data Access:** Otimizado para **Stored Procedures** do SQL Server (Prefixos: `SNG_`, `LST_`, `APP_`).
* **🛡️ NexusResult Pattern:** Padronização de respostas (`IsSuccess`, `ResultData`, `Messages`) garantindo previsibilidade para o Frontend.
* **🔒 Vault:** Gerenciamento seguro de credenciais (opcional).

---

## 🚀 Como Rodar o Projeto

Você pode rodar a aplicação localmente via .NET CLI ou utilizando Docker.

### Opção 1: Rodando Localmente (.NET CLI)

1.  **Pré-requisitos:** .NET SDK 9.0 instalado.
2.  **Configuração:** Verifique o arquivo `appsettings.Development.json`.
3.  **Execução:**

```bash
# Restaurar dependências e ferramentas
dotnet restore

# Rodar a API (Ambiente Development)
dotnet run --project Nexus.PortalAgendamento.MinimalApi --launch-profile "Nexus (Development)"

```

### Opção 2: Rodando com Docker Compose 🐳

Ideal para simular ambientes de Homologação/Produção sem instalar dependências na máquina.

1. **Configuração:** Crie um arquivo `.env` na raiz com seu token do Azure DevOps (`FEED_PAT`).
2. **Execução:**

```bash
# Sobe o ambiente completo (Dev e Prod)
docker-compose up --build -d

# Acessar API Dev: http://localhost:5000
# Acessar API Prod: http://localhost:8080

```

---

## 📚 Documentação (Scalar)

O projeto utiliza **Scalar** para documentação interativa da API. Diferente do Swagger tradicional, oferece uma interface mais limpa e exemplos de código em várias linguagens.

> **Acesso Local (Development/Simulation):**
> 👉 **[https://localhost:7144/scalar/v1](https://www.google.com/search?q=https://localhost:7144/scalar/v1)**

---

## 🔌 Principais Endpoints

A API organiza as rotas sob o prefixo global `/apis.portalagendamento/v1`. Abaixo estão as rotas principais implementadas:

| Grupo | Verbo | Rota (Resumida) | Descrição |
| --- | --- | --- | --- |
| **Health** | `GET` | `/health` | Status da API e Ambiente (Dev/Prod). |
| **Agendamento** | `POST` | `/portal-agendamento/confirmacao` | Aceita a data sugerida e confirma o agendamento. |
| **Agendamento** | `POST` | `/portal-agendamento/confirmar-com-anexo` | Confirma o agendamento vinculando um PDF previamente enviado. |
| **Alteração** | `GET` | `/portal-agendamento/solicitar-alteracao/{id}` | Busca dados e notas disponíveis para troca de data. |
| **Alteração** | `POST` | `/portal-agendamento/solicitar-alteracao` | Processa a solicitação de uma nova data manual. |
| **Upload** | `POST` | `/portal-agendamento/upload-analise` | Recebe PDF, aplica OCR e retorna datas encontradas. |

---

## 📂 Estrutura de Pastas

```bash
Nexus.PortalAgendamento
├── 📂 Nexus.PortalAgendamento.MinimalApi  # Entry Point
│   ├── 📂 Endpoints                       # Definição das Rotas
│   │   └── 📂 PortalAgendamento           # Lógica dos Endpoints (Handlers)
│   ├── 📄 Program.cs                      # Configuração, Middleware e DI
│   └── 📄 appsettings.json                # Configurações de Ambiente
│
└── 📂 Nexus.PortalAgendamento.Library     # Core / Domain
    ├── 📂 Infrastructure
    │   ├── 📂 Domain                      # InputModels, ViewModels
    │   ├── 📂 Helper                      # PdfHelper, EmailHelper
    │   ├── 📂 Repository                  # Acesso ao SQL (Dapper)
    │   └── 📂 Services                    # Regras de Negócio
    └── 📂 tessdata                        # Dados de treinamento OCR

```

```

```
