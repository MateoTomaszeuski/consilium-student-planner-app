FROM mcr.microsoft.com/dotnet/sdk:9.0 AS test

WORKDIR /src
COPY . .

WORKDIR /src/consilium

RUN dotnet sln remove consilium.maui/Consilium.Maui.csproj
RUN rm -rf consilium.maui

# Fix project paths in solution file (capital -> lowercase)
RUN sed -i 's/Consilium\.Tests/consilium.tests/g' Consilium.sln && \
    sed -i 's/Consilium\.API/consilium.api/g' Consilium.sln && \
    sed -i 's/Consilium\.IntegrationTests/consilium.integrationtests/g' Consilium.sln && \
    sed -i 's/Consilium\.Shared/consilium.shared/g' Consilium.sln

RUN dotnet restore
RUN dotnet format --verify-no-changes --no-restore

RUN dotnet build --warnaserror
