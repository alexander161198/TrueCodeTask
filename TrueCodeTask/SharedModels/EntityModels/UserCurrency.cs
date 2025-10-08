namespace SharedModels.EntityModels
{
    public class UserCurrency
    {
        public int UserId {  get; set; }
        public User User { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

    }
}
