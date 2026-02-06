using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PropertyOps.App
{
    public static class Security
    {
        public static byte[] NewSalt(int len = 32)
        {
            var salt = new byte[len];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            return salt;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            // SHA2_512(password + salt)
            using (var sha = SHA512.Create())
            {
                var pw = Encoding.UTF8.GetBytes(password ?? "");
                var data = new byte[pw.Length + salt.Length];
                Buffer.BlockCopy(pw, 0, data, 0, pw.Length);
                Buffer.BlockCopy(salt, 0, data, pw.Length, salt.Length);
                return sha.ComputeHash(data);
            }
        }

        public static int? ValidateUser(string username, string password)
        {
            var dt = Db.Query(
                "SELECT UserId, PasswordHash, PasswordSalt, IsActive FROM dbo.Users WHERE Username=@u",
                Db.P("@u", username));

            if (dt.Rows.Count != 1) return null;

            var row = dt.Rows[0];
            if (!(bool)row["IsActive"]) return null;

            var salt = (byte[])row["PasswordSalt"];
            var expected = (byte[])row["PasswordHash"];
            var actual = HashPassword(password, salt);

            if (!FixedTimeEquals(expected, actual)) return null;
            return (int)row["UserId"];
        }

        public static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        public static bool UserIsInRole(int userId, string role)
        {
            var o = Db.Scalar(@"
SELECT COUNT(*) 
FROM dbo.UserRoles ur
JOIN dbo.Roles r ON r.RoleId=ur.RoleId
WHERE ur.UserId=@id AND r.Name=@r", Db.P("@id", userId), Db.P("@r", role));
            return Convert.ToInt32(o) > 0;
        }

        public static string GetUsername(int userId)
        {
            var o = Db.Scalar("SELECT Username FROM dbo.Users WHERE UserId=@id", Db.P("@id", userId));
            return o == null ? "" : o.ToString();
        }

        public static void EnsureSeedAdmin()
        {
            // If no users exist, create admin/admin123!
            var count = Convert.ToInt32(Db.Scalar("SELECT COUNT(*) FROM dbo.Users"));
            if (count > 0) return;

            var salt = NewSalt();
            var hash = HashPassword("admin123!", salt);

            Db.Exec("INSERT INTO dbo.Users(Username,PasswordHash,PasswordSalt,IsActive) VALUES (@u,@h,@s,1)",
                Db.P("@u", "admin"), Db.P("@h", hash), Db.P("@s", salt));

            var userId = Convert.ToInt32(Db.Scalar("SELECT UserId FROM dbo.Users WHERE Username='admin'"));
            var adminRoleId = Convert.ToInt32(Db.Scalar("SELECT RoleId FROM dbo.Roles WHERE Name='Admin'"));
            Db.Exec("INSERT INTO dbo.UserRoles(UserId,RoleId) VALUES (@u,@r)", Db.P("@u", userId), Db.P("@r", adminRoleId));
        }
    }
}