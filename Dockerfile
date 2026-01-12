# --------------------------------------------------------
# 1. Etapa Base (Runtime)
# --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

# [OPCIONAL] Instalação de libs gráficas para PDF/OCR no Linux
# Descomente as linhas abaixo se tiver erro de "System.Drawing" ou "libgdiplus"
# RUN apt-get update && apt-get install -y libgdiplus libc6-dev && apt-get clean

# --------------------------------------------------------
# 2. Etapa de Build (SDK)
# --------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Recebe o token via argumento de build (Segurança)
ARG FEED_PAT

# 2.1 Copia APENAS arquivos de configuração primeiro (para Cache)
COPY ["nuget.config", "."]
COPY ["Nexus.PortalAgendamento.MinimalApi/Nexus.PortalAgendamento.MinimalApi.csproj", "Nexus.PortalAgendamento.MinimalApi/"]
COPY ["Nexus.PortalAgendamento.Library/Nexus.PortalAgendamento.Library.csproj", "Nexus.PortalAgendamento.Library/"]

# 2.2 Configura Nuget Autenticado
# O token vem da variável ARG, não fica salvo no arquivo
RUN dotnet nuget update source Feed-Tragetta \
    --username "nathan.silva.ext@tragetta.com.br" \
    --password "${FEED_PAT}" \
    --store-password-in-clear-text \
    --configfile nuget.config

# 2.3 Restaura dependências (Isso fica em cache se o csproj não mudar)
RUN dotnet restore "Nexus.PortalAgendamento.MinimalApi/Nexus.PortalAgendamento.MinimalApi.csproj"

# 2.4 Copia o resto do código fonte
COPY . .

# 2.5 Build e Publish
WORKDIR "/src/Nexus.PortalAgendamento.MinimalApi"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# --------------------------------------------------------
# 3. Etapa Final
# --------------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Garante que a pasta tessdata e outros arquivos estáticos venham junto
# (Se não estiverem configurados como "CopyAlways" no .csproj)
# COPY --from=build /src/Nexus.PortalAgendamento.Library/tessdata ./tessdata

ENTRYPOINT ["dotnet", "Nexus.PortalAgendamento.MinimalApi.dll"]