// public class SolutionDistributionBackgroundService : BackgroundService
// {
//     private readonly IServiceProvider _services;
//     private readonly ILogger<SolutionDistributionBackgroundService> _logger;

//     public SolutionDistributionBackgroundService(
//         IServiceProvider services,
//         ILogger<SolutionDistributionBackgroundService> logger)
//     {
//         _services = services;
//         _logger = logger;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         _logger.LogInformation("Solution Distribution Service running.");

//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 using (var scope = _services.CreateScope())
//                 {
//                     var distributionService = 
//                         scope.ServiceProvider.GetRequiredService<SolutionDistributionService>();
                    
//                     await distributionService.DistributeSolutionsAfterDeadline();
//                 }
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error occurred executing solution distribution.");
//             }

//             await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); 
//         }
//     }
// }