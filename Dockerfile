# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers to leverage Docker cache
COPY *.sln .
COPY DocumentManagement/*.csproj ./DocumentManagement/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR "/src/DocumentManagement"
RUN dotnet publish "DocumentManagement.csproj" -c Release -o /app/publish

# Stage 2: Create the final, smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports
# 8080 for HTTP, 8081 for HTTPS (ASP.NET Core default in container)
EXPOSE 8080
EXPOSE 8081

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "DocumentManagement.dll"]