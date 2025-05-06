# Use .NET 8 SDK for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY . ./

RUN dotnet restore
RUN dotnet publish -c Release -o out

# Use .NET 8 runtime for final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=build /app/out .

EXPOSE ${DOCKER_CONTAINER_PORT_NUMBER}

#Copy SQLite Database
WORKDIR /app
COPY --from=build /app/out .
COPY bin/Debug/net8.0/mpc_sqlite_users_database/ /app/mpc_sqlite_users_database/

ENTRYPOINT ["dotnet", "mpc_dotnetc_user_server.dll"]
