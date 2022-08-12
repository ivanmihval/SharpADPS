using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using CoreADPS.Filters;
using CoreADPS.MailModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestCoreADPS.Helpers;

namespace TestCoreADPS.TestMail
{
    [TestClass]
    public class TestMailFilter
    {
        public Coordinates MoscowCoordinates = new Coordinates(55.75222, 37.61556);
        public Coordinates YekaterinburgCoordinates = new Coordinates(56.8519, 60.6122);

        [TestMethod]
        public void TestDateRange()
        {
            var testData = new List<Tuple<DateTime?, DateTime?, bool>>
                {
                    new Tuple<DateTime?, DateTime?, bool>(new DateTime(2019, 4, 5), new DateTime(2020, 12, 31), false),
                    new Tuple<DateTime?, DateTime?, bool>(new DateTime(2019, 4, 5), null, true),
                    new Tuple<DateTime?, DateTime?, bool>(new DateTime(2021, 4, 5), new DateTime(2022, 12, 31), false),
                    new Tuple<DateTime?, DateTime?, bool>(new DateTime(2019, 4, 5), new DateTime(2022, 12, 31), true),
                    new Tuple<DateTime?, DateTime?, bool>(null, new DateTime(2022, 12, 31), true),
                };

            var dateCreated = new DateTime(2021, 1, 1);
            foreach (var tuple in testData)
            {
                var dateFrom = tuple.Item1;
                var dateTo = tuple.Item2;
                var isFilteredExpected = tuple.Item3;

                var filters = new List<IMailParamFilter> {new DateTimeCreatedRangeFilter(dateFrom, dateTo)};
                var mail = FabricateData.FabricateMail(dateCreated: dateCreated);
                var isFilteredActual = mail.IsFiltered(filters);
                Assert.IsTrue(isFilteredExpected == isFilteredActual, tuple.ToString());
            }
            
        }

        [TestMethod]
        public void TestDistance()
        {
            var testData = new List<Tuple<Coordinates, double, bool>>
                {
                    new Tuple<Coordinates, double, bool>(new Coordinates(53.359, -6.308), 1000.0, true),
                    new Tuple<Coordinates, double, bool>(new Coordinates(53.359, -6.308), 1.0, false),
                    new Tuple<Coordinates, double, bool>(new Coordinates(-23.531, -46.902), 2000.0, true),
                };
            foreach (var tuple in testData)
            {
                var location = tuple.Item1;
                var radiusMeters = tuple.Item2;
                var isFilteredExpected = tuple.Item3;

                var filters = new List<IMailParamFilter> {new LocationFilter(location, radiusMeters)};
                var mail = FabricateData.FabricateMail();
                var isFilteredActual = mail.IsFiltered(filters);
                Assert.IsTrue(isFilteredExpected == isFilteredActual, tuple.ToString());
            }
        }

        [TestMethod]
        public void TestDampingDistance()
        {
            var filter = new DampingDistanceFilter(MoscowCoordinates, baseDistanceMeters: 2000*1000,
                                                   randomGenerator: new Random(12345));
            var mail = FabricateData.FabricateMail(recipientsCoordinates: new List<Coordinates> {YekaterinburgCoordinates});
            var isFilteredActual = filter.IsFiltered(mail);
            var probability = filter.CalculateProbabilityForCoordinates(YekaterinburgCoordinates);

            Assert.IsTrue(Math.Abs(probability - 0.6108781088799166) < 0.01);
            Assert.IsTrue(isFilteredActual);
        }

        [TestMethod]
        public void TestName()
        {
            var testData = new List<Tuple<string, bool>>
                {
                    new Tuple<string, bool>("john_smith@mydomain.com", true),
                    new Tuple<string, bool>("john_smIth@mydomain.com", false),
                    new Tuple<string, bool>("john_smith@mydomain.com ", false),
                };
            foreach (var tuple in testData)
            {
                var name = tuple.Item1;
                var isFilteredExpected = tuple.Item2;

                var filters = new List<IMailParamFilter> {new NameFilter(name)};
                var mail = FabricateData.FabricateMail();
                var isFilteredActual = mail.IsFiltered(filters);
                Assert.IsTrue(isFilteredExpected == isFilteredActual, tuple.ToString());
            }
        }

        [TestMethod]
        public void TestAdditionalNotes()
        {
            var testData = new List<Tuple<string, bool>>
                {
                    new Tuple<string, bool>("for john smith", true),
                    new Tuple<string, bool>("not for john smith", false),
                };
            foreach (var tuple in testData)
            {
                var additionalNotes = tuple.Item1;
                var isFilteredExpected = tuple.Item2;

                var filters = new List<IMailParamFilter> {new AdditionalNotesFilter(additionalNotes)};
                var mail = FabricateData.FabricateMail(additionalNotes: "This message is for John Smith");
                var isFilteredActual = mail.IsFiltered(filters);
                Assert.IsTrue(isFilteredExpected == isFilteredActual, tuple.ToString());
            }
        }

        [TestMethod]
        public void TestInlineMessage()
        {
            var testData = new List<Tuple<string, bool>>
                {
                    new Tuple<string, bool>("for john smith", true),
                    new Tuple<string, bool>("not for john smith", false),
                };
            foreach (var tuple in testData)
            {
                var inlineMessage = tuple.Item1;
                var isFilteredExpected = tuple.Item2;

                var filters = new List<IMailParamFilter> {new InlineMessageFilter(inlineMessage)};
                var mail = FabricateData.FabricateMail(inlineMessage: "This message is for John Smith");
                var isFilteredActual = mail.IsFiltered(filters);
                Assert.IsTrue(isFilteredExpected == isFilteredActual, tuple.ToString());
            }
        }

        [TestMethod]
        public void TestAttachment()
        {
            var testData = new List<Tuple<string, bool>>
                {
                    new Tuple<string, bool>("0123456789abcdef", true),
                    new Tuple<string, bool>("12345", false),
                };
            foreach (var tuple in testData)
            {
                var hashsumHex = tuple.Item1;
                var isFilteredExpected = tuple.Item2;

                var filters = new List<IMailParamFilter> {new AttachmentFilter(hashsumHex)};
                var mail = FabricateData.FabricateMail(attachments: new List<Attachment> {new Attachment("123.mp4", 12345678, "0123456789abcdef")});
                var isFilteredActual = mail.IsFiltered(filters);
                Assert.IsTrue(isFilteredExpected == isFilteredActual, tuple.ToString());
            }
        }
    }
}
