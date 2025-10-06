using System.Xml.Serialization;
using System.Collections.Generic;

namespace SiriFeedService
{
    [XmlRoot("ServiceConfig")]
    public class ServiceConfiguration
    {
        [XmlElement("ConnectionString")]
        public string ConnectionString { get; set; } = string.Empty;

        [XmlElement("SiriEndpointUrl")]
        public string EndpointUrl { get; set; } = string.Empty;
        
        [XmlElement("RequestorRef")]
        public string RequestorRef { get; set; } = string.Empty;

        [XmlArray("StopCodes")]
        [XmlArrayItem("Stop")]
        public List<string> StopCodes { get; set; } = new();

        [XmlElement("OutputFolder")]
        public string OutputFolder { get; set; } = string.Empty;
    }
}
