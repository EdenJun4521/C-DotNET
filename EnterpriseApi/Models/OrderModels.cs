using System.ComponentModel.DataAnnotations;

namespace EnterpriseApi.Models
{
    /// <summary>
    /// 訂單請求模型
    /// </summary>
    public class OrderRequest
    {
        [Required(ErrorMessage = "客戶名稱為必填")]
        public string CustomerName { get; set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "金額必須在 1 到 100,000 之間")]
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// 訂單資料庫實體 (EF Core Entity)
    /// </summary>
    public class OrderEntity
    {
        [Key]
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// 訂單回應 Record (唯讀)
    /// </summary>
    public record OrderResponse(int OrderId, string Customer, decimal Amount, DateTime OrderTime, string Status);
}
