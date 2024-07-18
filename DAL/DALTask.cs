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
    public class DALTask
    {
        public static string AddTaskInfo(int ID, TaskInfo task)
        {
            if (string.IsNullOrWhiteSpace(task.name))
            {
                return "Task name cannot be empty.";
            }

            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();
                string query = "INSERT INTO Tasks (userID, name, description, date, taskList) VALUES (@userID, @name, @description, @date, @taskList)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userID", ID);
                cmd.Parameters.AddWithValue("@name", task.name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(task.description) ? (object)DBNull.Value : task.description);
                cmd.Parameters.AddWithValue("@date", task.date);
                cmd.Parameters.AddWithValue("@taskList", task.taskList);

                cmd.ExecuteNonQuery();
                con.Close();
                return "Task added successfully.";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique constraint violation error number
                {
                    return "A task with this description already exists.";
                }
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
        }




        public static List<TaskInfo> GetTasks(int userID)
        {
            List<TaskInfo> tasks = new List<TaskInfo>();

            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();
                string query = @"
SELECT 
    t.taskID, 
    t.userID, 
    t.name, 
    t.description, 
    t.date, 
    t.taskList,
    l.color,
    t.checkBox
FROM Tasks t
LEFT JOIN List l ON t.taskList = l.listName AND t.userID = l.userID
WHERE t.userID = @userID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userID", userID); // Set the userID parameter
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    TaskInfo task = new TaskInfo
                    {
                        taskID = reader.GetInt32(0),
                        userID = reader.GetInt32(1),
                        name = reader.GetString(2),
                        description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        date = reader.GetDateTime(4),
                        taskList = reader.IsDBNull(5) ? null : reader.GetString(5),
                        listColor = reader.IsDBNull(6) ? null : reader.GetString(6),
                        checkBox = reader.GetBoolean(7)
                    };
                    tasks.Add(task);
                }

                reader.Close();
                con.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return tasks;
        }





        public static List<TaskInfo> GetTodayTasks(int userID)
        {
            List<TaskInfo> tasks = new List<TaskInfo>();

            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();

                // Get the current date without time
                DateTime currentDate = DateTime.Today;

                string query = @"
SELECT 
    t.taskID, 
    t.userID, 
    t.name, 
    t.description, 
    t.date, 
    t.taskList,
    l.color,
    t.checkBox
FROM Tasks t
LEFT JOIN List l ON t.taskList = l.listName AND t.userID = l.userID
WHERE t.userID = @userID AND CAST(t.date AS DATE) = @currentDate";


                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@currentDate", currentDate);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    TaskInfo task = new TaskInfo
                    {
                        taskID = reader.GetInt32(0),
                        userID = reader.GetInt32(1),
                        name = reader.GetString(2),
                        description = reader.IsDBNull(3) ? null : reader.GetString(3),
                        date = reader.GetDateTime(4),
                        taskList = reader.IsDBNull(5) ? null : reader.GetString(5),
                        listColor = reader.IsDBNull(6) ? null : reader.GetString(6),
                        checkBox = reader.GetBoolean(7)
                    };
                    tasks.Add(task);
                }

                reader.Close();
                con.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }

            return tasks;
        }




        public static string SaveCheckBoxState(int taskID, bool checkBoxState)
        {
            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();

                // Update query to update the checkBox state based on taskID
                string query = "UPDATE Tasks SET checkBox = @checkBoxState WHERE taskID = @taskID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@checkBoxState", checkBoxState);
                cmd.Parameters.AddWithValue("@taskID", taskID);

                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    return "CheckBox state updated successfully.";
                }
                else
                {
                    return "No task found with the provided taskID.";
                }
            }
            catch (SqlException ex)
            {
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
        }

        public static string UpdateTaskInfo(int userID, int taskID, TaskInfo task)
        {
            if (string.IsNullOrWhiteSpace(task.name))
            {
                return "Task name cannot be empty.";
            }

            try
            {
                SqlConnection con = DBHelper.GetConnection();
                con.Open();
                string query = @"
            UPDATE Tasks 
            SET name = @name, 
                description = @description, 
                date = @date, 
                taskList = @taskList
            WHERE userID = @userID AND taskID = @taskID";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@taskID", taskID);
                cmd.Parameters.AddWithValue("@name", task.name);
                cmd.Parameters.AddWithValue("@description", string.IsNullOrEmpty(task.description) ? (object)DBNull.Value : task.description);
                cmd.Parameters.AddWithValue("@date", task.date);
                cmd.Parameters.AddWithValue("@taskList", task.taskList);

                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    return "Task updated successfully.";
                }
                else
                {
                    return "No task found with the provided ID.";
                }
            }
            catch (SqlException ex)
            {
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
        }

        public static string DeleteTaskInfo(int taskID)
        {
            try
            {
                using (SqlConnection con = DBHelper.GetConnection())
                {
                    con.Open();
                    string query = @"
                DELETE FROM Tasks 
                WHERE taskID = @taskID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@taskID", taskID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return "Task deleted successfully.";
                        }
                        else
                        {
                            return "No task found with the provided ID.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                return $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"General Error: {ex.Message}";
            }
        }





    }
}
