namespace FastFood.WebApi.Models
{
    public class UserLoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserRegisterViewModel
    {
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImageBase64 { get; set; }
        public string Password { get; set; }
    }
}