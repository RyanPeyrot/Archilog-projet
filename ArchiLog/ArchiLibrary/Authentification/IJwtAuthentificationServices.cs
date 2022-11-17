namespace ArchiLibrary.Authentification
{
    public interface IJwtAuthentificationServices
    {
        string GenerateToken(string secret);
    }
}
