using Microsoft.AspNetCore.Http;
using DemoSatva.Model;
using DemoSatva.Utility;
using System.Collections.Generic;

namespace DemoSatva.Manager.Interface
{
    public interface IUploadManager
    {
        APIResponse UploadImages(List<IFormFile> images);
    }
}

