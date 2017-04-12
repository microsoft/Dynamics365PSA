using Android.Content;
using Android.Graphics;
using Common.Android.Utilities;
using Common.Utilities.Camera;
using Common.Utilities.Resources;
using System;
using System.IO;

namespace Common.Android
{
    public class CameraAndroid : CameraUtil
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

        public CameraAndroid() { }

        public override void OpenCamera()
        {
            // Specify filename and directory name and start the camera intent.
            Intent intent = new Intent(context, typeof(CameraActivity));
            intent.PutExtra(CameraActivity.fileNameForPictures, String.Format(AppResources.ReceiptFileName, Guid.NewGuid()));
            intent.PutExtra(CameraActivity.directoryNameForPictures, AppResources.ReceiptsDirName);
            context.StartActivity(intent);
        }

        /// <summary>
        /// Resize the image to the standard compression quality set, obtaining the file from
        /// the file path provided.
        /// </summary>
        /// <param name="imageStream">Stream from which the resize the image.</param>
        /// <param name="filePath">File path for which to resize the image.</param>
        /// <returns></returns>        
        public override MemoryStream ResizeImage(Stream imageStream, String filePath)
        {
            MemoryStream memoryStream = null;

            // We do not expect imageStream to be an input for Android images. 
            // This function is therefore implemented only for filePath as an input.
            if (!String.IsNullOrEmpty(filePath))
            {
                var metrics = context.Resources.DisplayMetrics;
                memoryStream = new MemoryStream();
                Bitmap bitmap = BitmapFactory.DecodeFile(filePath);
                // Arbitrarily resize according to the defined compression quality to reduce filesize and save memory.               
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, compressionQuality, memoryStream);
            }            

            return memoryStream;
        }
    }
}