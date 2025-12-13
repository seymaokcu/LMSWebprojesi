namespace LmsProje.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    }
}
