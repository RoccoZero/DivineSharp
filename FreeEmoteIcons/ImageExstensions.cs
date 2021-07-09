using System.Drawing;
using System.Drawing.Drawing2D;

namespace FreeEmoteIcons
{
    public static class ImageExstensions
    {
        public static Bitmap Rect(this Image source, float x, float y, float w, float h)
        {
            var bitmap = new Bitmap((int)w, (int)h);

            //using var gp = new GraphicsPath();
            //gp.AddRectangle(new RectangleF(0, 0, w, h));
            using var gfx = Graphics.FromImage(bitmap);
            gfx.SmoothingMode = SmoothingMode.AntiAlias;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //gfx.SetClip(gp);
            //gfx.DrawImage(source, rectDestination,  new RectangleF( 0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
            gfx.DrawImage(source, new RectangleF(0, 0, w, h),  new RectangleF( x, y, w, h), GraphicsUnit.Pixel);

            return bitmap;
        }
    }
}
