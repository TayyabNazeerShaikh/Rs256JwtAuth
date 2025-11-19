using Rs256JwtAuth.Models;

namespace Rs256JwtAuth.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly List<RefreshToken> _refreshTokens = new();

        public void Add(RefreshToken token)
        {
            _refreshTokens.Add(token);
        }

        public RefreshToken? GetByToken(string token)
        {
            return _refreshTokens.FirstOrDefault(t => t.Token == token);
        }

        public void Revoke(string token)
        {
            var existingToken = GetByToken(token);
            if (existingToken != null)
            {
                existingToken.Revoked = DateTime.UtcNow;
            }
        }
        
        public void RevokeAllForUser(string email)
        {
            var tokens = _refreshTokens.Where(t => t.UserEmail == email && t.IsActive).ToList();
            foreach (var token in tokens)
            {
                token.Revoked = DateTime.UtcNow;
            }
        }
    }
}
