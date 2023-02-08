using System.Xml.Serialization;

[XmlRoot("AppInfo")]
public class AppInfo
{
    [XmlElement("ProductName")]
    public string ProductName { get; set; }

    [XmlElement("CompanyName")]
    public string CompanyName { get; set; }

    [XmlElement("Version")]
    public string Version { get; set; }
}
