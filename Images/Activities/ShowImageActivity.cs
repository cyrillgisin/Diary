using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using UK.CO.Senab.Photoview;
using System.Linq;
using Android.Runtime;
using Images.Model;

namespace Images.Activitïes
{
    [Activity(Label = "ShowImageActivity")]
    public class ShowImageActivity : ImageActivityBaseClass
    {
        ImageView imageView;
        ImageModel currentImage;

        // Colors for image-Buttons
        Color defaultBtnColor = Color.White;
        Color colorBtnClick = Color.Green;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Title = "View images";

            // Get GUI elements
            imageView = FindViewById<ImageView>(Resource.Id.imgView);

            // Get uris from intent 
            loadUrisFromIntentIntoImages(null);

            // Show image that was clicked
            showImage(Intent.GetIntExtra("CurrentImg", 1));
            
            // Show the image-Buttons
            showXButtons(loadedImagesCount + 1);

            // Additional eventhandler for the delete-buttons
            setEventHandlersForDeleteButtons(deleteButtons, deleteButton_Click);
        }

        protected override int getLayoutResourceId()
        {
            return Resource.Layout.ShowImageLayout;
        }

        protected override void imageButton_Click(object sender, EventArgs args)
        {
            var id = getIdFromImageButton(sender);

            // Change background of all images back to default color
            foreach (var img in images)
                img.ImageButton.SetBackgroundColor(defaultBtnColor);
            
            if (id > loadedImagesCount)
            {
                // new img
                openGallery();
            }
            else
            {
                // existing img
                showImage(id);
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            // Gallery closed
            if (requestCode == 0)
            {
                var countImagesBeforeGalleryChoosen = loadedImagesCount;

                if (resultCode == Result.Ok)
                {
                    handleGalleryActivityResultIntent(data);
                    showImage(countImagesBeforeGalleryChoosen + 1);
                }

                showXButtons(loadedImagesCount + 1);
            }
        }

        private void showImage(int id)
        {
            if (id >= 1)
            {
                // Get image
                currentImage = images.Where(img => img.ID == id).FirstOrDefault();
                // Set background color 
                currentImage.ImageButton.SetBackgroundColor(colorBtnClick);

                // Show current img that was clicked
                using (var bitmap = BitmapFactory.DecodeStream(ContentResolver.OpenInputStream(currentImage.URI)))
                {
                    imageView.SetImageBitmap(bitmap);
                    var attacher = new PhotoViewAttacher(imageView);
                }
            }
            else
            {
                imageView.SetImageResource(0);
            }
        }

        // If an image is delete, show the image to the left
        private new void deleteButton_Click(object sender, EventArgs args)
        {
            var id = getIdFromDeleteButton(sender);

            if (currentImage.ID >= id)
                showImage(currentImage.ID - 1);
        }

        public override void OnBackPressed()
        {
            Intent intent = prepareIntent(new Intent(this, typeof(MainActivity)));
            SetResult(Result.Ok, intent);
            Finish();

            Intent.Dispose();
            imageView.Dispose();
            GC.Collect();

            base.OnBackPressed();
        }
    }
}