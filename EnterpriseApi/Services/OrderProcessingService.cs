using System.Threading.Channels;
using EnterpriseApi.Data;
using EnterpriseApi.Models;

namespace EnterpriseApi.Services
{
    /// <summary>
    /// 非同步訂單處理服務 (消費者)
    /// </summary>
    public class OrderProcessingService : BackgroundService
    {
        private readonly Channel<OrderRequest> _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderProcessingService> _logger;

        public OrderProcessingService(
            Channel<OrderRequest> channel,
            IServiceProvider serviceProvider,
            ILogger<OrderProcessingService> logger)
        {
            _channel = channel;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("訂單處理服務已啟動...");

            // 使用 await foreach 監聽 Channel 中的資料
            await foreach (var request in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation($"正在處理來自 {request.CustomerName} 的訂單...");

                    // 模擬處理延遲
                    await Task.Delay(2000, stoppingToken);

                    // 使用 CreateScope 取得限時服務 (如 DbContext)
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var entity = new OrderEntity
                        {
                            CustomerName = request.CustomerName,
                            Amount = request.Amount,
                            CreatedAt = DateTime.Now,
                            Status = "交易成功"
                        };

                        await dbContext.Orders.AddAsync(entity, stoppingToken);
                        await dbContext.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation($"訂單已存入資料庫，ID: {entity.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "訂單處理過程中發生錯誤");
                }
            }
        }
    }
}
