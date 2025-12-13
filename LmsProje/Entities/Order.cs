namespace LmsProje.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; } 
        public AppUser User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; } 

        public string OrderStatus { get; set; } = "Success"; 

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}