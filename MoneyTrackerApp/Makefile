# Makefile for Running dotnet and Docker Compose Commands with Batch Scripts on Windows

.PHONY: start dotnet docker-compose

# Default target
start: dotnet docker-compose

# Run the dotnet application
dotnet:
	@echo Starting dotnet...
	@powershell -NoProfile -ExecutionPolicy Bypass -Command "& '.\dotnet_start.bat'"

# Run Docker Compose
docker-compose:
	@echo Starting Docker Compose...
	@powershell -NoProfile -ExecutionPolicy Bypass -Command "& '.\docker_compose_start.bat'"
