namespace XamarinStore.Core.Business
{
    public class ProductColor
    {
        public string Name { get; set; }

        public string[] ImageUrls { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
