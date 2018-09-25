using System;

namespace SampleData.Email
{
    public class Message: BindableBase
    {
        private Recipient _from = default(Recipient);
        public Recipient From { get => _from; set => Set(ref _from, value); }

        private Recipient _to = default(Recipient);
        public Recipient To { get => _to; set => Set(ref _to, value); }

        private DateTime _date = default(DateTime);
        public DateTime Date { get => _date; set => Set(ref _date, value); }

        private string _subject = default(string);
        public string Subject { get => _subject; set => Set(ref _subject, value); }

        private string _body = default(string);
        public string Body { get => _body; set => Set(ref _body, value); }
    }
}
