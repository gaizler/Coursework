using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestServer.Views;
using static System.Net.Mime.MediaTypeNames;

namespace TestServer.ViewModels
{
    public class MenuWithSubmenuViewModel
    {
        public List<MenuItemsData> MenuList
        {
            get
            {
                return new List<MenuItemsData>
                {
                    new MenuItemsData()
                    {
                        MenuText = "Group",
                        SubMenuList = new List<SubMenuItemsData>
                        {
                            new SubMenuItemsData() { SubMenuText = "Show Groups" },
                            new SubMenuItemsData() { SubMenuText = "Add Group" },
                            new SubMenuItemsData() { SubMenuText = "Edit Group" },
                            new SubMenuItemsData() { SubMenuText = "Delete Group" },
                            new SubMenuItemsData() { SubMenuText = "Add Users To Group" },
                            new SubMenuItemsData() { SubMenuText = "Show Users By Group" }


                        }
                    },
                    new MenuItemsData()
                    {
                        MenuText = "User",
                        SubMenuList = new List<SubMenuItemsData>
                        {
                            new SubMenuItemsData() { SubMenuText = "Show Users" },
                            new SubMenuItemsData() { SubMenuText = "Add User" },
                            new SubMenuItemsData() { SubMenuText = "Edit User" },
                            new SubMenuItemsData() { SubMenuText = "Delete User" }
                        }
                    },
                    new MenuItemsData()
                    {
                        MenuText = "Test",
                        SubMenuList = new List<SubMenuItemsData>
                        {
                            new SubMenuItemsData() { SubMenuText = "Load Test" },
                            new SubMenuItemsData() { SubMenuText = "Show Tests" },
                            new SubMenuItemsData() { SubMenuText = "Assign Test" },
                            new SubMenuItemsData() { SubMenuText = "Show Tests For Group" }
                        }
                    },
                    new MenuItemsData()
                    {
                        MenuText = "Results",
                        SubMenuList = new List<SubMenuItemsData>
                        {
                            new SubMenuItemsData() { SubMenuText = "Show Results" }
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
                    if (window.GetType() == typeof(ServerTestWindow))
                    {
                        (window as ServerTestWindow).MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "Pages/", Menu, ".xaml"), UriKind.RelativeOrAbsolute));
                    }
                }
            }
        }
    }
}
