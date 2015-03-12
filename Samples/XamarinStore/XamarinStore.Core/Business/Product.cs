using System;
using System.Linq;

namespace XamarinStore.Core.Business
{
    public class Product //: ICloneable
    {
        static Random _random = new Random();

        public double Price { get; set; }

        public ProductSize Size { get; set; }

        public ProductColor Color { get; set; }

        public ProductType ProductType { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string[] ImageUrls
        {
            get { return Colors == null ? new string[0] : Colors.SelectMany(x => x.ImageUrls).ToArray(); }
        }

        int _imageIndex = -1;
        public string ImageUrl
        {
            get
            {
                if (ImageUrls == null || ImageUrls.Length == 0)
                    return "";
                if (ImageUrls.Length == 1)
                    return ImageUrls[0];
                if (_imageIndex == -1)
                    _imageIndex = _random.Next(ImageUrls.Length);
                return ImageUrls[_imageIndex];
            }
        }

        public string ImageForSize(float width)
        {
            return ImageForSize(ImageUrl, width);
        }

        public static string ImageForSize(string url, float width)
        {
            return string.Format("{0}?width={1}", url, width);
        }

        public string PriceDescription
        {
            get { return Price < 0.01 ? "Free" : Price.ToString("C"); }
        }

        public ProductColor[] Colors { get; set; }

        public ProductSize[] Sizes { get; set; }

        //#region ICloneable implementation

        //public object Clone()
        //{
        //    return new Product
        //    {
        //        Price = Price,
        //        Size = Size,
        //        Name = Name,
        //        Color = Color,
        //        ProductType = ProductType,
        //        Colors = new ProductColor[]{
        //            Color,
        //        }
        //    };
        //}

        //#endregion
    }
}
