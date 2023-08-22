# Makefile for Running dotnet and Docker Compose Commands with Batch Scripts on Windows

# Run the dotnet application
build:
	dotnet build
migrate:
	dotnet ef migrations add "makemigrations"
update:
	dotnet ef database update
run:
	dotnet run

# Build Dockerfile and run Docker Compose
build Dockerfile:
	docker build -t money-tracker-app .
up:
	docker-compose up
