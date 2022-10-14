#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["mm-bot/mm-bot.csproj", "mm-bot/"]
RUN dotnet restore "mm-bot/mm-bot.csproj"
COPY . .
WORKDIR "/src/mm-bot"
RUN dotnet build "mm-bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "mm-bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "mm-bot.dll"]