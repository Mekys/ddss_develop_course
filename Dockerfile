FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish "./src/backend/SocialNetwork/API/API.csproj" -c Release -o "/app/publish" 

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app/publish
COPY --from=build /app/publish .
EXPOSE 5000

ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "API.dll"]