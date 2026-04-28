// Services/IUserSearchService.cs
using Projecttaskmanager.Helpers;

namespace Projecttaskmanager.Services
{
    public interface IUserSearchService
    {
        List<UserTrieData> SearchUsers(string prefix);
        
        Task BuildTrieAsync(); // rebuild trie from DB
    }
}