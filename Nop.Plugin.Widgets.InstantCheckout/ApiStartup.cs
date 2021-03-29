namespace Nop.Plugin.Widgets.InstantCheckout
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Nop.Core.Infrastructure;
    using Nop.Web.Framework.Infrastructure;

    public class ApiStartup : INopStartup
    {
        public void Configure(IApplicationBuilder app)
        {
            //need to enable rewind so we can read the request body multiple times (this should eventually be refactored, but both JsonModelBinder and all of the DTO validators need to read this stream)
            app.Use(async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            return;
        }

        public int Order => new AuthenticationStartup().Order + 1;
    }
}
