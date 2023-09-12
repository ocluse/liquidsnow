namespace Ocluse.LiquidSnow.Venus.Contracts
{
    /// <summary>
    /// A contract used by Venus services to resolve the representation state of components.
    /// </summary>
    public interface IVenusResolver
    {
        /// <summary>
        /// Returns the actual css style representation of the given color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        string ResolveColor(int color);

        /// <summary>
        /// Returns the css class that will be applied for the given text.
        /// </summary>
        string ResolveTextStyle(int textStyle);

        /// <summary>
        /// Returns the actual HTML tag that will be used for the given text hierarchy.
        /// </summary>
        string ResolveTextHierarchy(int textHierarchy);

        /// <summary>
        /// Returns a URL to the image that will be used as the user's avatar.
        /// </summary>
        string ResolveAvatarId(string userId);

        /// <summary>
        /// Returns an int representing the container state to be rendered for the given exception.
        /// </summary>
        int ResolveExceptionToContainerState(Exception exception);
    }
}
