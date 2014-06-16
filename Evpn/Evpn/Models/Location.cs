using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Evpn.Models
{
    [Serializable]
    public class Location
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("sort_order")]
        public int SortOrder { get; set; }

        [XmlAttribute("icon_id")]
        public int IconId { get; set; }

        [XmlIgnore]
        public Icon Icon { get; set; }

        [XmlElement("server")]
        public List<Ip> ServerIps { get; set; }

        public long PingServers()
        {
            // Returns the fastest ping response
            return ServerIps.Select(ip => ip.Ping()).Min();
        }
    }
}
