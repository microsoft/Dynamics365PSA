using Xamarin.Forms;

namespace Common.View.CustomControl
{
    /// <summary>
    /// To use custom renderer to render controls for each platform
    /// if default style didn't meet requirement.
    /// </summary>
    public class EditorEx : Editor
    {
        public void InvalidateLayout()
        {
            this.InvalidateMeasure();
        }
    }
}
