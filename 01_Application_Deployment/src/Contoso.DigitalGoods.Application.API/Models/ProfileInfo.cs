using Contoso.DigitalGoods.Application.Models;
using Model = Contoso.DigitalGoods.ContosoProfile.Service.Models;

namespace Contoso.DigitalGoods.Application.API.Models
{
    public class ProfileInfo : IMessage<Model.ContosoProfile>
    {
        public bool Success { get; set; }
        public Model.ContosoProfile Data { get; set; }
        public string Message { get; set; }
    }


}
