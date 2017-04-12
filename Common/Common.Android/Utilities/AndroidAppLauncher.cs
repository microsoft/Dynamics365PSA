using Android.Content;
using Common.Model.Map;
using Common.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;

namespace Common.Android.Utilities
{
    public class AndroidAppLauncher : AppLauncher
    {
        private Context context;
        public Context Context
        {
            get
            {
                return context;
            }
            internal set
            {
                context = value;
            }
        }

        /// <summary>
        /// Opens a file based off the provided extension in an external application.
        /// </summary>
        /// <param name="filePath">File path with the filename and extension.</param>
        /// <returns>Task object that represents this action.</returns>
        public override Task OpenFileAsync(string filePath)
        {
            Java.IO.File localFile = new Java.IO.File(filePath);
            Intent intent = new Intent(Intent.ActionView);
            Uri filePathUri = Uri.Parse(filePath);

            intent.SetDataAndType(Uri.FromFile(localFile), GetMIMEFromFilename(filePath));
            intent.SetFlags(ActivityFlags.NewTask);

            if (intent.ResolveActivity(context.PackageManager) != null)
            {
                context.StartActivity(intent);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Opens the Maps app on Android device.
        /// </summary>
        /// <param name="address">String representing the address.</param>
        /// <param name="coordinate">Coordinate object representing the location.</param>
        /// <returns>Task object that represents this action.</returns>
        public override async Task OpenMapAsync(string address, Coordinate coordinate)
        {
            Intent intent = new Intent(Intent.ActionView);

            if (address != null && coordinate != null)
            {
                intent.SetData(Uri.Parse(string.Format("geo:{0},{1}?q={2}", coordinate.Latitude, coordinate.Longitude, Uri.Encode(address))));
            }
            else if (address != null && coordinate == null)
            {
                intent.SetData(Uri.Parse(string.Format("geo:0,0?q={0}", Uri.Encode(address))));
            }
            else
            {
                intent.SetData(Uri.Parse(string.Format("geo:0,0")));
            }

            if (intent.ResolveActivity(context.PackageManager) != null)
            {
                context.StartActivity(intent);
            }

            await Task.FromResult(true);
        }

        /// <summary>
        /// Gets the file MIME type based off the file extension.
        /// </summary>
        /// <param name="filename">Filename with extension.</param>
        /// <returns>MIME type in the format image/*, application/* etc.</returns>
        private string GetMIMEFromFilename(string filename)
        {
            if (filename != null)
            {
                String extension = Path.GetExtension(filename);

                if (extension != String.Empty)
                {
                    // Add any expected file types as we progress.
                    switch (extension)
                    {
                        // Image MIME types
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                            return "image/*";

                        // Default to text file.
                        default:
                            return "application/text";
                    }
                }
            }
            return "application/text";
        }
    }
}