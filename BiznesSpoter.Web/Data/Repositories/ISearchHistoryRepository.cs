using BiznesSpoter.Web.Data.Entities;

namespace BiznesSpoter.Web.Data.Repositories;

public interface ISearchHistoryRepository
{
    Task AddAsync(SearchHistory searchHistory);
    Task<IEnumerable<SearchHistory>> GetByUserIdAsync(string userId);
}