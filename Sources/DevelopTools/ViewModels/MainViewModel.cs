using Autodesk.RevitAddIns;
using DevelopTools.Core;
using DevelopTools.Infrastructure;
using DevelopTools.Models;
using DevelopTools.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DevelopTools.ViewModels
{
    internal class MainViewModel : BindableBase
    {

        private IList<AddinData> _addinList;
        private AddinData _lookupAddin;
        private Regex _regex;
        public MainViewModel()
        {
            var allRevits = RevitProductUtility.GetAllInstalledRevitProducts();
            Products = allRevits.Select(r => new ProductData(r)).ToArray();
        }


        #region Bindings
        public IList<ProductData> Products { get; set; }

        public bool IsLookupChecked { get; set; }

        public bool IsManagerChecked { get; set; }

        public bool IsCurrentUser { get; set; }

        private double _progressValue;
        public double ProgressValue
        {
            get { return _progressValue; }
            set { SetProperty(ref _progressValue, value); }
        }

        private string _installStatus;
        public string InstallStatus
        {
            get { return _installStatus; }
            set { SetProperty(ref _installStatus, value); }
        }

        private bool _canInstall = true;
        public bool CanInstall
        {
            get { return _canInstall; }
            set { SetProperty(ref _canInstall, value); }
        }

        #endregion

        #region Commands
        private DelegateCommand _setupCommand;
        public DelegateCommand SetupCommand =>
            _setupCommand ?? (_setupCommand = new DelegateCommand(ExecuteSetupCommand, CanExecuteSetupCommand));
        async void ExecuteSetupCommand()
        {
            ProgressValue = 0;
            try
            {
                if (_regex is null)
                {
                    _regex = new Regex(@"[0-9]+");
                }
                CanInstall = false;
                #region AddinManager
                var type = typeof(AddinData);
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                ProgressValue = 20;
                var checkedVersion = Products.Where(p => p.IsSelected).ToArray();
                var total = (IsManagerChecked ? checkedVersion.Length : 0) + (IsLookupChecked ? checkedVersion.Length : 0);
                var per = 80d / total;
                if (IsManagerChecked)
                {
                    var title = "AddInManager";
                    if (_addinList is null)
                    {
                        _addinList = new AddinData[]
                        {
                    new AddinData()
                    {
                        AssemblyName=$"{title}.dll",
                        Type=AddinType.Command,
                        FullClassName="AddInManager.CAddInManager",
                        Text="Add-In Manager (Manual Mode)",
                    },
                    new AddinData()
                    {
                        AssemblyName=$"{title}.dll",
                        Type=AddinType.Command,
                        FullClassName="AddInManager.CAddInManagerFaceless",
                        Text="Add-In Manager (Manual Mode, Faceless)",
                    },
                    new AddinData()
                    {
                        AssemblyName=$"{title}.dll",
                        Type=AddinType.Command,
                        FullClassName="AddInManager.CAddInManagerReadOnly",
                        Text="Add-In Manager (ReadOnly Mode)",
                    },
                        };
                    }
                    foreach (var product in checkedVersion)
                    {
                        await InstallAddins(properties, per, title, product, _addinList, false);
                    }
                }
                #endregion

                #region RevitLookup
                if (IsLookupChecked)
                {
                    var title = "RevitLookup";
                    if (_lookupAddin is null)
                    {
                        _lookupAddin = new AddinData()
                        {
                            AssemblyName = $"{title}.dll",
                            Type = AddinType.Application,
                            FullClassName = "RevitLookup.App",
                            Text = "Revit Lookup"
                        };
                    }
                    foreach (var product in checkedVersion)
                    {
                        await InstallAddins(properties, per, title, product, new AddinData[] { _lookupAddin }, true);
                    }
                }
                InstallStatus = $"{DateTime.Now} 全部安装完成！";
                #endregion
            }
            catch (Exception e)
            {
                InstallStatus = e.Message;
            }
            finally
            {
                CanInstall = true;
            }
        }

        private async Task InstallAddins(PropertyInfo[] properties, double per, string title, ProductData product, IList<AddinData> addins, bool isLookup)
        {
            var directory = IsCurrentUser ? product.RevitProduct.CurrentUserAddInFolder : product.RevitProduct.AllUsersAddInFolder;
            var xmlFile = Path.Combine(directory, $"{title}.addin");
            var xmlDoc = CreateAddinFile(properties, product, addins);
            xmlDoc.Save(xmlFile);
            var dllFile = Path.Combine(directory, $"{title}.dll");
            var buffer = GetFileBuffer(product, isLookup);
            File.WriteAllBytes(dllFile, buffer);
            ProgressValue += per;
            InstallStatus = $"{DateTime.Now} 正在安装 {title} {product.RevitProduct.Name}";
            await Task.Delay(1000);
        }


        private XmlDocument CreateAddinFile(IList<PropertyInfo> properties, ProductData product, IList<AddinData> addins)
        {
            var xmlDoc = new XmlDocument();
            var xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDoc.AppendChild(xmlDeclaration);
            var root = xmlDoc.CreateElement("RevitAddIns");

            foreach (var addin in addins)
            {
                var addinDirectory = IsCurrentUser ? product.RevitProduct.CurrentUserAddInFolder : product.RevitProduct.AllUsersAddInFolder;
                addin.Assembly = Path.Combine(addinDirectory, addin.AssemblyName);
                var addinElement = xmlDoc.CreateElement("AddIn");
                addinElement.SetAttribute("Type", addin.Type.ToString());
                foreach (var property in properties)
                {
                    if (property.GetCustomAttribute<AddinNodeAttribute>()?.Serialize ?? false)
                    {
                        var childElement = xmlDoc.CreateElement(property.Name);
                        var value = property.GetValue(addin)?.ToString();
                        childElement.InnerText = value;
                        addinElement.AppendChild(childElement);
                    }
                }
                root.AppendChild(addinElement);
            }
            xmlDoc.AppendChild(root);
            return xmlDoc;
        }

        bool CanExecuteSetupCommand()
        {
            return Products.Any(p => p.IsSelected) && (IsLookupChecked || IsManagerChecked);
        }

        private byte[] GetFileBuffer(ProductData product, bool isLookup)
        {
            var primaryVersion = _regex.Match(product.RevitProduct.Name)?.Value;
            if (string.IsNullOrEmpty(primaryVersion))
            {
                return default;
            }
            byte[] buffer = null;
            switch (primaryVersion)
            {
                case "2016":
                    buffer = isLookup ? Resources.RevitLookup2016 : Resources.AddInManager2016;
                    break;
                case "2017":
                    buffer = isLookup ? Resources.RevitLookup2017 : Resources.AddInManager2017;
                    break;
                case "2018":
                    buffer = isLookup ? Resources.RevitLookup2018 : Resources.AddInManager2018;
                    break;
                case "2019":
                    buffer = isLookup ? Resources.RevitLookup2019 : Resources.AddInManager2019;
                    break;
                case "2020":
                    buffer = isLookup ? Resources.RevitLookup2020 : Resources.AddInManager2020;
                    break;
                case "2021":
                    buffer = isLookup ? Resources.RevitLookup2021 : Resources.AddInManager2021;
                    break;
            }
            return buffer;
        }

        #endregion
    }
}
