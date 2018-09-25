using Newtonsoft.Json;

namespace SampleData.StarTrek
{
    // represents the root of the json document read by the database

    public partial class JsonRoot
    {
        [JsonProperty("shows")]
        public Show[] Shows { get; set; }

        [JsonProperty("ships")]
        public Ship[] Ships { get; set; }

        [JsonProperty("members")]
        public Member[] Members { get; set; }

        public Image Image { get; set; }
    }
}
