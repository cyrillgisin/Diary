using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Content;
using System;

namespace Images.Activitïes
{
    [Activity(Label = "Images", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ImageActivityBaseClass
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Title = "My Image Test";

            // Show the image-Buttons
            showXButtons(1);
        }

        protected override int getLayoutResourceId()
        {
            return Resource.Layout.Main;
        }

        protected override void imageButton_Click(object sender, EventArgs args)
        {
            var id = getIdFromImageButton(sender);

            if (id > loadedImagesCount)
            {
                // new img
                openGallery();
            }
            else
            {
                // existing img
                Intent intent = prepareIntent(new Intent(this, typeof(ShowImageActivity)));
                        
                // Determine current img, that has been clicked
                intent.PutExtra("CurrentImg", id);

                this.StartActivityForResult(intent, 1);
            }
        }

        // Gets called when:
        // RequestCode 0: Gallery closed
        // RequestCode 1: ShowImageActivity closed
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            // Gallery closed
            if (requestCode == 0)
            {
                if (resultCode == Result.Ok)
                {
                    handleGalleryActivityResultIntent(data);
                }

                showXButtons(loadedImagesCount + 1);
            }
            // ShowImageActivity closed
            else if (requestCode == 1)
            {
                loadUrisFromIntentIntoImages(data);
            }
        }  
    }
}

