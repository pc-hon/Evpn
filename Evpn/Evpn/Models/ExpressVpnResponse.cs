using Evpn.Models.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Evpn.Models
{
    [Serializable()]
    [XmlRoot("expressvpn")]
    public class ExpressVpnResponse : ICustomSerializable
    {
        [XmlArray("icons")]
        [XmlArrayItem("icon")]
        public List<Icon> Icons { get; set; }

        [XmlArray("locations")]
        [XmlArrayItem("location")]
        public List<Location> Locations { get; set; }

        [XmlElement("button_text")]
        public string RefreshText { get; set; }


        public void OnDeserialized()
        {
            if (Locations != null && Icons != null)
            {
                Locations.ForEach(loc => loc.Icon = GetIcon(loc.IconId));
            }
        }

        private Icon GetIcon(int id)
        {
            return Icons.FirstOrDefault(icon => icon.Id == id);
        }
    }
}
