using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Web;

namespace ShopOnline.Services
{
    public class ImageServices
    {
        public ImageServices()
        {
        }

        public string SaveCroppedImage(string uploadDir, HttpPostedFileBase uploadFile, int width, int height)
        {
            try
            {
                if (uploadFile == null || uploadFile.ContentLength == 0)
                    return null;

                using (var image = Image.Load(uploadFile.InputStream))
                {
                    // Resize ảnh
                    image.Mutate(x => x.Resize(width, height));

                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    // Tạo tên file mới
                    var newFileName = Guid.NewGuid() + ".jpg";
                    var path = Path.Combine(uploadDir, newFileName);

                    // Nén ảnh
                    var encoder = new JpegEncoder
                    {
                        Quality = 75
                    };

                    image.Save(path, encoder);

                    return newFileName;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}