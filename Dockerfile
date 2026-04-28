FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN ls -R   

WORKDIR /src/mytown
RUN dotnet restore mytown.csproj
RUN dotnet publish mytown.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "mytown.dll", "--urls", "http://0.0.0.0:80"]