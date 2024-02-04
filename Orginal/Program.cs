using Orginal.Components;
using Dependent;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Orginal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddCircuitOptions(options => options.DetailedErrors = true);

            builder.Services.AddDependentServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapWhen(context => context.Request.Path.StartsWithSegments("/Depend") ||
                                   context.Request.Path.StartsWithSegments("/Account"),
                        depend =>
                        {
                            depend.UseRouting();
                            depend.UseHttpsRedirection();

                            depend.UseStaticFiles();
                            depend.UseAntiforgery();

                            depend.UseAuthentication();
                            depend.UseAuthorization();

                            depend.UseEndpoints(endpoints =>
                            {
                                endpoints.MapRazorComponents<Dependent.Components.App>()
                                .AddInteractiveServerRenderMode();
                            });
                        });

            app.MapWhen(context => context.Request.Path.StartsWithSegments("/seconddepend"),
                depend =>
                    {
                        depend.UseRouting();
                        depend.UseHttpsRedirection();

                        depend.UseStaticFiles();
                        depend.UseAntiforgery();

                        depend.UseAuthentication();
                        depend.UseAuthorization();

                        depend.UseEndpoints(endpoints =>
                        {
                            endpoints.MapRazorComponents<SecondDependent.Components.App>()
                            .AddInteractiveServerRenderMode();
                        });
                    });

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapDependents();

            app.Run();
        }
    }
}
