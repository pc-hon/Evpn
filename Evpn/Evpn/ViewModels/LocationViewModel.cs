using Evpn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Evpn.ViewModels
{
    public class LocationViewModel
    {
        protected Location Model;

        public LocationViewModel(Location locationModel)
        {
            Model = locationModel;
        }

        public string Name { get { return Model.Name; } }

        public BitmapSource ImageSource { get { return Model.Icon.BitmapSource; } }

        public int SortOrder { get { return Model.SortOrder; } }
    }
}
