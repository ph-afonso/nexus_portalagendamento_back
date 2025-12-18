# Etapa base de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia MinimalApi E Library (dependência)
COPY Nexus.PortalAgendamento.MinimalApi/ ./Nexus.PortalAgendamento.MinimalApi/
COPY Nexus.PortalAgendamento.Library/ ./Nexus.PortalAgendamento.Library/
COPY nuget.config ./

RUN echo "PAT recebido: ${FEED_PAT}"

# Configura o feed privado (SEM ponto-e-vírgula no final!)
RUN dotnet nuget update source Feed-Tragetta \
    --username "natan.veloso.ext@tragetta.com.br" \
    --password "1EzQ89tu17sqtAMgVC5aHYjZsHpuTA8wUVrr1KgVmvGfacDvvEvBJQQJ99BKACAAAAATkJiBAAASAZDO4ahz" \
    --store-password-in-clear-text \
    --configfile nuget.config

# Restore e Build
WORKDIR /src/Nexus.PortalAgendamento.MinimalApi
RUN dotnet restore
RUN dotnet build -c Release -o /app/build

# Publicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nexus.PortalAgendamento.MinimalApi.dll"]
