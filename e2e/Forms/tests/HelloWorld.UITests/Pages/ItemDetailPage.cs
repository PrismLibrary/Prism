using System;
using System.Linq;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace HelloWorld.UITests.Pages
{
    public class ItemDetailPage : BasePage
    {
        Query pageIdQuery = x => x.Marked("ItemDetailPage");
        Query itemIdLabelQuery = x => x.Marked("ItemIdLabel");

        protected override PlatformQuery Trait => new PlatformQuery
        {
            Android = pageIdQuery,
            iOS = pageIdQuery
        };

        public int GetItemIdLabelValue()
        {
            app.WaitForElement(itemIdLabelQuery);

            var result = app.Query(itemIdLabelQuery).Single();
            return Convert.ToInt32(result.Text);
        }
    }
}
