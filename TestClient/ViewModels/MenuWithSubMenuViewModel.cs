using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestClient.Views;

namespace TestClient.ViewModels
{
    class MenuWithSubMenuViewModel
    {
        public List<MenuItemsData> MenuList
        {
            get
            {
                return new List<MenuItemsData>
                {
                    new MenuItemsData()
                    {
                        MenuText = "Tests",
                        SubMenuList = new List<SubMenuItemsData>
                        {
                            new SubMenuItemsData() { SubMenuText = "Show My Tests" },
                            new SubMenuItemsData() { SubMenuText = "Pass Test" },
                        }
                    }
                };
            }
        }

        public class MenuItemsData
        {
            public string MenuText { get; set; }
            public List<SubMenuItemsData> SubMenuList { get; set; }
            public MenuItemsData() { }
        }

        public class SubMenuItemsData
        {
            public string SubMenuText { get; set; }
            public SubMenuItemsData()
            {
                SubMenuCommand = new RelayCommand(Execute);
            }

            public ICommand SubMenuCommand { get; }

            private void Execute()
            {
                string SMT = SubMenuText.Replace(" ", string.Empty);

                if (!string.IsNullOrEmpty(SMT))
                {
                    navigateToPage(SMT);
                }
            }

            private void navigateToPage(string Menu)
            {
                foreach (Window window in System.Windows.Application.Current.Windows)
                {
                    if (window.GetType() == typeof(ClientTestWindow))
                    {
                        (window as ClientTestWindow).MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "Pages/", Menu, ".xaml"), UriKind.RelativeOrAbsolute));
                    }
                }
            }
        }
    }
}
