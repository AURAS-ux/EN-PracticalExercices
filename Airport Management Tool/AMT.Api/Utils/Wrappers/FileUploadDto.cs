using System;
using Microsoft.AspNetCore.Http;

namespace AMT.Api.Utils.Wrappers;

public class FileUploadDto
{
    public required IFormFile File { get; set; }
}
