FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 80
EXPOSE 443
#ENV ASPNETCORE_URLS=http://0.0.0.0:80;https://0.0.0.0:443
ENV ASPNETCORE_URLS=http://0.0.0.0:80

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["HomeIMS.sln", "./"]
COPY ["HomeIMS.Client/HomeIMS.Client.csproj", "./HomeIMS.Client/"]
COPY ["HomeIMS.Server/HomeIMS.Server.csproj", "./HomeIMS.Server/"]
RUN dotnet restore
COPY . .
WORKDIR "/src/HomeIMS.Server"
RUN dotnet build "HomeIMS.Server.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "HomeIMS.Server.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeIMS.Server.dll"]
