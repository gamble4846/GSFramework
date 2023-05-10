using Microsoft.AspNetCore.Http;
using DemoSatva.DataAccess.Interface;
using DemoSatva.Manager.Interface;
using DemoSatva.Model;
using DemoSatva.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace DemoSatva.Manager.Impl
{
    public class UploadManager : IUploadManager
    {
        public APIResponse UploadImages(List<IFormFile> images)
        {
            Dictionary<string,string> uploadedFilePath = new Dictionary<string,string>();
            if (images.Count > 0)
            {
                foreach (var img in images)
                {
                    try
                    {
                        if (UtilityCommon.IsImage(img))
                        {
                            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                            var dir = Path.Combine(Directory.GetCurrentDirectory(), "uploadimage");
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                            var uploads = Path.Combine(dir, uniqueFileName);
                            using (var stream = new FileStream(uploads, FileMode.OpenOrCreate))
                            {
                                img.CopyTo(stream);
                                uploadedFilePath.Add(img.FileName, uniqueFileName);
                            }
                        }
                        else
                        {
                            uploadedFilePath.Add(img.FileName, "Error : Not a valid image file");
                        }
                    }
                    catch (Exception ex)
                    {
                        uploadedFilePath.Add(img.FileName, "Error : " + ex.Message);
                    }
                
                }
                return new APIResponse(ResponseCode.SUCCESS, "Image Uploaded Created", uploadedFilePath);
            }
            else
            {
                return new APIResponse(ResponseCode.ERROR, "No image found");
            }
        }

    }
}

