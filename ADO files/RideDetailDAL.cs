using Carpool_DB_Proj.Models;
using Microsoft.Data.SqlClient;

namespace Carpool_DB_Proj.ADO_files
{
    public class RideDetailDAL
    {
        string cs = ConnectionString.dbcs;

        public List<RideDetail> GetAllRideDetails(string query)
        {
            List<RideDetail> rideDetails_list = new List<RideDetail>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RideDetail rideDetail_obj = new RideDetail();
                    rideDetail_obj.RideId = Convert.ToInt32(reader["ride_id"]);
                    rideDetail_obj.UserId = Convert.ToInt32(reader["user_id"]);
                    rideDetail_obj.CarId = Convert.ToInt32(reader["car_id"]);
                    rideDetail_obj.ToFast = reader["to_fast"].ToString() ?? "";
                    rideDetail_obj.Routes = reader["routes"].ToString() ?? "";
                    rideDetail_obj.Fare = Convert.ToInt32(reader["fare"]);
                    rideDetail_obj.AvailableSeats = Convert.ToInt32(reader["available_seats"]);

                    // Convert TimeSpan to TimeOnly
                    var classTimeSpan = reader.GetTimeSpan(reader.GetOrdinal("class_time"));
                    var leavingTimeSpan = reader.GetTimeSpan(reader.GetOrdinal("leaving_time"));
                    rideDetail_obj.ClassTime = TimeOnly.FromTimeSpan(classTimeSpan);
                    rideDetail_obj.LeavingTime = TimeOnly.FromTimeSpan(leavingTimeSpan);

                    rideDetails_list.Add(rideDetail_obj);
                }
            }
            return rideDetails_list;
        }
    }
}
