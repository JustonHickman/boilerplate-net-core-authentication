using System.Threading.Tasks;
using boilerplate.API.Models;

namespace boilerplate.API.Data
{
    public interface IAuthRepository
    {
        Task<User> RegisterAsync(User user, string password);   
        Task<User> Login(string username, string password);  
        Task<bool> IsUser(string username); 
        Task<bool> IsEmail(string email);
        Task<User> EditUserByIdAsync(int id, User modifiedUserFields);
    }
}