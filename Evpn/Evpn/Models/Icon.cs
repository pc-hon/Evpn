using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace Evpn.Models
{
    [Serializable()]
    public class Icon
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlText]
        public string Base64String 
        { 
            get
            {
                return m_base64String;
            }
            set
            {
                m_base64String = value;
                m_bitmap = DecodeBase64StringImage(m_base64String);
            }
        }

        private string m_base64String;
        private Bitmap m_bitmap;

        [XmlIgnore]
        public BitmapSource BitmapSource
        {
            get
            {
                try
                {
                    var ip = m_bitmap.GetHbitmap();
                    return Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }
                catch (Exception) { /* Just ignore invalid icons for now */ }

                return null;

            }
        }

        private Bitmap DecodeBase64StringImage(string base64String)
        {
            try
            {
                using (var memoryStream = new MemoryStream(Convert.FromBase64String(base64String)))
                {
                    memoryStream.Position = 0;
                    return (Bitmap)Bitmap.FromStream(memoryStream);
                }
            }
            catch (Exception) { /* Just ignore invalid icons for now */ }

            return null;
        }
    }
}
