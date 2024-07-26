using fn_Review_Tracker.Contract;
using fn_Review_Tracker.Helper;
using fn_Review_Tracker.Repository;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Victra.Integrations;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.RegisterIntegrationServices();
        services.AddScoped<ServiceNowHelper>();
        services.AddScoped<ReviewTrackerHelper>();
        services.AddScoped<IEDWDataHelper, EDWDataHelper>();
        services.AddScoped<IDataRepository, DataRepository>();
    })
    .Build();

host.Run();
