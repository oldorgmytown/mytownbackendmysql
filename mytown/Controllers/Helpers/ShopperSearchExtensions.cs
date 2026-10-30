using mytown.Models;

namespace mytown.Controllers.Helpers
{
   
        public static class ShopperSearchExtensions
        {
            public static IQueryable<ShopperRegister> ApplyShopperSearch(
                this IQueryable<ShopperRegister> query, string? search)
            {
                if (string.IsNullOrWhiteSpace(search)) return query;

                var words = search
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Take(5);                      // ignore anything beyond 5 words

                foreach (var word in words)
                {
                    var term = word;
                    query = query.Where(s => s.Username.Contains(term) ||
                                             s.Town.Contains(term) ||
                                             s.City.Contains(term) ||
                                             s.State.Contains(term) ||
                                             s.Country.Contains(term));
                }

                return query;
            }
        
    }
}
