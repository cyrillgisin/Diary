using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Images.Model;
using Images.Helpers;
using Android.Runtime;

namespace Images.Activitïes
{
    [Activity(Label = "ImageActivityBaseClass")]
    public abstract class ImageActivityBaseClass : Activity
    {
        protected List<ImageModel> images;
        protected Dictionary<int, ImageButton> deleteButtons;

        // Image properties
        protected const short maximumImagesEnabled = 8;
        protected const int imageHeight = 100;
        protected const int imageWidth = 100;

        protected short loadedImagesCount = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(getLayoutResourceId());

            // GUI elements
            images = getImageButtonsList();
            deleteButtons = getDeleteButtonsDictionary();

            // Set event handlers
            setEventHandlersForImages(images, imageButton_Click);
            setEventHandlersForDeleteButtons(deleteButtons, deleteButton_Click);
        }

        protected abstract int getLayoutResourceId();
        protected abstract void imageButton_Click(object sender, EventArgs args);
        protected abstract override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data);

        protected void deleteButton_Click(object sender, EventArgs args)
        {
            var id = getIdFromDeleteButton(sender);

            // Move everything to the left
            for (int i = id; i < loadedImagesCount; i++)
            {
                // Current img, that has been deleted
                var currImg = images.Where(img => img.ID == i).FirstOrDefault();
                // Next image, on the right of the current img
                var nextImg = images.Where(img => img.ID == (i + 1)).FirstOrDefault();

                currImg.URI = nextImg.URI;
                currImg.ImageButton.SetImageBitmap(BitmapDecoder.DecodeBitmapFromStream(this, currImg.URI, imageWidth, imageHeight));
            }

            loadedImagesCount--;
            showXButtons(loadedImagesCount + 1);
        }

        protected void handleGalleryActivityResultIntent(Intent data)
        {
            if (data.Data != null)
            {
                // Single img
                if (loadedImagesCount < maximumImagesEnabled)
                {
                    var currentImg = images.Where(img => img.ID == (loadedImagesCount + 1)).FirstOrDefault();
                    currentImg.ImageButton.SetImageBitmap(BitmapDecoder.DecodeBitmapFromStream(this, data.Data, imageWidth, imageHeight));
                    currentImg.URI = data.Data;

                    loadedImagesCount++;
                }
            }
            else
            {
                // Multiple imgs
                var countItems = data.ClipData.ItemCount + loadedImagesCount > maximumImagesEnabled
                    ? maximumImagesEnabled - loadedImagesCount : data.ClipData.ItemCount;

                for (int i = 0; i < countItems; i++)
                {
                    var currentImg = images.Where(img => img.ID == (loadedImagesCount + 1)).FirstOrDefault();
                    currentImg.ImageButton.SetImageBitmap(BitmapDecoder.DecodeBitmapFromStream(this, data.ClipData.GetItemAt(i).Uri, imageWidth, imageHeight));
                    currentImg.URI = data.ClipData.GetItemAt(i).Uri;

                    loadedImagesCount++;
                }
            }
        }

        protected int getIdFromImageButton(object sender)
        {
            foreach (var img in images)
            {
                if (img.ImageButton.Id == ((ImageButton)sender).Id)
                {
                    return img.ID;
                }
            }

            return 0;
        }

        protected int getIdFromDeleteButton(object sender)
        {
            foreach (var deleteBtn in deleteButtons)
            {
                // Find delete-Button that has been clicked
                if (deleteBtn.Value.Id == ((ImageButton)sender).Id)
                {
                    return deleteBtn.Key;
                }
            }

            return 0;
        }

        protected void openGallery()
        {
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.PutExtra(Intent.ExtraAllowMultiple, true);
            intent.SetAction(Intent.ActionGetContent);
            this.StartActivityForResult(Intent.CreateChooser(intent, "Select pictures (max. 8)"), 0);
        }

        // Put all the image-uris to separate extras
        protected Intent prepareIntent(Intent intent)
        {
            for (int i = 1; i <= images.Count; i++)
                intent.PutExtra(i.ToString(), images.Where(im => im.ID == i).FirstOrDefault().URI);

            return intent;
        }

        protected void loadUrisFromIntentIntoImages(Intent intent)
        {
            loadedImagesCount = 0;
            var data = intent ?? Intent;

            if (data != null)
            {
                // Get uris from intent 
                for (int i = 1; i <= maximumImagesEnabled; i++)
                {
                    var extra = data.GetParcelableExtra(i.ToString());
                    var currImg = images.Where(img => img.ID == i).FirstOrDefault();

                    if (extra != null)
                    {
                        currImg.URI = (Android.Net.Uri)extra;
                        currImg.ImageButton.SetImageBitmap(BitmapDecoder.DecodeBitmapFromStream(this, currImg.URI, imageWidth, imageHeight));
                        loadedImagesCount++;
                    }
                    else
                        currImg.URI = null;
                }
            }

            showXButtons(loadedImagesCount + 1);
        }

        private void showDefaultPlusButtonOnLastImage()
        {
            // Show the plus-Btn on the last image
            var lastImg = images.Where(img => img.ID == (loadedImagesCount + 1)).FirstOrDefault();

            if (lastImg != null)
            {
                lastImg.URI = null;
                lastImg.ImageButton.SetImageBitmap(BitmapDecoder.DecodeBitmapFromResource(this.Resources, Resource.Drawable.plusImage, imageWidth, imageHeight));
            }
        }

        protected void showXButtons(int x)
        {
            int countDeleteBtns = 0;
            bool maxImagesLoaded = false;

            if (x > maximumImagesEnabled)
            {
                maxImagesLoaded = true;
                countDeleteBtns = maximumImagesEnabled;
                x = maximumImagesEnabled;
            }
            else if (x > 1)
                countDeleteBtns = x - 1;
            else if (x < 1)
                x = 1;

            foreach (var img in images)
                img.ImageButton.Visibility = Android.Views.ViewStates.Invisible;

            foreach (var btnDelete in deleteButtons)
                btnDelete.Value.Visibility = Android.Views.ViewStates.Invisible;

            for (int i = 1; i <= x; i++)
            {
                images.Where(img => img.ID == i).FirstOrDefault().ImageButton.Visibility = Android.Views.ViewStates.Visible;

                if (i <= countDeleteBtns)
                    deleteButtons[i].Visibility = Android.Views.ViewStates.Visible;
            }

            if (!maxImagesLoaded)
                showDefaultPlusButtonOnLastImage();
        }

        protected virtual List<ImageModel> getImageButtonsList()
        {
            return new List<ImageModel>()
            {
                new ImageModel(1, FindViewById<ImageButton>(Resource.Id.imgBtn1), null),
                new ImageModel(2, FindViewById<ImageButton>(Resource.Id.imgBtn2), null),
                new ImageModel(3, FindViewById<ImageButton>(Resource.Id.imgBtn3), null),
                new ImageModel(4, FindViewById<ImageButton>(Resource.Id.imgBtn4), null),
                new ImageModel(5, FindViewById<ImageButton>(Resource.Id.imgBtn5), null),
                new ImageModel(6, FindViewById<ImageButton>(Resource.Id.imgBtn6), null),
                new ImageModel(7, FindViewById<ImageButton>(Resource.Id.imgBtn7), null),
                new ImageModel(8, FindViewById<ImageButton>(Resource.Id.imgBtn8), null),
            };
        }

        protected virtual Dictionary<int, ImageButton> getDeleteButtonsDictionary()
        {
            return new Dictionary<int, ImageButton>()
            {
                { 1, FindViewById<ImageButton>(Resource.Id.deleteBtn1) },
                { 2, FindViewById<ImageButton>(Resource.Id.deleteBtn2) },
                { 3, FindViewById<ImageButton>(Resource.Id.deleteBtn3) },
                { 4, FindViewById<ImageButton>(Resource.Id.deleteBtn4) },
                { 5, FindViewById<ImageButton>(Resource.Id.deleteBtn5) },
                { 6, FindViewById<ImageButton>(Resource.Id.deleteBtn6) },
                { 7, FindViewById<ImageButton>(Resource.Id.deleteBtn7) },
                { 8, FindViewById<ImageButton>(Resource.Id.deleteBtn8) },
            };
        }

        private void setEventHandlersForImages(List<ImageModel> images, EventHandler eventDelegate)
        {
            foreach (var img in images)
                img.ImageButton.Click += eventDelegate;
        }

        protected void setEventHandlersForDeleteButtons(Dictionary<int, ImageButton> delButtons, EventHandler eventDelegate)
        {
            foreach (var delBtn in delButtons)
                delBtn.Value.Click += eventDelegate;
        }
    }
}