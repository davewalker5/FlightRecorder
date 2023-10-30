using FlightRecorder.Api.Entities;
using FlightRecorder.BusinessLogic.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FlightRecorder.BusinessLogic.Logic
{
    [ExcludeFromCodeCoverage]
    public abstract class BackgroundQueueProcessor<T> : BackgroundService where T : BackgroundWorkItem
    {
        private const int ProcessingLoopDelay = 500;

        private readonly IBackgroundQueue<T> _queue;

        protected IServiceScopeFactory ServiceScopeFactory { get; private set; }
        protected ILogger MessageLogger { get; private set; }

        protected BackgroundQueueProcessor(
            ILogger<BackgroundQueueProcessor<T>> logger,
            IBackgroundQueue<T> queue,
            IServiceScopeFactory serviceScopeFactory)
        {
            _queue = queue;
            ServiceScopeFactory = serviceScopeFactory;
            MessageLogger = logger;
        }

        /// <summary>
        /// Method called to process the work placed into the queue
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken token)
        {
            MessageLogger.LogInformation($"BackgroundQueueProcessor<{typeof(T).Name}> starting");

            while (!token.IsCancellationRequested)
            {
                // Wait, so there's not a busy loop, then get the next work item from the queue
                await Task.Delay(ProcessingLoopDelay, token);
                var item = _queue.Dequeue();

                // Item may be null if there's nothing in the queue or there's a de-queuing error, so
                // check it's valid
                if (item != null)
                {
                    MessageLogger.LogInformation($"Dequeued work item {item.ToString()}");

                    // Create a dependency resolution scope
                    using (var scope = ServiceScopeFactory.CreateScope())
                    {
                        // Get a scoped instance of the flight recorder factory
                        var factory = scope.ServiceProvider.GetService<FlightRecorderFactory>();

                        // Create the job status record
                        var status = await factory.JobStatuses.AddAsync(item.JobName, item.ToString());

                        try
                        {
                            // Process the work item and, if successful, complete the job status record with
                            // no error
                            MessageLogger.LogInformation($"Processing work item {item.ToString()}");
                            await ProcessWorkItemAsync(item, factory);
                            await factory.JobStatuses.UpdateAsync(status.Id, null);
                            MessageLogger.LogInformation($"Finished processing work item {item.ToString()}");
                        }
                        catch (Exception ex)
                        {
                            // Got an error during processing, so complete the job status record with the
                            // exception details
                            await factory.JobStatuses.UpdateAsync(status.Id, ex.ToString());
                            MessageLogger.LogInformation($"Error processing work item {item.ToString()} : {ex.Message}");
                        }
                    }
                }
            }

            MessageLogger.LogInformation($"BackgroundQueueProcessor<{typeof(T).Name}> exiting");
        }

        /// <summary>
        /// Process the specified work item from the queue
        /// </summary>
        /// <param name="item"></param>
#pragma warning disable CS1998
        protected virtual async Task ProcessWorkItemAsync(T item, FlightRecorderFactory factory)
        {
            // Ideally, this would be an abstract method with no body but that's not possible with
            // async methods so it's declared with an empty body in the expectation that the child
            // classes will override it
        }
#pragma warning restore CS1998
    }
}
