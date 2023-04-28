#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WellIntegrityCalculations/WellIntegrityCalculations.csproj", "WellIntegrityCalculations/"]
RUN dotnet restore "WellIntegrityCalculations/WellIntegrityCalculations.csproj"
COPY . .
WORKDIR "/src/WellIntegrityCalculations"
RUN dotnet build "WellIntegrityCalculations.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WellIntegrityCalculations.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final


# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-dotnet-agent

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WellIntegrityCalculations.dll"]