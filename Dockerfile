# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["ASP.csproj", "./"]
RUN dotnet restore "ASP.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "ASP.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "ASP.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Cấu hình cổng chạy (Dùng cổng 8080 cho môi trường Cloud)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ASP.dll"]
