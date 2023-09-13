using Shared;
using System.Data.SqlClient;
using System.Data;

namespace ParkingManagement.Api
{
    public interface IVehicleService
    {
        bool IsCarRegisteredInParkingLot(ParkingInformation vehicle);

        IEnumerable<ParkingInformation> GetParkingInformation();

        bool ParkVehicle(ParkingInformation vehicle);

        bool Out(ParkingInformation vehicle);

        StatsReport GetReport();
    }

    public class VehicleService: IVehicleService
    {
        IDataContext _context;

        public VehicleService(IDataContext dataContext)
        {
            _context = dataContext;
        }


        public bool ParkVehicle(ParkingInformation vehicle)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSetting["ConnectionStrings:SqlConnection"]))
                {
                   
                    string insertQuery = $@"INSERT INTO ParkingInformation 
                                                    (TagNumber, InTime, Rate) 
                                                VALUES ('{vehicle.TagNumber}','{vehicle.InTime}', '{vehicle.Rate}') ";

                    con.Open();
                    SqlCommand sqlCmd = new SqlCommand(insertQuery, con);
                    sqlCmd.ExecuteNonQuery();
                 
                }
            }
            catch (Exception ex)
            {
                //To-Do : Implement Logger
                Console.WriteLine(ex.Message);
            }

            return true;
        }

        public IEnumerable<ParkingInformation> GetParkingInformation()
        {
            var parkingInfoList = new List<ParkingInformation>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSetting["ConnectionStrings:SqlConnection"]))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SELECT * " +
                                                    " FROM ParkingInformation " +
                                                    " WHERE OutTime is NULL " +
                                                    " ORDER BY InTime DESC", con);
                    cmd.CommandType = CommandType.Text;
                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        var parkingInfo = new ParkingInformation();

                        parkingInfo.TagNumber = rdr["TagNumber"].ToString()!;
                        parkingInfo.InTime = Convert.ToDateTime(rdr["InTime"]);
                        parkingInfo.Rate = !Convert.IsDBNull(rdr["Rate"]) ? Convert.ToDouble(rdr["Rate"]) : null;
                        parkingInfo.OutTime = !Convert.IsDBNull(rdr["OutTime"]) ? Convert.ToDateTime(rdr["OutTime"]) : null;
                        parkingInfoList.Add(parkingInfo);
                    }
                }
                catch(Exception ex)
                {
                    //To-Do : Implement Logger

                    Console.WriteLine(ex.Message);
                }
            }

            return parkingInfoList.ToList();
        }

        public bool IsCarRegisteredInParkingLot(ParkingInformation vehicle)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSetting["ConnectionStrings:SqlConnection"]))
            {
                string query = $@"SELECT * 
                                        FROM ParkingInformation
                                    WHERE TagNumber = '{vehicle.TagNumber}' and OutTime IS NULL";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();

                return rdr.HasRows;
            }
        }

        public bool Out(ParkingInformation vehicle)
        {
            if (IsCarRegisteredInParkingLot(vehicle)) //Implies that Car was parked in the lot a
                                                      //and hasnt checked out yet
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSetting["ConnectionStrings:SqlConnection"]))
                {
                    try
                    {
                        string insertQuery = $@"UPDATE ParkingInformation 
                                            SET OutTime = GETDATE()
                                            WHERE TagNumber = '{vehicle.TagNumber}' and OutTime IS NULL";

                        con.Open();
                        SqlCommand sqlCmd = new SqlCommand(insertQuery, con);
                        sqlCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            return true;
        }

        public StatsReport GetReport()
        {
            StatsReport result = new StatsReport();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSetting["ConnectionStrings:SqlConnection"]))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("sp_GetReportData", con);
                cmd.CommandType = CommandType.Text;
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        result.TodaysRevenue = !Convert.IsDBNull(rdr["TodaysRevenue"]) ? Convert.ToDouble(rdr["TodaysRevenue"]) : null;
                        result.AvgRevenueThirtyDays = !Convert.IsDBNull(rdr["AvgRevenueThirtyDays"]) ? Convert.ToDouble(rdr["AvgRevenueThirtyDays"]) : null;
                        result.AverageCarsPerDay = !Convert.IsDBNull(rdr["AvgCarsThirtyDays"]) ? Convert.ToInt32(rdr["AvgCarsThirtyDays"]) : null;
                    }
                }
            }
            return result;
        }
    }
}
