using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireExemplo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Relaciona o serviço do hangfire
            services.AddHangfire(op =>
            {
                op.UseMemoryStorage();
            });
            services.AddHangfireServer();
            //

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Inicializacao do hangfire
            app.UseHangfireDashboard();
            //

            // Metodos hangfire            
            BackgroundJob.Enqueue(() => MeuPrimeiroJobFireAndForget());

            RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring Job"), Cron.Hourly);

            BackgroundJob.Schedule(() => Console.Write("Delayed Job"), TimeSpan.FromDays(2));

            string jobId = BackgroundJob.Enqueue(() => Console.Write("Tarefa Pai"));
            BackgroundJob.ContinueJobWith(jobId, () => Console.Write("Tarefa Filha"));
            //
        }

        public async Task MeuPrimeiroJobFireAndForget()
        {
            await Task.Run(() =>
            {
                //throw new Exception("Opa! Algo deu errado no Job...");
                Console.WriteLine("Bem vindo ao Hangfire.");
            });
        }
    }
}
