using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.FieldSerializers;
using System.Collections.Generic;

namespace my.Foundation.Layout.LayoutService.FieldSerializers
{
    public class SecurityFieldSerializer : BaseFieldSerializer
    {
        const string Inheritance = "^*";

        const string NoInheritance = "!*";

        public SecurityFieldSerializer(IFieldRenderer fieldRenderer) : base(fieldRenderer)
        {
        }

        public override void Serialize(Field field, JsonTextWriter writer)
        {
            Assert.ArgumentNotNull(field, nameof(field));
            Assert.ArgumentNotNull(writer, nameof(writer));
            writer.WritePropertyName(field.Name);
            writer.WriteStartObject();
            writer.WritePropertyName("value");
            new JsonSerializer().Serialize(writer, ParseSecurity(field.Value));
            writer.WriteEndObject();
        }

        public static IDictionary<string, IDictionary<string, IList<string>>> ParseSecurity(string securityValue)
        {
            IDictionary<string, IDictionary<string, IList<string>>> security = new Dictionary<string, IDictionary<string, IList<string>>>();
            if (string.IsNullOrEmpty(securityValue))
                return security;

            State state = State.au;
            string userOrRole = "";
            string[] tokens = securityValue.Split('|');
            foreach (string token in tokens)
            {
                switch (token)
                {
                    case "au":
                        state = State.au;
                        break;
                    case "ar":
                        state = State.ar;
                        break;
                    case "pe":
                        state = State.pe;
                        security[userOrRole].Add("Item", new List<string>());
                        break;
                    case "pd":
                        security[userOrRole].Add("Descendants", new List<string>());
                        state = State.pd;
                        break;
                    default:
                        if (token == Inheritance || token == NoInheritance)
                        {
                            switch (state)
                            {
                                case State.pe:
                                    security[userOrRole]["Item"].Add($"{(token == Inheritance ? '+' : '-')}inheritance");
                                    break;
                                case State.pd:
                                    security[userOrRole]["Descendants"].Add($"{(token == Inheritance ? '+' : '-')}inheritance");
                                    break;
                            }
                            break;
                        }
                        else if (!string.IsNullOrEmpty(token))
                        {
                            switch (state)
                            {
                                case State.au:
                                case State.ar:
                                    userOrRole = token;
                                    security.Add(userOrRole, new Dictionary<string, IList<string>>());
                                    break;
                                case State.pe:
                                    string[] permissionsItem = token.Split(':');
                                    security[userOrRole]["Item"].Add(permissionsItem[0][0] + permissionsItem[1]);
                                    break;
                                case State.pd:
                                    string[] permissiondDesc = token.Split(':');
                                    security[userOrRole]["Descendants"].Add(permissiondDesc[0][0] + permissiondDesc[1]);
                                    break;
                            }
                        }
                        break;
                }
            }
            return security;
        }

        enum State { au, ar, pe, pd };
    }
}