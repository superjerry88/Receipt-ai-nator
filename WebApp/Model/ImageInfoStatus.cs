namespace WebApp.Model;

public class ImageInfoStatus
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = "";

    public static ImageInfoStatus Success(string msg = "")
    {
        return new ImageInfoStatus { IsSuccess = true, Message = msg };
    }
    public static ImageInfoStatus Error(string msg = "")
    {
        return new ImageInfoStatus { IsSuccess = false, Message = msg };
    }
}