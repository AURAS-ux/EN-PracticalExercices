using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AMT.Api.Utils.Wrappers;

public class FileUploadDto
{
    [Required]
    public required IFormFile File { get; set; }
}
