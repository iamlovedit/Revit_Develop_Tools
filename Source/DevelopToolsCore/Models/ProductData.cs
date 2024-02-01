using Autodesk.RevitAddIns;
using DevelopToolsCore.Infrastructure;

namespace DevelopToolsCore.Models
{
    internal class ProductData : BindableBase
    {
        public ProductData(RevitProduct revitProduct)
        {
            RevitProduct = revitProduct;
        }
        public RevitProduct RevitProduct { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
    }
}
