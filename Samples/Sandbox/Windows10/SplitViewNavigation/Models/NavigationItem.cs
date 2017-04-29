using Prism.Mvvm;

namespace SplitViewNavigation.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigationItem : BindableBase
    {
        private string _key;
        private string _glyph;
        private string _text;

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }

        public string Glyph
        {
            get { return _glyph; }
            set { SetProperty(ref _glyph, value); }
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public NavigationItem()
        {

        }
    }
}
