namespace API.Configs
{
    public class Redis
    {
        public string ConnectString { get; set; }

        public int Database { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int TTL { get; set; }

        public Redis()
        {
            ConnectString = "";
            Database = 0;
            Username = "";
            Password = "";
            TTL = 300;
        }
    }
}