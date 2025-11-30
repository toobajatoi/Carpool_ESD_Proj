# FastCarpool - Carpool Management System

A web-based carpool management system built with ASP.NET Core MVC that allows students to share rides to and from FAST University.

## Features

- **User Authentication**: Secure login and registration system
- **Ride Management**: Drivers can register their cars and create ride listings
- **Ride Booking**: Passengers can search and book available rides
- **Location Filtering**: Search rides by pickup and drop-off locations
- **Request Management**: Drivers can accept or reject ride requests
- **User Profiles**: View and edit user profiles and reviews
- **Review System**: Submit reviews for completed rides
- **Messaging System**: Real-time chat between drivers and passengers with read receipts
- **Notifications**: Live notifications for ride requests, acceptances, rejections, and messages
- **Vehicle Management**: Add, edit, and delete vehicles with full CRUD operations
- **Emergency Contacts**: Manage emergency contacts for safety
- **Favorite Rides**: Save frequently used rides for quick access
- **Real-time Updates**: Live badge counts for unread messages and notifications
- **One Active Ride Rule**: Drivers can only accept one active ride at a time

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 8.0)
- **Database**: SQL Server Express
- **ORM**: Entity Framework Core
- **Frontend**: Bootstrap 5, jQuery, Razor Views
- **Architecture**: MVC Pattern with Data Access Layer (DAL)

## Prerequisites

- .NET 8.0 SDK
- SQL Server Express (or LocalDB)
- Visual Studio 2022 or VS Code (optional)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/YOUR_USERNAME/Carpool_ESD_Proj.git
   cd Carpool_ESD_Proj
   ```

2. Install SQL Server Express if not already installed:
   - Download from [Microsoft SQL Server Downloads](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
   - Or use SQL Server LocalDB

3. Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "dbcs": "Server=localhost\\SQLEXPRESS;Database=my_sql_project;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

4. Restore dependencies:
   ```bash
   dotnet restore
   ```

5. Build the project:
   ```bash
   dotnet build
   ```

6. Run the application:
   ```bash
   dotnet run
   ```

7. Open your browser and navigate to:
   - HTTP: `http://localhost:5189`
   - HTTPS: `https://localhost:7181`

## Database Setup

The database is automatically created on first run using Entity Framework Core's `EnsureCreated()` method. Missing tables (Notification, Message, Vehicle, EmergencyContact, FavoriteRide) are automatically created on application startup. Demo data is seeded automatically.

If you encounter database errors, you can manually run the `CREATE_TABLES.sql` script in SQL Server Management Studio.

## Project Structure

```
Carpool_ESD_Proj/
├── Controllers/          # MVC Controllers
│   └── HomeController.cs
├── Models/               # Data Models and ViewModels
│   ├── User.cs
│   ├── RideDetail.cs
│   ├── RideRequest.cs
│   ├── Vehicle.cs
│   ├── Message.cs
│   ├── Notification.cs
│   ├── EmergencyContact.cs
│   ├── ConversationViewModel.cs
│   └── ...
├── Views/                # Razor Views
│   ├── Home/
│   │   ├── Messages.cshtml
│   │   ├── Notifications.cshtml
│   │   ├── MyVehicles.cshtml
│   │   ├── EditVehicle.cshtml
│   │   ├── EmergencyContacts.cshtml
│   │   └── ...
│   └── Shared/
├── ADO files/           # Data Access Layer (ADO.NET)
├── wwwroot/             # Static files (CSS, JS, Images)
├── Program.cs           # Application entry point
└── appsettings.json     # Configuration
```

## Usage

### For Drivers:
1. Register/Login to your account
2. Go to "My Vehicles" to add, edit, or delete your vehicles
3. Create ride listings with available seats
4. Accept or reject ride requests from passengers (one active ride at a time)
5. Communicate with passengers via Messages
6. View notifications for new ride requests and messages
7. Manage emergency contacts for safety

### For Passengers:
1. Register/Login to your account
2. Browse available rides on the main page
3. Search rides by location (optional)
4. Click "Book" on any available ride
5. Wait for driver acceptance
6. Communicate with drivers via Messages
7. View notifications for ride status updates
8. Submit a review after completed ride
9. Save favorite rides for quick access
10. Manage emergency contacts for safety

## Development

### Key Components:
- **MySqlProjectContext**: EF Core DbContext managing database operations
- **HomeController**: Main controller handling all user interactions (rides, messages, notifications, vehicles, contacts)
- **UserDAL, RideDetailDAL, etc.**: ADO.NET data access for direct database queries
- **ViewModels**: Data transfer objects for views (UserRideDetailViewModel, ProfileReviewViewModel, ConversationViewModel, MessagesViewModel)
- **Real-time Features**: AJAX polling for live message and notification updates
- **Session Management**: Secure session-based authentication and authorization

### New Features (Latest Updates):
- **Messaging System**: Full chat functionality with read receipts and typing indicators
- **Notification System**: Real-time notifications with sender info and message previews
- **Vehicle CRUD**: Complete vehicle management (Create, Read, Update, Delete)
- **Emergency Contacts**: Add and delete emergency contacts
- **Improved Navigation**: Clear navigation items replacing vague "More" dropdown
- **Live Badge Counts**: Real-time updates for unread messages and notifications
- **Direct Actions**: Accept/Reject ride requests directly from notifications

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is part of a university course (ESD Project - Semester 7).

## Authors

- Tooba - Initial work
- Rameen
- Hunain

## Acknowledgments

- FAST University
- ASP.NET Core Documentation
- Bootstrap Team
