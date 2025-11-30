using Carpool_DB_Proj.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Carpool_DB_Proj.ADO_files;

namespace Carpool_DB_Proj.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MySqlProjectContext context;
        private readonly UserDAL userdal_obj;
        private readonly RideDetailDAL rideDetailDAL_obj;
        private readonly RideRequestDAL rideRequestDAL_obj;
        private readonly LocationFilterDAL locationFilterDAL_obj;

        public HomeController(ILogger<HomeController> logger, MySqlProjectContext context)
        {
            _logger = logger;
            this.context = context;
            userdal_obj = new UserDAL();
            rideDetailDAL_obj = new RideDetailDAL();
            rideRequestDAL_obj = new RideRequestDAL();
            locationFilterDAL_obj = new LocationFilterDAL();
        }   

        public IActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                return RedirectToAction("Main_Page");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user_obj)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                return RedirectToAction("Main_Page");
            }
            var user_tmp = context.Users
                .FirstOrDefault(x => x.Email == user_obj.Email && x.Password == user_obj.Password);

            if (user_tmp != null)
            {
                HttpContext.Session.SetString("UserSession", user_tmp.Name);
                HttpContext.Session.SetString("User_Nu", user_tmp.NuId);

                // Create a new LoginSession object
                LoginSession obj = new LoginSession
                {
                    UserId = user_tmp.UserId,
                    LoginTime = DateTime.Now,
                    LogoutTime = null,
                    Status = "active"
                };

                // Add it to the database
                await context.LoginSessions.AddAsync(obj);
                await context.SaveChangesAsync();

                // Retrieve the session ID for the created LoginSession
                var session = context.LoginSessions
                    .FirstOrDefault(s => s.UserId == obj.UserId && s.LoginTime == obj.LoginTime && s.Status == obj.Status);

                if (session != null)
                {
                    HttpContext.Session.SetInt32("SessionId", session.SessionId);
                }

                return RedirectToAction("Main_Page");
            }

            return View();
        }

        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            ViewBag.mySession = HttpContext.Session.GetString("UserSession").ToString();
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            var loginSessionobj = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSessionobj == null)
            {
                return RedirectToAction("Login");
            }
            
            var driverProfile = context.DriverProfiles.FirstOrDefault(s => s.DriverId == loginSessionobj.UserId);
            var user = context.Users.FirstOrDefault(s => s.UserId == loginSessionobj.UserId);
            
            ProfileReviewViewModel obj = new ProfileReviewViewModel()
            {
                myUsers = user,
                myReview = driverProfile != null 
                    ? context.Reviews.Where(s => s.DriverId == driverProfile.DriverId).ToList()
                    : new List<Review>()
            };
            return View(obj);
        }
        public IActionResult Review()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {   
                return RedirectToAction("Main_Page");
            }
            ViewBag.mySession = HttpContext.Session.GetString("UserSession").ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Review(Review review_obj)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            
            // Set ReviewDate to current date
            review_obj.ReviewDate = DateOnly.FromDateTime(DateTime.Now);

            // Get the session ID from HttpContext
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            var loginSessionobj = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSessionobj == null)
            {
                return RedirectToAction("Login");
            }

            // Find the corresponding RideRequest
            var ride = context.RideRequests.FirstOrDefault(s => s.RequesterId == loginSessionobj.UserId);

            if (ride == null)
            {
                TempData["ErrorMessage"] = "No ride request found. Please book a ride first.";
                return RedirectToAction("Main_Page");
            }

            // Assign properties to the review_obj
            review_obj.UserId = ride.RequesterId;
            review_obj.DriverId = ride.DriverId;
            review_obj.RideId = ride.RideId;

            // Add the review to the database and save
            await context.Reviews.AddAsync(review_obj);
            await context.SaveChangesAsync();

            return RedirectToAction("Main_Page");
        }


        public async Task<IActionResult> Main_Page(string searchstring, string toFast)
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.mySession = HttpContext.Session.GetString("UserSession").ToString();
                ViewBag.SearchString = searchstring;
                ViewBag.toFast = toFast;
                
                // Get current user ID for comparison
                int? sessionId = HttpContext.Session.GetInt32("SessionId");
                if (sessionId.HasValue)
                {
                    var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
                    if (loginSession != null)
                    {
                        ViewBag.SessionUserId = loginSession.UserId;
                    }
                }
                
                if (toFast == "Yes" && !string.IsNullOrEmpty(searchstring))
                {
                    LocationFilter locationFilters = new LocationFilter()
                    {
                        PickupLocation = searchstring,
                        DropLocation = "Fast"
                    };
                    await context.LocationFilters.AddAsync(locationFilters);
                    await context.SaveChangesAsync();
                    HttpContext.Session.SetInt32("LocationId", locationFilters.FilterId);
                }
                else if (toFast == "No" && !string.IsNullOrEmpty(searchstring))
                {
                    LocationFilter locationFilters = new LocationFilter()
                    {
                        PickupLocation = "Fast",
                        DropLocation = searchstring
                    };
                    await context.LocationFilters.AddAsync(locationFilters);
                    await context.SaveChangesAsync();
                    HttpContext.Session.SetInt32("LocationId", locationFilters.FilterId);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            if (string.IsNullOrEmpty(toFast))
            {
                ViewBag.SearchString = "";
                ViewBag.toFast = null;
                return View(new UserRideDetailViewModel
                {
                    myUsers = userdal_obj.getallUsers("select * from Users") ?? new List<User>(),
                    myRideDetails = rideDetailDAL_obj.GetAllRideDetails("select * from RideDetails") ?? new List<RideDetail>(),
                    myLocationFilter = new List<LocationFilter>(),
                    myRideRequest = new List<RideRequest>()
                });
            }
            UserRideDetailViewModel obj = new UserRideDetailViewModel()
            {
                myUsers = context.Users.ToList(),
                myRideDetails = context.RideDetails
                .Where(x => x.ToFast != null && x.ToFast.ToLower() == toFast.ToLower())
                .ToList(),
                myLocationFilter = context.LocationFilters.ToList(),
                myRideRequest = new List<RideRequest>()
            };

            if (!string.IsNullOrEmpty(searchstring))
            {
                obj.myRideDetails = obj.myRideDetails.Where(x => x.Routes != null && x.Routes.ToUpper().Contains(searchstring.ToUpper())).ToList();
            }
            return View(obj);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user_obj)
        {
            if (ModelState.IsValid)
            {
                await context.Users.AddAsync(user_obj);
                await context.SaveChangesAsync();

                return RedirectToAction("Login");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register_Car()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.mySession = HttpContext.Session.GetString("UserSession").ToString();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register_Car(RideDetail RideDetailobj)
        {
            string User_Nu = HttpContext.Session.GetString("User_Nu");

            if (string.IsNullOrEmpty(User_Nu))
            {
                return BadRequest("User_Nu not found in session.");
            }

            var user = context.Users.FirstOrDefault(u => u.NuId == User_Nu);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Assign the user ID to the ride detail
            RideDetailobj.UserId = user.UserId;

            // Save RideDetail to generate RideId
            await context.RideDetails.AddAsync(RideDetailobj);
            await context.SaveChangesAsync();

            // Now create the DriverProfile with the generated RideId
            DriverProfile driverProfile_obj = new DriverProfile()
            {
                DriverId = user.UserId,
                RideId = RideDetailobj.RideId // This is now available after SaveChanges
            };

            await context.DriverProfiles.AddAsync(driverProfile_obj);
            await context.SaveChangesAsync();

            return RedirectToAction("Main_Page");
        }

        [HttpGet]
        public async Task<IActionResult> BookRide(int? rideid)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            
            ViewBag.mySession = HttpContext.Session.GetString("UserSession").ToString();
            
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            var loginSessionobj = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSessionobj == null)
            {
                return RedirectToAction("Login");
            }
            
            var driver = context.DriverProfiles.FirstOrDefault(s => s.DriverId == loginSessionobj.UserId);

            if (driver != null && loginSessionobj.UserId == driver.DriverId)
            {
                UserRideDetailViewModel driver_obj = new UserRideDetailViewModel
                {
                    myUsers = context.Users.ToList(),
                    myRideDetails = context.RideDetails.ToList(),
                    myLocationFilter = context.LocationFilters.ToList(),
                    myRideRequest = context.RideRequests.Where(x => x.DriverId == driver.DriverId).ToList()
                };
                return View("BookRide2", driver_obj);
            }

            UserRideDetailViewModel obj = new UserRideDetailViewModel
            {
                myUsers = context.Users.ToList(),
                myRideDetails = context.RideDetails.ToList(),
                myLocationFilter = context.LocationFilters.ToList(),
                myRideRequest = new List<RideRequest>()
            };

            if (!rideid.HasValue)
            {
                var riderequestobj = context.RideRequests.FirstOrDefault(s => s.RequesterId == loginSessionobj.UserId);
                obj.myRideRequest = riderequestobj != null
                    ? new List<RideRequest> { riderequestobj }
                    : new List<RideRequest>();
                return View(obj);
            }

            // Get or create location filter
            int? locationId = HttpContext.Session.GetInt32("LocationId");
            LocationFilter? locationfilterobj = null;

            if (locationId.HasValue)
            {
                locationfilterobj = context.LocationFilters.FirstOrDefault(s => s.FilterId == locationId.Value);
            }

            // If no location filter exists, create a default one
            if (locationfilterobj == null)
            {
                locationfilterobj = new LocationFilter()
                {
                    PickupLocation = "Default",
                    DropLocation = "Default"
                };
                await context.LocationFilters.AddAsync(locationfilterobj);
                await context.SaveChangesAsync();
                HttpContext.Session.SetInt32("LocationId", locationfilterobj.FilterId);
            }

            var ridedetailobj = context.RideDetails.FirstOrDefault(s => s.RideId == rideid);

            if (ridedetailobj == null)
            {
                TempData["ErrorMessage"] = "No ride found with the specified ID.";
                return RedirectToAction("Main_Page");
            }

            // Check if user is trying to book their own ride
            if (ridedetailobj.UserId == loginSessionobj.UserId)
            {
                TempData["ErrorMessage"] = "You cannot book your own ride.";
                return RedirectToAction("Main_Page");
            }

            // Check if ride has available seats
            if (ridedetailobj.AvailableSeats <= 0)
            {
                TempData["ErrorMessage"] = "This ride has no available seats.";
                return RedirectToAction("Main_Page");
            }

            // Check if user already has a pending request for this ride
            var existingRequest = context.RideRequests
                .FirstOrDefault(r => r.RequesterId == loginSessionobj.UserId && 
                                     r.RideId == rideid && 
                                     (r.Status == "Not Accepted" || r.Status == "Pending"));

            if (existingRequest != null)
            {
                TempData["InfoMessage"] = "You already have a pending request for this ride.";
                obj.myRideRequest = new List<RideRequest> { existingRequest };
                return View(obj);
            }

            RideRequest rideRequestobj = new RideRequest()
            {
                RideId = ridedetailobj.RideId,
                DriverId = ridedetailobj.UserId,
                RequesterId = loginSessionobj.UserId,
                LocationId = locationfilterobj.FilterId,
                Status = "Not Accepted"
            };

            await context.RideRequests.AddAsync(rideRequestobj);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ride request submitted successfully!";
            obj.myRideRequest = new List<RideRequest> { rideRequestobj };
            return View(obj);
        }

        public async Task<IActionResult> AcceptRideRequest(int requestId)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            
            // Find the ride request by the RequestId
            var rideRequest = await context.RideRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (rideRequest == null)
            {
                return NotFound("Ride request not found.");
            }

            if(rideRequest.Status == "Accepted")
            {
                return RedirectToAction("BookRide");
            }

            rideRequest.Status = "Accepted";

            var rideDetails = await context.RideDetails.FirstOrDefaultAsync(s => s.RideId == rideRequest.RideId);
            if (rideDetails != null && rideDetails.AvailableSeats > 0)
            {
                rideDetails.AvailableSeats = rideDetails.AvailableSeats - 1;
            }

            await context.SaveChangesAsync();

            return RedirectToAction("BookRide");
        }


        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
                HttpContext.Session.Remove("UserSession");
            }
            int? sessionId = HttpContext.Session.GetInt32("SessionId");

            if (sessionId.HasValue)
            {
                var loginSessionobj = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
                if (loginSessionobj != null)
                {
                    loginSessionobj.LogoutTime = DateTime.Now;
                    loginSessionobj.Status = "expired";
                    context.SaveChanges();
                }
            }

            return RedirectToAction("Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
