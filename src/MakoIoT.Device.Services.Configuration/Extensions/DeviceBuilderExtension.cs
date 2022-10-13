using MakoIoT.Device.Services.DependencyInjection;
using MakoIoT.Device.Services.Interface;

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
            DI.RegisterSingleton(typeof(IConfigurationService), typeof(ConfigurationService));
            builder.ConfigureDefaultsAction = configureDefaultsAction;
            return builder;
        }
    }
}
