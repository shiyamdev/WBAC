using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WBAC.DeveloperInterview;
using WBAC.DeveloperInterview.Model;
using Moq;

namespace WBAC.DeveloperInterview.Tests.ValuationServiceTests
{
    [TestClass]
    public class GenerateTests
    {
        private Mock<IVehicleFinder> _vehicleFinderMock;
        private decimal _minimumValuation;

        [TestInitialize]
        public void SetUp()
        {
            _vehicleFinderMock = new Mock<IVehicleFinder>();
            _minimumValuation = 250;
        }

        private Vehicle GetVehicle(decimal? baseValuation, int ageInYears)
        {
            Vehicle vehicle = new Vehicle()
            {
                AgeInYears = ageInYears,
                BaseValuation = baseValuation,
                Derivative = "ZETEC",
                Manufacturer = "FORD",
                Model = "FIESTA",
                RegPlate = "MA53JRO"
            };
            return vehicle;
        }


        [TestMethod]
        public void Generate_ReturnsPopulatedValuation()
        {
            // arrange
            string regPlate = "MA53JRO";
            int mileage = 100000;

            var vehicle = GetVehicle(40000, 10);
            
            _vehicleFinderMock.Setup(x => x.FindByRegPlate(regPlate)).Returns(vehicle);

            IValuationEngine valuationService = new ValuationEngine(_vehicleFinderMock.Object, _minimumValuation);

            // act
            var result = valuationService.Generate(regPlate, mileage);

            // assert
            Assert.IsInstanceOfType(result, typeof(Valuation));

            Assert.AreEqual(mileage, result.Mileage);
            Assert.AreEqual(regPlate, result.RegPlate);
            Assert.IsNotNull(result.AgeInYears);
            Assert.IsNotNull(result.PriceOffered);
        }

        // Final new price
        [TestMethod]
        public void Generate_VehicleValuation_ReturnsNewPrice()
        {
            // arrange
            string regPlate = "MA53JRO";
            int mileage = 100000;
            var expectedPrice = (decimal)17243.62;

            var vehicle = GetVehicle(40000, 10);

            _vehicleFinderMock.Setup(x => x.FindByRegPlate(regPlate)).Returns(vehicle);

            IValuationEngine valuationService = new ValuationEngine(_vehicleFinderMock.Object, _minimumValuation);

            // act
            var result = valuationService.Generate(regPlate, mileage);

            // assert
            Assert.AreEqual(expectedPrice, result.PriceOffered);
        }


        // Reduce based on mileage
        [TestMethod]
        public void PriceReductionForMileage_ReducesValuation_BasedOnMileageAges()
        {
            // arrange
            int mileage = 50000;

            var vehicleLessThan3YearsOldExpectedResult = 33750;
            var vehicleLessThan9YearsOldExpectedResult = 38250;
            var vehicleGreaterThan10YearsOldExpectedResult = 42750;

            ValuationEngine valuationService = new ValuationEngine(_vehicleFinderMock.Object, _minimumValuation);

            // act
            var resultLessThan3YearsOld = valuationService.PriceReductionForMileage(mileage, 3, 45000);
            var resultLessThan9YearsOld = valuationService.PriceReductionForMileage(mileage,   9, 45000);
            var resultGreaterThan10YearsOld = valuationService.PriceReductionForMileage(mileage, 12, 45000);

            // assert
            Assert.AreEqual(vehicleLessThan3YearsOldExpectedResult, resultLessThan3YearsOld);
            Assert.AreEqual(vehicleLessThan9YearsOldExpectedResult, resultLessThan9YearsOld);
            Assert.AreEqual(vehicleGreaterThan10YearsOldExpectedResult, resultGreaterThan10YearsOld);
        }

        // Reduce based on age
        [TestMethod]
        public void PriceReductionForAge_ReducesValuation_BasedOnAge()
        {
            var baseValuation = 45000;

            // arrange
            var resultAge0ExpectedResult = 36000;
            var resultAge1ExpectedResult = 34200;
            var resultAge2ExpectedResult = 32490;
            var resultAge4ExpectedResult = (decimal) 29322.23;
            var resultAge10ExpectedResult = (decimal)21554.53;
            var resultAge15ExpectedResult = (decimal)16678.48;

            ValuationEngine valuationService = new ValuationEngine(_vehicleFinderMock.Object, _minimumValuation);

            // act
            var resultAge0 = decimal.Round(valuationService.PriceReductionForAge(0, baseValuation),2, MidpointRounding.AwayFromZero);
            var resultAge1 = decimal.Round(valuationService.PriceReductionForAge(1, baseValuation), 2, MidpointRounding.AwayFromZero);
            var resultAge2 = decimal.Round(valuationService.PriceReductionForAge(2, baseValuation), 2, MidpointRounding.AwayFromZero);
            var resultAge4 = decimal.Round(valuationService.PriceReductionForAge(4, baseValuation), 2, MidpointRounding.AwayFromZero);
            var resultAge10 = decimal.Round(valuationService.PriceReductionForAge(10, baseValuation), 2, MidpointRounding.AwayFromZero);
            var resultAge15 = decimal.Round(valuationService.PriceReductionForAge(15, baseValuation), 2, MidpointRounding.AwayFromZero);

            // assert

            Assert.AreEqual(resultAge0ExpectedResult, resultAge0);
            Assert.AreEqual(resultAge1ExpectedResult, resultAge1);
            Assert.AreEqual(resultAge2ExpectedResult, resultAge2);
            Assert.AreEqual(resultAge4ExpectedResult, resultAge4);
            Assert.AreEqual(resultAge10ExpectedResult, resultAge10);
            Assert.AreEqual(resultAge15ExpectedResult, resultAge15);
        }

        // Minimum Valuation
        [TestMethod]
        public void Generate_VehicleIsLessThanMinimum_Returns250()
        {
            // arrange
            string regPlate = "MA53JRO";
            int mileAge = 200000;

            var vehicle = GetVehicle(15000, 3);
            _vehicleFinderMock.Setup(x => x.FindByRegPlate(regPlate)).Returns(vehicle);

            IValuationEngine valuationService = new ValuationEngine(_vehicleFinderMock.Object, _minimumValuation);

            // act
            var result = valuationService.Generate(regPlate, mileAge);

            // assert
            Assert.AreEqual(250, result.PriceOffered);
        }

        // Errors
        [TestMethod]
        public void Generate_VehicleIsNull_ReturnsCannotIdentifyErrorMessage()
        {
            // arrange
            string regPlate = "LP64NNV";
            int mileage = 18000;

            _vehicleFinderMock.Setup(x => x.FindByRegPlate(regPlate));

            IValuationEngine valuationService = new ValuationEngine(_vehicleFinderMock.Object, _minimumValuation);

            // act
            var result = valuationService.Generate(regPlate, mileage);

            var expectedErrorMessage = "we can't identify the car";

            Assert.IsTrue(result.Errors.Contains(expectedErrorMessage));
        }

        [TestMethod]
        public void Generate_VehicleBaseValueIsNull_ReturnsCannotValueErrorMessage()
        {
            // arrange
            string regPlate = "MA53JRO";
            int mileage = 10000;

            Vehicle vehicle = GetVehicle(null, 10);

            _vehicleFinderMock.Setup(x => x.FindByRegPlate(regPlate)).Returns(vehicle);

            IValuationEngine valuationService = new ValuationEngine(_vehicleFinderMock.Object , _minimumValuation);

            // act
            var result = valuationService.Generate(regPlate, mileage);

            var expectedErrorMessage = "we can't value a car!";

            // assert
            Assert.IsTrue(result.Errors.Contains(expectedErrorMessage));
        }
    }
}

