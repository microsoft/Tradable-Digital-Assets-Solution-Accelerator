using Contoso.DigitalGoods.DigitalLocker.App.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.DigitalGoods.DigitalLocker.App
{
    public interface IDigitalLocker
    {
        Task<ICollection<DigitalLockerItem>> GetAllUserDigitalLockerItems(string ABTUserID);
        Task<DigitalLockerItem> GetCryptoGoodFromDigitalLockerByTokenNumber(string ABTUserID, long TokenNumber);
    }
}