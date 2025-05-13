using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

public static class ImageHelper
{
    private static readonly string ImageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploaded");

    static ImageHelper()
    {
        // Ensure the directory exists
        if (!Directory.Exists(ImageDirectory))
        {
            Directory.CreateDirectory(ImageDirectory);
        }
    }

    public static async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided.");

        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        string filePath = Path.Combine(ImageDirectory, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    public static async Task<string> UpdateImageAsync(IFormFile newFile, string existingFileName)
    {
        if (newFile == null || newFile.Length == 0)
            throw new ArgumentException("No file provided.");

        DeleteImage(existingFileName);
        return await UploadImageAsync(newFile);
    }

    public static void DeleteImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return;

        string filePath = Path.Combine(ImageDirectory, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
