using Newtonsoft.Json;

namespace SampleData.StarTrek
{
    public partial class Show 
    {
        
        [JsonProperty("ordinal")]
        public string Ordinal { get; set; }

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }

        public Image Image { get; set; }

        public override string ToString()
        {
            return $"{Ordinal} {Abbreviation} {Name}";
        }
    }
}