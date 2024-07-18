using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using ML;

namespace DAL
{
    public class DALList
    {
        public static string AddListInfo(int userID, ListInfo list)
        {
            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();
                string query = "INSERT INTO List (listName, color, userID) VALUES (@listName, @color, @userID)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@listName", list.listName);
                cmd.Parameters.AddWithValue("@color", list.color);
                cmd.Parameters.AddWithValue("@userID", userID); // Add userID parameter
                cmd.ExecuteNonQuery();
                con.Close();
                return "List added successfully.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation error number
                {
                    return "A list with this name already exists.";
                }
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
        }


        public static List<ListInfo> GetAllLists(int userID)
        {
            List<ListInfo> lists = new List<ListInfo>();
            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();
                string query = @"
    SELECT listID, listName, color 
    FROM List
    WHERE userID = @userID"; // Filter by userID
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@userID", userID);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ListInfo list = new ListInfo
                    {
                        listID = reader.GetInt32(0),
                        listName = reader.GetString(1),
                        color = reader.GetString(2)
                    };
                    lists.Add(list);
                }

                reader.Close();
                con.Close();
            }
            catch (SqlException ex)
            {
                // Handle SQL exceptions here
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle general exceptions here
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return lists;
        }


    }
}
