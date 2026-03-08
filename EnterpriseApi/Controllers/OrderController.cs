using System.Threading.Channels;
using EnterpriseApi.Data;
using EnterpriseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly Channel<OrderRequest> _channel;
        private readonly AppDbContext _dbContext;

        public OrderController(Channel<OrderRequest> channel, AppDbContext dbContext)
        {
            _channel = channel;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 建立訂單 (非同步生產者)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 直接寫入 Channel (生產者)
            await _channel.Writer.WriteAsync(request);

            // 回傳 202 Accepted 代表已接受請求但尚未處理完成
            return Accepted(new { Message = "訂單已排入佇列" });
        }

        /// <summary>
        /// 獲取所有訂單歷史紀錄
        /// </summary>
        /// <returns></returns>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrderHistory()
        {
            var history = await _dbContext.Orders
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new OrderResponse(
                    x.Id,
                    x.CustomerName,
                    x.Amount,
                    x.CreatedAt,
                    x.Status
                ))
                .ToListAsync();

            return Ok(history);
        }
    }
}
