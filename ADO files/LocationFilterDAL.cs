using Carpool_DB_Proj.Models;
using Microsoft.Data.SqlClient;

namespace Carpool_DB_Proj.ADO_files
{
    public class LocationFilterDAL
    {
        string cs = ConnectionString.dbcs;

        public List<LocationFilter> GetAllLocationFilters(string query)
        {
            List<LocationFilter> locationFilterList = new List<LocationFilter>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    LocationFilter locationFilter = new LocationFilter();
                    locationFilter.FilterId = Convert.ToInt32(reader["filter_id"]);
                    locationFilter.DropLocation = reader["drop_location"].ToString() ?? "";
                    locationFilter.PickupLocation = reader["pickup_location"].ToString() ?? "";
                    locationFilterList.Add(locationFilter);
                }
            }
            return locationFilterList;
        }
    }
}