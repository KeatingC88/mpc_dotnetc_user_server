While in cmd prompt of this folder where the docker file is, then do the following:

1) Create Docker Image ROM

CMD: docker build -t {container-rom/img} .
{} is the name of the container/usually the folder name that gets saved into docker engine or repo.

Example CMD: (dont forget that . at the end of that cmd)

docker build -t mpc_sqlite_users_db_rom .

2) Run the image-container and access Sqlite3.exe in the cmd prompt (you've connected to Sqlite-cli).

CMD: 
docker run -it --name {folder-name} --rm {sqlite-container-name}

Example CMD: 
docker run -it --name mpc_sqlite_users_db --rm mpc_sqlite_users_db_rom


3) With Yaml: (not working yet).
docker run -d -p 5172:80 --name mpc_sqlite_users_db mpc_sqlite_users_db_rom
docker compose -f mpc_sqlite_users_db.yaml up -d
