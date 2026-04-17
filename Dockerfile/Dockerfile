# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file giải pháp và file dự án để restore dependencies
COPY ["VANTHINGOCHANG-2123110352.sln", "./"]
COPY ["VANTHINGOCHANG-2123110352/VANTHINGOCHANG-2123110352.csproj", "VANTHINGOCHANG-2123110352/"]

# Restore các gói NuGet
RUN dotnet restore "VANTHINGOCHANG-2123110352/VANTHINGOCHANG-2123110352.csproj"

# Copy toàn bộ mã nguồn và build
COPY . .
WORKDIR "/src/VANTHINGOCHANG-2123110352"
RUN dotnet build "VANTHINGOCHANG-2123110352.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "VANTHINGOCHANG-2123110352.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Cấu hình cổng chạy (ASP.NET Core 8 mặc định dùng cổng 8080)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "VANTHINGOCHANG-2123110352.dll"]