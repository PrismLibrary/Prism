// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Prism.Logging;

namespace Prism.Wpf.Tests.Mocks
{
    internal class MockLogger : ILoggerFacade
    {
        public string LastMessage;
        public Category LastMessageCategory;
        public void Log(string message, Category category, Priority priority)
        {
            LastMessage = message;
            LastMessageCategory = category;
        }
    }
}