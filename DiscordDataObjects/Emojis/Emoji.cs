using DiscordDataObjects.Users;

namespace DiscordDataObjects.Emojis
{
    public class Emoji : IEmoji
    {
		const string UrlEncodedSplitter = "%3A";

		public Emoji(string identifier, 
					 string name, 
					 bool? animated, 
					 string[] availableForRoles, 
					 IUser createdBy, 
					 bool? requireColons, 
					 bool? managed, 
					 bool? available)
		{
			Identifier = identifier;
			Name = name;
			Animated = animated;
			AvailableForRoles = availableForRoles;
			CreatedBy = createdBy;
			RequireColons = requireColons;
			Managed = managed;
			Available = available;
		}

		public virtual string Identifier { get; private set; }
        public string Name { get; private set; }
        public bool? Animated { get; private set; }
        public string Mention => $"<{(Animated ?? false ? "a" : string.Empty)}:{Name}:{Identifier}>";
        public bool IsUnicodeEmoji => true;
        public string UrlEncoded => $"{Name}{UrlEncodedSplitter}{Identifier}";
        public string[] AvailableForRoles { get; private set; }
        public IUser CreatedBy { get; private set; }
		public bool? RequireColons { get; private set; }
		public bool? Managed { get; private set; }
		public bool? Available { get; private set; }
	}
}
