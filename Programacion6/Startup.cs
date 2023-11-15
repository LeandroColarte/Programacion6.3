using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Programacion6
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuración de servicios (puedes agregar tus servicios aquí)
            // services.AddSomeService();

            // Configuración de autenticación y errores
            ConfigureAuthServices(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Configuración de middlewares y endpoints
            Configure(app);
        }

        private void ConfigureAuthServices(IServiceCollection services)
        {
            // Aquí colocas el código relacionado con la autenticación y el manejo de errores
        }

        private void Configure(IApplicationBuilder app)
        {
            // Aquí colocas el resto de tu configuración de middlewares y endpoints
        }
    }
}