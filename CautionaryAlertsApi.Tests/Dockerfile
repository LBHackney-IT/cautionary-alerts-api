FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

RUN dotnet tool install --global dotnet-sonarscanner --version 5.6.0

# Copy csproj and restore as distinct layers
COPY ./CautionaryAlertsApi.sln ./
COPY ./CautionaryAlertsApi/CautionaryAlertsApi.csproj ./CautionaryAlertsApi/
COPY ./CautionaryAlertsApi.Tests/CautionaryAlertsApi.Tests.csproj ./CautionaryAlertsApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./CautionaryAlertsApi.Tests/CautionaryAlertsApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out CautionaryAlertsApi.Tests/CautionaryAlertsApi.Tests.csproj

CMD dotnet test
