# Manual Startup locate mpc_dotnetc_user_server.exe in the bin/... folder

# Docker Startup using compose in CLI.
docker compose -f mpc_dotnetc_user_server.yaml up -d

# Docker Startup using manual build commands in CLI.
docker build -t mpc_dotnetc_user_server_rom .
docker run -d -p {SERVER_NETWORK_PORT_NUMBER}:{DOCKER_CONTAINER_PORT_NUMBER} --name mpc_dotnetc_user_server mpc_dotnetc_user_server_rom

# Get to API List via Swagger via browser {domain}/swagger/index.html
In Example: locahost:8080/swagger/index.html except whatever is configured in your .env file.