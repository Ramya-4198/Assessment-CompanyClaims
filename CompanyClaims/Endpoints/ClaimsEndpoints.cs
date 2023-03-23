using System.Runtime.InteropServices;
using CompanyClaims.Mappers;
using DataLayer;

namespace CompanyClaims.Endpoints
{
    public static class ClaimsEndpoints
    {
        public static void MapClaimsEndpoints(this WebApplication app)
        {
            app.MapGet("/company/{companyId}/claims", GetByCompany).WithOpenApi();
            app.MapGet("/claims/{claimId}", GetByClaimById).WithOpenApi();
        }

        private static async Task<IResult> GetByCompany(int companyId, IClaimsRepository repository, ICompanyRepository companyRepository)
        {
            var claims = await repository.GetClaimsByCompanyId(companyId);
            
            var result = claims.Select(async c =>
            {
                var company = await companyRepository.GetCompany(c.CompanyId);
                return c.ToDto(company, "");
            }).Select(t => t.Result).ToList();

            return Results.Ok(result);
        }

        private static async Task<IResult> GetByClaimById(string ucr, IClaimsRepository repository, ICompanyRepository companyRepository)
        {
            var claim = await repository.GetClaimById(ucr);
            if (claim is null)
                return Results.NotFound($"The Unique Claim Reference {ucr} doesn't exist.");

            var company = await companyRepository.GetCompany(claim.CompanyId);


            return Results.Ok(claim);
        }
    }
}
