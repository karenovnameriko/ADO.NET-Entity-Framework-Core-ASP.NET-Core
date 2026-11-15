namespace KT5_WebAPITemp;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<Product> Products { get; }
    public IRepository<Category> Categories { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new Repository<Product>(context);
        Categories = new Repository<Category>(context);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}