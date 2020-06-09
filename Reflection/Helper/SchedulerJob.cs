using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reflection.Helper
{
    public class SchedulerJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            //Implement the lgoc to read timing from db
            throw new NotImplementedException();
        }
    }
}
