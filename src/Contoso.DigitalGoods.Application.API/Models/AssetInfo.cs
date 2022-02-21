using Contoso.DigitalGoods.Application.Models;
using Contoso.DigitalGoods.DigitalLocker.App.Models;
using System.Collections.Generic;

namespace Contoso.DigitalGoods.Application.API.Models
{
    public class AssetInfo : IMessage<DigitalLockerItem>
    {
        public bool Success { get; set; }

        public DigitalLockerItem Data { get; set; }
        public string Message { get; set; }
    }

    public class AssetInfos : IObjectCollectionMessage<DigitalLockerItem>
    {
        public AssetInfos()
        {
            Data = new List<DigitalLockerItem>();
        }

        public bool Success { get; set; }
        public ICollection<DigitalLockerItem> Data { get; set; }
        public string Message { get; set; }
    }


}
