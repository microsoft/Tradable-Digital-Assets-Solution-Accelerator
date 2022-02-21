using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Contoso.DigitalGoods.Application.API.Utils
{
    public class JWTUtil
    {
        public static string GetContosoIDFromJWT(IHttpContextAccessor httpContextAccessor)
        {
            //Retrive Contoso User Identifier from Contoso's JWT definition 
            if (httpContextAccessor.HttpContext.User.HasClaim(c => c.Type == "prn"))
            {
                return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "prn").Value;
            }
            else
            {
                return null;
            }
        }
    }
}
