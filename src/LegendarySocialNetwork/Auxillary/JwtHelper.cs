using LegendarySocialNetwork.DataClasses.Internals;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LegendarySocialNetwork.Auxillary
{
    public static class JwtHelper
    {
        public static JWTSettings JWTSettings { get; set; } = new JWTSettings();

        public static JwtSecurityToken GenerateJWToken()
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTSettings.Key));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            return new JwtSecurityToken(
                issuer: JWTSettings.Issuer,
                audience: JWTSettings.Audience,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(JWTSettings.DurationInMinutes)),
                signingCredentials: signinCredentials
            );
        }
    }
}
