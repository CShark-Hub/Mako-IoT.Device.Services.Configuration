# Mako-IoT.Device.Services.Configuration
Provides easy way of reading and updating configuration settings with strongly-typed objects. Settings are stored in persistent storage via [_IStorageService_](https://github.com/CShark-Hub/Mako-IoT.Device.Services.FileStorage).

## Usage
Create class for your settings. You may use multiple classes, one for every component. Provide section name as static/const string.
```c#
public class MyAppConfig
{
    public string ApiUrl { get; set; }
    public string SslCertificate { get; set; }
    public string Timezone { get; set; }
    public static string SectionName => "MyAppConfig";
}
```

Use _IConfigurationService_ to read and/or update settings.
```c#
public class MyAppService : IMyAppService
{
    private readonly MyAppConfig _config;

    public MyAppService(IConfigurationService configService)
    {
        _config = (MyAppService)configService.GetConfigSection(MyAppService.SectionName, typeof(MyAppService));
    }
//[...]
}
```

Add _Configuration_ and [_FileStorage_](https://github.com/CShark-Hub/Mako-IoT.Device.Services.FileStorage) components in your device builder. You can specify default settings, which will be written on startup.
```c#
public class Program
{
    public static void Main()
    {
        DeviceBuilder.Create()
          .AddConfiguration(c =>
          {
              c.WriteDefault(MyAppConfig.SectionName, new MyAppConfig
              {
                  ApiUrl = "http://my-app.my-company.com/api"
              });
          })
          .AddFileStorage()
          .Build()
          .Start();

        Thread.Sleep(Timeout.InfiniteTimeSpan);
    }
}
```
Note: setting _overwrite = true_ in the _WriteDefault_ will overwrite settings with the defaults every time (even if settings already exist).
