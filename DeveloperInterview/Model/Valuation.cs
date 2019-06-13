using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBAC.DeveloperInterview.Model
{
    public class Valuation
    {
        public int? AgeInYears { get; set; }
        public int Mileage { get; set; }
        public decimal? PriceOffered { get; set; }
        public string RegPlate { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
