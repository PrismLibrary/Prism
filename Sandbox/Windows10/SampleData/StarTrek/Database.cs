using Newtonsoft.Json;
using SampleData.StarTrek;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace SampleData.StarTrek
{
    public class Database
    {
        public Member[] Members { get; private set; }

        public string[] Species { get; private set; }

        public string[] Genders { get; private set; }

        public Ship[] Ships { get; private set; }

        public Show[] Shows { get; private set; }

        public bool Open { get; private set; } = false;

        public async Task<bool> OpenAsync()
        {
            if (Open)
            {
                return Open;
            }

            var root = await ReadJson();

            Species = root.Members.Select(x => x.Species).Distinct().OrderBy(x => x).ToArray();

            Genders = root.Members.Select(x => x.Gender).Distinct().OrderBy(x => x).ToArray();

            foreach (var ship in Ships = root.Ships)
            {
                ship.Image = UpdateImagePaths(ship.Show, ship.Images);
            }

            foreach (var member in Members = root.Members)
            {
                member.Image = UpdateImagePaths(member.Show, member.Images);
            }

            foreach (var show in Shows = root.Shows)
            {
                UpdateImagePaths(show.Abbreviation, show.Images);
                show.Image = Ships.First(x => x.Show == show.Abbreviation)?.Images?.First();
            }

            return Open = true;

            Image UpdateImagePaths(string show, params Image[] images)
            {
                if (images != null)
                {
                    foreach (var image in images)
                    {
                        image.Path = $"ms-appx:///SampleData/StarTrek/Images/{show}/{image.Path}";
                    }
                    return images.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }

            async Task<JsonRoot> ReadJson()
            {
                try
                {
                    var path = new Uri("ms-appx:///SampleData/StarTrek/Data.json");
                    var file = await StorageFile.GetFileFromApplicationUriAsync(path);
                    var json = await FileIO.ReadTextAsync(file);
                    return JsonConvert.DeserializeObject<JsonRoot>(json);
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                    throw;
                }
            }
        }
    }
}
