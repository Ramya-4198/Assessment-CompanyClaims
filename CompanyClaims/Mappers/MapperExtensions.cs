using DataLayer.Entities;

namespace CompanyClaims.Mappers;

public static class MapperExtensions
{

    public static ClaimDto ToDto(this Claim claim, Company company)
        => new(claim.Ucr, claim.CompanyId, claim.ClaimDate,
            claim.LossDate, claim.AssuredName,
            claim.IncurredLoss, claim.Closed, company.Name,
            (DateTime.Now.Date - claim.ClaimDate.Date).Days);

    private static AddressDto ToDto(this Address address)
        => new(address.Address1, address.Address2, address.Address3, address.Postcode, address.Country);

    public static CompanyDto ToDto(this Company company)
        => new(company.Id, company.Name, company.Address.ToDto(), company.Active, company.InsuranceEndDate);

    public static Claim ToEntity(this ClaimDto claimDto) =>
        new()
        {
            AssuredName = claimDto.AssuredName,
            ClaimDate = claimDto.ClaimDate,
            CompanyId = claimDto.CompanyId,
            Closed = claimDto.Closed,
            IncurredLoss = claimDto.IncurredLoss,
            LossDate = claimDto.LossDate,
            Ucr = claimDto.Ucr
        };
}

public record ClaimDto(string Ucr, int CompanyId, DateTime ClaimDate, DateTime LossDate, string AssuredName,
    decimal IncurredLoss, bool Closed, string CompanyName, int AgeInDays);


public record AddressDto(string Address1, string Address2, string Address3, string Postcode, string Country);

public record CompanyDto(int Id, string Name, AddressDto Address, bool Active, DateTime InsuranceEndDate);

