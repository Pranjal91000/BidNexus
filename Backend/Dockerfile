FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Backend/BidNexus/BidNexus/API.csproj", "Backend/BidNexus/BidNexus/"]
COPY ["Backend/BidNexus/Core/Core.csproj", "Backend/BidNexus/Core/"]
COPY ["Backend/BidNexus/Infrastructure/Infrastructure.csproj", "Backend/BidNexus/Infrastructure/"]

RUN dotnet restore "Backend/BidNexus/BidNexus/API.csproj"

COPY . .
WORKDIR "/src/Backend/BidNexus/BidNexus"
RUN dotnet publish "API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:\${PORT}
ENTRYPOINT ["dotnet", "API.dll"]
