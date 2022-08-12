using System;
using System.Collections.Generic;
using CoreADPS.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCoreADPS.TestHelpers
{
    [TestClass]
    public class CoocrdinatesCalculatorTests
    {
        public Tuple<double, double> MoscowCoordinates = Tuple.Create(55.7558, 37.6173);
        public Tuple<double, double> PodolskCoordinates = Tuple.Create(55.4312, 37.5458);
        public Tuple<double, double> OdintsovoCoordinates = Tuple.Create(55.6733745, 37.2818569);

        public struct CoordsTestData
        {
            public double Lat1;
            public double Lon1;

            public double Lat2;
            public double Lon2;

            public double ExpectedDistanceMeters;
            public double Epsilon;

            public CoordsTestData(double lat1, double lon1, double lat2, double lon2, double expectedDistanceMeters,
                                  double epsilon = 1e-4)
            {
                Lat1 = lat1;
                Lon1 = lon1;

                Lat2 = lat2;
                Lon2 = lon2;

                ExpectedDistanceMeters = expectedDistanceMeters;
                Epsilon = epsilon;
            }

            public CoordsTestData(Tuple<double, double> coords1, Tuple<double, double> coords2, double expectedDistanceMeters, double epsilon = 1e-4)
                : this(coords1.Item1, coords1.Item2, coords2.Item1, coords2.Item2, expectedDistanceMeters, epsilon)
            {}


        }

        [TestMethod]
        public void TestDistanceCalculator()
        {
            var testData = new List<CoordsTestData>
                {
                    new CoordsTestData(PodolskCoordinates, PodolskCoordinates, 0.0),
                    new CoordsTestData(MoscowCoordinates, PodolskCoordinates, 36403.77457293187),
                    new CoordsTestData(PodolskCoordinates, MoscowCoordinates, 36403.77457293187),
                    new CoordsTestData(OdintsovoCoordinates, MoscowCoordinates, 22943.17528478241),
                };

            foreach (var coordsTestData in testData)
            {
                var actualDistance = DistanceCalculator.Distance(coordsTestData.Lat1, coordsTestData.Lon1,
                                                                                coordsTestData.Lat2, coordsTestData.Lon2);
                Assert.IsTrue((actualDistance > coordsTestData.ExpectedDistanceMeters - coordsTestData.Epsilon) && (actualDistance < coordsTestData.ExpectedDistanceMeters + coordsTestData.Epsilon));
            }
        }
    }
}
