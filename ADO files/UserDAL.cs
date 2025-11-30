using Carpool_DB_Proj.Models;
using Microsoft.Data.SqlClient;

namespace Carpool_DB_Proj.ADO_files
{
    public class UserDAL
    {
        string cs = ConnectionString.dbcs;

        public List<User> getallUsers(string query)
        {
            List<User> user_list = new List<User>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    User user_obj = new User();
                    user_obj.UserId = Convert.ToInt32(reader["user_id"]);
                    user_obj.NuId = reader["nu_id"].ToString() ?? "";
                    user_obj.Name = reader["name"].ToString() ?? "";
                    user_obj.PhoneNumber = reader["phone_number"].ToString() ?? "";
                    user_obj.Email = reader["email"].ToString() ?? "";
                    user_obj.Password = reader["password"].ToString() ?? "";
                    user_list.Add(user_obj);
                }
            }
            return user_list;
        }
    }
}
