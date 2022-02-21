#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Contoso.DigitalGoods.TokenService.API/Contoso.DigitalGoods.TokenService.API.csproj", "Contoso.DigitalGoods.TokenService.API/"]
COPY ["Contoso.DigitalGoods.TokenService/Contoso.DigitalGoods.TokenService.csproj", "Contoso.DigitalGoods.TokenService/"]
COPY ["Contoso.DigitalGoods.TokenService.TransactionIndexer/Contoso.DigitalGoods.TokenService.TokenTransactionIndexer.csproj", "Contoso.DigitalGoods.TokenService.TransactionIndexer/"]
COPY ["Contoso.DigitalGoods.TokenService.OffChain/Contoso.DigitalGoods.OffChain.csproj", "Contoso.DigitalGoods.TokenService.OffChain/"]
COPY ["Contoso.DigitalGoods.TokenService.TokenNumberManager/Contoso.DigitalGoods.TokenService.TokenNumberManager.csproj", "Contoso.DigitalGoods.TokenService.TokenNumberManager/"]
COPY ["Contoso.DigitalGoods.TokenService.ServiceWrapper/Contoso.DigitalGoods.TokenService.ServiceWrapper.csproj", "Contoso.DigitalGoods.TokenService.ServiceWrapper/"]
COPY ["Contoso.DigitalGoods.DigitalLocker.Service/Contoso.DigitalGoods.DigitalLocker.Service.csproj", "Contoso.DigitalGoods.DigitalLocker.Service/"]
RUN dotnet restore "Contoso.DigitalGoods.TokenService.API/Contoso.DigitalGoods.TokenService.API.csproj"
COPY . .
WORKDIR "/src/Contoso.DigitalGoods.TokenService.API"
RUN dotnet build "Contoso.DigitalGoods.TokenService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contoso.DigitalGoods.TokenService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contoso.DigitalGoods.TokenService.API.dll"]