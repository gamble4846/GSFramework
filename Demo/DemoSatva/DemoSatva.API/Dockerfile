FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["DemoSatva.API/DemoSatva.API.csproj", "DemoSatva.API/"]
RUN dotnet restore "DemoSatva.API/DemoSatva.API.csproj"
COPY . .
WORKDIR "/src/DemoSatva.API"
RUN dotnet build "DemoSatva.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DemoSatva.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DemoSatva.API.dll"]
