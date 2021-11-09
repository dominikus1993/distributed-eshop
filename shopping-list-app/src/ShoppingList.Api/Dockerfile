FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/nightly/sdk:6.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/src/Chat.App
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build -c Release -o /app


FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
ENV TRACER_VERSION=1.29.0
RUN mkdir -p /var/log/datadog
RUN mkdir -p /opt/datadog
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER_PATH=/app/datadog/linux-x64/Datadog.Trace.ClrProfiler.Native.so
ENV DD_INTEGRATIONS=/app/datadog/integrations.json
ENV DD_DOTNET_TRACER_HOME=/app/datadog
ENV DD_LOGS_INJECTION=true
ENV DD_ADONET_ENABLED=true
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShoppingList.Api.dll"]