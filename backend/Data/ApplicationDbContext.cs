using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TrainBooking.Models;

namespace TrainBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Entity DbSets
        public DbSet<Station> Stations { get; set; }
        public DbSet<TrainClass> TrainClasses { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainSchedule> TrainSchedules { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure Identity table names (optional)
            builder.Entity<IdentityUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // Configure entity relationships and constraints
            ConfigureStationEntity(builder);
            ConfigureTrainClassEntity(builder);
            ConfigureTrainEntity(builder);
            ConfigureTrainScheduleEntity(builder);
            ConfigureFlightEntity(builder);
            ConfigureBookingEntity(builder);
            ConfigurePassengerEntity(builder);
            ConfigurePaymentEntity(builder);
        }    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        private void UpdateAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            var currentUser = GetCurrentUser();
            var currentTime = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                    entity.UpdatedAt = currentTime;
                    entity.UpdatedBy = currentUser;
                    entity.IsActive = true;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entity.UpdatedAt = currentTime;
                    entity.UpdatedBy = currentUser;
                    
                    // Prevent modification of CreatedAt
                    entry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                }
            }
        }

        private string GetCurrentUser()
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
            }
            return "System";
        }

        private void ConfigureStationEntity(ModelBuilder builder)
        {
            builder.Entity<Station>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.City).IsRequired().HasMaxLength(50);
                entity.Property(e => e.State).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Code).IsUnique();
                
                // Configure navigation properties to avoid cycles
                entity.HasMany(e => e.DepartureTrains)
                    .WithOne(e => e.SourceStation)
                    .HasForeignKey(e => e.SourceStationId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasMany(e => e.ArrivalTrains)
                    .WithOne(e => e.DestinationStation)
                    .HasForeignKey(e => e.DestinationStationId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasMany(e => e.DepartureFlights)
                    .WithOne(e => e.SourceStation)
                    .HasForeignKey(e => e.SourceStationId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasMany(e => e.ArrivalFlights)
                    .WithOne(e => e.DestinationStation)
                    .HasForeignKey(e => e.DestinationStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureTrainClassEntity(ModelBuilder builder)
        {
            builder.Entity<TrainClass>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.HasIndex(e => e.Code).IsUnique();
            });
        }

        private void ConfigureTrainEntity(ModelBuilder builder)
        {
            builder.Entity<Train>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TrainNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.TrainNumber).IsUnique();
                
                // Configure foreign key relationships with NO ACTION to avoid cycles
                entity.HasOne(e => e.SourceStation)
                    .WithMany()
                    .HasForeignKey(e => e.SourceStationId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.DestinationStation)
                    .WithMany()
                    .HasForeignKey(e => e.DestinationStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureTrainScheduleEntity(ModelBuilder builder)
        {
            builder.Entity<TrainSchedule>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Train)
                    .WithMany(e => e.TrainSchedules)
                    .HasForeignKey(e => e.TrainId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.TrainClass)
                    .WithMany(e => e.TrainSchedules)
                    .HasForeignKey(e => e.TrainClassId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.SourceStation)
                    .WithMany(e => e.DepartureTrains)
                    .HasForeignKey(e => e.SourceStationId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.DestinationStation)
                    .WithMany(e => e.ArrivalTrains)
                    .HasForeignKey(e => e.DestinationStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureFlightEntity(ModelBuilder builder)
        {
            builder.Entity<Flight>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FlightNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Airline).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => new { e.FlightNumber, e.TravelDate }).IsUnique();
            });
        }

        private void ConfigureBookingEntity(ModelBuilder builder)
        {
            builder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BookingReference).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.BookingReference).IsUnique();
                
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Train)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.TrainId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.TrainSchedule)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.TrainScheduleId)
                    .OnDelete(DeleteBehavior.Restrict);
                    
                entity.HasOne(e => e.Flight)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(e => e.FlightId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigurePassengerEntity(ModelBuilder builder)
        {
            builder.Entity<Passenger>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                
                entity.HasOne(e => e.Booking)
                    .WithMany(e => e.Passengers)
                    .HasForeignKey(e => e.BookingId);
            });
        }

        private void ConfigurePaymentEntity(ModelBuilder builder)
        {
            builder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TransactionId).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.TransactionId).IsUnique();
                
                entity.HasOne(e => e.Booking)
                    .WithOne(e => e.Payment)
                    .HasForeignKey<Payment>(e => e.BookingId);
            });
        }
    }
}