using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Entities
{
    public class Role
    {
        [JsonProperty(PropertyName = "id")]
        public string Identifier { get; private set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; private set; }
        [JsonProperty(PropertyName = "color")]
        public int Color { get; private set; } //Hexadecimal
        [JsonProperty(PropertyName = "position")]
        public int Position { get; private set; }
        [JsonProperty(PropertyName = "permissions")]
        public int Permissions { get; private set; }
        [JsonProperty(PropertyName = "hoist")]
        public bool Hoist { get; private set; }
        [JsonProperty(PropertyName = "managed")]
        public bool Managed { get; private set; }
        [JsonProperty(PropertyName = "mentionable")]
        public bool Mentionable { get; private set; }

        internal string UpdateRole(Role newRoleInfo)
        {
            StringBuilder result = new StringBuilder();
            if(Identifier != newRoleInfo.Identifier)
            {
                Identifier = newRoleInfo.Identifier;
                result.Append("Identifier | ");
            }
            if(Name != newRoleInfo.Name)
            {
                Name = newRoleInfo.Name;
                result.Append("Name | ");
            }
            if(Color != newRoleInfo.Color)
            {
                Color = newRoleInfo.Color;
                result.Append("Color | ");
            }
            if(Position != newRoleInfo.Position)
            {
                Position = newRoleInfo.Position;
                result.Append("Position | ");
            }
            if(Permissions != newRoleInfo.Permissions)
            {
                Permissions = newRoleInfo.Permissions;
                result.Append("Permissions | ");
            }
            if (Hoist != newRoleInfo.Hoist)
            {
                Hoist = newRoleInfo.Hoist;
                result.Append("Hoist | ");
            }
            if (Managed != newRoleInfo.Managed)
            {
                Managed = newRoleInfo.Managed;
                result.Append("Managed | ");
            }
            if (Mentionable != newRoleInfo.Mentionable)
            {
                Mentionable = newRoleInfo.Mentionable;
                result.Append("Mentionable | ");
            }
            return result.ToString();
        }
    }
}
