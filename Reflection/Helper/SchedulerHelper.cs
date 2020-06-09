using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Helper
{
    public class SchedulerHelper
    {
        private readonly IScheduler _scheduler;
        public SchedulerHelper(IScheduler factory)
        {
            _scheduler = factory;
        }

        public async Task RunRecurringReflect()
        {
            ITrigger trigger = TriggerBuilder.Create()
                                             .WithIdentity($"Check and Run.(DateTime.Now)") //need to change this
                                             .StartNow()
                                             .WithPriority(1)
                                             .Build();
            IJobDetail job = JobBuilder.Create<SchedulerJob>()
                .WithIdentity("Check and Run")
                .Build();

            await _scheduler.ScheduleJob(job, trigger);
                
        }
    }
}
