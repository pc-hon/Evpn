using System.IO;

namespace Evpn.Test.Utils
{
    public static class TestDataReader
    {
        public static string ReadLocations()
        {
            return Read(@"testlocations.xml");
        }

        public static string ReadLocations2()
        {
            return Read(@"testlocations2.xml");
        }

        private static string Read(string file)
        {
            using (var reader = new StreamReader(file))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
