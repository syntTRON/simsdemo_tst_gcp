docker build -t simsdb .
docker run --name simsdb1 -p5432:5432 -e POSTGRES_PASSWORD=1234 -e POSTGRES_USER=postgresadmin -e POSTGRES_DB=db1 -d simsdb
