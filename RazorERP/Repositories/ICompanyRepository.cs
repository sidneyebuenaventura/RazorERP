using RazorERP.Models;

namespace RazorERP.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company> GetCompanyById(int id);
        Task<IEnumerable<Company>> GetAllCompanies();
        Task<int> CreateCompany(Company company);
        Task<int> UpdateCompany(Company company);
        Task<int> DeleteCompany(int id);
    }
}
