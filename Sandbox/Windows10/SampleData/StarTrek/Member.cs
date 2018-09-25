using Newtonsoft.Json;

namespace SampleData.StarTrek
{
    public partial class Member
    {
        [JsonProperty("show")]
        public string Show { get; set; }

        [JsonProperty("actor")]
        public string Actor { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("character")]
        public string Character { get; set; }

        [JsonProperty("images")]
        public Image[] Images { get; set; }

        [JsonProperty("species")]
        public string Species { get; set; }

        public Image Image { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this);
        public static bool TryFromJson(string json, out Member member)
        {
            try
            {
                member = JsonConvert.DeserializeObject<Member>(json);
                return true;
            }
            catch (System.Exception)
            {
                member = default(Member);
                return false;
            }
        }

        public override string ToString()
        {
            return $"{Character} by {Actor}";
        }
    }
}