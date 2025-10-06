using System.IO;
using System.Xml.Serialization;

namespace SiriFeedService
{
    public static class ConfigLoader
    {
        public static ServiceConfiguration? Load(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                using var stream = File.OpenRead(path);
                var serializer = new XmlSerializer(typeof(ServiceConfiguration));
                return serializer.Deserialize(stream) as ServiceConfiguration;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load config: {ex.Message}");
                return null;
            }
        }
    }
}
