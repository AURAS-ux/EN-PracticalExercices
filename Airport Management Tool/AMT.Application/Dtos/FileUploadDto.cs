using System;
using Microsoft.AspNetCore.Http;

namespace AMT.Application.Dtos;

public class FileUploadDto
{
    public required IFormFile File { get; set; }
}
