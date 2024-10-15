namespace SportsStore.Models
{
    public interface IStoreRepository
    {
        // IQueryable is ervied form the IEnumerable interface and allows for stop/start LINQ querying
        // where we can obtain some data from the underlying db in one statment and then query again to
        // further restrict the resultSet.
        IQueryable<Product> Products { get; }
    }
}