FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Client.AMQP.Consumers/Client.AMQP.Consumers.csproj", "Client.AMQP.Consumers/"]
RUN dotnet restore "Client.AMQP.Consumers/Client.AMQP.Consumers.csproj"
COPY . .
WORKDIR "/src/Client.AMQP.Consumers"
RUN dotnet build "Client.AMQP.Consumers.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Client.AMQP.Consumers.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Client.AMQP.Consumers.dll"]
