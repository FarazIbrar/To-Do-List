using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML;

namespace DAL
{
    public class DALUserInfo
    {
        public static string AddUser(UserInfo user)
        {
            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();
                string query = "INSERT INTO Users (userName, email, password) VALUES (@userName, @email, @password)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userName", user.userName);
                cmd.Parameters.AddWithValue("@email", user.email);
                cmd.Parameters.AddWithValue("@password", user.password);
                cmd.ExecuteNonQuery();
                con.Close();
                return "User added successfully.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation error number
                {
                    return "A user with this email already exists.";
                }
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
        }


        public static int Auth(UserInfo person)
        {
            SqlConnection con = DBHelper.GetConnection();
            con.Open();
            string query = "SELECT userID FROM Users WHERE email = @email AND password = @password";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@email", person.email);
            cmd.Parameters.AddWithValue("@password", person.password);
            var result = cmd.ExecuteScalar();
            con.Close();

            // Return the userID if a match is found, otherwise return null
            if (result != null && result != DBNull.Value)
            {
                return (int)result;
            }
            return 0;
        }


    }
}
