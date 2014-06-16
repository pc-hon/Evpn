using System.IO;
using System.Xml.Serialization;

namespace Evpn.Models.Serializer
{
    public interface ICustomSerializable
    {
        void OnDeserialized();
    }

    /// <summary>
    ///     Encapsulate a XmlSerializer to support OnDerializer
    /// </summary>
    public class CustomSerializer<T> where T : ICustomSerializable
    {
        XmlSerializer m_serializer;
        public CustomSerializer() 
        {
            m_serializer = new XmlSerializer(typeof(T)); 
        }

        public T Deserialize(Stream stream)
        {
            var response = (T)m_serializer.Deserialize(stream);
            response.OnDeserialized();
            return response;
        }
    }
}
