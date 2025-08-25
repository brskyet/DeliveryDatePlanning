FROM mcr.microsoft.com/dotnet/aspnet:6.0.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DeliveryDatePlanning.BackgroundService/DeliveryDatePlanning.BackgroundService.csproj", "DeliveryDatePlanning.BackgroundService/"]
COPY ["DeliveryDatePlanning.Data/DeliveryDatePlanning.Data.csproj", "DeliveryDatePlanning.Data/"]
COPY ["DeliveryDatePlanning.Domain/DeliveryDatePlanning.Domain.csproj", "DeliveryDatePlanning.Domain/"]
COPY ["DeliveryDatePlanning.Protocol/DeliveryDatePlanning.Protocol.csproj", "DeliveryDatePlanning.Protocol/"]
COPY ["DeliveryDatePlanning.Application/DeliveryDatePlanning.Application.csproj", "DeliveryDatePlanning.Application/"]
RUN dotnet restore --source "https://api.nuget.org/v3/index.json" --source "http://10.7.8.172/nuget" "DeliveryDatePlanning.BackgroundService/DeliveryDatePlanning.BackgroundService.csproj"
COPY . .
WORKDIR "/src/DeliveryDatePlanning.BackgroundService"
RUN dotnet build "DeliveryDatePlanning.BackgroundService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeliveryDatePlanning.BackgroundService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN ln -sf /usr/share/zoneinfo/Europe/Moscow /etc/localtime
ENTRYPOINT dotnet DeliveryDatePlanning.BackgroundService.dll --environment=$ENVIRONMENT
