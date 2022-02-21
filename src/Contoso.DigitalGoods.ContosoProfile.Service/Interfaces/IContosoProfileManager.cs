using System.Threading.Tasks;

namespace Contoso.DigitalGoods.ContosoProfile.Service
{
    public interface IContosoProfileManager
    {
        Models.ContosoProfile GetUserProfileByContosoID(string ContosoID);
        Task<Models.ContosoProfile> ProvisionContosoProfile(string ContosoID);
        bool DeleteProfile(string ContosoId);
    }
}