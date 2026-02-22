namespace BiznesSpoter.Web.Data.Repositories;

public interface IUnitOfWork : IDisposable
{
    ISearchHistoryRepository SearchHistories { get; }
    Task<int> SaveChangesAsync();
}