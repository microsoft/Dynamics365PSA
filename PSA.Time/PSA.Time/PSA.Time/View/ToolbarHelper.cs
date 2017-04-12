using Common.Utilities.Resources;
using Xamarin.Forms;

namespace PSA.Time.View
{
    /// <summary>
    /// Helper methods for creating toolbar buttons.
    /// </summary>
    static class ToolbarHelper
    {
        /// <summary>
        /// Create a submit button.
        /// </summary>
        /// <returns>A ToolbarItem configured as a Submit button.</returns>
        public static ToolbarItem createSubmitButton()
        {
            return ToolbarHelper.createButton(
                AppResources.Submit,
                Device.OnPlatform("upload.png", "upload.png", "Assets/Icons/upload.png"),
                ToolbarItemOrder.Primary);
        }

        /// <summary>
        /// Create a save button.
        /// </summary>
        /// <returns>A ToolbarItem configured as a Save button.</returns>
        public static ToolbarItem createSaveButton()
        {
            return ToolbarHelper.createButton(
                AppResources.Save,
                Device.OnPlatform("save.png", "save.png", "Assets/Icons/save.png"),
                ToolbarItemOrder.Primary); 
        }

        /// <summary>
        /// Create a delete button.
        /// </summary>
        /// <returns>A ToolbarItem configured as a Delete button.</returns>
        public static ToolbarItem createDeleteButton()
        {
            ToolbarItem deleteItem = ToolbarHelper.createButton(
                AppResources.Delete,
                Device.OnPlatform("delete.png", "delete.png", "Assets/Icons/delete.png"),
                ToolbarItemOrder.Primary);
            deleteItem.IsDestructive = true;
            return deleteItem;
        }

        /// <summary>
        /// Create a recall button.
        /// </summary>
        /// <returns>A ToolbarItem configured as a Recall button.</returns>
        public static ToolbarItem createRecallButton()
        {
            return ToolbarHelper.createButton(
               AppResources.Recall,
               // TODO: get correct icon for recall
               Device.OnPlatform("back.png", "back.png", "Assets/Icons/back.png"),
               ToolbarItemOrder.Primary);
        }

        /// <summary>
        /// Create a multiselect button.
        /// </summary>
        /// <returns>A ToolbarItem configured as a Multiselect button.</returns>
        public static ToolbarItem createMultiselectButton()
        {
            return ToolbarHelper.createButton(
                AppResources.Select,
                // TODO: Get correct Icon for multiselect
                Device.OnPlatform("upload.png", "upload.png", "Assets/Icons/upload.png"),
                ToolbarItemOrder.Primary);
        }

        /// <summary>
        /// Create a new button.
        /// </summary>
        /// <returns>A ToolbarItem configured as a Create button.</returns>
        public static ToolbarItem createAddTimeEntryButton()
        {
            return ToolbarHelper.createButton(
                AppResources.Create,
                Device.OnPlatform("add.png", "add.png", "Assets/Icons/add.png"),
                ToolbarItemOrder.Primary);
        }

        private static ToolbarItem createButton(string text, string icon, ToolbarItemOrder order)
        {
            return new ToolbarItem
            {
                Text = text,
                Icon = icon,
                Order = order
            };
        }

    }
}
