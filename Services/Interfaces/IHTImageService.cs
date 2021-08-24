using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonuTasks.Services.Interfaces
{
    public interface IHTImageService
    {
        Task<byte[]> EncodeFileAsync(string fileName);
        string ContentType(IFormFile file);
        int Size(IFormFile file);

        bool ValidateFileType(IFormFile file);
        bool ValidateFileType(IFormFile file, List<string> filetypes);

        bool ValidateFileSize(IFormFile file);
        bool ValidateFileSize(IFormFile file, int maxSize);


        Task<byte[]> EncodeFileAsync(IFormFile File);

        //returning the image/displaying it on the view
        string DecodeImage(byte[] data, string type);
    }
}
