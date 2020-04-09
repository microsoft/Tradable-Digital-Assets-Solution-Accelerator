using Contoso.DigitalGoods.DigitalLocker.App.Models;
using Contoso.DigitalGoods.ProductCatalog.Service;
using Contoso.DigitalGoods.TokenAPI.Proxy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalLocker.App
{
    public class CryptoGoodLocker : IDigitalLocker
    {
        //private string tokenServiceURL;
        private Client tokenServiceClient;
        private IProductCatalogManager productCatalogManager;

        public CryptoGoodLocker(IProductCatalogManager ProductCatalogManager, string TokenServiceURL)
        {
            //tokenServiceURL = TokenServiceURL;
            tokenServiceClient = new Client(TokenServiceURL, new System.Net.Http.HttpClient());
            productCatalogManager = ProductCatalogManager;
        }

        public async Task<ICollection<DigitalLockerItem>> GetAllUserDigitalLockerItems(string ABTUserID)
        {
            try
            {
                var assets = await tokenServiceClient.UsersAllAsync(ABTUserID);
                if (assets.Count > 0)
                {
                    List<DigitalLockerItem> items = new List<DigitalLockerItem>();

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
                    return new DigitalLockerItem[] { };
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DigitalLockerItem> GetCryptoGoodFromDigitalLockerByTokenNumber(string ABTUserID, long TokenNumber)
        {
            try
            {
                var asset = await tokenServiceClient.TokensAsync(ABTUserID, TokenNumber);

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
