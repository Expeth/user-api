FROM mcr.microsoft.com/dotnet/sdk:5.0 as base

WORKDIR /src
COPY . .

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime

WORKDIR /app
COPY --from=base /src/out .

ENTRYPOINT ["dotnet", "UserAPI.Host.dll"]