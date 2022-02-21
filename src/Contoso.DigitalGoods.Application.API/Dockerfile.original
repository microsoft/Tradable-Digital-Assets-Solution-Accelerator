#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Contoso.DigitalGoods.Application.API/Contoso.DigitalGoods.Application.API.csproj", "Contoso.DigitalGoods.Application.API/"]
COPY ["Contoso.DigitalGoods.ProductCatalog.Service/Contoso.DigitalGoods.ProductCatalog.Service.csproj", "Contoso.DigitalGoods.ProductCatalog.Service/"]
COPY ["Contoso.DigitalGoods.TokenService.OffChain/Contoso.DigitalGoods.OffChain.csproj", "Contoso.DigitalGoods.TokenService.OffChain/"]
COPY ["Contoso.DigitalGoods.Application.Models/Contoso.DigitalGoods.Application.Models.csproj", "Contoso.DigitalGoods.Application.Models/"]
COPY ["Contoso.DigitalGoods.DigitalLocker.App/Contoso.DigitalGoods.DigitalLocker.App.csproj", "Contoso.DigitalGoods.DigitalLocker.App/"]
COPY ["Contoso.DigitalGoods.TokenAPI.Proxy/Contoso.DigitalGoods.TokenAPI.Proxy.csproj", "Contoso.DigitalGoods.TokenAPI.Proxy/"]
COPY ["Contoso.DigitalGoods.DigitalGoodsGift.Service/Contoso.DigitalGoods.DigitalGoodsGift.Service.csproj", "Contoso.DigitalGoods.DigitalGoodsGift.Service/"]
COPY ["Contoso.DigitalGoods.ContosoProfile.Service/Contoso.DigitalGoods.ContosoProfile.Service.csproj", "Contoso.DigitalGoods.ContosoProfile.Service/"]
COPY ["Contoso.DigitalGoods.TokenService.ServiceWrapper/Contoso.DigitalGoods.TokenService.ServiceWrapper.csproj", "Contoso.DigitalGoods.TokenService.ServiceWrapper/"]
COPY ["Microsoft.Azure.TokenService/Microsoft.Azure.TokenService.csproj", "Microsoft.Azure.TokenService/"]
RUN dotnet restore "Contoso.DigitalGoods.Application.API/Contoso.DigitalGoods.Application.API.csproj"
COPY . .
WORKDIR "/src/Contoso.DigitalGoods.Application.API"
RUN dotnet build "Contoso.DigitalGoods.Application.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contoso.DigitalGoods.Application.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contoso.DigitalGoods.Application.API.dll"]