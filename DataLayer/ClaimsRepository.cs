using System.Collections.Concurrent;
using DataLayer.Entities;

namespace DataLayer;

public class ClaimsRepository : IClaimsRepository
{
    private static readonly ConcurrentDictionary<string, Claim> ClaimStore = new();
    private static readonly Random Random = new();

    static ClaimsRepository()
    {
        for (var i = 0; i < 20; i++)
        {
            var companyId = Random.Next(4, 9);
            var ucr = GenerateUcr(companyId, i);
            var claimDate = DateTime.Now.AddDays(-Random.Next(500));

            ClaimStore[ucr] = new Claim
            {
                Ucr = ucr,
                ClaimDate = claimDate,
                ClaimTypeName = $"A claim {i}",
                IncurredLoss = (decimal)Random.Next(1, 1000) * 1000 / 100,
                Closed = Random.Next(1, 3) % 2 == 0,
                LossDate = claimDate.AddDays(-Random.Next(1, 21)),
                CompanyId = companyId,
                AssuredName = $"Assured {companyId}",
            };
        }
    }

    private static string GenerateUcr(int companyId, int idx) => $"CLM/UCR{companyId:D3}/{idx:D3}";

    public Task<List<Claim>> GetClaimsByCompanyId(int companyId)
        => Task.FromResult(ClaimStore.Where(kv => kv.Value.CompanyId == companyId).Select((kv) => kv.Value).ToList());

    public Task<Claim> GetClaimById(string ucr)
        => Task.FromResult(ClaimStore.Where(kv => kv.Key == ucr).Select(kv => kv.Value).FirstOrDefault());

    public Task UpdateClaim(Claim claim)
    {
        if (ClaimStore.ContainsKey(claim.Ucr))
        {
            ClaimStore[claim.Ucr] = claim;
            return Task.CompletedTask;
        }

        throw new InvalidOperationException($"Update failed: Claim with UCR {claim.Ucr} does not exist.");
    }

    public Task<List<Claim>> GetAllClaims(int page = 0, int pageSize = 0)
    {
        var result = ClaimStore.Select(kv => kv.Value);

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 30 : pageSize;
        result = result.Skip(page * pageSize).Take(pageSize);

        return Task.FromResult(result.ToList());
    }

    public Task CreateClaim(Claim claim)
    {
        if (!ClaimStore.ContainsKey(claim.Ucr))
            ClaimStore[claim.Ucr] = claim;

        throw new InvalidOperationException($"Insert failed: Claim with UCR {claim.Ucr} already exists.");
    }
}

