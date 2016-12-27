using Android.Widget;

namespace Images.Model
{
    public class ImageModel
    {
        public int ID { get; set; }
        public ImageButton ImageButton { get; set; }
        public Android.Net.Uri URI { get; set; }

        public ImageModel(int id, ImageButton imgButton, Android.Net.Uri uri)
        {
            ID = id;
            ImageButton = imgButton;
            URI = uri;
        }
    }
}