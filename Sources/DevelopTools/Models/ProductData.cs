using Autodesk.RevitAddIns;
using DevelopTools.Infrastructure;

namespace DevelopTools.Models
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
