# Etap 1: Base - obraz uruchomieniowy
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8082
EXPOSE 8083

# Zmiana domyślnego portu ASP.NET Core na 8082
ENV ASPNETCORE_HTTP_PORTS=8082

# Etap 2: Build - obraz SDK do kompilacji
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Kopiujemy csproj i przywracamy zależności (dla lepszego cache'owania warstw)
COPY ["BiznesSpoter.Web/BiznesSpoter.Web.csproj", "BiznesSpoter.Web/"]
RUN dotnet restore "./BiznesSpoter.Web/BiznesSpoter.Web.csproj"

# Kopiujemy resztę plików kodu źródłowego
COPY . .
WORKDIR "/src/BiznesSpoter.Web"
RUN dotnet build "./BiznesSpoter.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etap 3: Publish - przygotowanie plików do wdrożenia
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BiznesSpoter.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etap 4: Final - ostateczny obraz
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BiznesSpoter.Web.dll"]