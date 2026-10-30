using MyTown.Models;

namespace mytown.Controllers.Helpers
{
    public static class BusinessSearchExtensions
    {
        public static IQueryable<BusinessRegister> ApplyBusinessSearch(
            this IQueryable<BusinessRegister> query, string? search)
        {
            if (string.IsNullOrWhiteSpace(search)) return query;

            var words = search
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Take(5);

            foreach (var word in words)
            {
                var term = word;
                query = query.Where(b => b.BusinessName.Contains(term) ||
                                         b.Town.Contains(term) ||
                                         b.BusinessCity.Contains(term) ||
                                         b.BusinessState.Contains(term) ||
                                         b.BusinessCountry.Contains(term));
            }

            return query;
        }
    }
}
