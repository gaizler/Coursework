using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TestDesigner.Views;

namespace TestDesigner.ViewModels
{
    public class StartWindowViewModel
    {
        public RelayCommand<Window> CreateTestCommand { get; }
        public RelayCommand<Window> EditTestCommand { get; }
        public StartWindowViewModel()
        {
            CreateTestCommand = new RelayCommand<Window>(CreateTest);
            EditTestCommand = new RelayCommand<Window>(EditTest);
        }
        private void CreateTest(Window window)
        {
            CreateTestWindow createWindow = new CreateTestWindow();
            createWindow.Show();
            if (window != null)
                window.Close();
        }
        private void EditTest(Window window)
        {
            EditTestWindow editTestWindow = new EditTestWindow();
            editTestWindow.Show();
            if (window != null)
                window.Close();
        }

    }
}
