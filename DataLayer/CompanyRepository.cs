using System.Collections.Concurrent;
using DataLayer.Entities;

namespace DataLayer
{
    public class CompanyRepository : ICompanyRepository
    {
        private static readonly ConcurrentDictionary<int, Company> Companies = new();

        static CompanyRepository() {
            var rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                Companies[i] = new Company()
                {
                    Id = i,
                    Address = new Address() { Address1 = $"{i} Insurance Way", Address2 = "Burlough", Country = "United Kingdom", Postcode = $"BL{rand.Next(1,50)} {rand.Next(1,9)}PW" },
                    IsActive = i % 4 != 0,
                    Name = $"Company {i} Ltd.",
                    InsuranceEndDate = DateTime.Now.AddDays(rand.Next(-30, 400)),
                };
            }
        }

        public Task<Company> GetCompany(int id)
        {
            return Task.FromResult(Companies[id]);
        }

        public Task<List<Company>> GetAll()
        {
            return Task.FromResult(Companies.Select(kv => kv.Value).ToList());
        }
    }
}
