using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Common.Utilities.Camera;
using Common.Utilities.Resources;
using System;
using System.Collections.Generic;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace Common.Android.Utilities
{
    [Activity (ConfigurationChanges = ConfigChanges.KeyboardHidden | ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class CameraActivity : Activity
    {
        public File file;
        public File dir;  
        public static string fileNameForPictures = "filename";
        public static string directoryNameForPictures = "dirname";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AndroidAppUtilities.SetContext(this);

            // Check if there are apps to take pictures, then create a directory for the pictures if none exists.
            if (AppExistsToTakePictures())
            {
                String filename = Intent.GetStringExtra(fileNameForPictures);
                String dirname = Intent.GetStringExtra(directoryNameForPictures);            

                CreateDirectoryForPictures(filename, dirname);
                TakeAPicture();
            }
        }

        /// <summary>
        /// Once the image is taken, make it available in the gallery and store the date in a this.imageStream
        /// to be saved.
        /// </summary>
        /// <param name="requestCode">The request code of the activity intent.</param>
        /// <param name="resultCode">The result code, whether the activity/intent was successful.</param>
        /// <param name="data"></param>
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                // Make it available in the gallery
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(file);
                mediaScanIntent.SetData(contentUri);
                SendBroadcast(mediaScanIntent);

                // Compress image and store in imagestream to be saved.
                CameraUtil.Current.ImageStream = CameraUtil.Current.ResizeImage(null, file.Path);
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
            this.Finish();
        }

        /// <summary>
        /// Create an intent to take a picture.
        /// </summary>
        public void TakeAPicture()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);            
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(file));
            StartActivityForResult(intent, 0);
        }

        /// <summary>
        /// Validate if the phone has an application to take pictures.
        /// </summary>
        /// <returns>True if the phone has a camera application, false otherwise.</returns>
        private bool AppExistsToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        /// <summary>
        /// Creates a directory to store the pictures.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="dirname"></param>
        private void CreateDirectoryForPictures(string filename, string dirname)
        {            
            if (String.IsNullOrEmpty(dirname))
            {
                dirname = AppResources.DefaultDirectory;
            }

            if (String.IsNullOrEmpty(filename))
            {
                filename = AppResources.DefaultFilename;
            }

            dir = new File(Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures), dirname);

            if (!dir.Exists())
            {
                dir.Mkdirs();
            }

            file = new File(dir, filename);
        }
    }
}