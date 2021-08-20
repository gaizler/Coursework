using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DALTest;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using PostSharp.Patterns.Model;
using Repository;
using TestClient.Views;

namespace TestClient.ViewModels
{
    [NotifyPropertyChanged]
    class LoginViewModel:ViewModelBase
    {
        #region Variables
        public string Password { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string IpAddress { get; set; } = "127.0.0.1";
        public string Port { get; set; } = "8888";

        public RelayCommand<Window> OpenClient { get; }
        public RelayCommand<Window> Cancel { get; }

        #endregion Variables

        public LoginViewModel()
        {
            OpenClient = new RelayCommand<Window>(OpenClientWindow, CanOpenClientExecute, false);
            Cancel = new RelayCommand<Window>(CloseWindow);
        }

        async void OpenClientWindow(Window window)
        {
            User currentUser = await getUserByLogin();

            if (currentUser != null && currentUser.Password == Password)
            {
                ClientTestWindow clientTestWindow = new ClientTestWindow();
                Application.Current.Resources.Add("curUser",currentUser);//таким чином передаю в наступне вікно дані про користувача
                clientTestWindow.Show();
                CloseWindow(window);
            }
            else
                MessageBox.Show("Wrong login or password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        bool CanOpenClientExecute(Window window)
        {
            int temp;
            return Password.Length != 0 && Login.Length != 0&&IpAddress.Length!=0&&Port.Length!=0&&Int32.TryParse(Port,out temp);
        }

        void CloseWindow(Window window)
        {
            if (window != null)
                window.Close();
        }

        private Task<User> getUserByLogin()
        {
            return Task.Run(() =>
            {
                TcpClient client = new TcpClient();
                try
                {
                    client.Connect(IpAddress, Int32.Parse(Port));
                    NetworkStream stream = client.GetStream();

                    byte[] data = Encoding.Unicode.GetBytes(Login);
                    stream.Write(data, 0, data.Length);//передача логіну

                    data = new byte[2048];

                    stream.Read(data, 0, data.Length);//отримання користувача або повідомлення про помилку


                    string errorString = Encoding.Unicode.GetString(data, 0, data.Length);
                    if (errorString != null && errorString == "error")//невірний логін чи пароль
                        throw new Exception();

                    using (var ms=new MemoryStream(data))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        User user = formatter.Deserialize(ms) as User;
                        return user;
                    }                        
                }
                catch
                {
                    return null;
                }
                finally
                {
                    client.Close();
                }

            });
        }
    }
}
