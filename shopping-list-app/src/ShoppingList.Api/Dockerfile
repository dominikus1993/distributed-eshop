FROM mcr.microsoft.com/dotnet/nightly/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/nightly/sdk:6.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/src/ShoppingList.Api
RUN dotnet restore -nowarn:msb3202,nu1503
RUN dotnet build -c Release -o /app


FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENV DD_VERSION=v1.0.0
ENV TRACER_VERSION=1.29.0
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/app/datadog/linux-x64/Datadog.Trace.ClrProfiler.Native.so
ENV DD_INTEGRATIONS=/app/datadog/integrations.json
ENV DD_DOTNET_TRACER_HOME=/app/datadog

RUN /app/datadog/createLogPath.sh
ENTRYPOINT ["dotnet", "ShoppingList.Api.dll"]