using Bogus;
using Newtonsoft.Json;
using SampleData.StarTrek;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace SampleData.Email
{
    public class Database
    {
        public Message[] Messages { get; private set; }

        public bool Open { get; private set; } = false;

        public Task<bool> OpenAsync()
        {
            if (Open)
            {
                return new Task<bool>(() => true);
            }

            Randomizer.Seed = new Random(8675309);
            Messages = GenerateMessages(100).ToArray();
            return new Task<bool>(() => Open = true);

            IEnumerable<Message> GenerateMessages(int count)
            {
                var to = GenerateRecipient();
                var messages = new Faker<Message>()
                    .RuleFor(x => x.From, f => GenerateRecipient())
                    .RuleFor(x => x.To, f => to)
                    .RuleFor(x => x.Subject, f => f.Lorem.Sentence())
                    .RuleFor(x => x.Date, f => f.Date.Recent())
                    .RuleFor(x => x.Body, f => f.Lorem.Paragraphs(2, 5));
                return messages.Generate(count);
            }

            Recipient GenerateRecipient()
            {
                var recipient = new Faker<Recipient>()
                    .RuleFor(x => x.Name, f => f.Name.FullName())
                    .RuleFor(x => x.Email, f => f.Internet.Email())
                    .RuleFor(x => x.Image, f => f.Image.People());
                return recipient.Generate();
            }
        }
    }
}
