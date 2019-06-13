using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBAC.DeveloperInterview.Model
{
    public class Vehicle
    {
        public string RegPlate { get; set; }

        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Derivative { get; set; }

        public int AgeInYears { get; set; }
         
        public decimal? BaseValuation { get; set; }
    }
}
