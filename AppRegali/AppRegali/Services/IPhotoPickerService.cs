using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DependencyServiceDemos
{
    public interface IPhotoPickerService
    {
        Task<Stream> GetImageStreamAsync();
    }
}