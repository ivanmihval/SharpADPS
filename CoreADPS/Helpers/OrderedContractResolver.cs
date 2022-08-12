using System;
using System.Collections.Generic;
using System.Linq;

namespace CoreADPS.Helpers
{
    // https://stackoverflow.com/a/56933615
    public class OrderedContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
    {
        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var @base = base.CreateProperties(type, memberSerialization);
            var ordered = @base
                .OrderBy(p => p.PropertyName)
                .ToList();
            return ordered;
        }
    }
}
