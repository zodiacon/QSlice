using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using QSlice.ViewModels;

namespace QSlice {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public App() {
            LoadAssemblies();
        }

        readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();



        private void LoadAssemblies() {
            var appAssembly = typeof(App).Assembly;
            foreach(var resourceName in appAssembly.GetManifestResourceNames()) {
                if(resourceName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)) {
                    using(var stream = appAssembly.GetManifestResourceStream(resourceName)) {
                        var assemblyData = new byte[(int)stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        var assembly = Assembly.Load(assemblyData);
                        _assemblies.Add(assembly.GetName().Name, assembly);
                    }
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }



        Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
            var shortName = new AssemblyName(args.Name).Name;
            Assembly assembly;
            if(_assemblies.TryGetValue(shortName, out assembly)) {
                return assembly;
            }
            return null;
        }

        MainViewModel _mainViewModel;

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var win = new MainWindow();
            var vm = new MainViewModel();
            win.DataContext = _mainViewModel = vm;
            win.Show();
        }

        protected override void OnExit(ExitEventArgs e) {
            _mainViewModel.Close();

            base.OnExit(e);
        }
    }
}
