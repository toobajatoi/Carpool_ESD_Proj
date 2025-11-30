using Carpool_DB_Proj.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Carpool_DB_Proj.ADO_files;
using System.Linq;

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

        // Helper method to set common ViewBag values
        private void SetViewBagForAuthenticatedUser()
        {
            var userSession = HttpContext.Session.GetString("UserSession");
            if (userSession != null)
            {
                ViewBag.mySession = userSession;
                int? sessionId = HttpContext.Session.GetInt32("SessionId");
                if (sessionId.HasValue)
                {
                    var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
                    if (loginSession != null)
                    {
                        ViewBag.SessionUserId = loginSession.UserId;
                        // Safely check if Notifications table exists before querying
                        try
                        {
                            ViewBag.UnreadNotificationCount = context.Notifications
                                .Count(n => n.UserId == loginSession.UserId && !n.IsRead);
                        }
                        catch
                        {
                            // Table doesn't exist yet - set to 0
                            ViewBag.UnreadNotificationCount = 0;
                        }
                        // Get unread message count
                        try
                        {
                            ViewBag.UnreadMessageCount = context.Messages
                                .Count(m => m.ReceiverId == loginSession.UserId && !m.IsRead);
                        }
                        catch
                        {
                            ViewBag.UnreadMessageCount = 0;
                        }
                    }
                }
            }
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

        [HttpGet]
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            SetViewBagForAuthenticatedUser();
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

        [HttpGet]
        public IActionResult EditProfile()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            SetViewBagForAuthenticatedUser();
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
            
            var user = context.Users.FirstOrDefault(s => s.UserId == loginSessionobj.UserId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User user_obj)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            
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
            
            var existingUser = context.Users.FirstOrDefault(u => u.UserId == loginSessionobj.UserId);
            if (existingUser == null)
            {
                return RedirectToAction("Login");
            }
            
            if (ModelState.IsValid)
            {
                // Update only allowed fields (don't allow changing UserId or NuId)
                existingUser.Name = user_obj.Name;
                existingUser.PhoneNumber = user_obj.PhoneNumber;
                existingUser.Email = user_obj.Email;
                if (!string.IsNullOrWhiteSpace(user_obj.Password))
                {
                    existingUser.Password = user_obj.Password;
                }
                
                context.Users.Update(existingUser);
                await context.SaveChangesAsync();
                
                // Update session name if name changed
                HttpContext.Session.SetString("UserSession", existingUser.Name);
                
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }
            
            SetViewBagForAuthenticatedUser();
            return View(user_obj);
        }
        public IActionResult Review()
        {
            var userSession = HttpContext.Session.GetString("UserSession");
            if (userSession == null)
            {   
                return RedirectToAction("Main_Page");
            }
            ViewBag.mySession = userSession;
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
            var userSession = HttpContext.Session.GetString("UserSession");
            if (userSession != null)
            {
                ViewBag.mySession = userSession;
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
                        // Get unread notification count
                        try
                        {
                            ViewBag.UnreadNotificationCount = context.Notifications
                                .Count(n => n.UserId == loginSession.UserId && !n.IsRead);
                        }
                        catch (Microsoft.Data.SqlClient.SqlException)
                        {
                            // Table doesn't exist yet - set to 0
                            ViewBag.UnreadNotificationCount = 0;
                        }
                        catch
                        {
                            ViewBag.UnreadNotificationCount = 0;
                        }
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
                SetViewBagForAuthenticatedUser();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register_Car(RideDetail RideDetailobj)
        {
            string? User_Nu = HttpContext.Session.GetString("User_Nu");

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
            
            SetViewBagForAuthenticatedUser();
            
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

            // Get requester info for notification
            var requester = context.Users.FirstOrDefault(u => u.UserId == loginSessionobj.UserId);
            var requesterName = requester?.Name ?? "A passenger";
            
            // Create notification for the driver
            var notification = new Notification
            {
                UserId = ridedetailobj.UserId,
                Title = "New Ride Request",
                Message = $"{requesterName} requested a ride. Click to view details.",
                Type = "Info",
                IsRead = false,
                CreatedDate = DateTime.Now,
                RelatedEntityType = "RideRequest",
                RelatedEntityId = rideRequestobj.RequestId
            };

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ride request submitted successfully!";
            obj.myRideRequest = new List<RideRequest> { rideRequestobj };
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRideRequest(int requestId)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }
            
            // Find the ride request by the RequestId
            var rideRequest = await context.RideRequests
                .Include(r => r.Requester)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (rideRequest == null)
            {
                TempData["ErrorMessage"] = "Ride request not found.";
                return RedirectToAction("Notifications");
            }

            // Verify the driver owns this ride request
            if (rideRequest.DriverId != loginSession.UserId)
            {
                TempData["ErrorMessage"] = "You are not authorized to accept this ride request.";
                return RedirectToAction("Notifications");
            }

            if(rideRequest.Status == "Accepted")
            {
                TempData["InfoMessage"] = "This ride request has already been accepted.";
                return RedirectToAction("Notifications");
            }

            // ENFORCE RULE: Driver can only accept one active ride at a time
            var activeRideRequest = await context.RideRequests
                .FirstOrDefaultAsync(r => r.DriverId == loginSession.UserId && 
                                         r.Status == "Accepted" && 
                                         r.RequestId != requestId);

            if (activeRideRequest != null)
            {
                TempData["ErrorMessage"] = "You already have an active ride. You can only accept one ride at a time. Please complete or cancel your current ride before accepting a new one.";
                return RedirectToAction("Notifications");
            }

            rideRequest.Status = "Accepted";

            var rideDetails = await context.RideDetails.FirstOrDefaultAsync(s => s.RideId == rideRequest.RideId);
            if (rideDetails != null && rideDetails.AvailableSeats > 0)
            {
                rideDetails.AvailableSeats = rideDetails.AvailableSeats - 1;
            }

            // Get driver info for notification
            var driver = context.Users.FirstOrDefault(u => u.UserId == loginSession.UserId);
            var driverName = driver?.Name ?? "The driver";
            
            // Create notification for the requester
            var notification = new Notification
            {
                UserId = rideRequest.RequesterId,
                Title = "Ride Request Accepted",
                Message = $"{driverName} accepted your ride request. You're all set!",
                Type = "Success",
                IsRead = false,
                CreatedDate = DateTime.Now,
                RelatedEntityType = "RideRequest",
                RelatedEntityId = rideRequest.RequestId
            };

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ride request accepted successfully!";
            return RedirectToAction("Notifications");
        }

        [HttpPost]
        public async Task<IActionResult> RejectRideRequest(int requestId)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }
            
            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }
            
            var rideRequest = await context.RideRequests
                .Include(r => r.Requester)
                .Include(r => r.Driver)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (rideRequest == null)
            {
                TempData["ErrorMessage"] = "Ride request not found.";
                return RedirectToAction("Notifications");
            }

            if (rideRequest.DriverId != loginSession.UserId)
            {
                TempData["ErrorMessage"] = "You are not authorized to reject this ride request.";
                return RedirectToAction("Notifications");
            }

            if (rideRequest.Status == "Rejected" || rideRequest.Status == "Accepted")
            {
                TempData["InfoMessage"] = $"This ride request has already been {rideRequest.Status.ToLower()}.";
                return RedirectToAction("Notifications");
            }

            rideRequest.Status = "Rejected";

            // Get driver info for notification
            var driver = context.Users.FirstOrDefault(u => u.UserId == loginSession.UserId);
            var driverName = driver?.Name ?? "The driver";
            
            // Create notification for the requester
            var notification = new Notification
            {
                UserId = rideRequest.RequesterId,
                Title = "Ride Request Rejected",
                Message = $"{driverName} was unable to accept your ride request.",
                Type = "Warning",
                IsRead = false,
                CreatedDate = DateTime.Now,
                RelatedEntityType = "RideRequest",
                RelatedEntityId = rideRequest.RequestId
            };

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Ride request rejected.";
            return RedirectToAction("Notifications");
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
            var userSession = HttpContext.Session.GetString("UserSession");
            if (userSession != null)
            {
                ViewBag.mySession = userSession;
            }
            return View();
        }

        // ========== VEHICLE MANAGEMENT ==========
        [HttpGet]
        public IActionResult MyVehicles()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            SetViewBagForAuthenticatedUser();
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            var vehicles = context.Vehicles.Where(v => v.UserId == loginSession.UserId).ToList();
            return View(vehicles);
        }

        [HttpGet]
        public IActionResult AddVehicle()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }
            SetViewBagForAuthenticatedUser();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVehicle([Bind("Make,Model,Year,LicensePlate,Color,Capacity,VehicleType,RegistrationNumber,InsuranceNumber")] Vehicle vehicle)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            // Remove User navigation property from validation since we only need UserId
            ModelState.Remove("User");
            ModelState.Remove("UserId"); // We'll set this manually
            ModelState.Remove("VehicleId"); // Auto-generated
            ModelState.Remove("CreatedDate"); // We'll set this manually
            ModelState.Remove("IsActive"); // We'll set this manually

            // Log ModelState for debugging
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                var errorDetails = string.Join(", ", errors);
                _logger.LogWarning("ModelState validation failed: {Errors}", errorDetails);
                TempData["ErrorMessage"] = "Please correct the following errors: " + errorDetails;
            }
            else
            {
                try
                {
                    vehicle.UserId = loginSession.UserId;
                    vehicle.CreatedDate = DateTime.Now;
                    vehicle.IsActive = true;

                    _logger.LogInformation("Attempting to add vehicle: Make={Make}, Model={Model}, UserId={UserId}", 
                        vehicle.Make, vehicle.Model, vehicle.UserId);

                    await context.Vehicles.AddAsync(vehicle);
                    var result = await context.SaveChangesAsync();

                    _logger.LogInformation("Vehicle added successfully. SaveChanges returned: {Result}", result);

                    TempData["SuccessMessage"] = "Vehicle added successfully!";
                    return RedirectToAction("MyVehicles");
                }
                catch (Microsoft.Data.SqlClient.SqlException ex)
                {
                    _logger.LogError(ex, "SQL Exception when adding vehicle. Error Number: {ErrorNumber}, Message: {Message}", 
                        ex.Number, ex.Message);
                    
                    if (ex.Message.Contains("Invalid object name") && ex.Message.Contains("Vehicle"))
                    {
                        // Try to create the Vehicle table automatically
                        try
                        {
                            var createTableSql = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vehicle')
BEGIN
    CREATE TABLE Vehicle (
        vehicle_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        make NVARCHAR(50) NOT NULL,
        model NVARCHAR(50) NOT NULL,
        year INT NOT NULL,
        license_plate NVARCHAR(20) NOT NULL,
        color NVARCHAR(30),
        capacity INT NOT NULL,
        registration_number NVARCHAR(50),
        insurance_number NVARCHAR(50),
        vehicle_type NVARCHAR(30),
        created_date DATETIME NOT NULL,
        is_active BIT NOT NULL DEFAULT 1,
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
END";
                            
                            context.Database.ExecuteSqlRaw(createTableSql);
                            _logger.LogInformation("Vehicle table created successfully");
                            
                            // Retry adding the vehicle
                            vehicle.UserId = loginSession.UserId;
                            vehicle.CreatedDate = DateTime.Now;
                            vehicle.IsActive = true;
                            
                            await context.Vehicles.AddAsync(vehicle);
                            await context.SaveChangesAsync();
                            
                            TempData["SuccessMessage"] = "Vehicle added successfully! (Table was created automatically)";
                            return RedirectToAction("MyVehicles");
                        }
                        catch (Exception createEx)
                        {
                            _logger.LogError(createEx, "Failed to create Vehicle table");
                            TempData["ErrorMessage"] = "Failed to create Vehicle table. Please restart the application or run CREATE_TABLES.sql manually.";
                        }
                    }
                    else if (ex.Number == 547) // Foreign key constraint
                    {
                        TempData["ErrorMessage"] = "Invalid user ID. Please log in again.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Database error: {ex.Message} (Error #{ex.Number})";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception when adding vehicle: {Message}", ex.Message);
                    TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                }
            }

            SetViewBagForAuthenticatedUser();
            return View(vehicle);
        }

        // ========== NOTIFICATION MANAGEMENT ==========
        [HttpGet]
        public IActionResult Notifications()
        {
            var userSession = HttpContext.Session.GetString("UserSession");
            if (userSession == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.mySession = userSession;
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            var notifications = context.Notifications
                .Where(n => n.UserId == loginSession.UserId)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();

           
var rideRequestDict = new Dictionary<int, object>();
var messageDict = new Dictionary<int, object>();

foreach (var notification in notifications)
{
    if (notification.RelatedEntityType == "RideRequest" && notification.RelatedEntityId.HasValue)
    {
        var rideRequest = context.RideRequests
            .Include(r => r.Requester)
            .Include(r => r.Driver)
            .Include(r => r.Ride)
            .FirstOrDefault(r => r.RequestId == notification.RelatedEntityId.Value);

        if (rideRequest != null)
        {
            rideRequestDict[notification.NotificationId] = rideRequest;
        }
    }
    else if (notification.RelatedEntityType == "Message" && notification.RelatedEntityId.HasValue)
    {
        var message = context.Messages
            .Include(m => m.Sender)
            .FirstOrDefault(m => m.MessageId == notification.RelatedEntityId.Value);

        if (message != null)
        {
            messageDict[notification.NotificationId] = message;
        }
    }
}

// Attach dictionaries to ViewBag for the view to read
ViewBag.RideRequestDict = rideRequestDict;
ViewBag.MessageDict = messageDict;

            ViewBag.UnreadNotificationCount = notifications.Count(n => !n.IsRead);
            ViewBag.SessionUserId = loginSession.UserId;
            SetViewBagForAuthenticatedUser();
            return View(notifications);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MarkNotificationRead([FromBody] NotificationRequest request)
        {
            int notificationId = request.notificationId;
            var notification = await context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult GetUnreadNotificationCount()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return Json(new { count = 0 });
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return Json(new { count = 0 });
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return Json(new { count = 0 });
            }

            try
            {
                var count = context.Notifications
                    .Count(n => n.UserId == loginSession.UserId && !n.IsRead);
                return Json(new { count = count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        [HttpGet]
        public IActionResult GetUnreadMessageCount()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return Json(new { count = 0 });
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return Json(new { count = 0 });
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return Json(new { count = 0 });
            }

            try
            {
                var count = context.Messages
                    .Count(m => m.ReceiverId == loginSession.UserId && !m.IsRead);
                return Json(new { count = count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }

        // ========== MESSAGE MANAGEMENT ==========
        [HttpGet]
        public async Task<IActionResult> Messages(int? conversationId = null)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            SetViewBagForAuthenticatedUser();
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            // Get all conversations (unique users the current user has messaged with)
            var conversations = context.Messages
                .Where(m => m.SenderId == loginSession.UserId || m.ReceiverId == loginSession.UserId)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToList()
                .GroupBy(m => m.SenderId == loginSession.UserId ? m.ReceiverId : m.SenderId)
                .Select(g => new ConversationViewModel
                {
                    UserId = g.Key,
                    User = g.First().SenderId == loginSession.UserId ? g.First().Receiver : g.First().Sender,
                    LastMessage = g.OrderByDescending(m => m.SentDate).First(),
                    UnreadCount = g.Count(m => m.ReceiverId == loginSession.UserId && !m.IsRead)
                })
                .OrderByDescending(c => c.LastMessage.SentDate)
                .ToList();

            // Get messages for selected conversation
            List<Message> conversationMessages = new List<Message>();
            if (conversationId.HasValue)
            {
                conversationMessages = context.Messages
                    .Where(m => (m.SenderId == loginSession.UserId && m.ReceiverId == conversationId.Value) ||
                                (m.SenderId == conversationId.Value && m.ReceiverId == loginSession.UserId))
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .OrderBy(m => m.SentDate)
                    .ToList();

                // Mark messages as read when viewing conversation
                var unreadMessages = conversationMessages.Where(m => m.ReceiverId == loginSession.UserId && !m.IsRead).ToList();
                if (unreadMessages.Any())
                {
                    foreach (var msg in unreadMessages)
                    {
                        msg.IsRead = true;
                    }
                    await context.SaveChangesAsync();
                    
                    // Update unread message count in ViewBag
                    ViewBag.UnreadMessageCount = context.Messages
                        .Count(m => m.ReceiverId == loginSession.UserId && !m.IsRead);
                }
            }

            ViewBag.Conversations = conversations;
            ViewBag.CurrentUserId = loginSession.UserId;
            ViewBag.SelectedConversationId = conversationId;
            ViewBag.ConversationsList = conversations; // For the view model
            
            // Update unread message count
            ViewBag.UnreadMessageCount = context.Messages
                .Count(m => m.ReceiverId == loginSession.UserId && !m.IsRead);
            
            return View(conversationMessages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int receiverId, int? rideRequestId, string content)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Message content cannot be empty.";
                return RedirectToAction("Messages", new { conversationId = receiverId });
            }

            var message = new Message
            {
                SenderId = loginSession.UserId,
                ReceiverId = receiverId,
                RideRequestId = rideRequestId,
                Content = content,
                SentDate = DateTime.Now,
                IsRead = false
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            // Create notification for receiver with message preview
            var sender = context.Users.FirstOrDefault(u => u.UserId == loginSession.UserId);
            var senderName = sender?.Name ?? "Someone";
            var messagePreview = content.Length > 50 ? content.Substring(0, 50) + "..." : content;
            
            var notification = new Notification
            {
                UserId = receiverId,
                Title = "New Message",
                Message = $"{senderName}: \"{messagePreview}\"",
                Type = "Info",
                IsRead = false,
                CreatedDate = DateTime.Now,
                RelatedEntityType = "Message",
                RelatedEntityId = message.MessageId
            };

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message sent successfully!";
            return RedirectToAction("Messages", new { conversationId = receiverId });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SendMessageAjax([FromBody] MessageRequest request)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return Json(new { success = false, message = "Please login first" });
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            if (string.IsNullOrWhiteSpace(request.content))
            {
                return Json(new { success = false, message = "Message cannot be empty" });
            }

            var message = new Message
            {
                SenderId = loginSession.UserId,
                ReceiverId = request.receiverId,
                RideRequestId = request.rideRequestId,
                Content = request.content,
                SentDate = DateTime.Now,
                IsRead = false
            };

            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();

            // Create notification for receiver with message preview
            var sender = context.Users.FirstOrDefault(u => u.UserId == loginSession.UserId);
            var senderName = sender?.Name ?? "Someone";
            var messagePreview = request.content.Length > 50 ? request.content.Substring(0, 50) + "..." : request.content;
            
            var notification = new Notification
            {
                UserId = request.receiverId,
                Title = "New Message",
                Message = $"{senderName}: \"{messagePreview}\"",
                Type = "Info",
                IsRead = false,
                CreatedDate = DateTime.Now,
                RelatedEntityType = "Message",
                RelatedEntityId = message.MessageId
            };

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();

            return Json(new { success = true, messageId = message.MessageId, sentDate = message.SentDate.ToString("MMM dd, yyyy HH:mm") });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetNewMessages([FromBody] GetMessagesRequest request)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return Json(new { success = false });
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return Json(new { success = false });
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return Json(new { success = false });
            }

            var messages = await context.Messages
                .Where(m => (m.SenderId == loginSession.UserId && m.ReceiverId == request.conversationId) ||
                            (m.SenderId == request.conversationId && m.ReceiverId == loginSession.UserId))
                .Where(m => m.MessageId > request.lastMessageId)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.SentDate)
                .Select(m => new
                {
                    messageId = m.MessageId,
                    senderId = m.SenderId,
                    receiverId = m.ReceiverId,
                    senderName = m.Sender.Name,
                    content = m.Content,
                    sentDate = m.SentDate.ToString("MMM dd, yyyy HH:mm"),
                    isRead = m.IsRead
                })
                .ToListAsync();

            return Json(new { success = true, messages = messages });
        }

        public class MessageRequest
        {
            public int receiverId { get; set; }
            public int? rideRequestId { get; set; }
            public string content { get; set; } = string.Empty;
        }

        public class GetMessagesRequest
        {
            public int conversationId { get; set; }
            public int lastMessageId { get; set; }
        }

        // ========== FAVORITE RIDES ==========
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddToFavorites([FromBody] FavoriteRequest request)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return Json(new { success = false, message = "Please login first" });
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return Json(new { success = false, message = "Session expired" });
            }

            int rideId = request.rideId;
            var existing = context.FavoriteRides
                .FirstOrDefault(f => f.UserId == loginSession.UserId && f.RideId == rideId);

            if (existing != null)
            {
                return Json(new { success = false, message = "Already in favorites" });
            }

            var favorite = new FavoriteRide
            {
                UserId = loginSession.UserId,
                RideId = rideId,
                CreatedDate = DateTime.Now
            };

            await context.FavoriteRides.AddAsync(favorite);
            await context.SaveChangesAsync();

            return Json(new { success = true, message = "Added to favorites" });
        }

        [HttpGet]
        public IActionResult FavoriteRides()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            SetViewBagForAuthenticatedUser();
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            var favorites = context.FavoriteRides
                .Where(f => f.UserId == loginSession.UserId)
                .Include(f => f.Ride)
                    .ThenInclude(r => r.User)
                .OrderByDescending(f => f.CreatedDate)
                .ToList();

            return View(favorites);
        }

        // ========== EMERGENCY CONTACTS ==========
        [HttpGet]
        public IActionResult EmergencyContacts()
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            SetViewBagForAuthenticatedUser();
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            var contacts = context.EmergencyContacts
                .Where(e => e.UserId == loginSession.UserId)
                .ToList();

            return View(contacts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmergencyContact([Bind("ContactName,ContactPhone,Relationship,IsPrimary")] EmergencyContact contact)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            // Remove User navigation property from validation since we only need UserId
            ModelState.Remove("User");
            ModelState.Remove("UserId"); // We'll set this manually
            ModelState.Remove("ContactId"); // Auto-generated
            ModelState.Remove("CreatedDate"); // We'll set this manually

            if (ModelState.IsValid)
            {
                try
                {
                    contact.UserId = loginSession.UserId;
                    contact.CreatedDate = DateTime.Now;

                    await context.EmergencyContacts.AddAsync(contact);
                    await context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Emergency contact added successfully!";
                    return RedirectToAction("EmergencyContacts");
                }
                catch (Microsoft.Data.SqlClient.SqlException ex)
                {
                    if (ex.Message.Contains("Invalid object name") && ex.Message.Contains("EmergencyContact"))
                    {
                        // Try to create the EmergencyContact table automatically
                        try
                        {
                            var createTableSql = @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmergencyContact')
BEGIN
    CREATE TABLE EmergencyContact (
        contact_id INT PRIMARY KEY IDENTITY(1,1),
        user_id INT NOT NULL,
        contact_name NVARCHAR(100) NOT NULL,
        contact_phone NVARCHAR(20) NOT NULL,
        relationship NVARCHAR(50),
        is_primary BIT NOT NULL DEFAULT 0,
        created_date DATETIME NOT NULL,
        FOREIGN KEY (user_id) REFERENCES Users(user_id)
    );
END";
                            
                            context.Database.ExecuteSqlRaw(createTableSql);
                            _logger.LogInformation("EmergencyContact table created successfully");
                            
                            // Retry adding the contact
                            contact.UserId = loginSession.UserId;
                            contact.CreatedDate = DateTime.Now;
                            
                            await context.EmergencyContacts.AddAsync(contact);
                            await context.SaveChangesAsync();
                            
                            TempData["SuccessMessage"] = "Emergency contact added successfully! (Table was created automatically)";
                            return RedirectToAction("EmergencyContacts");
                        }
                        catch (Exception createEx)
                        {
                            _logger.LogError(createEx, "Failed to create EmergencyContact table");
                            TempData["ErrorMessage"] = "Failed to create EmergencyContact table. Please restart the application or run CREATE_TABLES.sql manually.";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Error adding emergency contact: {ex.Message}";
                        _logger.LogError(ex, "Error adding emergency contact");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"An error occurred while adding the emergency contact: {ex.Message}";
                    _logger.LogError(ex, "Error adding emergency contact");
                }
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Please correct the following errors: " + string.Join(", ", errors);
            }

            SetViewBagForAuthenticatedUser();
            return View("EmergencyContacts", context.EmergencyContacts.Where(e => e.UserId == loginSession.UserId).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEmergencyContact(int contactId)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            var contact = await context.EmergencyContacts.FindAsync(contactId);
            if (contact == null)
            {
                TempData["ErrorMessage"] = "Emergency contact not found.";
                return RedirectToAction("EmergencyContacts");
            }

            // Verify the contact belongs to the current user
            if (contact.UserId != loginSession.UserId)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this contact.";
                return RedirectToAction("EmergencyContacts");
            }

            context.EmergencyContacts.Remove(contact);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Emergency contact deleted successfully.";
            return RedirectToAction("EmergencyContacts");
        }

        [HttpGet]
        public IActionResult EditVehicle(int id)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            SetViewBagForAuthenticatedUser();
            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            var vehicle = context.Vehicles.FirstOrDefault(v => v.VehicleId == id && v.UserId == loginSession.UserId);
            if (vehicle == null)
            {
                TempData["ErrorMessage"] = "Vehicle not found or you don't have permission to edit it.";
                return RedirectToAction("MyVehicles");
            }

            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVehicle([Bind("VehicleId,Make,Model,Year,LicensePlate,Color,Capacity,VehicleType,RegistrationNumber,InsuranceNumber")] Vehicle vehicle)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            // Verify the vehicle belongs to the current user
            var existingVehicle = await context.Vehicles.FindAsync(vehicle.VehicleId);
            if (existingVehicle == null || existingVehicle.UserId != loginSession.UserId)
            {
                TempData["ErrorMessage"] = "Vehicle not found or you don't have permission to edit it.";
                return RedirectToAction("MyVehicles");
            }

            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("CreatedDate");
            ModelState.Remove("IsActive");

            if (ModelState.IsValid)
            {
                try
                {
                    // Update only the editable fields
                    existingVehicle.Make = vehicle.Make;
                    existingVehicle.Model = vehicle.Model;
                    existingVehicle.Year = vehicle.Year;
                    existingVehicle.LicensePlate = vehicle.LicensePlate;
                    existingVehicle.Color = vehicle.Color;
                    existingVehicle.Capacity = vehicle.Capacity;
                    existingVehicle.VehicleType = vehicle.VehicleType;
                    existingVehicle.RegistrationNumber = vehicle.RegistrationNumber;
                    existingVehicle.InsuranceNumber = vehicle.InsuranceNumber;

                    await context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Vehicle updated successfully!";
                    return RedirectToAction("MyVehicles");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating vehicle");
                    TempData["ErrorMessage"] = $"An error occurred while updating the vehicle: {ex.Message}";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["ErrorMessage"] = "Please correct the following errors: " + string.Join(", ", errors);
            }

            SetViewBagForAuthenticatedUser();
            return View(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVehicle(int vehicleId)
        {
            if (HttpContext.Session.GetString("UserSession") == null)
            {
                return RedirectToAction("Login");
            }

            int? sessionId = HttpContext.Session.GetInt32("SessionId");
            if (!sessionId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var loginSession = context.LoginSessions.FirstOrDefault(s => s.SessionId == sessionId.Value);
            if (loginSession == null)
            {
                return RedirectToAction("Login");
            }

            var vehicle = await context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null)
            {
                TempData["ErrorMessage"] = "Vehicle not found.";
                return RedirectToAction("MyVehicles");
            }

            // Verify the vehicle belongs to the current user
            if (vehicle.UserId != loginSession.UserId)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this vehicle.";
                return RedirectToAction("MyVehicles");
            }

            // Check if vehicle is being used in any active rides
            var activeRides = context.RideDetails.Any(r => r.CarId == vehicle.VehicleId);
            if (activeRides)
            {
                TempData["ErrorMessage"] = "Cannot delete vehicle that is associated with active rides. Please deactivate it instead.";
                return RedirectToAction("MyVehicles");
            }

            context.Vehicles.Remove(vehicle);
            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vehicle deleted successfully.";
            return RedirectToAction("MyVehicles");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
