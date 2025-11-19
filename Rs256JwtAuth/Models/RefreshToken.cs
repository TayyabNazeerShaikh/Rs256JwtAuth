namespace Rs256JwtAuth.Models
{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Revoked { get; set; }
        public bool IsActive => Revoked == null && DateTime.UtcNow < Expires;
    }
}
