FROM mcr.microsoft.com/dotnet/sdk:5.0 as base

COPY src/UserAPI.Contracts /src/UserAPI.Contracts
COPY tests/UserAPI.E2E /tests/UserAPI.E2E

WORKDIR /tests/UserAPI.E2E

RUN dotnet build -c Release

ENTRYPOINT ["dotnet", "test", "--no-build"]