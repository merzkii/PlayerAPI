namespace PlayerApi.Models
{
    public class User
    {
     public string Id { get; set; }
     public string UserName { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; } 
     public string Email { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; } 
        public int Gender { get; set; } 
        public string Currency { get; set; } 
     public decimal Balance { get; set; }

    }
}
