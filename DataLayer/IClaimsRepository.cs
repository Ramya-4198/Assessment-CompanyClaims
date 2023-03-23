using DataLayer.Entities;

namespace DataLayer;

public interface IClaimsRepository
{
    Task<List<Claim>> GetClaimsByCompanyId(int companyId);
    Task<Claim> GetClaimById(string ucr);
    Task UpdateClaim(Claim claim);
    Task CreateClaim(Claim claim);
}