FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["OrderFlow.Worker/OrderFlow.Worker.csproj", "OrderFlow.Worker/"]
COPY ["OrderFlow.Application/OrderFlow.Application.csproj", "OrderFlow.Application/"]
COPY ["OrderFlow.Domain/OrderFlow.Domain.csproj", "OrderFlow.Domain/"]
COPY ["OrderFlow.Infrastructure/OrderFlow.Infrastructure.csproj", "OrderFlow.Infrastructure/"]

RUN dotnet restore "OrderFlow.Worker/OrderFlow.Worker.csproj"

COPY . .

RUN dotnet publish "OrderFlow.Worker/OrderFlow.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderFlow.Worker.dll"]
