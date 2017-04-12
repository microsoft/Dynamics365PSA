using System.Windows.Media;

namespace Common.WinPhone.Renderer
{
    public static class RendererUtil
    {
        /// <summary>
        /// Returns a windows solid color brush from a xamarin color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SolidColorBrush FromXamarinColorToWindowsBrush(Xamarin.Forms.Color color)
        {
            return new SolidColorBrush(Color.FromArgb( (byte)(color.A * 255),
                    (byte)(color.R * 255),
                    (byte)(color.G * 255),
                    (byte)(color.B * 255)));
        }
    }
}
