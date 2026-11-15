namespace KT5_WebAPITemp;

public interface IUnitOfWork
{
    IRepository<Product> Products { get; }
    IRepository<Category> Categories { get; }
    Task SaveChangesAsync();
}