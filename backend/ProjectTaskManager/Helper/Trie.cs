namespace Projecttaskmanager.Helpers
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children { get; set; } = new();
        public List<UserTrieData> Users { get; set; } = new(); // users at this node
        public bool IsEndOfWord { get; set; } = false;
    }

    public class UserTrieData
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class Trie
    {
        private readonly TrieNode _root = new();

        // Insert a user into the trie
        public void Insert(UserTrieData user)
        {
            var node = _root;
            foreach (var ch in user.Username.ToLower())
            {
                if (!node.Children.ContainsKey(ch))
                    node.Children[ch] = new TrieNode();
                node = node.Children[ch];
                node.Users.Add(user); // store user at every prefix node
            }
            node.IsEndOfWord = true;
        }

        // Search users by prefix
        public List<UserTrieData> Search(string prefix)
        {
            var node = _root;
            foreach (var ch in prefix.ToLower())
            {
                if (!node.Children.ContainsKey(ch))
                    return new List<UserTrieData>(); // no match
                node = node.Children[ch];
            }
            return node.Users; // all users matching prefix
        }
    }
}