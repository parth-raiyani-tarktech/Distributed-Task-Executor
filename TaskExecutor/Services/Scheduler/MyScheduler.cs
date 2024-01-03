using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using Quartz.Impl.AdoJobStore.Common;
using TaskExecutor.Models;
using TaskExecutor.Services.Job;
using SystemTask = System.Threading.Tasks.Task;

namespace TaskExecutor.Services.Scheduler
{
    public class MyScheduler : IMyScheduler
    {
        private static IScheduler _scheduler;
        public MyScheduler(IJobFactory myJobFactory)
        {
            _scheduler = new StdSchedulerFactory().GetScheduler().Result;
            _scheduler.JobFactory = myJobFactory;
        }

        public async SystemTask Start(Node node)
        {
            try
            {
                await _scheduler.Start();
                var job = CreateJob<NodeHealthCheckerJob>(node);
                var trigger = TriggerBuilder.Create()
                    .WithIdentity(node.Id.ToString())
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error scheduling job: " + ex.ToString());
            }
        }

        private IJobDetail CreateJob<T>(Node node) where T : IJob
        {
            return JobBuilder.Create<T>()
                .WithIdentity(node.Id.ToString())
                .UsingJobData("name", node.Name)
                .Build();
        }

        public async SystemTask StopAsync(Node node)
        {
            await _scheduler.DeleteJob(new JobKey(node.Id.ToString()));
            var triggerKey = new TriggerKey(node.Id.ToString());
            await _scheduler.UnscheduleJob(triggerKey);
        }
    }
}
