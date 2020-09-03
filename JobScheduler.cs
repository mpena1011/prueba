using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descripción breve de JobScheduler
/// </summary>
public class JobScheduler
{  
    /// <summary>
    /// Constructor de clase JobScheduler
    /// </summary>
    public JobScheduler()
    {      
    }
    /// <summary>
    /// Metodo Start para definir el Job y el periodo de ejecucion.
    /// </summary>
    public static void Start()
    {

        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        scheduler.Start();
        IJobDetail job = JobBuilder.Create<EmailJob>().Build();
        ITrigger trigger = TriggerBuilder.Create().WithCronSchedule("0 0/3 * 1/1 * ? *").Build();
        ///Cada 3 minutos 0 0/3 * 1/1 * ? * 
        ///Cada hora  0 0 0/1 1/1 * ? *     
        scheduler.ScheduleJob(job, trigger);
    }
}