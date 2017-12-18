namespace DTO.Messages
{
    public class SigningInUser
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool AdminMode { get; set; }

        public SigningInUser()
        {
            Email = "NONE";
            Password = "DEFAULT";
            AdminMode = false;
        }
    }
}