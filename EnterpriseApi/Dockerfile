# 1. 編譯階段
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 複製專案檔並還原 NuGet
COPY *.csproj ./
RUN dotnet restore

# 複製其餘程式碼並發佈
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 2. 執行階段
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# 設定環境變數
ENV ASPNETCORE_URLS=http://+:80

# 暴露連接埠
EXPOSE 80

# 啟動應用程式
ENTRYPOINT ["dotnet", "EnterpriseApi.dll"]
