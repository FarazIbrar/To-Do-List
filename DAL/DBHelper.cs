using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DBHelper
    {
        public static SqlConnection GetConnection()
        {
            SqlConnection con = new SqlConnection("Data Source=FARAZ\\SQLEXPRESS;Initial Catalog=ToDoApp;Integrated Security=True;Encrypt=False");
            return con;
        }
    }
}
