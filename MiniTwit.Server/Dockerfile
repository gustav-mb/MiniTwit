FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

ENV ASPNETCORE_URLS=https://+:5001;http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MiniTwit.Server/MiniTwit.Server.csproj", "MiniTwit.Server/"]
RUN dotnet restore "MiniTwit.Server/MiniTwit.Server.csproj"
COPY . .
WORKDIR "/src/MiniTwit.Server"
RUN dotnet build "MiniTwit.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MiniTwit.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MiniTwit.Server.dll"]