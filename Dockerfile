FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todos os arquivos csproj e restaurar dependências
COPY ["src/Motorcycle.Domain/Motorcycle.Domain.csproj", "src/Motorcycle.Domain/"]
COPY ["src/Motorcycle.Application/Motorcycle.Application.csproj", "src/Motorcycle.Application/"]
COPY ["src/Motorcycle.Infrastructure/Motorcycle.Infrastructure.csproj", "src/Motorcycle.Infrastructure/"]
COPY ["src/Motorcycle.API/Motorcycle.API.csproj", "src/Motorcycle.API/"]

# Restaurar pacotes
RUN dotnet restore "src/Motorcycle.API/Motorcycle.API.csproj"

# Copiar o resto do código-fonte
COPY . .

# Compilar o projeto
WORKDIR "/src/src/Motorcycle.API"
RUN dotnet build "Motorcycle.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Motorcycle.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Motorcycle.API.dll"]