namespace WebApp.Services;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddSiteBinding(this IConfigurationBuilder builder)
    {

        var configuration = builder.Build();
        configuration.GetSection("SiteSetting").Bind(RezApi.Settings);
        RezApi.Setup();
        return builder;
    }
}