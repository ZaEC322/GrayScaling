using System.Drawing;
using System.Drawing.Drawing2D;

namespace GrayScaling
{
    internal static class Program
    {
        private static void Main()
        {
            const string ImgPath = "E:\\School\\Master\\Diploma\\test datasets\\www";

            const int DesiredImageCount = 1000;
            const int ImageWidth = 224;
            const int ImageHeight = 224;

            const int maxRotation = 179;

            static List<string> GetFiles(string DirectoryPath)
            {
                return Directory.GetFiles(DirectoryPath, "*.png*", SearchOption.AllDirectories).ToList();
            }

            //Без зележності від каталогу збираємо усі фото з усіх каталогів і виконуємо Resize
            Perform_Resize(GetFiles(ImgPath), ImageWidth, ImageHeight);

            //отримуємо колекцію шляхів всіх каталогів(класів розпізнавання)
            List<string> DirectoriesList = CustomSearcher.GetDirectories(ImgPath);

            //для кожного каталогу окремо рахуемо кількість зображень
            Parallel.ForEach(DirectoriesList, DirectoryPath =>
            {
                //потрібна кількість/те шо є
                double Result = DesiredImageCount/(double)GetFiles(DirectoryPath).Count;
                if (Result<=1)
                {
                    return;
                }

                // половина або більше
                if (Result<=2)
                {
                    int u = Convert.ToInt32(Math.Round(GetFiles(DirectoryPath).Count / (DesiredImageCount - (double)GetFiles(DirectoryPath).Count)));

                    if (u<1)
                        u=1;

                    List<string> files = new();

                    for (int i = 0; i < GetFiles(DirectoryPath).Count; i += u)
                    {
                        files.Add(GetFiles(DirectoryPath)[i]);
                    }
                    Perform_FlipBitmapHorizontal(files);
                }

                //приблизно чверть від необхідного
                else if (Result>2 && Result<=4)
                {
                    Perform_FlipBitmapHorizontal(GetFiles(DirectoryPath));

                    int u = Convert.ToInt32(Math.Round(GetFiles(DirectoryPath).Count / (DesiredImageCount - (double)GetFiles(DirectoryPath).Count)));

                    if (u<1)
                        u=1;

                    List<string> files = new();

                    for (int i = 0; i < GetFiles(DirectoryPath).Count; i += u)
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

                    //скільки не вистачає фоток

                    double MissingPicturesCount = DesiredImageCount - GetFiles(DirectoryPath).Count;

                    int fradus = Convert.ToInt32(Math.Round((double)maxRotation / (DesiredImageCount/GetFiles(DirectoryPath).Count)));

                    // якщо сгенерованих файлів більше ніж тих що не вистачає
                    if (GetFiles(DirectoryPath).Count > MissingPicturesCount)
                    {
                        //ліст щоб не обертати усі фото що є, а тількі стількі скількі треба
                        List<string> files2 = new();

                        //заповнюємо ліст потрібною кількістю фото
                        for (int i = 0; i < MissingPicturesCount; i++)
                        {
                            files2.Add(GetFiles(DirectoryPath)[i]);
                        }
                        Perform_RotationOnDegree(files2, fradus);
                    }

                    // якщо сгенерованих файлів менше ніж тих що не вистачає
                    else if (GetFiles(DirectoryPath).Count <= MissingPicturesCount)
                    {
                        //ліст щоб не обертати усі фото що є, а тількі стількі скількі треба
                        List<string> files2 = new();

                        //заповнюємо ліст потрібною кількістю фото
                        for (int i = 0; i < GetFiles(DirectoryPath).Count; i++)
                        {
                            files2.Add(GetFiles(DirectoryPath)[i]);
                        }
                        Perform_RotationOnDegree(files2, fradus);
                    }
                }
            });
        }

        //виконуємо обертання
        private static void Perform_RotationOnDegree(List<string> ImagesFromDirectoryPath, int Degree)
        {
            //беремо усі файли у директорії
            Parallel.ForEach(ImagesFromDirectoryPath, (ImagePath) =>
            {
                //x*y

                using (var ms = new MemoryStream(File.ReadAllBytes(ImagePath)))
                {
                    using Bitmap? image = new(ms);
                    for (int i = Degree; i < 180; i += Degree)
                    {
                        RotationOnDegree(image, i).Save(Path.GetDirectoryName(ImagePath)
                                                     + Path.DirectorySeparatorChar
                                                     + Path.GetFileNameWithoutExtension(ImagePath)
                                                     + "_RotationOnDegree" + i
                                                     + Path.GetExtension(ImagePath));
                    }
                }

                Console.WriteLine("RotationOnDegree " + ImagePath);
            });
        }

        // Виконуємо зміну розміру зображень
        private static void Perform_Resize(List<string> ImagesFromDirectoryPath, int Width, int Height)
        {
            //змінююємо розмір всім зображенням не залежно від каталогу
            Parallel.ForEach(ImagesFromDirectoryPath, ImgPath =>
                           {
                               using (var ms = new MemoryStream(File.ReadAllBytes(ImgPath)))
                               {
                                   using Bitmap? image = new(ms);
                                   if (image.Height == Height && image.Width == Width)
                                   {
                                       return;
                                   }
                                   File.Delete(ImgPath);
                                   ResizeBitmap(image, Width, Height).Save(ImgPath);
                               }
                               Console.WriteLine("Resize " + ImgPath);
                           });
        }
        //виконуємо переворот горизонтальний
        private static void Perform_FlipBitmapHorizontal(List<string> ImagesFromDirectoryPath)
        {
            Parallel.ForEach(ImagesFromDirectoryPath, ImagePath =>
            {
                //x*2
                using (var ms = new MemoryStream(File.ReadAllBytes(ImagePath)))
                {
                    using Bitmap? image = new(ms);
                    FlipBitmapHorizontal(image).Save(Path.GetDirectoryName(ImagePath)
                                                     + Path.DirectorySeparatorChar
                                                     + Path.GetFileNameWithoutExtension(ImagePath)
                                                     + "_FlipedHorizontal"
                                                     + Path.GetExtension(ImagePath));
                }
                Console.WriteLine("FlipHorizontal " + ImagePath);
            });
        }
        //виконуємо переворот вертикальний
        private static void Perform_FlipBitmapVertical(List<string> ImagesFromDirectoryPath)
        {
            Parallel.ForEach(ImagesFromDirectoryPath, ImagePath =>
            {
                //x*2
                using (var ms = new MemoryStream(File.ReadAllBytes(ImagePath)))
                using (Bitmap? image = new(ms))
                {
                    FlipBitmapVertical(image).Save(Path.GetDirectoryName(ImagePath)
                                                     + Path.DirectorySeparatorChar
                                                     + Path.GetFileNameWithoutExtension(ImagePath)
                                                     + "_FlipVertical"
                                                     + Path.GetExtension(ImagePath));
                }
                Console.WriteLine("FlipVertical " + ImagePath);
            });
        }

        private static Bitmap FlipBitmapHorizontal(Bitmap original)
        {
            original.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return original;
        }

        private static Bitmap FlipBitmapVertical(Bitmap original)
        {
            original.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return original;
        }

        private static Bitmap RotationOnDegree(Bitmap bmp, float angle)
        {
            //створення нового порожнього бітового растра для зберігання оберненого зображення
            Bitmap returnBitmap = new(bmp.Width, bmp.Height);
            //створити графічний об'єкт з порожнього растрового зображення
            using (Graphics g = Graphics.FromImage(returnBitmap))
            {
                //перемістити точку обертання в центр зображення
                g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                //обернути
                g.RotateTransform(angle);
                //перемістити зображення назад
                g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                //намалювати передане зображення на графічний об'єкт
                g.DrawImage(bmp, new Point(0, 0));
            }
            return returnBitmap;
        }

        private static Bitmap ResizeBitmap(Bitmap original, int canvasWidth, int canvasHeight)
        {
            Bitmap result = new(canvasWidth, canvasHeight);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                // Розрахувати співвідношення
                double ratioX = (double)canvasWidth / original.Width;
                double ratioY = (double)canvasHeight / original.Height;
                // використовувати той мультиплікатор, який є меншим
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                // тепер ми можемо отримати нові висоту і ширину
                int newHeight = Convert.ToInt32(original.Height * ratio);
                int newWidth = Convert.ToInt32(original.Width * ratio);

                // Тепер обчислюємо положення X,Y верхнього лівого кута (одне з них завжди буде дорівнювати нулю)
                int posX = Convert.ToInt32((canvasWidth - (original.Width * ratio)) / 2);
                int posY = Convert.ToInt32((canvasHeight - (original.Height * ratio)) / 2);

                g.Clear(Color.Transparent); // Прозора оболонка
                g.DrawImage(original, posX, posY, newWidth, newHeight);

            }
            return result;
        }
    }

    public static class CustomSearcher
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
}