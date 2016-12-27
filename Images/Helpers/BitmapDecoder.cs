using Android.App;
using Android.Graphics;

namespace Images.Helpers
{
    public class BitmapDecoder
    {
        public static Bitmap DecodeBitmapFromResource(Android.Content.Res.Resources resource, int id, int requestedWidth, int requestedHeight)
        {
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            var TempBitmap = BitmapFactory.DecodeResource(resource, id, options);

            // Calculate InSampleSize
            options.InSampleSize = calculateInSampleSize(options, requestedWidth, requestedHeight);

            // Decode bitmap with InSampleSize set
            options.InJustDecodeBounds = false;
            Bitmap bitmap = BitmapFactory.DecodeResource(resource, id, options);
            return bitmap;
        }

        public static Bitmap DecodeBitmapFromStream(Activity activity, Android.Net.Uri data, int requestedWidth, int requestedHeight)
        {
            BitmapFactory.Options options;
            Bitmap bitmap;

            try {
                using (var stream = activity.ContentResolver.OpenInputStream(data))
                {
                    options = new BitmapFactory.Options { InJustDecodeBounds = true };
                    var TempBitmap = BitmapFactory.DecodeStream(stream, null, options);
                }

                // Calculate InSampleSize
                options.InSampleSize = calculateInSampleSize(options, requestedWidth, requestedHeight);

                // Decode bitmap with InSampleSize set
                using (var stream = activity.ContentResolver.OpenInputStream(data))
                {
                    options.InJustDecodeBounds = false;
                    bitmap = BitmapFactory.DecodeStream(stream, null, options);
                }
            }
            catch
            {
                return null;
            }
            
            return bitmap;
        }

        private static int calculateInSampleSize(BitmapFactory.Options options, int requestedWidth, int requestedHeight)
        {
            // Raw height and width of image
            int width = options.OutWidth;
            int height = options.OutHeight;
            int inSampleSize = 1;

            if (height > requestedHeight || width > requestedWidth)
            {
                // img is bigger than we want it to be
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                while (halfHeight / inSampleSize > requestedHeight && halfWidth / inSampleSize > requestedWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }
    }
}