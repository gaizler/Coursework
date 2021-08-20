using DALTest;
using GalaSoft.MvvmLight.CommandWpf;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestServer.Views;
using PostSharp.Patterns.Model;

namespace TestServer.ViewModels
{
    [NotifyPropertyChanged]
    public class LoginViewModel
    {
        #region Variables
        public string Password { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;

        public RelayCommand<Window> OpenServer { get; }
        public RelayCommand<Window> Cancel { get; }
        #endregion Variables

        public LoginViewModel()
        {
            OpenServer = new RelayCommand<Window>(OpenServerWindow, CanOpenServerExecute, false);
            Cancel = new RelayCommand<Window>(CloseWindow);
        }

        async void OpenServerWindow(Window window)
        {
            User currentUser =await getUserByLogin();

            if (currentUser!=null&&currentUser.Password == Password)
            {
                if (!currentUser.IsAdmin)
                {
                    MessageBox.Show("Only admin has a permission to open a server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ServerTestWindow serverTestWindow = new ServerTestWindow();
                serverTestWindow.Show();
                CloseWindow(window);
            }
            else
                MessageBox.Show("Wrong login or password","Error",MessageBoxButton.OK,MessageBoxImage.Error);
        }

        bool CanOpenServerExecute(Window window)
        {
            return Password.Length != 0 && Login.Length != 0;
        }

        void CloseWindow(Window window)
        {
            if (window != null)
                window.Close();
        }

        Task<User> getUserByLogin()
        {
            var res = Task.Run(() =>
            {
                GenericUnitOfWork work = new GenericUnitOfWork(new TestContext(ConfigurationManager.ConnectionStrings["conStr"].
                ConnectionString));
                var repo = work.Repository<User>();

                return repo.FindAll(x => x.Login == Login).FirstOrDefault();
            });
            return res;
        }
    }
}
