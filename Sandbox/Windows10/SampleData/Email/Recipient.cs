namespace SampleData.Email
{
    public class Recipient : BindableBase
    {
        private string _name = default(string);
        public string Name { get => _name; set => Set(ref _name, value); }

        private string _email = default(string);
        public string Email { get => _email; set => Set(ref _email, value); }

        private string _image = default(string);
        public string Image { get => _image; set => Set(ref _image, value); }

        public override string ToString()
        {
            return $"{Name} <{Email}>";
        }
    }
}
