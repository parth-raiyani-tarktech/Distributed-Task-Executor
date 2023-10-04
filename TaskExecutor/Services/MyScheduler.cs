using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using Quartz.Impl.AdoJobStore.Common;
using TaskExecutor.Models;

namespace TaskExecutor.Services
{
    public class MyScheduler : IMyScheduler
    {
        private static IScheduler _scheduler;
        public MyScheduler(IJobFactory myJobFactory)
        {
            _scheduler = new StdSchedulerFactory().GetScheduler().Result;
            _scheduler.JobFactory = myJobFactory;
        }

        public async System.Threading.Tasks.Task Start(Node node)
        {
            try
            {
                await _scheduler.Start();
                var job = CreateJob<NodeHealthCheckerJob>(node);
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("NodeHealthCheckerJob-" + Guid.NewGuid(), "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);
            } catch(Exception ex)
            {
                Console.WriteLine("Error scheduling job: " + ex.ToString());
            }
        }

        private IJobDetail CreateJob<T>(Node node) where T : IJob
        {
            return JobBuilder.Create<T>()
                .WithIdentity(typeof(T).Name + "-" +Guid.NewGuid(), "group1")
                .UsingJobData("address", node.Address)
                .UsingJobData("name", node.Name)
                .Build();
        }
    }
}
