FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./CautionaryAlertsApi.sln ./
COPY ./CautionaryAlertsApi/CautionaryAlertsApi.csproj ./CautionaryAlertsApi/
COPY ./CautionaryAlertsApi.Tests/CautionaryAlertsApi.Tests.csproj ./CautionaryAlertsApi.Tests/

RUN dotnet restore ./CautionaryAlertsApi/CautionaryAlertsApi.csproj
RUN dotnet restore ./CautionaryAlertsApi.Tests/CautionaryAlertsApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out CautionaryAlertsApi.Tests/CautionaryAlertsApi.Tests.csproj

CMD dotnet test
