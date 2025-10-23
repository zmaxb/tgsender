namespace tgsenderd.Models;

public record Response(string Message = "")
{
    public bool Success { get; set; } = string.IsNullOrEmpty(Message);
    public string Message { get; set; } = Message;
}