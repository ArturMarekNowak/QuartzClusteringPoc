FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["QuartzClusteringPoC/QuartzClusteringPoC.csproj", "QuartzClusteringPoC/"]
RUN dotnet restore "QuartzClusteringPoC/QuartzClusteringPoC.csproj"
COPY . .
WORKDIR "/src/QuartzClusteringPoC"
RUN dotnet build "QuartzClusteringPoC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuartzClusteringPoC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuartzClusteringPoC.dll"]
