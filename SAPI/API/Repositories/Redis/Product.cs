namespace API.Repositories.Redis
{
    public class Product : Base<DTO.Databases.Product>
    {
        public Product(Databases.Redis client) : base(client, "Product")
        {
        }
    }
}