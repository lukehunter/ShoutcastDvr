using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;

using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using log4net.Config;

namespace ShoutcastDvr
{
    public class ShoutcastDvrBootstrapper : BootstrapperBase
    {
        private CompositionContainer mContainer;
        private static log4net.ILog mLog = log4net.LogManager.GetLogger(typeof(ShoutcastDvrBootstrapper));

        public ShoutcastDvrBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            mContainer = new CompositionContainer(
                new AggregateCatalog(
                    AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()
                    )
                );

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new Controller.AppWindowManager());
            batch.AddExportedValue(EvtAgg.Current);
            batch.AddExportedValue(mContainer);

            mContainer.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = mContainer.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return mContainer.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            mContainer.SatisfyImportsOnce(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            if (Properties.Settings.Default.SaveFolder == "")
            {
                Properties.Settings.Default.SaveFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Properties.Settings.Default.Save();
            }

            // log4net configuration
            XmlConfigurator.Configure();

            mLog.Info("App started...");

            Dictionary<string, object> settings = new Dictionary<string, object>();
            settings.Add("MinHeight", 480);
            settings.Add("MinWidth", 640);
            DisplayRootViewFor<IShell>(settings);
        }
    }
}
