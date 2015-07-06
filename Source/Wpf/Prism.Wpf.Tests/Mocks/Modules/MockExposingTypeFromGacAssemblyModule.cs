

using System;
using Prism.Modularity;

namespace Prism.Wpf.Tests.Mocks.Modules
{
    public class MockExposingTypeFromGacAssemblyModule : IModule
    {
        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }

    public class SomeContractReferencingTransactionsAssembly : System.Transactions.IDtcTransaction
    {
        public void Commit(int retaining, int commitType, int reserved)
        {
            throw new System.NotImplementedException();
        }

        public void Abort(IntPtr reason, int retaining, int async)
        {
            throw new System.NotImplementedException();
        }

        public void GetTransactionInfo(IntPtr transactionInformation)
        {
            throw new System.NotImplementedException();
        }
    }
}
