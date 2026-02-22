using BiznesSpoter.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiznesSpoter.Web.Data.Repositories;

public class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public SearchHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SearchHistory searchHistory)
    {
        await _context.SearchHistories.AddAsync(searchHistory);
    }

    public async Task<IEnumerable<SearchHistory>> GetByUserIdAsync(string userId)
    {
        return await _context.SearchHistories
            .Where(sh => sh.UserId == userId)
            .OrderByDescending(sh => sh.SearchDate)
            .ToListAsync();
    }
}

