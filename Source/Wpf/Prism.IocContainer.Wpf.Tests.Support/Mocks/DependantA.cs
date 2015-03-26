// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.


namespace Prism.IocContainer.Tests.Support.Mocks
{
    public class DependantA : IDependantA
    {
        public DependantA(IDependantB dependantB)
        {
            MyDependantB = dependantB;
        }

        public IDependantB MyDependantB { get; set; }
    }

    public interface IDependantA
    {
        IDependantB MyDependantB { get; }
    }
}