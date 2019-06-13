using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBAC.DeveloperInterview.Model;

namespace WBAC.DeveloperInterview
{
    public class ValuationEngine : IValuationEngine
    {
        private readonly IVehicleFinder _vehicleFinder;
        private readonly decimal _minimumValuation;

        public ValuationEngine(IVehicleFinder vehicleFinder , decimal minimumValuation)
        {
            _vehicleFinder = vehicleFinder;
            _minimumValuation = minimumValuation;
        }

        public Valuation Generate(string regPlate, int mileage)
        {
            var vehicle = _vehicleFinder.FindByRegPlate(regPlate);

            if (vehicle == null)
            {
                return new Valuation
                {
                    Errors = new List<string>
                    {
                        "we can't identify the car"
                    }
                };
            }

            if (!vehicle.BaseValuation.HasValue)
            {
                return new Valuation
                {
                    Errors = new List<string>
                    {
                        "we can't value a car!"
                    }
                };
            }

            var priceOffered = PriceReductionForAge(vehicle.AgeInYears, vehicle.BaseValuation.Value);

            priceOffered = PriceReductionForMileage(mileage, vehicle.AgeInYears,  priceOffered);

            return new Valuation()
            {
                Mileage = mileage,
                RegPlate = regPlate,
                AgeInYears = vehicle.AgeInYears,
                PriceOffered = priceOffered < _minimumValuation
                    ? decimal.Round(_minimumValuation, 2, MidpointRounding.AwayFromZero)
                    : decimal.Round(priceOffered, 2, MidpointRounding.AwayFromZero)
            };
        }

        public decimal PriceReductionForMileage(int mileage, int age, decimal priceOffered)
        {
            if (mileage > 0)
            {
                // 3 years old or less, reduce by 0.5% per 1000 miles.
                if (age <= 3)
                {
                    PriceReducedBy(mileage, 0.5, ref priceOffered);
                }

                // 4 to 9 years old, reduce by 0.3 % per 1000 miles.
                if (age >= 4 && age <= 9)
                {
                    PriceReducedBy(mileage, 0.3, ref priceOffered);
                }

                // 10 years and over, reduce by 0.1% per 1000 miles.
                if (age >= 10)
                {
                    PriceReducedBy(mileage,0.1, ref priceOffered);
                }
            }

            return priceOffered;
        }

        private void PriceReducedBy(int mileage, double reduceBy, ref decimal priceOffered)
        {
            var percentageReduceBy = (decimal) (mileage / 1000.0 * reduceBy);
            priceOffered -= priceOffered/100 * percentageReduceBy;
        }


        public decimal PriceReductionForAge(int age, decimal baseValuation)
        {
            // Age 0
            var currentValuation = baseValuation - baseValuation * (decimal) 0.2;

            // reduce by 5% for each year
            if (age > 0)
            {
                for (int currentAge = 1; currentAge <= age; currentAge++)
                {
                    currentValuation -= currentValuation * (decimal) 0.05;
                }
            }
            return currentValuation;
        }
    }
}
