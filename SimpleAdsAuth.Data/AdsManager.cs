using BCrypt.Net;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Data
{
    public class AdsManager
    {
        private string _connectionString;

        public AdsManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool AddUser(User user)
        {
            if (CheckIfUserExists(user.Email))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Name, EmailAddress, PasswordHash) VALUES (@name, @email, @passwordHash)";
            cmd.Parameters.AddWithValue("@email", user.Email);
            cmd.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@name", user.Name);
            conn.Open();
            cmd.ExecuteNonQuery();

            return true;
        }

        public bool CheckIfUserExists(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE EmailAddress = @email";
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddAd(Ad ad)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Ads (Description, PhoneNumber, Date, UserId) VALUES (@description, @phoneNumber, GETDATE(), @userId)";
            cmd.Parameters.AddWithValue("@description", ad.Description);
            cmd.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            cmd.Parameters.AddWithValue("@userId", ad.User.Id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public int GetUserIdFromEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id FROM Users WHERE EmailAddress = @email";
            cmd.Parameters.AddWithValue("@email", email);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void DeleteAd(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public int GetUserIdForAd(int adId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT UserId FROM Ads WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", adId);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public List<Ad> GetAds()
        {
            var ads = new List<Ad>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name, u.Id AS UserId FROM Ads a " +
                "JOIN Users u ON u.Id = a.UserId " +
                "ORDER BY Date DESC";
            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    User = new User
                    {
                        Name = (string)reader["Name"],
                        Id = (int)reader["UserId"]
                    }
                });
            }
            return ads;
        }

        public List<Ad> GetAdsById(int id)
        {
            var ads = new List<Ad>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT a.*, u.Name, u.Id AS UserId FROM Ads a " +
                "JOIN Users u ON u.Id = a.UserId " +
                "WHERE a.UserId = @id " +
                "ORDER BY Date DESC";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Description = (string)reader["Description"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    Date = (DateTime)reader["Date"],
                    User = new User
                    {
                        Name = (string)reader["Name"],
                        Id = (int)reader["UserId"]
                    }
                });
            }
            return ads;
        }

        public User Login(User user)
        {
            var existingUser = GetByEmail(user.Email);
            if (existingUser == null)
            {
                return null;
            }

            var hash = existingUser.PasswordHash;
            if (!BCrypt.Net.BCrypt.Verify(user.Password, existingUser.PasswordHash))
            {
                return null;
            }

            return existingUser;

        }

        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Users WHERE EmailAddress = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = cmd.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }

            return new User
            {
                Id = (int)reader["Id"],
                Email = (string)reader["EmailAddress"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
    }
}
