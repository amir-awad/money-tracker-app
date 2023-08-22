# Run the dotnet application
dotnet-build:
	dotnet build
migrate:
	dotnet ef migrations add "makemigrations"
update:
	dotnet ef database update
run:
	dotnet run

# Build Dockerfile and run Docker Compose
docker-build:
	docker build -t money-tracker-app .
up:
	docker-compose up
down:
	docker-compose down
