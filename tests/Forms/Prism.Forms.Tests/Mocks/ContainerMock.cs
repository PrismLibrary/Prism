using System;
using Moq;
using Prism.Forms.Tests.Mocks.Views;
using Prism.Forms.Tests.Navigation.Mocks.Views;
using Prism.Ioc;

namespace Prism.Forms.Tests.Mocks
{
    public static class ContainerMock
    {
        public static Mock<IContainerExtension> CreateMock()
        {
            var mock = new Mock<IContainerExtension>();
            mock.Setup(x => x.Register(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(mock.Object);
            mock.Setup(x => x.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(mock.Object);
            mock.Setup(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(mock.Object);
            mock.Setup(x => x.RegisterSingleton(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()))
                .Returns(mock.Object);

            mock.Setup(x => x.Resolve(typeof(object), "PageMock"))
                .Returns(new PageMock());

            mock.Setup(x => x.Resolve(typeof(object), "ContentPage"))
                .Returns(new ContentPageMock());
            mock.Setup(x => x.Resolve(typeof(object), "ContentPage1"))
                .Returns(new ContentPageMock1());
            mock.Setup(x => x.Resolve(typeof(object), "typeof(ContentPageMockViewModel).FullName"))
                .Returns(new ContentPageMock());
            mock.Setup(x => x.Resolve(typeof(object), "typeof(ContentPageMock1ViewModel).FullName"))
                .Returns(new ContentPageMock1());

            mock.Setup(x => x.Resolve(typeof(object), "SecondContentPageMock"))
                .Returns(new SecondContentPageMock());

            mock.Setup(x => x.Resolve(typeof(object), "NavigationPage"))
                .Returns(new NavigationPageMock());
            mock.Setup(x => x.Resolve(typeof(object), "NavigationPage-Empty"))
                .Returns(new NavigationPageEmptyMock());
            mock.Setup(x => x.Resolve(typeof(object), "NavigationPage-Empty-Reused"))
                .Returns(new NavigationPageEmptyMock_Reused());
            mock.Setup(x => x.Resolve(typeof(object), "NavigationPageWithStack"))
                .Returns(new NavigationPageWithStackMock());
            mock.Setup(x => x.Resolve(typeof(object), "NavigationPageWithStackNoMatch"))
                .Returns(new NavigationPageWithStackNoMatchMock());

            mock.Setup(x => x.Resolve(typeof(object), "MasterDetailPage"))
                .Returns(new MasterDetailPageMock());
            mock.Setup(x => x.Resolve(typeof(object), "MasterDetailPage-Empty"))
                .Returns(new MasterDetailPageEmptyMock());

            mock.Setup(x => x.Resolve(typeof(object), "TabbedPage"))
                .Returns(new TabbedPageMock());
            mock.Setup(x => x.Resolve(typeof(object), "TabbedPage-Empty"))
                .Returns(new TabbedPageEmptyMock());
            mock.Setup(x => x.Resolve(typeof(object), "Tab1"))
                .Returns(new Tab1Mock());
            mock.Setup(x => x.Resolve(typeof(object), "Tab2"))
                .Returns(new Tab2Mock());
            mock.Setup(x => x.Resolve(typeof(object), "Tab3"))
                .Returns(new Tab3Mock());

            mock.Setup(x => x.Resolve(typeof(object), "CarouselPage"))
                .Returns(new CarouselPageMock());
            return mock;
        }
    }
}
