using Gateway.Entities.Channels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Gateway.Entities.Audit.LogEntry.EntryChange
{
    [JsonConverter(typeof(EntryChangeConverter))]
    public abstract class EntryChange : IEntryChange
    {
        public LogEntryChangeKey ChangeType { get; private set; }
        public abstract object OldValueUntyped { get; }
        public abstract object NewValueUntyped { get; }
        public EntryDataType DataType { get; private protected set; }
        private protected EntryChange(LogEntryChangeKey @event, EntryDataType dataType)
        {
            DataType = dataType;
            ChangeType = @event;
        }
    }
    public class EntryChange<T> : EntryChange
    {
        public override object OldValueUntyped => OldValue;
        public override object NewValueUntyped => NewValue;
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }
        public EntryChange(LogEntryChangeKey @event, EntryDataType dataType, T oldValue, T newValue)
            : base(@event, dataType)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    internal class EntryChangeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(EntryChange);
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            EntryChange result = null;
            JObject json = JObject.Load(reader);
            string jsonString = json.ToString();
            string keyString = json["key"].ToString(),
                   newValue = json["new_value"]?.ToString(),
                   oldValue = json["old_value"]?.ToString();
            if(TryToParseKey(keyString, out LogEntryChangeKey key))
            {
                switch (key)
                {
                    case LogEntryChangeKey.Icon:
                    case LogEntryChangeKey.Splash:
                    case LogEntryChangeKey.Owner:
                    case LogEntryChangeKey.Region:
                    case LogEntryChangeKey.AfkChannel:
                    case LogEntryChangeKey.Name:
                    case LogEntryChangeKey.VanityUrlCode:
                    case LogEntryChangeKey.WidgetChannel:
                    case LogEntryChangeKey.SystemChannel:
                    case LogEntryChangeKey.Topic:
                    case LogEntryChangeKey.ApplicationId:
                    case LogEntryChangeKey.PermissionsNew:
                    case LogEntryChangeKey.AllowNew:
                    case LogEntryChangeKey.DenyNew:
                    case LogEntryChangeKey.Code:
                    case LogEntryChangeKey.ChannelId:
                    case LogEntryChangeKey.InviterId:
                    case LogEntryChangeKey.Nickname:
                    case LogEntryChangeKey.Avatar:
                    case LogEntryChangeKey.Id:
                    case LogEntryChangeKey.Type:
                        result = new EntryChange<string>(key, EntryDataType.String, oldValue, newValue);
                        break;
                    case LogEntryChangeKey.AfkTimeout:
                    case LogEntryChangeKey.MFALevel: // TODO: enum?
                    case LogEntryChangeKey.VerificationLevel:
                    case LogEntryChangeKey.ExplicitContentFilter:
                    case LogEntryChangeKey.DefaultMessageNotifications:
                    case LogEntryChangeKey.PruneDaysCount:
                    case LogEntryChangeKey.Position:
                    case LogEntryChangeKey.Bitrate:
                    case LogEntryChangeKey.RateLimitPerUser:
                    case LogEntryChangeKey.PermissionsLegacy:
                    case LogEntryChangeKey.Color:
                    case LogEntryChangeKey.AllowLegacy:
                    case LogEntryChangeKey.DenyLegacy:
                    case LogEntryChangeKey.MaxUses:
                    case LogEntryChangeKey.Uses:
                    case LogEntryChangeKey.MaxAge:
                    case LogEntryChangeKey.BehaviorExpire:
                    case LogEntryChangeKey.GracePeriodExpire:
                        int? oldValueInteger = null,
                             newValueInteger = null;
                        if (!string.IsNullOrWhiteSpace(oldValue))
                        {
                            oldValueInteger = int.Parse(oldValue);
                        }
                        if (!string.IsNullOrWhiteSpace(newValue))
                        {
                            newValueInteger = int.Parse(newValue);
                        }
                        result = new EntryChange<int?>(key, EntryDataType.Integer, oldValueInteger, newValueInteger);
                        break;
                    case LogEntryChangeKey.WidgetState:
                    case LogEntryChangeKey.NsfwState:
                    case LogEntryChangeKey.HoistState:
                    case LogEntryChangeKey.MentionableState:
                    case LogEntryChangeKey.Temporary:
                    case LogEntryChangeKey.Deaf:
                    case LogEntryChangeKey.Mute:
                    case LogEntryChangeKey.EmoticonsState:
                        bool? oldValueBoolean = null,
                              newValueBoolean = null;
                        if (!string.IsNullOrWhiteSpace(oldValue))
                        {
                            oldValueBoolean = bool.Parse(oldValue);
                        }
                        if (!string.IsNullOrWhiteSpace(newValue))
                        {
                            newValueBoolean = bool.Parse(newValue);
                        }
                        result = new EntryChange<bool?>(key, EntryDataType.Bool, oldValueBoolean, newValueBoolean);
                        break;
                    case LogEntryChangeKey.Added:
                    case LogEntryChangeKey.Removed:
                        Role[] oldValueRoles = JsonConvert.DeserializeObject<Role[]>(oldValue ?? string.Empty),
                               newValueRoles = JsonConvert.DeserializeObject<Role[]>(newValue ?? string.Empty);
                        result = new EntryChange<Role[]>(key, EntryDataType.IRoleArray, oldValueRoles, newValueRoles);
                        break;
                    case LogEntryChangeKey.PermissionsOverwrite:
                        PermissionOverwrite[] oldValuePermissions = JsonConvert
                                                                   .DeserializeObject<PermissionOverwrite[]>(oldValue ?? string.Empty),
                                              newValuePermissions = JsonConvert
                                                                   .DeserializeObject<PermissionOverwrite[]>(newValue ?? string.Empty);
                        result = new EntryChange<PermissionOverwrite[]>(key, EntryDataType.IPermissionsOverwriteArray,
                                                                        oldValuePermissions, newValuePermissions);
                        break;
                }
            }
            else
            {
                // TODO: логирование ошибки
            }
            JsonConvert.PopulateObject(jsonString, result);
            return result;
        }
        private bool TryToParseKey(string key, out LogEntryChangeKey result)
        {
            bool state = true;
            result = default;
            switch (key)
            {
                case "name":
                    result = LogEntryChangeKey.Name;
                    break;
                case "icon_hash":
                    result = LogEntryChangeKey.Icon;
                    break;
                case "splash_hash":
                    result = LogEntryChangeKey.Splash;
                    break;
                case "owner_id":
                    result = LogEntryChangeKey.Owner;
                    break;
                case "region":
                    result = LogEntryChangeKey.Region;
                    break;
                case "afk_channel_id":
                    result = LogEntryChangeKey.AfkChannel;
                    break;
                case "afk_timeout":
                    result = LogEntryChangeKey.AfkTimeout;
                    break;
                case "mfa_level":
                    result = LogEntryChangeKey.MFALevel;
                    break;
                case "verification_level":
                    result = LogEntryChangeKey.VerificationLevel;
                    break;
                case "explicit_content_filter":
                    result = LogEntryChangeKey.ExplicitContentFilter;
                    break;
                case "default_message_notifications":
                    result = LogEntryChangeKey.DefaultMessageNotifications;
                    break;
                case "vanity_url_code":
                    result = LogEntryChangeKey.VanityUrlCode;
                    break;
                case "$add":
                    result = LogEntryChangeKey.Added;
                    break;
                case "$remove":
                    result = LogEntryChangeKey.Removed;
                    break;
                case "prune_delete_days":
                    result = LogEntryChangeKey.PruneDaysCount;
                    break;
                case "widget_enabled":
                    result = LogEntryChangeKey.WidgetState;
                    break;
                case "widget_channel_id":
                    result = LogEntryChangeKey.WidgetChannel;
                    break;
                case "system_channel_id":
                    result = LogEntryChangeKey.SystemChannel;
                    break;
                case "position":
                    result = LogEntryChangeKey.Position;
                    break;
                case "topic":
                    result = LogEntryChangeKey.Topic;
                    break;
                case "bitrate":
                    result = LogEntryChangeKey.Bitrate;
                    break;
                case "permission_overwrites":
                    result = LogEntryChangeKey.PermissionsOverwrite;
                    break;
                case "nsfw":
                    result = LogEntryChangeKey.NsfwState;
                    break;
                case "application_id":
                    result = LogEntryChangeKey.ApplicationId;
                    break;
                case "rate_limit_per_user":
                    result = LogEntryChangeKey.RateLimitPerUser;
                    break;
                case "permissions":
                    result = LogEntryChangeKey.PermissionsLegacy;
                    break;
                case "permissions_new":
                    result = LogEntryChangeKey.PermissionsNew;
                    break;
                case "color":
                    result = LogEntryChangeKey.Color;
                    break;
                case "hoist":
                    result = LogEntryChangeKey.HoistState;
                    break;
                case "mentionable":
                    result = LogEntryChangeKey.MentionableState;
                    break;
                case "allow":
                    result = LogEntryChangeKey.AllowLegacy;
                    break;
                case "allow_new":
                    result = LogEntryChangeKey.AllowNew;
                    break;
                case "deny":
                    result = LogEntryChangeKey.DenyLegacy;
                    break;
                case "deny_new":
                    result = LogEntryChangeKey.DenyNew;
                    break;
                case "code":
                    result = LogEntryChangeKey.Code;
                    break;
                case "channel_id":
                    result = LogEntryChangeKey.ChannelId;
                    break;
                case "inviter_id":
                    result = LogEntryChangeKey.InviterId;
                    break;
                case "max_uses":
                    result = LogEntryChangeKey.MaxUses;
                    break;
                case "uses":
                    result = LogEntryChangeKey.Uses;
                    break;
                case "max_age":
                    result = LogEntryChangeKey.MaxAge;
                    break;
                case "temporary":
                    result = LogEntryChangeKey.Temporary;
                    break;
                case "deaf":
                    result = LogEntryChangeKey.Deaf;
                    break;
                case "mute":
                    result = LogEntryChangeKey.Mute;
                    break;
                case "nick":
                    result = LogEntryChangeKey.Nickname;
                    break;
                case "avatar_hash":
                    result = LogEntryChangeKey.Avatar;
                    break;
                case "id":
                    result = LogEntryChangeKey.Id;
                    break;
                case "type":
                    result = LogEntryChangeKey.Type;
                    break;
                case "enable_emoticons":
                    result = LogEntryChangeKey.EmoticonsState;
                    break;
                case "expire_behavior":
                    result = LogEntryChangeKey.BehaviorExpire;
                    break;
                case "expire_grace_period":
                    result = LogEntryChangeKey.GracePeriodExpire;
                    break;
                default:
                    state = false;
                    break;
            }
            return state;
        }
        /// <summary>
        /// Not implemented
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}
