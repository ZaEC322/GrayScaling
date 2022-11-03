
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;

internal class Program
{
    private static void Main(string[] args)
    {
        string ImgPath = "E:\\School\\Master\\Diploma\\test";

        const int OptimalImageCount = 1000;

        //Спочатку всі фото в потрібний розмір (224х224)
        foreach (var item in Directory.GetFiles(ImgPath, "*.png*", SearchOption.AllDirectories)
                       .ToList())
        {
            Perform_Resize(item);
            Console.WriteLine("Resize " + item);
        }

        /*int countDirectories = Directory.GetDirectories(ImgPath, "*.png", SearchOption.AllDirectories).Count();*/

        //отримуємо колекцію шляхів всіх каталогів(класів розпізнавання)
        var directories = CustomSearcher.GetDirectories(ImgPath);

        //у кожному каталозі
        foreach (string item in directories)
        {

            //Кількість фото у каталозі
            int imageCount = Directory.GetFiles(item, "*.png*", SearchOption.AllDirectories).Count();

            //якщо не вистачає файлів у каталозі 
            if (imageCount<OptimalImageCount)
            {
                //беремо усі файли у директорії
                foreach (var item1 in Directory.GetFiles(item, "*.png*", SearchOption.AllDirectories)
                       .ToList())
                {
                    //x*2
                    Perform_FlipBitmapHorizontal(item1);
                    Console.WriteLine("FlipHorizontal " + item1);
                }
                imageCount = imageCount * 2;
            }

            //якщо не вистачає файлів у каталозі 
            if (imageCount<OptimalImageCount)
            {
                //беремо усі файли у директорії
                foreach (var item1 in Directory.GetFiles(item, "*.png*", SearchOption.AllDirectories)
                       .ToList())
                {
                    //x*2
                    Perform_FlipBitmapVertical(item1);
                    Console.WriteLine("FlipVertical " + item1);
                }
                imageCount = imageCount * 2;
            }

            if (imageCount<OptimalImageCount)
            {

                int y = Convert.ToInt32(Math.Ceiling(180/Math.Ceiling(OptimalImageCount/(double)imageCount)));

                //int y = 0;

                //беремо усі файли у директорії
                foreach (var item1 in Directory.GetFiles(item, "*.png*", SearchOption.AllDirectories)
                       .ToList())
                {
                    //x*y
                    Perform_RotationOnDegree(item1, y);
                    Console.WriteLine("RotationOnDegree " + item1);
                }
            }

        }
        /*ЗРОБИТИ ПАРАЛЕЛІЗМ*/
    }

    // x*1
    static void Perform_Resize(string FileName1)
    {
        using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
        {
            using (Bitmap? image = new Bitmap(ms))
            {
                File.Delete(FileName1.ToString());
                ResizeBitmap(image, 224, 224).Save(FileName1);
            }
        }



    }

    // x * 2
    static void Perform_FlipBitmapHorizontal(string FileName1)
    {
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

    // x * 2 кількість зображень подвоююється
    static void Perform_FlipBitmapVertical(string FileName1)
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

    //x * y де 'x' це кількість зображень а 'y' це у скількі раз треба збільшити кільксть зображень до необхідної (OptimalImageCount)
    static void Perform_RotationOnDegree(string FileName1, int y)
    {

        using (var ms = new MemoryStream(File.ReadAllBytes(FileName1)))
        {
            using (Bitmap? image = new Bitmap(ms))
            {
                for (int i = y; i < 180; i = i + y)
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


    static Bitmap FlipBitmapHorizontal(Bitmap original)
    {
        original.RotateFlip(RotateFlipType.RotateNoneFlipX);
        return original;
    }

    static Bitmap FlipBitmapVertical(Bitmap original)
    {
        original.RotateFlip(RotateFlipType.RotateNoneFlipY);
        return original;
    }

    static Bitmap RotationOnDegree(Bitmap bmp, float angle)
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

    static Bitmap ResizeBitmap(Bitmap original, int canvasWidth, int canvasHeight)
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
            int posX = Convert.ToInt32((canvasWidth - original.Width * ratio) / 2);
            int posY = Convert.ToInt32((canvasHeight - original.Height * ratio) / 2);

            g.Clear(Color.Transparent); // Transparent padding
            g.DrawImage(original, posX, posY, newWidth, newHeight);

            // ------------- end new code ---------------- 
        }
        return result;
    }

}

public class CustomSearcher
{
    public static List<string> GetDirectories(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.AllDirectories)
    {
        if (searchOption == SearchOption.TopDirectoryOnly)
            return Directory.GetDirectories(path, searchPattern).ToList();

        var directories = new List<string>(GetDirectories(path, searchPattern));

        for (var i = 0; i < directories.Count; i++)
            directories.AddRange(GetDirectories(directories[i], searchPattern));

        return directories;
    }

    private static List<string> GetDirectories(string path, string searchPattern)
    {
        try
        {
            return Directory.GetDirectories(path, searchPattern).ToList();
        }
        catch (UnauthorizedAccessException)
        {
            return new List<string>();
        }
    }
}