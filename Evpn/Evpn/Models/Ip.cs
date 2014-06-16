using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml.Serialization;

namespace Evpn.Models
{
    [Serializable()]
    public class Ip
    {
        [XmlAttribute("ip")]
        public string Address { get; set; }

        public virtual long Ping()
        {
            var ping = new Ping();
            long time = 0;
            IPAddress ipAddress;

            if (!IPAddress.TryParse(Address, out ipAddress))
                return long.MaxValue;

            for (int i = 0; i < 5; i++)
            {
                var reply = ping.Send(ipAddress);
                time += reply.RoundtripTime;
            }

            return time / 5 ;
        }
    }
}
