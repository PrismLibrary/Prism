using System.Web;
using Prism.Common;
using Prism.Mvvm;

namespace Prism.Navigation.Builder;

internal class CreateTabBuilder : ICreateTabBuilder, IUriSegment, IRegistryAware
{
    private List<IUriSegment> _segments { get; }

    public CreateTabBuilder(IViewRegistry registry)
    {
        _segments = new List<IUriSegment>();
        Registry = registry;
    }

    public string Segment => BuildSegment();

    public IViewRegistry Registry { get; }

    public ICreateTabBuilder AddSegment(string segmentName, Action<ISegmentBuilder> configureSegment)
    {
        var builder = new SegmentBuilder(segmentName);
        configureSegment?.Invoke(builder);
        _segments.Add(builder);
        return this;
    }

    private string BuildSegment()
    {
        var uri = string.Join("/", _segments.Select(x => x.Segment));
        return HttpUtility.HtmlEncode(uri);
    }
}
