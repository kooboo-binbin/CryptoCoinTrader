FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY *.sln ./
COPY Web/Web.csproj Web/
COPY Core/Core.csproj Core/
COPY Manifest/Manifest.csproj Manifest/
COPY ThirdParties/PusherClient/PusherClient.csproj ThirdParties/PusherClient/
COPY ThirdParties/WebsocketSharp/WebsocketSharp.csproj ThirdParties/WebsocketSharp/
RUN dotnet restore
COPY . .
WORKDIR /src/Web
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Web.dll"]
