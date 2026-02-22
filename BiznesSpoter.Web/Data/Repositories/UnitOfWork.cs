namespace BiznesSpoter.Web.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public ISearchHistoryRepository SearchHistories { get; }

    public UnitOfWork(ApplicationDbContext context, ISearchHistoryRepository searchHistories)
    {
        _context = context;
        SearchHistories = searchHistories;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

