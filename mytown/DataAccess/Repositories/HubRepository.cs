using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlConnector;
using mytown.DataAccess.Interfaces;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Models.mytown.DataAccess;
using mytown.Services.Interfaces;
using MyTown.Models;
using Razorpay.Api;
using System.Data;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace mytown.DataAccess.Repositories
{
    public class HubRepository : IHubRepository
    {

        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly string _connectionString;
        private readonly IMemoryCache _cache;

        public HubRepository(AppDbContext context, IEmailService emailservice, IConfiguration config, IMemoryCache cache)
        {
            _context = context;
            _emailService = emailservice;
            _connectionString = config.GetConnectionString("mysqlConnection");
            _cache = cache;
        }
        
            public async Task<List<HubAddressDto>> GetAllHubLocationsAsync()
            {
                return await _context.HubDetails
                    .AsNoTracking()
                    .OrderBy(h => h.State).ThenBy(h => h.City).ThenBy(h => h.HubName)
                    .Select(h => new HubAddressDto
                    {
                        HubId = h.HubId,
                        HubAddressId = h.HubAddressId,
                        HubName = h.HubName,
                        AddressLine = h.AddressLine,
                        Town = h.Town,
                        City = h.City,
                        State = h.State,
                        Country = h.Country,
                        Pin = h.Pin,
                        Phone = h.Phone
                    })
                    .ToListAsync();
            }
            
        
    }
        
}
