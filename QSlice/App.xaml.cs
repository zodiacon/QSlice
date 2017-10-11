using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using QSlice.ViewModels;
using Syncfusion.Windows.Shared;
using System.Windows.Media;
using Syncfusion.SfSkinManager;

namespace QSlice {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        MainViewModel _mainViewModel;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var win = new MainWindow();
            var vm = new MainViewModel();
            win.DataContext = _mainViewModel = vm;
            win.Show();

            SfSkinManager.ApplyStylesOnApplication = true;
            SfSkinManager.SetVisualStyle(win, VisualStyles.Metro);
        }

        protected override void OnExit(ExitEventArgs e) {
            _mainViewModel.Close();

            base.OnExit(e);
        }
    }
}
