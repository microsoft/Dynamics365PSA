using Android.Content;
using Android.Graphics;
using System.Collections.Generic;
using System.Threading;

namespace Common.Android.Helpers
{
    public static class TypefaceHelper
    {
        private static SemaphoreSlim sl = new SemaphoreSlim(1);
        private static Dictionary<string, Typeface> typefaces = new Dictionary<string, Typeface>();

        public static Typeface GetTypeface(Context context, string fontFileLocation)
        {
            if (typefaces.ContainsKey(fontFileLocation) && typefaces[fontFileLocation] != null)
            {
                return typefaces[fontFileLocation];
            }
            else
            {
                // The typeface only needs to load once and shared among all list adapter objects.
                // Using semaphore with countdown 1 here to prevent the situation when multiple objects
                // try to load the typeface concurrently
                // Also there is an issue of memory leak if Typeface.CreateFromAssets is called mutiple times,
                // therefore it's good to reuse
                try
                {
                    sl.Wait();
                    var typeface = Typeface.CreateFromAsset(context.Assets, fontFileLocation);
                    typefaces[fontFileLocation] = typeface;
                    return typeface;
                }
                finally
                {
                    sl.Release();
                }
            }
        }
    }
}