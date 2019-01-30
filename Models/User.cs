namespace boilerplate.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        
    }
}