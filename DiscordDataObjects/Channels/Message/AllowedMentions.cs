using Newtonsoft.Json;
using System;
using System.Linq;

namespace DiscordDataObjects.Channels.Message
{
    public enum AllowedMentionTypes : byte
    {
        Roles,
        Users,
        Everyone
    }
    [JsonObject(MemberSerialization.OptIn)]
    internal class AllowedMentions
    {
        public AllowedMentionTypes[] MentionsToParse { get; private set; }
        [JsonProperty(PropertyName = "users", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Users { get; private set; }
        [JsonProperty(PropertyName = "roles", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Roles { get; private set; }
        [JsonProperty(PropertyName = "parse", NullValueHandling = NullValueHandling.Include)]
        private string[] parse => MentionsToParse?.Select(x => x.ToString().ToLower()).ToArray()
                                                 ?? new string[0];

        public static AllowedMentions SupressAllMentions()
        {
            return new AllowedMentions(null, null, null);
        }
        // Will also supress @here
        public static AllowedMentions SupressEveryoneMention(string[] usersToMention = null, string[] rolesToMention = null)
        {
            AllowedMentionTypes[] supress = new AllowedMentionTypes[1] { AllowedMentionTypes.Everyone };
            return new AllowedMentions(supress, usersToMention, rolesToMention);
        }
        public static AllowedMentions SupressRolesMention(bool supressEveryone = false, string[] usersToMention = null)
        {
            AllowedMentionTypes[] supress;
            if (supressEveryone)
            {
                supress = new AllowedMentionTypes[] { AllowedMentionTypes.Roles, AllowedMentionTypes.Everyone };
            }
            else
            {
                supress = new AllowedMentionTypes[] { AllowedMentionTypes.Roles };
            }
            return new AllowedMentions(supress, usersToMention, null);
        }
        public static AllowedMentions SupressUsersMention(bool supressEveryone = false, string[] rolesToMention = null)
        {
            AllowedMentionTypes[] supress;
            if (supressEveryone)
            {
                supress = new AllowedMentionTypes[] { AllowedMentionTypes.Users, AllowedMentionTypes.Everyone };
            }
            else
            {
                supress = new AllowedMentionTypes[] { AllowedMentionTypes.Users };
            }
            return new AllowedMentions(supress, rolesToMention, null);
        }
        public AllowedMentions(AllowedMentionTypes[] mentionsToParse,
                               string[] users,
                               string[] roles)
        {
            if (mentionsToParse != null || mentionsToParse.Length != 0)
            {
                if (mentionsToParse.Contains(AllowedMentionTypes.Roles))
                {
                    if (roles != null || roles.Length != 0)
                    {
                        throw new ArgumentException("Conditions cannot be " +
                                                    "fulfilled simultaneously. " +
                                                    "They are mutually exclusive.", "roles");
                    }
                }
                else if (mentionsToParse.Contains(AllowedMentionTypes.Users))
                {
                    if (users != null || users.Length != 0)
                    {
                        throw new ArgumentException("Conditions cannot be " +
                                                    "fulfilled simultaneously. " +
                                                    "They are mutually exclusive.", "users");
                    }
                }
            }
            MentionsToParse = mentionsToParse;
            Users = users;
            Roles = roles;
        }
    }
}
