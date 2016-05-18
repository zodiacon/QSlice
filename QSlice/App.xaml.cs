using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QSlice.ViewModels;

namespace QSlice
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			var win = new MainWindow();
			var vm = new MainViewModel();
			win.DataContext = vm;
			win.Show();
		}
	}
}
