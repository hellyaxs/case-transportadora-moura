FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY Api.csproj ./

RUN dotnet restore Api.csproj

COPY . .

RUN dotnet publish Api.csproj \
  --configuration Release \
  --output /app/publish \
  --no-restore \
  /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
