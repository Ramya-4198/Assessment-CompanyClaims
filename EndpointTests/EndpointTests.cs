using CompanyClaims.Endpoints;
using CompanyClaims.Mappers;
using CompanyClaims.Validators;
using DataLayer;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace EndpointTests;

public class EndPointTests
{
    // Would normally mock these up - but as they're not backed by a DB, little point in doing so.
    private readonly IClaimsRepository _claimsRepository = new ClaimsRepository();
    private readonly ICompanyRepository _companyRepository = new CompanyRepository();
    private readonly IValidator<ClaimDto> _validator = new ClaimValidator();

    [Fact]
    public async Task UpdateClaimEndpoint_AmendAssuredName_Should_UpdateAssuredNameInRepo()
    {
        var claim = (await _claimsRepository.GetAllClaims(1, 1)).First();
        Assert.NotNull(claim);

        var company = await _companyRepository.GetCompany(claim.CompanyId);
        Assert.NotNull(company);

        claim.AssuredName = "Amended Assured Name";
        var claimDto = claim.ToDto(company);


        await UpdateClaim(claimDto);

        var updated = await _claimsRepository.GetClaimById(claim.Ucr);
        Assert.NotNull(updated);

        Assert.Equal(claim.AssuredName, updated.AssuredName);
    }


    [Fact]
    public async Task UpdateClaimEndpoint_UpdateNonExistentClaim_Should_ReturnBadRequest()
    {
        var claimDto = new ClaimDto("NON-EXISTENT-UCR", 7, DateTime.Now, DateTime.Now.AddDays(-1), "Another Name", 0.00m, false, String.Empty, 0);
        var result = (await UpdateClaim(claimDto)) as BadRequest<string>;
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task UpdateClaimEndpoint_UpdateDateOfLossGreaterThanClaimDate_Should_ReturnValidationError()
    {
        var claim = (await _claimsRepository.GetAllClaims(1, 1)).First();
        Assert.NotNull(claim);

        var company = await _companyRepository.GetCompany(claim.CompanyId);
        Assert.NotNull(company);

        claim.LossDate = DateTime.Now;
        var claimDto = claim.ToDto(company);

        var result = (await UpdateClaim(claimDto)) as ProblemHttpResult;
        Assert.NotNull(result);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal(1, (result.ProblemDetails as HttpValidationProblemDetails)?.Errors.Count );
        Assert.Equal("Loss date must be before or on the claim date",
            (result.ProblemDetails as HttpValidationProblemDetails)?.Errors.First().Value[0]);

    }

    private async Task<IResult> UpdateClaim(ClaimDto claimDto) => await ClaimsEndpoints.UpdateClaim(claimDto, _validator, _claimsRepository, _companyRepository);
}
