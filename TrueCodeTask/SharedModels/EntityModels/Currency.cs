
namespace SharedModels.EntityModels
{
    public class Currency
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }

        public ICollection<UserCurrency> Users { get; set; } = new List<UserCurrency>();
        //public ICollection<User> Users { get; set; }
    }
}
