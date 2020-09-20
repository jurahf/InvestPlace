using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InvestPlace.Scheduler
{
    /// <summary>
    /// Создает Scope и запускает задачи
    /// </summary>
    public class ScopedShceduleServiceStarter : BackgroundService, IDisposable
    {
        private readonly ILogger<ScopedShceduleServiceStarter> logger;
        private Timer timer;

        public ScopedShceduleServiceStarter(
            IServiceProvider services,
            ILogger<ScopedShceduleServiceStarter> logger)
        {
            Services = services;
            this.logger = logger;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ScopedShceduleServiceStarter running.");

            await ScheduleWork(stoppingToken);
        }


        public Task ScheduleWork(CancellationToken stoppingToken)
        {
            logger.LogInformation("UnusedBasketHostedService running.");

            timer = new Timer(DoWork, null, TimeSpan.FromMinutes(1), TimeSpan.FromHours(6));

            return Task.CompletedTask;
        }


        private void DoWork(object state)
        {
            logger.LogInformation("ScopedShceduleServiceStarter is working.");

            using (var scope = Services.CreateScope())
            {
                var unusedBasketHostedService = scope.ServiceProvider.GetRequiredService<IUnusedBasketHostedService>();
                unusedBasketHostedService.DoWork();

                var overfloweredLotsHostedService = scope.ServiceProvider.GetRequiredService<IOverfloweredLotsHostedService>();
                overfloweredLotsHostedService.DoWork();
            }
        }


        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("ScopedShceduleServiceStarter is stopping.");

            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            timer?.Dispose();
            base.Dispose();
        }
    }

}
