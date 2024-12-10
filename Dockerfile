# Use the official .NET SDK as a build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY ["mpc_dotnetc_user_server.csproj","/app/"]
RUN dotnet restore "mpc_dotnetc_user_server.csproj"
COPY . .

#Build and Publish the Application
WORKDIR /app
RUN dotnet publish "mpc_dotnetc_user_server.csproj" -c Release -o /app/publish
FROM mcr.microsoft.com/dotnet/aspnet:7.0

#Expose Container Port
EXPOSE 80

#Copy SQLite Database
WORKDIR /app
COPY --from=build /app/publish .
COPY mpc_sqlite_users_db/Users.db /app/Users.db

CMD ["dotnet", "mpc_dotnetc_user_server.dll"]