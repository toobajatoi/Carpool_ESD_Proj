using Carpool_DB_Proj.Models;
using Microsoft.Data.SqlClient;

namespace Carpool_DB_Proj.ADO_files
{
    public class RideRequestDAL
    {
        string cs = ConnectionString.dbcs;

        public List<RideRequest> GetAllRideRequests(string query)
        {
            List<RideRequest> rideRequestList = new List<RideRequest>();
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    RideRequest rideRequest = new RideRequest();
                    rideRequest.RequestId = Convert.ToInt32(reader["request_id"]);
                    rideRequest.RideId = Convert.ToInt32(reader["ride_id"]);
                    rideRequest.DriverId = Convert.ToInt32(reader["driver_id"]);
                    rideRequest.RequesterId = Convert.ToInt32(reader["requester_id"]);
                    rideRequest.LocationId = Convert.ToInt32(reader["location_id"]);
                    rideRequest.Status = reader["status"].ToString() ?? "";
                    rideRequestList.Add(rideRequest);
                }
            }
            return rideRequestList;
        }
    }
}