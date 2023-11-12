using MakoIoT.Device.Services.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace MakoIoT.Device.Services.Configuration.Extensions
{
    public static class DeviceBuilderExtension
    {
        public static IDeviceBuilder AddConfiguration(this IDeviceBuilder builder)
        {
            return AddConfiguration(builder, null);
        }

        public static IDeviceBuilder AddConfiguration(this IDeviceBuilder builder, ConfigureDefaultsDelegate configureDefaultsAction)
        {
            builder.Services.AddSingleton(typeof(IConfigurationService), typeof(ConfigurationService));
            builder.ConfigureDefaultsAction = configureDefaultsAction;
            return builder;
        }
    }
}
