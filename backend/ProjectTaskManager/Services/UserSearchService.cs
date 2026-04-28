using Projecttaskmanager.Data;
using Projecttaskmanager.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Projecttaskmanager.Services
{
    public class UserSearchService(IServiceScopeFactory scopeFactory) : IUserSearchService
    {
        //  use this instead of AppDbContext
        private Trie _trie = new();
        private bool _isBuilt = false;

//it starts once the app starts
        public async Task BuildTrieAsync()
        {
            _trie = new Trie();

            //  created a temporary scope to get AppDbContext
            using var scope = scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var users = await context.User
                .Select(u => new UserTrieData
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role
                })
                .ToListAsync();

            foreach (var user in users)
                _trie.Insert(user);

            _isBuilt = true;
        }

        public List<UserTrieData> SearchUsers(string prefix)
        {
            if (!_isBuilt)
                return new List<UserTrieData>(); //  return empty instead of throwing

            if (string.IsNullOrWhiteSpace(prefix))
                return new List<UserTrieData>();

            return _trie.Search(prefix);
        }
    }
}