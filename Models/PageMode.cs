namespace RazorPagesDotCMS.Models
{
    /// <summary>
    /// Represents the different modes a page can be viewed in
    /// </summary>
    public enum PageMode
    {
        /// <summary>
        /// Edit mode for content editors
        /// </summary>
        EDIT_MODE,
        
        /// <summary>
        /// Preview mode for content editors
        /// </summary>
        PREVIEW_MODE,
        
        /// <summary>
        /// Live mode for end users
        /// </summary>
        LIVE_MODE
    }
}
