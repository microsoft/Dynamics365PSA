using Common.Utilities.Camera;
using Microsoft.Phone;
using Microsoft.Phone.Tasks;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Common.WinPhone
{
    public class CameraWindows : CameraUtil
    {
        protected CameraCaptureTask cameraCaptureTask;
        public const int MaxImageDimension = 1280;

        public CameraWindows()
        {
            cameraCaptureTask = new CameraCaptureTask();
            cameraCaptureTask.Completed += new EventHandler<PhotoResult>(cameraCaptureTask_Completed);
        }

        private void cameraCaptureTask_Completed(object sender, PhotoResult photoResult)
        {
            if (photoResult.TaskResult == TaskResult.OK)
            {
                this.ImageStream = ResizeImage(photoResult.ChosenPhoto, null);
            }
        }

        public override void OpenCamera()
        {
            cameraCaptureTask.Show();
        }

        /// <summary>
        /// Shrinks the image if necessary to keep it within the maximum allowed size,
        /// maintaining the aspect ratio of the original image.
        /// </summary>
        /// <remarks>
        /// Images taken with high resolutions are too expensive to send on the wire and
        /// unnecessary for the capture scenario, so we limit their size.
        /// </remarks>
        public override MemoryStream ResizeImage(Stream imageStream, String filePath)
        {
            int pixWidth;
            int pixHeight;
            MemoryStream streamOut = null;

            // We do not expect filePath to be an input for Windows images. 
            // This function is therefore implemented only for imageStream as an input.
            if (imageStream != null)
            {
                WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(imageStream);
                pixWidth = bitmap.PixelWidth;
                pixHeight = bitmap.PixelHeight;

                double ratio = (double)pixWidth / (double)pixHeight;
                streamOut = new MemoryStream();

                if (pixHeight < pixWidth && pixWidth > MaxImageDimension)
                {
                    pixWidth = MaxImageDimension;
                    pixHeight = (int)(MaxImageDimension / ratio);
                }
                else if (pixHeight > MaxImageDimension)
                {
                    pixHeight = MaxImageDimension;
                    pixWidth = (int)(MaxImageDimension * ratio);
                }
                bitmap.SaveJpeg(streamOut, pixWidth, pixHeight, 0, 100);
            }

            return streamOut;
        }
    }
}
