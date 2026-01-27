FROM mcr.microsoft.com/dotnet/sdk:9.0 AS test

WORKDIR /src
COPY . .

WORKDIR /src/consilium

RUN dotnet sln remove consilium.maui/Consilium.Maui.csproj
RUN rm -rf consilium.maui

RUN dotnet restore
RUN dotnet format --verify-no-changes --no-restore

RUN dotnet build --warnaserror
