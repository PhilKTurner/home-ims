FROM hims-build-env:latest AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
#COPY *.csproj ./
#RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet restore HomeIMS
RUN dotnet build HomeIMS -c Release -o /app/build
RUN dotnet publish HomeIMS -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "HomeIMS.dll"]
