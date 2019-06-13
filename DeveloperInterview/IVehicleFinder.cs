using WBAC.DeveloperInterview.Model;

namespace WBAC.DeveloperInterview
{
    public interface IVehicleFinder
    {
        Vehicle FindByRegPlate(string regPlate);
    }
}