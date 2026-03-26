# =============================================
# Stage 1: BUILD
# Uses the full .NET 9 SDK to restore and publish
# =============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies first
# (Cached as a separate layer — speeds up rebuilds if code changes but not deps)
COPY FinancialTwin.API.csproj .
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Publish the app in Release mode to /app/publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# =============================================
# Stage 2: RUNTIME
# Uses the lightweight ASP.NET 9 runtime image
# The final image contains NO SDK or source code
# =============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy only the published output from the build stage
COPY --from=build /app/publish .

# Tell Docker the app listens on port 8080
EXPOSE 8080

# Set the environment (can be overridden in docker-compose)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Command to run when the container starts
ENTRYPOINT ["dotnet", "FinancialTwin.API.dll"]
