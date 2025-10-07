# Stage build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/netflix-clone-auth.Api/netflix-clone-auth.Api.csproj ./
RUN dotnet restore "netflix-clone-auth.Api.csproj"

COPY src/netflix-clone-auth.Api/ ./

RUN dotnet publish "netflix-clone-auth.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    /p:PublishReadyToRun=true \
    /p:SelfContained=false

# Stage final
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "netflix-clone-auth.Api.dll"]
