using Contoso.DigitalGoods.DigitalLocker.App.Models;
using Contoso.DigitalGoods.ProductCatalog.Service;
using Contoso.DigitalGoods.TokenAPI.Proxy;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalLocker.App
{
    public class CryptoGoodLocker : IDigitalLocker
    {
        //private string tokenServiceURL;
        private Client tokenServiceClient;
        private IProductCatalogManager productCatalogManager;
       
        public CryptoGoodLocker(HttpClient HttpClient, IConfiguration Configuration, IProductCatalogManager ProductCatalogManager)
        {
            tokenServiceClient = new Client(Configuration["Values:tokenAPIURL"], HttpClient);
            productCatalogManager = ProductCatalogManager;
        }

        public async Task<ICollection<DigitalLockerItem>> GetAllUserDigitalLockerItems(string NFTServiceUserID)
        {
            try
            {
                var assets = await tokenServiceClient.GetUserDigitalKicsFromDigitalLockerAsync(NFTServiceUserID);
                if (assets.Count > 0)
                {
                    List<DigitalLockerItem> items = new ();

                    foreach (var asset in assets)
                    {
                        items.Add(new DigitalLockerItem()
                        {
                            Name = asset.Name,
                            ProductId = asset.ProductId,
                            TokenNumber = asset.TokenNumber,
                            ProductDetail = productCatalogManager.GetProductInfo(asset.ProductId)
                        });
                    }

                    return items;
                }
                else
                {
                    return Array.Empty<DigitalLockerItem>();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DigitalLockerItem> GetCryptoGoodFromDigitalLockerByTokenNumber(string NFTServiceUserID, long TokenNumber)
        {
            try
            {
                var asset = await tokenServiceClient.GetDigitalGoodFromDigitalLockerAsync(NFTServiceUserID, TokenNumber);

                if (asset != null)
                {
                    return new DigitalLockerItem()
                    {
                        Name = asset.Name,
                        ProductId = asset.ProductId,
                        TokenNumber = asset.TokenNumber,
                        ProductDetail = productCatalogManager.GetProductInfo(asset.ProductId)
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
