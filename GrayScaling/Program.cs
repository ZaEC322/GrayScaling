
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

string ImgPath = "E:\\School\\Master\\Diploma\\fishTEST";



Console.WriteLine("ResizeBitmap + FlipBitmapHorizontal");
var paths = Directory.GetFiles(ImgPath, "*.png*", SearchOption.AllDirectories)
                .ToList();
foreach (var item in paths)
{
    Console.WriteLine(item);
    transform1(item);
}
Console.WriteLine("FlipBitmapVertical");
paths = Directory.GetFiles(ImgPath, "*.png*", SearchOption.AllDirectories)
                .ToList();
foreach (var item in paths)
{
    Console.WriteLine(item);
    transform2(item);
}
Console.WriteLine("RotationOnDegree");

paths = Directory.GetFiles(ImgPath, "*.png*", SearchOption.AllDirectories)
                .ToList();
foreach (var item in paths)
{
    Console.WriteLine(item);
    transform3(item);
}

void transform1(string FileName1)
{
    using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
    {
        using (Bitmap? image = new Bitmap(ms))
        {
            File.Delete(FileName1.ToString());
            ResizeBitmap(image, 224, 224).Save(FileName1);
        }
    }
    using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
    {
        using (Bitmap? image = new Bitmap(ms))
        {
            FlipBitmapHorizontal(image).Save(Path.GetDirectoryName(FileName1)
                                             + Path.DirectorySeparatorChar
                                             + Path.GetFileNameWithoutExtension(FileName1)
                                             + "_FlipedHorizontal"
                                             + Path.GetExtension(FileName1));
        }
    }


}


void transform2(string FileName1)
{


    using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
    {
        using (Bitmap? image = new Bitmap(ms))
        {
            FlipBitmapVertical(image).Save(Path.GetDirectoryName(FileName1)
                                             + Path.DirectorySeparatorChar
                                             + Path.GetFileNameWithoutExtension(FileName1)
                                             + "_FlipVertical"
                                             + Path.GetExtension(FileName1));
        }
    }
}


void transform3(string FileName1)
{


    using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
    {
        using (Bitmap? image = new Bitmap(ms))
        {
            for (int i = 15; i < 180; i = i + 15)
            {
                RotationOnDegree(image, i).Save(Path.GetDirectoryName(FileName1)
                                             + Path.DirectorySeparatorChar
                                             + Path.GetFileNameWithoutExtension(FileName1)
                                             + "_RotationOnDegree" + i
                                             + Path.GetExtension(FileName1));
            }
           

        }
    }

}


Bitmap FlipBitmapHorizontal(Bitmap original)
{
    original.RotateFlip(RotateFlipType.RotateNoneFlipX);
    return original;
}

Bitmap FlipBitmapVertical(Bitmap original)
{
    original.RotateFlip(RotateFlipType.RotateNoneFlipY);
    return original;
}

Bitmap RotationOnDegree(Bitmap bmp, float angle)
{
    //create a new empty bitmap to hold rotated image
    Bitmap returnBitmap = new Bitmap(bmp.Width, bmp.Height);
    //make a graphics object from the empty bitmap
    using (Graphics g = Graphics.FromImage(returnBitmap))
    {
        //move rotation point to center of image
        g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
        //rotate
        g.RotateTransform(angle);
        //move image back
        g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
        //draw passed in image onto graphics object
        g.DrawImage(bmp, new Point(0, 0));
    }
    return returnBitmap;
}


Bitmap ResizeBitmap(Bitmap original, int canvasWidth, int canvasHeight)
{
    Bitmap result = new Bitmap(canvasWidth, canvasHeight);
    using (Graphics g = Graphics.FromImage(result))
    {
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        g.CompositingQuality = CompositingQuality.HighQuality;

        // Figure out the ratio
        double ratioX = (double)canvasWidth / original.Width;
        double ratioY = (double)canvasHeight / original.Height;
        // use whichever multiplier is smaller
        double ratio = ratioX < ratioY ? ratioX : ratioY;

        // now we can get the new height and width
        int newHeight = Convert.ToInt32(original.Height * ratio);
        int newWidth = Convert.ToInt32(original.Width * ratio);

        // Now calculate the X,Y position of the upper-left corner 
        // (one of these will always be zero)
        int posX = Convert.ToInt32((canvasWidth - (original.Width * ratio)) / 2);
        int posY = Convert.ToInt32((canvasHeight - (original.Height * ratio)) / 2);

        g.Clear(Color.Transparent); // Transparent padding
        g.DrawImage(original, posX, posY, newWidth, newHeight);

        /* ------------- end new code ---------------- */
    }
    return result;
}
