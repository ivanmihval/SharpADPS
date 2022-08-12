using System;
using CoreADPS.Helpers;
using CoreADPS.MailModels;

namespace CoreADPS.Filters
{
    public class DampingDistanceFilter: IMailParamFilter
    {
        public readonly Coordinates LocationCoordinates;
        public readonly double BaseDistanceMeters;
        public readonly double ThresholdProbability;
        public readonly Random RandomGenerator;

        public DampingDistanceFilter(Coordinates locationCoordinates, double baseDistanceMeters,
                                     double thresholdProbability = 0.05, Random randomGenerator = null)
        {
            LocationCoordinates = locationCoordinates;
            BaseDistanceMeters = baseDistanceMeters;
            ThresholdProbability = thresholdProbability;

            if (randomGenerator == null)
            {
                randomGenerator = new Random();
            }

            RandomGenerator = randomGenerator;
        }

        // "coefficient" is an empiric number which is needed for calculating the base distance. Unit: people per meter.
        public DampingDistanceFilter(Coordinates locationCoordinates, ulong population,
                                     double thresholdProbability = 0.05, double coefficient = 10.0,
                                     Random randomGenerator = null)
            : this(locationCoordinates, population/coefficient, thresholdProbability, randomGenerator)
        {

        }

        

        private bool _isMatchedWithProbability(double probability)
        {
            return RandomGenerator.NextDouble() < probability;
        }

        public double CalculateProbabilityForCoordinates(Coordinates recipientCoordinate)
        {
            var distance = DistanceCalculator.Distance(recipientCoordinate, LocationCoordinates);
            var probability = Math.Pow(2, -distance / BaseDistanceMeters);
            return probability;
        }

        public bool IsFiltered(Mail mail)
        {
            foreach (var recipientCoordinate in mail.RecipientsCoordinates)
            {
                var probability = CalculateProbabilityForCoordinates(recipientCoordinate);
                if ((probability > ThresholdProbability) && _isMatchedWithProbability(probability))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
