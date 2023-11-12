namespace WebCam_Widget.Models;

public class CamDevice
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }

    public List<CamDeviceDetail> CamDeviceDetails { get; set; }
}