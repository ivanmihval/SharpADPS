using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreADPS.Filters;
using CoreADPS.Helpers;
using Newtonsoft.Json;

namespace CoreADPS.MailModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Mail
    {
        public const string DefaultVersion = "1.0";

        [JsonProperty(PropertyName = "date_created")]
        public DateTime DateCreated { get; set; }

        [JsonProperty(PropertyName = "recipient_coords")]
        public List<Coordinates> RecipientsCoordinates { get; set; }

        private string _name;

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return _name; } 
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                _name = value;
            }
        }

        [JsonProperty(PropertyName = "additional_notes")]
        public string AdditionalNotes { get; set; }

        [JsonProperty(PropertyName = "inline_message")]
        public string InlineMessage { get; set; }

        [JsonProperty(PropertyName = "attachments")]
        public List<Attachment> Attachments { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [JsonProperty(PropertyName = "min_version")]
        public string MinVersion { get; set; }

        public Mail(DateTime dateCreated, List<Coordinates> recipientsCoordinates, string name, string additionalNotes,
                    string inlineMessage, List<Attachment> attachments, string version = DefaultVersion, string minVersion = DefaultVersion)
        {
            DateCreated = dateCreated;
            RecipientsCoordinates = recipientsCoordinates;
            Name = name;
            AdditionalNotes = additionalNotes;
            InlineMessage = inlineMessage;
            Attachments = attachments;
            Version = version;
            MinVersion = minVersion;
        }

        public static Mail FromJson(string jsonText)
        {
            return JsonConvert.DeserializeObject<Mail>(jsonText);
        }

        public static Mail EmptyMail()
        {
            return new Mail(DateTimeGenerator.UtcNow(), new List<Coordinates>(), "", "", "", new List<Attachment>());
        }

        public string ToJson()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new OrderedContractResolver(), 
                Formatting = Formatting.Indented,
            };

            var sb = new StringBuilder();
            var sw = new System.IO.StringWriter(sb) {NewLine = "\n"};
            using (var writer = new JsonTextWriter(sw))
            {
                writer.Indentation = 4;
                var serializer = JsonSerializer.Create(jsonSerializerSettings);
                serializer.Serialize(writer, this);
            }

            return sb.ToString();
        }

        public byte[] ToJsonBytes()
        {
            var mailJsonString = ToJson();
            return Unicode.Utf8WithouBom.GetBytes(mailJsonString);
        }

        public bool IsFiltered(IEnumerable<IMailParamFilter> filters)
        {
            // ReSharper disable PossibleMultipleEnumeration
            var locationFilters = filters.Where(f => (f is DampingDistanceFilter) || (f is LocationFilter)).ToArray();
            var otherFilters = filters.Where(f => !(f is DampingDistanceFilter) && !(f is LocationFilter)).ToArray();
            // ReSharper restore PossibleMultipleEnumeration

            var resultedFilters = new List<IMailParamFilter>(otherFilters);
            if (locationFilters.Any())
            {
                resultedFilters.Add(new ParallelGroupFiltersFilter(locationFilters));
            }

            return resultedFilters.All(f => f.IsFiltered(this));
        }
    }
}
