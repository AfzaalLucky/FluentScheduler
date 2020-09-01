﻿namespace FluentScheduler.TestApplication
{
    using Serilog;
    using System;
    using static Serilog.Log;
    using static System.Threading.Thread;

    public static class Program
    {
        static void Main(string[] args)
        {
            InitializeLogger();

            var schedules = new[] {
                Welcome(),

                NonReentrant(),
                Faulty(),

                FiveMinutes(),
                TenMinutes(),
                Hour(),
                Day(),
                Weekday(),
                Week(),

                Sunday(),
                Monday(),
                Tuesday(),
                Thursday(),
                Friday(),
                Saturday(),
            };

            schedules.ListenJobStarted(JobStartedHandler);
            schedules.ListenJobEnded(JobEndedHandler);

            schedules.Start();
            Sleep(-1);
        }


        static Schedule Welcome() => new Schedule(
            () => Logger.Information("3, 2, 1, live!"),
            run => run.Now()
        );

        static Schedule NonReentrant() => new Schedule(() =>
            {
                Logger.Information("NonReentrant: sleeping a minute");
                Sleep(TimeSpan.FromMinutes(1));
            },
            run => run.Every(1).Seconds()
        );

        static void InitializeLogger()
        {
            var outputTemplate = "[{Timestamp:HH:mm:ss}] {Message}{NewLine}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: outputTemplate)
                .WriteTo.File("logs/.txt", outputTemplate: outputTemplate, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        static Schedule Faulty() => new Schedule(
            () => { throw new Exception("Exception from Faulty job"); },
            run => run.Now().AndEvery(20).Minutes()
        );

        static Schedule FiveMinutes() => new Schedule(
            () => Logger.Information("FiveMinutes: five minutes has passed"),
            run => run.OnceAt(DateTime.Now.AddMinutes(5)).AndEvery(5).Minutes()
        );

        static Schedule TenMinutes() => new Schedule(
            () => Logger.Information("TenMinutes: ten minutes has passed"),
            run => run.Every(10).Minutes()
        );

        static Schedule Hour() => new Schedule(
            () => Logger.Information("Hour: a hour has passed"),
            run => run.Every(1).Hours()
        );

        static Schedule Day() => new Schedule(
            () => Logger.Information("Day: a day has passed"),
            run => run.Every(1).Days()
        );

        static Schedule Weekday() => new Schedule(
            () => Logger.Information("Weekday: a new weekday has started"),
            run => run.EveryWeekday()
        );

        static Schedule Sunday() => new Schedule(
            () => Logger.Information("Sunday: a new sunday has started "),
            run => run.Every(DayOfWeek.Sunday)
        );

        static Schedule Monday() => new Schedule(
            () => Logger.Information("Monday: a new monday has started "),
            run => run.Every(DayOfWeek.Monday)
        );

        static Schedule Tuesday() => new Schedule(
            () => Logger.Information("Tuesday: a new tuesday has started "),
            run => run.Every(DayOfWeek.Tuesday)
        );

        static Schedule Thursday() => new Schedule(
            () => Logger.Information("Thursday: a new thursday has started "),
            run => run.Every(DayOfWeek.Thursday)
        );

        static Schedule Friday() => new Schedule(
            () => Logger.Information("Friday: a new friday has started "),
            run => run.Every(DayOfWeek.Friday)
        );

        static Schedule Saturday() => new Schedule(
            () => Logger.Information("Saturday: a new saturday has started "),
            run => run.Every(DayOfWeek.Saturday)
        );

        static Schedule Week() => new Schedule(
            () => Logger.Information("Week: a new week has started"),
            run => run.Every(1).Weeks()
        );

        static void JobStartedHandler(object sender, JobStartedEventArgs ea) =>
            Logger.Information("JobStarted");

        static void JobEndedHandler(object sender, JobEndedEventArgs ea) =>
            Logger.Information(ea.Exception is null ? $"JobEnded" : $"JobEnded: {ea.Exception}");
    }
}
