using CompanyClaims.Mappers;
using DataLayer;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CompanyClaims.Endpoints
{
    public static class ClaimsEndpoints
    {
        public static void MapClaimsEndpoints(this WebApplication app)
        {
            app.MapGet("/company/{companyId}/claims", GetByCompanyId)
                .WithOpenApi().Produces<List<ClaimDto>>().WithDescription("Get list of all claims for a company.");
            app.MapGet("/claims/{ucr}", GetByClaimById)
                .WithOpenApi().Produces<List<ClaimDto>>().WithDescription("Get a specific claim by it's unique reference.");
            app.MapGet("/company/{companyId}", GetCompanyById)
                .WithOpenApi().Produces<CompanyDto>().WithDescription("Get a company's details from it's id.");
            app.MapGet("/claims", GetAllClaims)
                .WithOpenApi().Produces<List<ClaimDto>>().WithDescription("Get a list of all claims.  To help with testing.");
            app.MapPut("/claims", UpdateClaim)
                .WithOpenApi().Produces<ClaimDto>().WithDescription("Update claims details.");
        }

        public static async Task<IResult> GetByCompanyId(int companyId, IClaimsRepository repository, ICompanyRepository companyRepository)
        {
            var company = await companyRepository.GetCompany(companyId);
            if (company is null)
                return Results.NotFound($"Company id {companyId} not found!");
            
            var claims = await repository.GetClaimsByCompanyId(companyId);

            var result = claims.Select(c => c.ToDto(company, "")).ToList();

            return Results.Ok(result);
        }

        public static async Task<IResult> GetAllClaims(IClaimsRepository repository, ICompanyRepository companyRepository, [FromQuery] int page = 0, [FromQuery] int pageSize = 0)
        {
            var claims = await repository.GetAllClaims(page, pageSize);

            var result = claims.Select(async c =>
            {
                var company = await companyRepository.GetCompany(c.CompanyId);
                return c.ToDto(company, "");
            }).Select(t => t.Result).ToList();

            return Results.Ok(result);
        }

        public static async Task<IResult> GetByClaimById(string ucr, IClaimsRepository repository, ICompanyRepository companyRepository)
        {
            if (string.IsNullOrWhiteSpace(ucr) || ucr.Length < 5)
                return Results.BadRequest("A valid claim UCR is required.");

            var claim = await repository.GetClaimById(ucr);
            if (claim is null)
                return Results.NotFound($"The Unique Claim Reference {ucr} doesn't exist.");

            var company = await companyRepository.GetCompany(claim.CompanyId);


            return Results.Ok(claim.ToDto(company, ""));
        }

        public static async Task<IResult> GetCompanyById(int companyId, ICompanyRepository repository)
        {
            var company = await repository.GetCompany(companyId);
            if (company is null)
                return Results.NotFound($"The company with id {companyId} doesn't exist.");


            return Results.Ok(company.ToDto());
        }

        public static async Task<IResult> UpdateClaim(ClaimDto claim, IValidator<ClaimDto> validator, IClaimsRepository repository, ICompanyRepository companyRepository)
        {
            var validation = await validator.ValidateAsync(claim);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var company = await companyRepository.GetCompany(claim.CompanyId);
            if (company is null)
                return Results.BadRequest($"Invalid company id: {claim.CompanyId}.");

            try
            {
                await repository.UpdateClaim(claim.ToEntity());
            }
            catch (InvalidOperationException)
            {
                return Results.BadRequest("The claim doesn't exit.");
            }

            var updatedClaim = await repository.GetClaimById(claim.Ucr);
            if (updatedClaim is null)
                return Results.NotFound("Unable to retrieve the claim after update.");

            return Results.Ok(updatedClaim.ToDto(company, ""));
        }
    }
}
