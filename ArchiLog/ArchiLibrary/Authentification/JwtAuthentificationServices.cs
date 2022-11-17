using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace ArchiLibrary.Authentification
{
    public class JwtAuthentificationServices : IJwtAuthentificationServices
    {
        public string GenerateToken(string secret)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)); // La clé généré
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(60), // La durée de vie du token est de 60 minutes
                SigningCredentials = new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256Signature) // Algorythme de sécurité
            };

            // Création du token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
