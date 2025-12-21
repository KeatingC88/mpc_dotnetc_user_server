# Launch SQLITE to Set the Database Tables
dotnet ef migrations add InitialCreate
dotnet ef database update

# Manual CLI build command: 
1) Navigate CLI to folder and use
dotnet build

# Manual CLI startup command: 
1) Navigate CLI to folder and use
dotnet run

# Manual file startup (post build)
1) Locate the mpc_dotnetc_user_server.exe in the bin folder and run it

# Docker Option 1) for CLI command:
1) Navigate CLI to folder and use
docker compose -f mpc_dotnetc_user_server.yaml up -d

# Docker Option 2) startup for CLI command:
1) Navigate CLI to folder and use
docker build -t mpc_dotnetc_user_server_rom .
docker run -d -p {SERVER_NETWORK_PORT_NUMBER}:{DOCKER_CONTAINER_PORT_NUMBER} --name mpc_dotnetc_user_server mpc_dotnetc_user_server_rom

# Get to API List via Swagger using a browser {domain}/swagger/index.html
1) Launch the API
2) Open your browser
3) Navigate to something like in this example: locahost:8080/swagger/index.html except instead of locahost:8080 it shall be whatever is configured in your .env file.