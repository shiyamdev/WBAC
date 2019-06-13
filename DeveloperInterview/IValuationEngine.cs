using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBAC.DeveloperInterview.Model;

namespace WBAC.DeveloperInterview
{
    public interface IValuationEngine
    {
        Valuation Generate(string regPlate, int mileage);
    }
}
