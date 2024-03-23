using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageUploader.Services.Abstract
{
    public interface IImageSaver
    {
        Task SaveImageAsync(string fileName, string path);
        Task<string> SaveImageAsync(Stream fileStream, string fileName, string container);
    }
}
