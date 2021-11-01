using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CloudinaryNet.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class FileController : ControllerBase
    {

        private readonly Cloudinary _cloudinary;

        public FileController (Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        [HttpPost]
        public ActionResult UploadFile([FromForm] List<IFormFile> files, [FromForm] string rut)
        {
            try
            {
                ImageUploadResult result = null;

                if (files == null)
                {
                    return BadRequest();
                }

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(formFile.FileName, formFile.OpenReadStream()),
                            UseFilename = true,
                            UniqueFilename = false,
                            Overwrite = true,
                            PublicId = rut ?? formFile.FileName
                        };

                        result = _cloudinary.Upload(uploadParams);
                    }
                }

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("image/{img}")]
        public ActionResult GetImage(string img)
        {
            SearchResult result = _cloudinary.Search().Expression("public_id=" + img).Execute();

            if (result.TotalCount == 0)
            {
                return NotFound();
            }

            return Ok(_cloudinary.Api.UrlImgUp.Secure(true).Transform(new Transformation().Height(200).Width(200).Crop("fit")).BuildUrl(img + "." + result.Resources[0].Format));
        }
    }
}
