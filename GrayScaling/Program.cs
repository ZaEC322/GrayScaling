
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
internal class Program
{
    private static void Main(string[] args)
    {
        const string ImgPath = "E:\\School\\Master\\Diploma\\test\\qqq";

        const int DesiredImageCount = 200;
        const int ImageWidth = 224;
        const int ImageHeight = 224;

        const int HorisontalFlipMultiplier = 2;
        const int VerticalFlipMultiplier = 2;

        const int maxRotation = 179;
        const int minRotation = 1;


        static List<string> GetFiles(string DirectoryPath)
        {
            List<string> list = Directory.GetFiles(DirectoryPath, "*.png*", SearchOption.AllDirectories).ToList();

            return list;
        }

      




        //Без зележності від каталогу збираємо усі фото з усіх каталогів і виконуємо Resize
        Perform_Resize(GetFiles(ImgPath), ImageWidth, ImageHeight);

        //отримуємо колекцію шляхів всіх каталогів(класів розпізнавання)
        List<string> DirectoriesList = CustomSearcher.GetDirectories(ImgPath);



        //для кожного каталогу окремо рахуемо кількість зображень
        Parallel.ForEach(DirectoriesList, DirectoryPath => {

            double Result = (double)DesiredImageCount/(double)GetFiles(DirectoryPath).Count();

            // половина або більше
            if (Result<=2)
                Perform_FlipBitmapHorizontal(GetFiles(DirectoryPath).Take(DesiredImageCount - GetFiles(DirectoryPath).Count()).ToList());

            //приблизно чверть від необхідного
            else if (Result>2 && Result<=4)
            {
                Perform_FlipBitmapHorizontal(GetFiles(DirectoryPath));

                // List<string> ass = (List<string>)GetFiles(DirectoryPath).Where((x, i) => i % 2 == 0);



                int u = GetFiles(DirectoryPath).Count / ( DesiredImageCount - GetFiles(DirectoryPath).Count);

                List<string> files = new List<string>();

                for (int i = 0; i < GetFiles(DirectoryPath).Count; i = i + u)
                {
                    files.Add(GetFiles(DirectoryPath)[i]);
                }






                //обирати кожен u елемент з ліста
                Perform_FlipBitmapVertical(files);
            }


            




            //менше четверті від необхідного
            else if (Result>4)
            {

                Perform_FlipBitmapHorizontal(GetFiles(DirectoryPath));
                Perform_FlipBitmapVertical(GetFiles(DirectoryPath));


                /*                int u = GetFiles(DirectoryPath).Count / ( DesiredImageCount - GetFiles(DirectoryPath).Count);

                List<string> files = new List<string>();

                for (int i = 0; i < GetFiles(DirectoryPath).Count; i = i + u)
                {
                    files.Add(GetFiles(DirectoryPath)[i]);
                }
                
                 шоб заработало чекнуть совместимость єтого куска куда с degree()
                 
                 */


                Perform_RotationOnDegree(GetFiles(DirectoryPath), Degree());

            }

            int Degree()
            {
                Result = maxRotation/(Result/(HorisontalFlipMultiplier * VerticalFlipMultiplier));

                if (Result<minRotation)
                    return minRotation;
                else if (Result>maxRotation)
                    return maxRotation;
                else
                    return Convert.ToInt32(Math.Round(Result));
            }




        });
    }

    //x * y де 'x' це кількість зображень а 'y' це у скількі раз треба збільшити кільксть зображень до необхідної (OptimalImageCount)
    static void Perform_RotationOnDegree(List<string> ImagesFromDirectoryPath, int Degree)
    {
        //беремо усі файли у директорії
        Parallel.ForEach(ImagesFromDirectoryPath, (ImagePath) =>
        {
            //x*y

            using (var ms = new MemoryStream(File.ReadAllBytes(ImagePath)))
            {
                using (Bitmap? image = new Bitmap(ms))
                {
                    for (int i = Degree; i < 180; i = i + Degree)
                    {
                        RotationOnDegree(image, i).Save(Path.GetDirectoryName(ImagePath)
                                                     + Path.DirectorySeparatorChar
                                                     + Path.GetFileNameWithoutExtension(ImagePath)
                                                     + "_RotationOnDegree" + i
                                                     + Path.GetExtension(ImagePath));
                    }
                }
            }

            Console.WriteLine("RotationOnDegree " + ImagePath);
        });
    }


    static void Perform_Resize(List<string> ImagesFromDirectoryPath, int Width, int Height)
    {
        //змінююємо розмір всім зображенням не залежно від каталогу
        Parallel.ForEach(ImagesFromDirectoryPath, ImgPath =>
                       {
                           using (var ms = new MemoryStream(File.ReadAllBytes(ImgPath)))
                           {
                               using (Bitmap? image = new Bitmap(ms))
                               {
                                   File.Delete(ImgPath.ToString());
                                   ResizeBitmap(image, Width, Height).Save(ImgPath);
                               }
                           }
                           Console.WriteLine("Resize " + ImgPath);
                       });
    }


    static void Perform_FlipBitmapHorizontal(List<string> ImagesFromDirectoryPath)
    {
        Parallel.ForEach(ImagesFromDirectoryPath, ImagePath =>
        {
            //x*2
            using (var ms = new MemoryStream(File.ReadAllBytes(ImagePath)))
            {
                using (Bitmap? image = new Bitmap(ms))
                {
                    FlipBitmapHorizontal(image).Save(Path.GetDirectoryName(ImagePath)
                                                     + Path.DirectorySeparatorChar
                                                     + Path.GetFileNameWithoutExtension(ImagePath)
                                                     + "_FlipedHorizontal"
                                                     + Path.GetExtension(ImagePath));
                }
            }
            Console.WriteLine("FlipHorizontal " + ImagePath);
        });
    }

    static void Perform_FlipBitmapVertical(List<string> ImagesFromDirectoryPath)
    {
        Parallel.ForEach(ImagesFromDirectoryPath, ImagePath =>
        {
            //x*2
            using (var ms = new MemoryStream(File.ReadAllBytes(ImagePath)))
            {
                using (Bitmap? image = new Bitmap(ms))
                {
                    FlipBitmapVertical(image).Save(Path.GetDirectoryName(ImagePath)
                                                     + Path.DirectorySeparatorChar
                                                     + Path.GetFileNameWithoutExtension(ImagePath)
                                                     + "_FlipVertical"
                                                     + Path.GetExtension(ImagePath));
                }
            }
            Console.WriteLine("FlipVertical " + ImagePath);
        });
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