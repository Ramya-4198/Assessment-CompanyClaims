using DataLayer.Entities;

namespace DataLayer
{
    public interface ICompanyRepository
    {
        Task<Company> GetCompany(int id);
        Task<List<Company>> GetAll();
    }
}
