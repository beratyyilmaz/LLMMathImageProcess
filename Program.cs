using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing; // Referans eklemeyi unutma!
using System.IO;

namespace SimpleML_ImageProcess
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "ML Görüntü İşleme - Aydınlık/Karanlık Sınıflandırma";

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================================");
                Console.WriteLine("   ML & MATEMATİKSEL GÖRÜNTÜ ANALİZ PROJESİ");
                Console.WriteLine("=================================================");
                Console.WriteLine("Program, piksellerin RGB değerlerini matematiksel formülle");
                Console.WriteLine("işleyerek resmin 'Aydınlık' veya 'Karanlık' olduğuna karar verir.\n");

                Console.Write("Lütfen resim yolunu yapıştırın (Çıkış için 'exit' yazın): ");
                string input = Console.ReadLine().Trim('"'); // Tırnakları temizle

                if (input.ToLower() == "exit") break;

                if (File.Exists(input))
                {
                    try
                    {
                        ProcessImage(input);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("HATA: Resim okunamadı. Formatı kontrol edin.");
                        Console.WriteLine("Hata detayı: " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("\n[!] Dosya bulunamadı. Lütfen yolu kontrol edin.");
                }

                Console.WriteLine("\nDevam etmek için bir tuşa basın...");
                Console.ReadKey();
            }
        }

        static void ProcessImage(string imagePath)
        {
            Console.WriteLine("\n>>> Görüntü işleniyor, lütfen bekleyin...");

            // 1. Resmi Yükle
            Bitmap bmp = new Bitmap(imagePath);
            long totalBrightness = 0;
            int pixelCount = 0;

            // 2. Görüntü İşleme Döngüsü (Piksel Piksel Gezme)
            // Not: Çok büyük resimlerde bu işlem 2-3 saniye sürebilir.
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    // Mevcut pikselin rengini al
                    Color pixel = bmp.GetPixel(x, y);

                    // 3. MATEMATİK KISMI (Luminance Formülü)
                    // İnsan gözü Yeşile daha duyarlıdır, bu yüzden katsayılar farklıdır.
                    // Formül: Y = 0.299*R + 0.587*G + 0.114*B
                    double brightness = (0.299 * pixel.R) + (0.587 * pixel.G) + (0.114 * pixel.B);

                    totalBrightness += (long)brightness;
                    pixelCount++;
                }
            }

            // 4. İSTATİSTİK ÇIKARMA
            // Ortalama parlaklığı hesapla (0 ile 255 arasında bir değer çıkar)
            double averageBrightness = totalBrightness / pixelCount;

            // 5. ML (Makine Öğrenmesi) KARAR MEKANİZMASI / SINIFLANDIRMA
            // Eşik değeri (Threshold) belirliyoruz. Genelde 0-255 arası tam orta 127.5'tir.
            // Biz 100 diyelim, çünkü loş ortamlar da karanlık sayılsın.
            double threshold = 100;

            string predictionLabel = "";
            string statusColor = "";

            if (averageBrightness > threshold)
            {
                predictionLabel = "AYDINLIK / GÜNDÜZ";
                statusColor = "Success";
            }
            else
            {
                predictionLabel = "KARANLIK / GECE";
                statusColor = "Warning";
            }

            // --- SONUÇLARI YAZDIR ---
            Console.WriteLine("\n---------------- SONUÇLAR ----------------");
            Console.WriteLine($"Toplam Piksel Sayısı  : {pixelCount:N0}"); // Binlik ayraçlı yaz
            Console.WriteLine($"Ortalama Parlaklık    : {averageBrightness:F2} / 255");
            Console.WriteLine($"Eşik Değeri (Limit)   : {threshold}");
            Console.WriteLine("------------------------------------------");

            Console.Write("YAPAY ZEKA TAHMİNİ: ");
            Console.BackgroundColor = averageBrightness > threshold ? ConsoleColor.White : ConsoleColor.DarkGray;
            Console.ForegroundColor = averageBrightness > threshold ? ConsoleColor.Black : ConsoleColor.White;
            Console.Write($" {predictionLabel} ");
            Console.ResetColor();
            Console.WriteLine("\n------------------------------------------");

            // Kaynakları serbest bırak (Ram şişmemesi için önemli)
            bmp.Dispose();
        }
    }
}