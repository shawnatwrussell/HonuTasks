using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace HonuTasks.Services
{
    public class HTImageService
    {
        private const int DefaultMaxFileSize = 1024;

        public string ContentType(IFormFile file)
        {

            return file?.ContentType.Split('/')[1];
        }

        public string DecodeImage(byte[] data, string type)

        {
            if (data is null || type is null) return null;
            return $"data:{type};base64,{Convert.ToBase64String(data)}";
        }

        public async Task<byte[]> EncodeFileAsync(IFormFile File)
        {
            if (File is null) return null;

            using var ms = new MemoryStream();
            await File.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeFileAsync(string fileName)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/img/{fileName}";
            return await File.ReadAllBytesAsync(file);
        }

        public async Task<byte[]> EncodeAndReduceFileAsync(IFormFile file)
        {
            if (file is null) return null;

            var image = SixLabors.ImageSharp.Image.Load(file.OpenReadStream(), out IImageFormat format);
            image.Mutate(X => X.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Min,
                Size = new Size(DefaultMaxFileSize)
            }));

            using (var memoryStream = new MemoryStream())
            {
                var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(format);
                await image.SaveAsync(memoryStream, imageEncoder);
                return memoryStream.ToArray();
            }
        }

        public int Size(IFormFile file)
        {
            return Convert.ToInt32(file?.Length);
        }

        public bool ValidateFileSize(IFormFile file)
        {
            return Size(file) < DefaultMaxFileSize;
        }

        public bool ValidateFileSize(IFormFile file, int maxSize)
        {
            return Size(file) < maxSize;
        }

        public bool ValidateFileType(IFormFile file)
        {
            var authorizedTypes = new List<string> { "jpg", "jpeg", "png", "gif" };
            var validExt = authorizedTypes.Contains(ContentType(file));
            return validExt;
        }

        public bool ValidateFileType(IFormFile file, List<string> filetypes)
        {
            var validExt = filetypes.Contains(ContentType(file));
            return validExt;
        }
    }
}
