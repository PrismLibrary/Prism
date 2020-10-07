using System;
using System.Threading.Tasks;
using Xunit;

namespace Prism.Core.Tests.Extensions
{
    public class TaskExtensionsFixture
    {
        [Fact]
        public void TaskIsCompleted()
        {
            var sus = new SystemUnderTest();

            Assert.False(sus.TaskCompleted);
            sus.RunATask().Await(() =>
            {
                Assert.True(sus.TaskCompleted);
            });
        }

        [Fact]
        public void TaskIsCompleted_WithResult()
        {
            var sus = new SystemUnderTest();

            Assert.False(sus.TaskCompleted);
            sus.RunATaskWithResult().Await((result) =>
            {
                Assert.True(sus.TaskCompleted);
                Assert.Equal(SystemUnderTest.Result, result);
            });
        }

        [Fact]
        public void TaskHandlesException()
        {
            var sus = new SystemUnderTest();

            Assert.False(sus.TaskCompleted);
            sus.TaskThrowsException().Await((ex) =>
            {
                Assert.NotNull(ex);
                Assert.IsType<Exception>(ex);
                Assert.False(sus.TaskCompleted);
            });
        }

        [Fact]
        public void ExceptionInCompletedCallback_TriggersErrorCallback()
        {
            var sus = new SystemUnderTest();

            sus.RunATask().Await(() =>
            {
                throw new Exception();
            },
            (ex) =>
            {
                Assert.NotNull(ex);
                Assert.IsType<Exception>(ex);
            });
        }

        [Fact]
        public void ExceptionInCompletedCallback_WithResult_TriggersErrorCallback()
        {
            var sus = new SystemUnderTest();

            sus.RunATaskWithResult().Await((result) =>
            {
                throw new Exception();
            },
            (ex) =>
            {
                Assert.NotNull(ex);
                Assert.IsType<Exception>(ex);
            });
        }

        class SystemUnderTest
        {
            public const string Result = "RESULT";

            public bool TaskCompleted { get; set; } = false;

            public async Task RunATask()
            {
                await Task.Delay(500);
                TaskCompleted = true;
            }

            public async Task<string> RunATaskWithResult()
            {
                await Task.Delay(500);
                TaskCompleted = true;
                return Result;
            }

            public async Task TaskThrowsException()
            {
                await Task.Delay(500);
                throw new System.Exception();
            }
        }
    }
}
