namespace Prism.Services.Dialogs
{
    public class DialogParameterAttribute : Navigation.NavigationParameterAttribute
    {
        public DialogParameterAttribute(string name) : base(name)
        {
        }

        public DialogParameterAttribute(bool isRequired) : base(isRequired)
        {
        }

        public DialogParameterAttribute(string name, bool isRequired) : base(name, isRequired)
        {
        }
    }
}
