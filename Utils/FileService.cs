namespace FoodApp.Utils;

public class FileService(IConfiguration config, IWebHostEnvironment env)
{
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        string uploadFolder = Path.Combine(env.ContentRootPath, config["UploadFolder"] ?? "Uploads/Images");
        // Tạo thư mục nếu chưa 
        if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
        
        // Create name file unique 
        string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        string filePath = Path.Combine(uploadFolder, fileName);
        
        // Save file folder
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/images/{fileName}";
    }

    public void DeleteImage(string imageUrl)
    {
        if(string.IsNullOrEmpty(imageUrl)) return;
        string filePath = Path.Combine(env.ContentRootPath, imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if(File.Exists(filePath)) File.Delete(filePath);
    }
}