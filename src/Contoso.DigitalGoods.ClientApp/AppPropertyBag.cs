using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contoso.DigitalGoods.Application;
using Microsoft.Extensions.Configuration;

namespace Contoso.DigitalGoods.ClientApp
{
    public class AppPropertyBag
    {
        public UserInfo ContosoProductManager { get; set; }
        public UserInfo Contoso { get; set; }
        public UserInfo ContosoUserA { get; set; }
        public UserInfo ContosoUserB { get; set; }
        
        
        public ICollection<ProductInfo>? ProductCatalog { get; set; }
        public ICollection<DigitalGift>? Gifts { get; set; }

        public string TokenID { get; set; }

        public AppPropertyBag(IConfiguration Config)
        {
            this.ContosoProductManager = new UserInfo { Profile = new ContosoProfile() };
            this.Contoso = new UserInfo { Profile = new ContosoProfile() };
            this.ContosoUserA = new UserInfo { Profile = new ContosoProfile { NftServiceUserID = "Should be provisioned", PublicAddress = "Should be provisioned" } };
            this.ContosoUserB = new UserInfo { Profile = new ContosoProfile { NftServiceUserID = "Should be provisioned", PublicAddress = "Should be provisioned" } };

            this.ContosoProductManager.Profile.NftServiceUserID = Config["App:ContosoProductManager_NFTId"];
            this.Contoso.Profile.NftServiceUserID = Config["App:Contoso_NFTId"];
            this.TokenID = Config["App:TokenID"];

            this.ContosoUserA.Profile.ContosoID = Guid.NewGuid().ToString();
            this.ContosoUserB.Profile.ContosoID = Guid.NewGuid().ToString();
        }
    }

    public class UserInfo
    {
        public ContosoProfile? Profile { get; set; }
        public ICollection<DigitalLockerItem>? DigitalLockerItems { get; set; }
    }
}
