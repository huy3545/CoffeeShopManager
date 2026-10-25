FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish CoffeeShopManager.Web/CoffeeShopManager.Web.csproj -c Release -o /out
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/data/CoffeeShopDB.db"
EXPOSE 8080
ENTRYPOINT ["dotnet", "CoffeeShopManager.Web.dll"]
