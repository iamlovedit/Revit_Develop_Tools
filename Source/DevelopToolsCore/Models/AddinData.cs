namespace DevelopTools.Core
{
    internal class AddinData
    {

        [AddinNode(false)]
        public string AssemblyName { get; set; }

        [AddinNode(false)]
        public AddinType Type { get; set; }

        [AddinNode(true)]
        public string Assembly { get; set; }

        [AddinNode(true)]
        public Guid ClientId { get; } = Guid.NewGuid();

        [AddinNode(true)]
        public string FullClassName { get; set; }

        [AddinNode(true)]
        public string Text { get; set; }

        [AddinNode(true)]
        public string Name { get => Text; }

        [AddinNode(true)]
        public VisibilityMode VisibilityMode { get; set; }

        [AddinNode(true)]
        public LanguageType LanguageType { get; set; }

        [AddinNode(true)]
        public string VendorId { get; set; } = "ADSK";

        [AddinNode(true)]
        public string VendorDescription { get; set; } = "Autodesk, www.autodesk.com";

    }

    internal enum AddinType
    {
        Command,
        Application
    }

    internal enum VisibilityMode
    {
        AlwaysVisible
    }

    internal enum LanguageType
    {
        Unknown
    }
}
