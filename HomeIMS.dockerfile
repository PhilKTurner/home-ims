FROM hims-build-env AS base
WORKDIR /app

EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80;https://+:443

FROM base AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet build "HomeIMS/HomeIMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeIMS/HomeIMS.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
WORKDIR /app

EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_URLS=http://+:80;https://+:443

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeIMS.dll"]
