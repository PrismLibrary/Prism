using System;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using Xunit;

namespace Prism.Core.Tests.Events
{
    public class AsyncPubSubEventFixture
    {
        [Fact]
        public async Task PublishAsyncAwaitsAllSubscribers()
        {
            var pubSubEvent = new AsyncPubSubEvent<string>();
            var firstPublished = false;
            var secondPublished = false;

            pubSubEvent.Subscribe(async payload =>
            {
                await Task.Delay(10);
                firstPublished = payload == "testPayload";
            }, true);
            pubSubEvent.Subscribe(payload =>
            {
                secondPublished = payload == "testPayload";
                return Task.CompletedTask;
            }, true);

            await pubSubEvent.PublishAsync("testPayload");

            Assert.True(firstPublished);
            Assert.True(secondPublished);
        }

        [Fact]
        public async Task PublishAsyncHonorsFilters()
        {
            var pubSubEvent = new AsyncPubSubEvent<string>();
            var publishedCount = 0;

            pubSubEvent.Subscribe(payload =>
            {
                publishedCount++;
                return Task.CompletedTask;
            }, payload => payload == "match");

            await pubSubEvent.PublishAsync("skip");
            await pubSubEvent.PublishAsync("match");

            Assert.Equal(1, publishedCount);
        }

        [Fact]
        public async Task PublishAsyncPropagatesSubscriberExceptions()
        {
            var pubSubEvent = new AsyncPubSubEvent<string>();
            pubSubEvent.Subscribe(_ => throw new InvalidOperationException("subscriber failed"), true);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => pubSubEvent.PublishAsync("testPayload"));

            Assert.Equal("subscriber failed", ex.Message);
        }

        [Fact]
        public async Task PublishAsyncCancellationCancelsWaitingOnly()
        {
            var pubSubEvent = new AsyncPubSubEvent<string>();
            var subscriberStarted = new TaskCompletionSource<object>();
            var subscriberCanComplete = new TaskCompletionSource<object>();
            var subscriberCompleted = false;
            using var cancellationTokenSource = new CancellationTokenSource();

            pubSubEvent.Subscribe(async _ =>
            {
                subscriberStarted.SetResult(null);
                await subscriberCanComplete.Task;
                subscriberCompleted = true;
            }, true);

            var publishTask = pubSubEvent.PublishAsync("testPayload", cancellationTokenSource.Token);
            await subscriberStarted.Task;
            cancellationTokenSource.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() => publishTask);
            Assert.False(subscriberCompleted);

            subscriberCanComplete.SetResult(null);
            await Task.Delay(10);

            Assert.True(subscriberCompleted);
        }

        [Fact]
        public void CanUnsubscribeAsyncSubscriber()
        {
            var pubSubEvent = new AsyncPubSubEvent<string>();
            Func<string, Task> action = _ => Task.CompletedTask;

            pubSubEvent.Subscribe(action);
            Assert.True(pubSubEvent.Contains(action));

            pubSubEvent.Unsubscribe(action);

            Assert.False(pubSubEvent.Contains(action));
        }
    }
}
