#Initial setup creating everything if not found
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

#Copy the published output into another final image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS final-env
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet" , "MoneyTrackerApp.dll" ]