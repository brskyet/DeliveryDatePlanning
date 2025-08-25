FROM mcr.microsoft.com/dotnet/aspnet:6.0.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DeliveryDatePlanning.WebApi/DeliveryDatePlanning.WebApi.csproj", "DeliveryDatePlanning.WebApi/"]
COPY ["DeliveryDatePlanning.Data/DeliveryDatePlanning.Data.csproj", "DeliveryDatePlanning.Data/"]
COPY ["DeliveryDatePlanning.Domain/DeliveryDatePlanning.Domain.csproj", "DeliveryDatePlanning.Domain/"]
COPY ["DeliveryDatePlanning.Protocol/DeliveryDatePlanning.Protocol.csproj", "DeliveryDatePlanning.Protocol/"]
COPY ["DeliveryDatePlanning.Application/DeliveryDatePlanning.Application.csproj", "DeliveryDatePlanning.Application/"]
RUN dotnet restore --source "https://api.nuget.org/v3/index.json" "DeliveryDatePlanning.WebApi/DeliveryDatePlanning.WebApi.csproj"
COPY . .
WORKDIR "/src/DeliveryDatePlanning.WebApi"
RUN dotnet build "DeliveryDatePlanning.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DeliveryDatePlanning.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

RUN ln -sf /usr/share/zoneinfo/Europe/Moscow /etc/localtime
ENTRYPOINT dotnet DeliveryDatePlanning.WebApi.dll --environment=$ENVIRONMENT
