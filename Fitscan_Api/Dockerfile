# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["Fitscan_Api.csproj", "."]
RUN dotnet restore "Fitscan_Api.csproj"

# Copy all files and build
COPY . .
RUN dotnet publish "Fitscan_Api.csproj" -c Release -o /app/publish

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fitscan_Api.dll"]
