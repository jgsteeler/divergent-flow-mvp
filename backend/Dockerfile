# Fly.io deployment Dockerfile (backend)
# Build context: backend/

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Restore (cache-friendly)
COPY DivergentFlow.sln ./
COPY DivergentFlow.Api/DivergentFlow.Api.csproj DivergentFlow.Api/
COPY DivergentFlow.Services/DivergentFlow.Services.csproj DivergentFlow.Services/
COPY DivergentFlow.Api.Tests/DivergentFlow.Api.Tests.csproj DivergentFlow.Api.Tests/

RUN dotnet restore DivergentFlow.sln

# Build/publish
COPY . ./
RUN dotnet publish DivergentFlow.Api/DivergentFlow.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./

# Use a single wildcard binding (avoids IPv4/IPv6 double-bind conflicts).
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "DivergentFlow.Api.dll"]
