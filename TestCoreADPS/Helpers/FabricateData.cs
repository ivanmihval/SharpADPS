using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreADPS.MailModels;

namespace TestCoreADPS.Helpers
{
    public class FabricateData
    {
        public static Mail FabricateMail(
            DateTime? dateCreated = null,
            List<Coordinates> recipientsCoordinates = null,
            string name = "john_smith@mydomain.com",
            string additionalNotes = null,
            string inlineMessage = null,
            List<Attachment> attachments = null
            )
        {
            if (dateCreated == null)
            {
                dateCreated = new DateTime(2020, 5, 5);
            }

            if (recipientsCoordinates == null)
            {
                recipientsCoordinates = new List<Coordinates>
                    {
                        new Coordinates(53.3595118, -6.3086148),
                        new Coordinates(-23.5311317, -46.9026668)
                    };
            }

            return new Mail(dateCreated.Value, recipientsCoordinates, name, additionalNotes, inlineMessage, attachments ?? new List<Attachment>());
        }
    }
}
