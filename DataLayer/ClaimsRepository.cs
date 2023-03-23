using System.Collections.Concurrent;
using DataLayer.Entities;

namespace DataLayer;

public class ClaimsRepository: IClaimsRepository
{
    private static readonly ConcurrentDictionary<string, Claim> ClaimStore = new();
    private static readonly string _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static readonly Random _random = new();

    static ClaimsRepository()
    {
        for (var i = 0; i < 20; i++)
        {
            var companyId = _random.Next(4, 9);
            var ucr = GenerateUcr(companyId);
            var claimDate = DateTime.Now.AddDays(-_random.Next(500));

            ClaimStore[ucr] = new Claim
            {
                Ucr = ucr,
                ClaimDate = claimDate,
                ClaimTypeName = $"A claim {i}",
                IncurredLoss = (decimal)_random.Next(1, 1000) * 1000 / 100,
                IsClosed = _random.Next(1, 2) % 2 == 0,
                LossDate = claimDate.AddDays(_random.Next(1, 15)),
                CompanyId = companyId,
                AssuredName = $"Assured {companyId}",
            };
        }
    }

    private static string GenerateUcr(int companyId)
    {
        var suffix = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            suffix += _alphabet[_random.Next(_alphabet.Length)];
        }

        return $"CLM/{companyId:D3}/{DateTime.Now.Millisecond:D7}/{suffix}";
    }

    public Task<List<Claim>> GetClaimsByCompanyId(int companyId) 
        => Task.FromResult(ClaimStore.Where(kv => kv.Value.CompanyId == companyId).Select((kv) => kv.Value).ToList());

    public Task<Claim> GetClaimById(string ucr) 
        => Task.FromResult(ClaimStore.Where(kv => kv.Key == ucr).Select(kv => kv.Value).FirstOrDefault());

    public Task UpdateClaim(Claim claim)
    {
        if (ClaimStore.ContainsKey(claim.Ucr))
            ClaimStore[claim.Ucr] = claim;

        throw new InvalidOperationException($"Update failed: Claim with UCR {claim.Ucr} does not exist.");
    }

    public Task CreateClaim(Claim claim)
    {
        if (!ClaimStore.ContainsKey(claim.Ucr))
            ClaimStore[claim.Ucr] = claim;

        throw new InvalidOperationException($"Insert failed: Claim with UCR {claim.Ucr} already exists.");
    }
}

