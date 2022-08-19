
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

string ImgPath = "E:\\School\\Master\\Diploma\\fishTEST";

var paths = Directory.GetFiles(ImgPath, "*.png*", SearchOption.AllDirectories)
                .ToList();
long temp = 0;
foreach (var item in paths)
{
    Console.WriteLine(item);
    transform(item, temp);
    Console.WriteLine("done");
    temp++;
}


//
// ПРИДУМАТЬ КАК ДЕЛАТЬ НАЗВАНИЯ ДЛЯ НОВЫХ ФАЙЛОВ
// 
//
void transform(string FileName1, long incre)
{
    using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
    {
        using (Bitmap? image = new Bitmap(ms))
        {
            //ToWhiteBlack(image);

            File.Delete(FileName1.ToString());
            ResizeBitmap(image, 224, 224).Save(FileName1);
        }
    }
   /* using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
    {
        using (Bitmap? image = new Bitmap(ms))
        {
            FlipBitmapHorizontal(image).Save(Path.GetDirectoryName(FileName1)
                                             + Path.DirectorySeparatorChar
                                             + Path.GetFileNameWithoutExtension(FileName1)
                                             + incre.ToString()
                                             + Path.GetExtension(FileName1));
        }
    }*/

}



/*void ToWhiteBlack(Bitmap original)
{
    for (var i = 0; i < original.Width; i++)
    {
        for (var j = 0; j < original.Height; j++)
        {
            var originalColor = original.GetPixel(i, j);
            var grayScale = (int)((originalColor.R*0.3) + (originalColor.G*0.59) + (originalColor.B*0.11));
            var corEmEscalaDeCinza = Color.FromArgb(originalColor.A, grayScale, grayScale, grayScale);
            original.SetPixel(i, j, corEmEscalaDeCinza);
        }
    }
}*/

Bitmap FlipBitmapHorizontal(Bitmap original)
{
    original.RotateFlip(RotateFlipType.RotateNoneFlipX);
    return original;
}

void RotationOnDegree(Bitmap original)
{

}

void Scale(Bitmap original)
{

}

void Crop(Bitmap original)
{

}

void GaussianNoise(Bitmap original)
{

}

/*Bitmap ResizeBitmap(Bitmap original, int width, int height)
{
    Bitmap result = new Bitmap(width, height);
    using (Graphics g = Graphics.FromImage(result))
    {
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.DrawImage(original, 0, 0, width, height);
    }
    return result;
}*/


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

       /* ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
        EncoderParameters encoderParameters;
        encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
        thumbnail.Save(path + newWidth + "." + originalFilename, info[1], encoderParameters);*/
    }
    return result;
}
/*
static void Swap<T>(ref T x, ref T y)
{
    var temp = x; x = y; y = temp;
}*/
/*
public class DirectBitmap : IDisposable
{
    public Bitmap Bitmap { get; private set; }
    public Int32[] Bits { get; private set; }
    public bool Disposed { get; private set; }
    public int Height { get; private set; }
    public int Width { get; private set; }

    protected GCHandle BitsHandle { get; private set; }

    public DirectBitmap(int width, int height)
    {
        Width = width;
        Height = height;
        Bits = new Int32[width * height];
        BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
    }

    public void SetPixel(int x, int y, Color colour)
    {
        int index = x + (y * Width);
        int col = colour.ToArgb();

        Bits[index] = col;
    }

    public Color GetPixel(int x, int y)
    {
        int index = x + (y * Width);
        int col = Bits[index];
        Color result = Color.FromArgb(col);

        return result;
    }

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        Bitmap.Dispose();
        BitsHandle.Free();
    }
}*/