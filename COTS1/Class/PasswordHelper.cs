using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace COTS1.Class
{
	public static class PasswordHelper
	{
		public static string HashPassword(string password, out string salt)
		{
			// Tạo salt ngẫu nhiên
			byte[] saltBytes = new byte[16];
			using (var rng = new RNGCryptoServiceProvider())
			{
				rng.GetBytes(saltBytes);
			}
			salt = Convert.ToBase64String(saltBytes);

			// Băm mật khẩu
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: saltBytes,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 10000,
				numBytesRequested: 256 / 8));

			return hashed;
		}

		public static bool VerifyPassword(string password, string salt, string hashedPassword)
		{
			string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: Convert.FromBase64String(salt),
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 10000,
				numBytesRequested: 256 / 8));

			return hashed == hashedPassword;
		}
	}
}
