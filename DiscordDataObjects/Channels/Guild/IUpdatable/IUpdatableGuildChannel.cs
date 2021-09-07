using System.Collections.Generic;

namespace DiscordDataObjects.Channels.Guild.IUpdatable
{
    public interface IUpdatableGuildChannel : IUpdatableChannel
    {
        //
        // Summary:
        //     Sets new text channel rate limit.
        //
        // Parameters:
        //     name:
        //          Channel's new name. Must be in range 2-100 characters.
        // Exceptions:
        //     T:System.ArgumentOutOfRangeException:
        //          Name is less then 2 or greater then 100 characters.
        void SetNewName(string name);
        //
        // Summary:
        //     Sets position of the channel in the left-hand listing.
        //
        // Parameters:
        //     position:
        //          Channel's new position.
        // Exceptions:
        //     T:System.ArgumentOutOfRangeException:
        //          Position is less then 0.
        void SetNewPosition(int position);
        //
        // Summary:
        //     Set channel's new NSFW state.
        //
        // Parameters:
        //     nsfw:
        //          Channel's new NSFW state.
        void SetNewNsfw(bool nsfw);
        //
        // Summary:
        //     Sets channel's new permissions.
        //
        // Parameters:
        //     overwrite:
        //          Channel's new permissions.
        //          Null = no permissions needed.
        void SetNewPermissionsOverwrire(List<PermissionOverwrite> overwrite);
        //
        // Summary:
        //     Add new permission overwrite to channel.
        //
        // Parameters:
        //     overwrite:
        //          Overwrite to add to channel.
        // Exceptions:
        //     T:System.ArgumentNullException:
        //          overwrite is null.
        //     T:System.ArgumentException:
        //          Channel's permission overwrites allready contains overwrite with provided id.
        void AddNewPermissionOverwrite(PermissionOverwrite overwrite);
        //
        // Summary:
        //     Remove permission overwrite from channel.
        //
        // Parameters:
        //     id:
        //          Role or user id in overwrite.
        //          If corresponding to provided id overwite wont be found no exception will be passed.
        // Exceptions:
        //     T:System.ArgumentNullException:
        //          id is null.
        void RemovePermissionOverwrite(string id);
        //
        // Summary:
        //     Sets channel's new category.
        //
        // Parameters:
        //     categoryId:
        //          Identifier of channel's new category.
        //          Null = no category.
        void SetNewCategory(string categoryId);
        //
        // Summary:
        //     Sets channel's new category.
        //
        // Parameters:
        //     guildId:
        //          Identifier of channel's new guild.
        // Exceptions:
        //     T:System.ArgumentNullException:
        //          guildId is null or empty.
        void SetNewGuildId(string guildId);
    }
}
