using Common.Utilities.Camera;
using Foundation;
using MobileCoreServices;
using System;
using System.IO;
using UIKit;

namespace Common.iOS.Utilities
{
    public class CameraiOS : CameraUtil
    {
        public override void OpenCamera()
        {
            if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
            {
                // Define the camera parameters.
                UIImagePickerController imagePicker = new UIImagePickerController();
                imagePicker.MediaTypes = new string[] { UTType.Image.ToString() };
                imagePicker.AllowsEditing = true;
                imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;                

                imagePicker.Canceled += (sender, e) => this.closeCamera(imagePicker);

                imagePicker.FinishedPickingMedia += (sender, e) =>
                {
                    // Save the image taken locally and trigger the event to save the receipt.
                    string filepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Expense.jpg");
                    UIImage image = (UIImage)e.Info.ObjectForKey(new NSString("UIImagePickerControllerOriginalImage"));

                    // Arbitrarily resize according to the defined compression quality to reduce filesize and save memory.
                    image.AsJPEG(new nfloat(compressionQuality / 100)).Save(filepath, false);
                    CameraUtil.Current.ImageStream = image.AsJPEG().AsStream();

                    this.closeCamera(imagePicker);
                };
                
                // Load the camera app on the current page.
                UIViewController currentController = this.getCurrentViewController();
                currentController.AddChildViewController(imagePicker);
                imagePicker.DidMoveToParentViewController(currentController);
                currentController.View.AddSubview(imagePicker.View);
            }
        }

        public override MemoryStream ResizeImage(Stream imageStream, string filePath)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Close the controller for the camera and bring the parent view back to focus.
        /// </summary>
        /// <param name="imagePicker">The controller for the camera.</param>
        private void closeCamera(UIImagePickerController imagePicker)
        {
            imagePicker.View.RemoveFromSuperview();
            imagePicker.RemoveFromParentViewController();
        }

        /// <summary>
        /// Gets the top most view controller on the application.
        /// </summary>
        /// <returns>The <c>UIViewController</c> object.</returns>
        private UIViewController getCurrentViewController()
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            UIViewController currentController = window.RootViewController;

            while (currentController.PresentedViewController != null)
            {
                currentController = currentController.PresentedViewController;
            }

            return currentController;
        }
    }
}
