using Contoso.DigitalGoods.Application.API.Models;
using Contoso.DigitalGoods.Application.Models;
using Contoso.DigitalGoods.ProductCatalog.Service;
using Contoso.DigitalGoods.ProductCatalog.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.Application.API.Controllers
{
    //[Authorize]
    [Route("DigitalGoodsApp/Management/Products")]
    [ApiController]
    public class ProductCatalogController : ControllerBase
    {

        private IProductCatalogManager productCatalogManager;

        public ProductCatalogController(IHttpContextAccessor httpContextAccessor, IProductCatalogManager ProductCatalogManager)
        {
            productCatalogManager = ProductCatalogManager;

            //get ContosoUserIdentifier from JwtToken
            //var ContosoUserIdentifier = JWTUtil.GetContosoIDFromJWT(httpContextAccessor);
        }



        [HttpGet]
        [Route("{ProductID}")]
        public IMessage<ProductInfo> GetProductInfoAsync(string ProductID)
        {
            var result = productCatalogManager.GetProductInfo(ProductID);

            if (result != null)
            {
                var productCatalog = new ProductCatalogInfo()
                {
                    Success = true,
                    Data = result
                };

                return productCatalog;
            }
            else
            {
                return new ProductCatalogInfo()
                {
                    Success = false,
                    Data = null,
                    Message = $"There is no Product for this id : {ProductID}"
                };
            }
        }

        [HttpGet]
        public IObjectCollectionMessage<ProductInfo> GetAllProductInfos()
        {
            var result = productCatalogManager.GetAllProducts().ToList();

            if (result != null)
            {
                var productCatalog = new ProductCatalogInfos()
                {
                    Success = true,
                    Data = result
                };

                return productCatalog;
            }
            else
            {
                return new ProductCatalogInfos()
                {
                    Success = false,
                    Data = null,
                    Message = $"There is no Products"
                };
            }
        }

        [HttpPost]
        public async Task<IMessage<ProductInfo>> RegisterProduct(ProductInfo Product)
        {
            try
            {
                var result = await productCatalogManager.RegisterProductCatalog(Product);
                var productCatalog = new ProductCatalogInfo()
                {
                    Success = true,
                    Data = result
                };

                return productCatalog;

            }
            catch (Exception)
            {
                var productCatalog = new ProductCatalogInfo()
                {
                    Success = false,
                    Data = null,
                    Message = "Failed adding a Product in ProductCatalog"

                };
                return productCatalog;
            }
        }

        [HttpDelete]
        [Route("{ProductID}")]
        public IMessage<ProductInfo> UnRegisterProduct(string ProductID)
        {
            try
            {
                var result = productCatalogManager.UnRegisterProductCatalog(ProductID);
                var productCatalog = new ProductCatalogInfo()
                {
                    Success = true,
                    Data = null,
                    Message = $"the Product - {ProductID} has been removed from Product Catalog"
                };

                return productCatalog;

            }
            catch (Exception)
            {
                var productCatalog = new ProductCatalogInfo()
                {
                    Success = false,
                    Data = null,
                    Message = $"the Product - {ProductID} didn't removed from Product Catalog"
                };
                return productCatalog;
            }
        }


    }
}
