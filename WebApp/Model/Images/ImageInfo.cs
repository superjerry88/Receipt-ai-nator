using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Components.Forms;
using MongoDB.Bson.Serialization.Attributes;
using WebApp.Services;

namespace WebApp.Model.Images;

#pragma warning disable CA1416
public class ImageInfo
{
    [BsonId]
    public string Id { get; set; }
    public string LocalFilePath { get; set; }
    public string UrlPath { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public string FileExtension { get; set; }

    public bool ContainsLocation { get; set; } = false;
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public bool IsImage { get; set; } = false;
    public bool IsProcessing { get; set; } = false;
    public bool IsDigitized { get; set; } = false;
    public bool ContainsReceipt { get; set; } = false;

    public string UserId { get; set; }

    public ImageInfo()
    {
        
    }

    public ImageInfo(IBrowserFile imageFile)
    {
        Id = DateTime.Now.Ticks + "_" + Guid.NewGuid().ToString("N")[..6];
        var fileExtension = Path.GetExtension(imageFile.Name);
        var fileNameCleanse = Id + fileExtension;
        LocalFilePath = Path.Combine(RezApi.Files.UnsafeFolderPath, fileNameCleanse);
        UrlPath = RezApi.Files.UnsafeFolderUrl(fileNameCleanse);
        FileExtension = Path.GetExtension(LocalFilePath);
    }

    public async Task<ImageInfoStatus> CopyToLocalPath(IBrowserFile imageFile)
    {
        try
        {
            await using FileStream fs = new(LocalFilePath, FileMode.Create);
            await imageFile.OpenReadStream(RezApi.Settings.MaxImageSize).CopyToAsync(fs);
            fs.Close();
            return ImageInfoStatus.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ImageInfoStatus.Error(e.Message);
        }
    }

    public ImageInfoStatus ProcessImage()
    {
        try
        {
            using var image = Image.FromFile(LocalFilePath);
            Height = image.Height;
            Width = image.Width;
            TryExtractGps();
            IsImage = true;
            IsProcessing = true;
            return ImageInfoStatus.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ImageInfoStatus.Error(e.Message);
        }

    }

    private void TryExtractGps()
    {
        using var image = Image.FromFile(LocalFilePath);
        var latitude = GetGpsProperty(image, 2);
        var longitude = GetGpsProperty(image, 4);

        if (latitude != null && longitude != null)
        {
            ContainsLocation = true;
            Latitude = ConvertToDecimalDegrees(latitude);
            Longitude = ConvertToDecimalDegrees(longitude);
        }
        else
        {
            ContainsLocation = false;
        }
    }

    private static byte[]? GetGpsProperty(Image image, int propertyId)
    {
        var propItem = image.PropertyItems.FirstOrDefault(item => item.Id == propertyId);
        return propItem?.Value;
    }

    private static double ConvertToDecimalDegrees(byte[]? coordinates)
    {
        if (coordinates == null) return 0;

        double degrees = BitConverter.ToInt32(coordinates, 0);
        double minutes = BitConverter.ToInt32(coordinates, 4);
        double seconds = BitConverter.ToInt32(coordinates, 8);

        return degrees + minutes / 60 + seconds / 3600;
    }
}
#pragma warning restore CA1416