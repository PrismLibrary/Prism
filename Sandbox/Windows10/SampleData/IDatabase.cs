using System.Threading.Tasks;

namespace SampleData.StarTrek
{
    public interface IDatabase
    {
        string[] Genders { get; }
        Member[] Members { get; }
        bool Open { get; }
        Ship[] Ships { get; }
        Show[] Shows { get; }
        string[] Species { get; }

        Task<bool> OpenAsync();
    }
}