# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Katman cache'i için önce sadece csproj'u kopyala
COPY Chat.Api.csproj .
RUN dotnet restore ./Chat.Api.csproj --disable-parallel

# Sonra tüm kaynak kodu kopyala
COPY . .

# Daha hızlı publish: self-contained değil, app host yok
RUN dotnet publish ./Chat.Api.csproj -c Release -o /out \
    --no-self-contained -p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

# Render 8080'i bekliyor
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_EnableDiagnostics=0

EXPOSE 8080
COPY --from=build /out .
ENTRYPOINT ["dotnet","Chat.Api.dll"]
