namespace XamarinStore.Core.Business
{
    public class ProductSize
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return Description;
        }

        public override bool Equals(object obj)
        {
            var item = obj as ProductSize;
            if (item == null)
                return false;
            return item.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
