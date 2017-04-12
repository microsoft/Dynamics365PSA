using System;
using System.IO;

namespace Common.Utilities.Camera
{
    /// <summary>
    /// A delegate type for hooking up after the camera completes successfully
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="eventArgs"></param>
    public delegate void CaptureCompletedHandler(object sender, EventArgs eventArgs);

    public abstract class CameraUtil
    {
        /// <summary>
        /// An event that clients can use to be notified whever the camera finished the capture task
        /// </summary>
        public event CaptureCompletedHandler OnCaptureCompleted;

        public static CameraUtil Current { get; private set;}

        // Arbitrary compression quality of 70%.
        public const int compressionQuality = 70;

        /// <summary>
        /// A stream containing the data for the photo
        /// </summary>
        protected Stream imageStream;
        public Stream ImageStream
        {
            get
            {
                return imageStream;
            }
            set
            {
                imageStream = value;
                if (OnCaptureCompleted != null)
                {
                    OnCaptureCompleted(this, EventArgs.Empty);
                }
            }
        }

        public static void Initialize<TCameraUtil>()
            where TCameraUtil : CameraUtil, new()
        {
            if (Current == null)
            {
                Current = new TCameraUtil();
            }
        }

        /// <summary>
        /// Shrinks the image if necessary to keep it within the maximum allowed size,
        /// maintaining the aspect ratio of the original image.
        /// </summary>
        /// <remarks>
        /// Images taken with high resolutions are too expensive to send on the wire and
        /// unnecessary for the capture scenario, so we limit their size.
        /// </remarks>
        public abstract MemoryStream ResizeImage(Stream imageStream, String filePath);

        /// <summary>
        /// Open the camera of the current device
        /// </summary>
        public abstract void OpenCamera();

        /// <summary>
        /// Get the current image as bytes
        /// </summary>
        /// <returns></returns>
        public byte[] GetImageBytes()
        {
            byte[] imgArray = null;

            if (this.imageStream != null)
            {
                imgArray = new byte[(int)imageStream.Length];
                imageStream.Position = 0;
                imageStream.Read(imgArray, 0, (int)imageStream.Length);
                imageStream.Seek(0, SeekOrigin.Begin);
            }
            return imgArray;
        }
    }
}
