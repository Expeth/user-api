FROM mcr.microsoft.com/dotnet/sdk:5.0 as base

WORKDIR /build

COPY UserAPI.sln .
COPY src/ /build/src

RUN dotnet publish src/UserAPI.Host/ -c Release -o /build/out

FROM mcr.microsoft.com/dotnet/aspnet:5.0 as runtime

WORKDIR /app
COPY --from=base /build/out .

ENTRYPOINT ["dotnet", "UserAPI.Host.dll"]