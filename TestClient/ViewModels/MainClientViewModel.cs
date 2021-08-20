using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient.ViewModels
{
    class MainClientViewModel
    {
        public Uri frameSource { get; set; }
        public MainClientViewModel()
        {
            frameSource = new Uri(string.Format("../Pages/DefaultPage.xaml"), UriKind.RelativeOrAbsolute);
        }
    }
}
