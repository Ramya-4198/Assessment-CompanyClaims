using DataLayer.Entities;

namespace CompanyClaims.Mappers;

public static class MapperExtensions
{

    public static ClaimDto ToDto(this Claim claim, Company company, string claimType) 
        => new ClaimDto(claim.Ucr, claim.CompanyId, claim.ClaimDate,
            claim.LossDate, claim.AssuredName,
            claim.IncurredLoss, claim.IsClosed, company.Name, claimType);

    private static AddressDto ToDto(this Address address) 
        => new AddressDto(address.Address1, address.Address2, address.Address3, address.Postcode, address.Country);

    public static CompanyDto ToDto(this Company company)
        => new CompanyDto(company.Id, company.Name, company.Address.ToDto(), company.IsActive,
            company.InsuranceEndDate);
}

public record ClaimDto(string Ucr, int CompanyId, DateTime ClaimDate, DateTime LossDate, string AssuredName,
    decimal IncurredLoss, bool Closed, string CompanyName, string ClaimType);


public record AddressDto(string Address1, string Address2, string Address3, string Postcode, string Country);

public record CompanyDto(int Id, string Name, AddressDto Address, bool Active, DateTime InsuranceEndDate);

