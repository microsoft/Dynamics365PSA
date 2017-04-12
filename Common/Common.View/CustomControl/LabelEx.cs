using Xamarin.Forms;

namespace Common.View.CustomControl
{
     /// <summary>
     /// A wrap to Label object to add Margin
     /// </summary>
    public class LabelEx : Label
    {
        public Thickness? Margin { get; protected set; }

        public LabelEx(Thickness? margin) : base()
        {
            Margin = margin;
        }

        public LabelEx() : base() { }
    }
}
