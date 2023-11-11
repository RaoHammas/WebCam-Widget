namespace WebCam_Widget.Models;

public class CamDevice
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }

    public List<CamDeviceDetail> CamDeviceDetails { get; set; }
}

public class CamDeviceDetail
{
    //props.Add($"[Info][{c.Description}]-[PF][{c.PixelFormat}]-[FPS][{c.FramesPerSecond}]-[H/W][{c.Height}/{c.Width}]");

    public string Description { get; set; }
    public string PixelFormat { get; set; }
    public string RawPixFormat { get; set; }
    public string FramesPerSecond { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
}