using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using NeedBodies.Auth;

namespace NeedBodies;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthenticationCore();
        services.AddAuthorizationCore();
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddScoped<ProtectedSessionStorage>();
        services.AddScoped<AuthenticationStateProvider, UserAuthenticationStateProvider>();
        services.AddSingleton<UserService>();

        AddBlazorise(services);

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        // this is required to be here or otherwise the messages between server and client will be too large and
        // the connection will be lost.
        //app.UseSignalR( route => route.MapHub<ComponentHub>( ComponentHub.DefaultPath, o =>
        //{
        //    o.ApplicationMaxBufferSize = 1024 * 1024 * 100; // larger size
        //    o.TransportMaxBufferSize = 1024 * 1024 * 100; // larger size
        //} ) );

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }

    public void AddBlazorise(IServiceCollection services)
    {
        services
            .AddBlazorise();
        services
            .AddBootstrap5Providers()
    .AddFontAwesomeIcons();
    }
}
