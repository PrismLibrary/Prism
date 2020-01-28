namespace HelloWorld.UITests.Pages
{
    public class ViewB : BasePage
    {
        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = x => x.Marked("TitleLabel"),
            iOS = x => x.Marked("TitleLabel")
        };
    }
}
