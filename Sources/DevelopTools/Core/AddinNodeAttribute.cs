using System;

namespace DevelopTools.Core
{
    internal class AddinNodeAttribute : Attribute
    {
        public bool Serialize { get; }

        public AddinNodeAttribute(bool serialize)
        {
            Serialize = serialize;
        }
    }
}
