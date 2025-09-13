using TransitHub.Models;

namespace TransitHub.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(TransitHubDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed TrainQuotaTypes
            if (!context.TrainQuotaTypes.Any())
            {
                var quotaTypes = new[]
                {
                    new TrainQuotaType { QuotaName = "Normal", Description = "Regular booking quota" },
                    new TrainQuotaType { QuotaName = "Tatkal", Description = "Emergency booking quota (premium charges)" },
                    new TrainQuotaType { QuotaName = "Premium Tatkal", Description = "Premium emergency booking quota" },
                    new TrainQuotaType { QuotaName = "Senior Citizen", Description = "Senior citizen quota" },
                    new TrainQuotaType { QuotaName = "Ladies", Description = "Ladies quota" },
                    new TrainQuotaType { QuotaName = "Physically Handicapped", Description = "Quota for physically handicapped passengers" }
                };
                
                await context.TrainQuotaTypes.AddRangeAsync(quotaTypes);
            }

            // Seed BookingStatusTypes
            if (!context.BookingStatusTypes.Any())
            {
                var statusTypes = new[]
                {
                    new BookingStatusType { StatusName = "Confirmed", Description = "Booking confirmed with seat allocation" },
                    new BookingStatusType { StatusName = "Waitlisted", Description = "Booking in waitlist queue" },
                    new BookingStatusType { StatusName = "Cancelled", Description = "Booking cancelled by user or system" },
                    new BookingStatusType { StatusName = "RAC", Description = "Reservation Against Cancellation" },
                    new BookingStatusType { StatusName = "Chart Prepared", Description = "Final chart prepared, no more bookings" }
                };
                
                await context.BookingStatusTypes.AddRangeAsync(statusTypes);
            }

            // Seed PaymentModes
            if (!context.PaymentModes.Any())
            {
                var paymentModes = new[]
                {
                    new PaymentMode { ModeName = "Credit Card", Description = "Payment via credit card" },
                    new PaymentMode { ModeName = "Debit Card", Description = "Payment via debit card" },
                    new PaymentMode { ModeName = "UPI", Description = "Unified Payments Interface" },
                    new PaymentMode { ModeName = "Net Banking", Description = "Internet banking payment" },
                    new PaymentMode { ModeName = "Wallet", Description = "Digital wallet payment" }
                };
                
                await context.PaymentModes.AddRangeAsync(paymentModes);
            }

            // Seed TrainClasses
            if (!context.TrainClasses.Any())
            {
                var trainClasses = new[]
                {
                    new TrainClass { ClassName = "SL", Description = "Sleeper Class" },
                    new TrainClass { ClassName = "3A", Description = "Third AC" },
                    new TrainClass { ClassName = "2A", Description = "Second AC" },
                    new TrainClass { ClassName = "1A", Description = "First AC" },
                    new TrainClass { ClassName = "CC", Description = "Chair Car" },
                    new TrainClass { ClassName = "2S", Description = "Second Sitting" },
                    new TrainClass { ClassName = "FC", Description = "First Class" }
                };
                
                await context.TrainClasses.AddRangeAsync(trainClasses);
            }

            // Seed FlightClasses
            if (!context.FlightClasses.Any())
            {
                var flightClasses = new[]
                {
                    new FlightClass { ClassName = "Economy", Description = "Economy class seating" },
                    new FlightClass { ClassName = "Premium Economy", Description = "Premium economy class" },
                    new FlightClass { ClassName = "Business", Description = "Business class seating" },
                    new FlightClass { ClassName = "First", Description = "First class seating" }
                };
                
                await context.FlightClasses.AddRangeAsync(flightClasses);
            }

            // Seed Stations
            if (!context.Stations.Any())
            {
                var stations = new[]
                {
                    new Station { StationName = "New Delhi Railway Station", City = "New Delhi", State = "Delhi", StationCode = "NDLS" },
                    new Station { StationName = "Mumbai Central", City = "Mumbai", State = "Maharashtra", StationCode = "BCT" },
                    new Station { StationName = "Howrah Junction", City = "Kolkata", State = "West Bengal", StationCode = "HWH" },
                    new Station { StationName = "Chennai Central", City = "Chennai", State = "Tamil Nadu", StationCode = "MAS" },
                    new Station { StationName = "Bangalore City Junction", City = "Bangalore", State = "Karnataka", StationCode = "SBC" },
                    new Station { StationName = "Hyderabad Deccan", City = "Hyderabad", State = "Telangana", StationCode = "HYB" },
                    new Station { StationName = "Pune Junction", City = "Pune", State = "Maharashtra", StationCode = "PUNE" },
                    new Station { StationName = "Ahmedabad Junction", City = "Ahmedabad", State = "Gujarat", StationCode = "ADI" },
                    new Station { StationName = "Jaipur Junction", City = "Jaipur", State = "Rajasthan", StationCode = "JP" },
                    new Station { StationName = "Lucknow Junction", City = "Lucknow", State = "Uttar Pradesh", StationCode = "LJN" }
                };
                
                await context.Stations.AddRangeAsync(stations);
            }

            // Seed Airports
            if (!context.Airports.Any())
            {
                var airports = new[]
                {
                    new Airport { AirportName = "Indira Gandhi International Airport", City = "New Delhi", State = "Delhi", Code = "DEL" },
                    new Airport { AirportName = "Chhatrapati Shivaji International Airport", City = "Mumbai", State = "Maharashtra", Code = "BOM" },
                    new Airport { AirportName = "Netaji Subhas Chandra Bose International Airport", City = "Kolkata", State = "West Bengal", Code = "CCU" },
                    new Airport { AirportName = "Chennai International Airport", City = "Chennai", State = "Tamil Nadu", Code = "MAA" },
                    new Airport { AirportName = "Kempegowda International Airport", City = "Bangalore", State = "Karnataka", Code = "BLR" },
                    new Airport { AirportName = "Rajiv Gandhi International Airport", City = "Hyderabad", State = "Telangana", Code = "HYD" },
                    new Airport { AirportName = "Pune Airport", City = "Pune", State = "Maharashtra", Code = "PNQ" },
                    new Airport { AirportName = "Sardar Vallabhbhai Patel International Airport", City = "Ahmedabad", State = "Gujarat", Code = "AMD" },
                    new Airport { AirportName = "Jaipur International Airport", City = "Jaipur", State = "Rajasthan", Code = "JAI" },
                    new Airport { AirportName = "Chaudhary Charan Singh International Airport", City = "Lucknow", State = "Uttar Pradesh", Code = "LKO" }
                };
                
                await context.Airports.AddRangeAsync(airports);
            }

            // Save all changes
            await context.SaveChangesAsync();
        }
    }
}