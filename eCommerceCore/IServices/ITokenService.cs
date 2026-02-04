using eCommerceInfarsructure.Identity;

namespace eCommerceCore.IServices
{
    public interface ITokenService
    {

        Task<string> GenerateToken(ApplicationUser applicationUser);
    }
}
