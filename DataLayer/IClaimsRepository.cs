using DataLayer.Entities;

namespace DataLayer;

public interface IClaimsRepository
{
    Task<List<Claim>> GetClaimsByCompanyId(int companyId);
    Task<Claim> GetClaimById(string ucr);
    Task UpdateClaim(Claim claim);
    Task<List<Claim>> GetAllClaims(int page = 0, int pageSize = 0);
    Task CreateClaim(Claim claim);
}