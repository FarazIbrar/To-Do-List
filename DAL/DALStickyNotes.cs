using ML;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DALStickyNotes
    {
        public static string AddStickyNote(int userID, StickyNote note)
        {
            if (string.IsNullOrWhiteSpace(note.title))
            {
                return "Sticky note title cannot be empty.";
            }

            try
            {
                using (SqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string query = "INSERT INTO StickyNote (userID, title, description) VALUES (@userID, @title, @description)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@title", note.title);
                        cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(note.description) ? (object)DBNull.Value : note.description);

                        cmd.ExecuteNonQuery();
                    }
                }
                return "Sticky note added successfully.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation error number
                {
                    return "A sticky note with this title already exists.";
                }
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
        }



        public static List<StickyNote> GetStickyNotes(int userID)
        {
            List<StickyNote> notes = new List<StickyNote>();

            try
            {
                using (SqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string query = @"
SELECT 
    stickyID, 
    userID, 
    title, 
    description
FROM StickyNote
WHERE userID = @userID"; // Filter by userID

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@userID", userID); // Set the userID parameter
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                StickyNote note = new StickyNote
                                {
                                    stickyID = reader.GetInt32(0),
                                    userID = reader.GetInt32(1),
                                    title = reader.GetString(2),
                                    description = reader.IsDBNull(3) ? null : reader.GetString(3)
                                };
                                notes.Add(note);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return notes;
        }
    }

}
