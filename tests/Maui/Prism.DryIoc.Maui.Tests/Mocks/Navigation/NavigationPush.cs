namespace Prism.DryIoc.Maui.Tests.Mocks.Navigation;

public record NavigationPush(Page CurrentPage, Page Page, bool? UseModalNavigation, bool? Animated, bool InsertBeforeLast, int NavigationOffset);
