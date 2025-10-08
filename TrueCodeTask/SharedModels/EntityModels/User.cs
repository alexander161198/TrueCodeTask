namespace SharedModels.EntityModels
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public ICollection<UserCurrency> Currencies { get; set; } = new List<UserCurrency>();
    }
}
