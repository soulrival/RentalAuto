using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;
using System;
using System.Collections.Generic;

namespace RentalAuto.Models;

public partial class RentalDbContext : DbContext
{
    public RentalDbContext()
    {
    }

    public RentalDbContext(DbContextOptions<RentalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingDetail> BookingDetails { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Inspector> Inspectors { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148.   For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.  
        => optionsBuilder.UseNpgsql("Server=localhost;Database=rental_db;Username=postgres;Password=buolktcs228;Persist Security Info=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка конвертера для всех DateTime полей
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new NpgsqlDateTimeConverter());
                }
            }
        }

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("booking_pkey");

            entity.ToTable("booking", "rental_schema", tb => tb.HasComment("Информация о бронированиях автомобилей"));

            entity.HasIndex(e => new { e.StartDate, e.EndDate }, "idx_booking_dates");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.ContractGenerated)
                .HasDefaultValue(false)
                .HasColumnName("contract_generated");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.PickupLocationId).HasColumnName("pickup_location_id");
            entity.Property(e => e.ReturnLocationId).HasColumnName("return_location_id");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");

            entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_car_id_fkey");

            entity.HasOne(d => d.Client).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ClientId)
                .HasConstraintName("booking_client_id_fkey");

            entity.HasOne(d => d.PickupLocation).WithMany(p => p.BookingPickupLocations)
                .HasForeignKey(d => d.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_pickup_location_id_fkey");

            entity.HasOne(d => d.ReturnLocation).WithMany(p => p.BookingReturnLocations)
                .HasForeignKey(d => d.ReturnLocationId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("booking_return_location_id_fkey");

            entity.HasMany(d => d.Services).WithMany(p => p.Bookings)
                .UsingEntity<Dictionary<string, object>>(
                    "BookingService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("ServiceId")
                        .HasConstraintName("booking_service_service_id_fkey"),
                    l => l.HasOne<Booking>().WithMany()
                        .HasForeignKey("BookingId")
                        .HasConstraintName("booking_service_booking_id_fkey"),
                    j =>
                    {
                        j.HasKey("BookingId", "ServiceId").HasName("booking_service_pkey");
                        j.ToTable("booking_service", "rental_schema");
                        j.IndexerProperty<int>("BookingId").HasColumnName("booking_id");
                        j.IndexerProperty<int>("ServiceId").HasColumnName("service_id");
                    });
        });

        modelBuilder.Entity<BookingDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("booking_details", "rental_schema");

            entity.Property(e => e.BookingId).HasColumnName("booking_id");
            entity.Property(e => e.CarModel).HasColumnName("car_model");
            entity.Property(e => e.ClientFullname).HasColumnName("client_fullname");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.PickupLocation)
                .HasMaxLength(100)
                .HasColumnName("pickup_location");
            entity.Property(e => e.ReturnLocation)
                .HasMaxLength(100)
                .HasColumnName("return_location");
            entity.Property(e => e.Services).HasColumnName("services");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasColumnName("total_price");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("car_pkey");

            entity.ToTable("car", "rental_schema", tb => tb.HasComment("Хранение информации об автомобилях автопарка"));

            entity.HasIndex(e => e.RegistrationNumber, "car_registration_number_key").IsUnique();

            entity.HasIndex(e => e.Vin, "car_vin_key").IsUnique();

            entity.HasIndex(e => e.Status, "idx_car_status");

            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.Class)
                .HasMaxLength(20)
                .HasColumnName("class");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Engine)
                .HasMaxLength(50)
                .HasColumnName("engine");
            entity.Property(e => e.FuelType)
                .HasMaxLength(20)
                .HasColumnName("fuel_type");
            entity.Property(e => e.Mileage).HasColumnName("mileage");
            entity.Property(e => e.ModelName)
                .HasMaxLength(100)
                .HasColumnName("model_name");
            entity.Property(e => e.PricePerDay)
                .HasPrecision(10, 2)
                .HasColumnName("price_per_day");
            entity.Property(e => e.RegistrationNumber)
                .HasMaxLength(12)
                .HasColumnName("registration_number");
            entity.Property(e => e.Seats).HasColumnName("seats");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'available'::character varying")
                .HasComment("Статус доступности: available/unavailable")
                .HasColumnName("status");
            entity.Property(e => e.Transmission)
                .HasMaxLength(20)
                .HasColumnName("transmission");
            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .HasColumnName("vin");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.ClientId).HasName("client_pkey");

            entity.ToTable("client", "rental_schema");

            entity.HasIndex(e => e.DriverLicenseNumber, "client_driver_license_number_key").IsUnique();

            entity.HasIndex(e => e.Email, "client_email_key").IsUnique();

            entity.HasIndex(e => e.PassportNumber, "client_passport_number_key").IsUnique();

            entity.HasIndex(e => e.Phone, "client_phone_key").IsUnique();

            entity.Property(e => e.ClientId).HasColumnName("client_id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DriverLicenseNumber)
                .HasMaxLength(10)
                .HasColumnName("driver_license_number");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(10)
                .HasColumnName("passport_number");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Inspector>(entity =>
        {
            entity.HasKey(e => e.InspectorId).HasName("inspector_pkey");

            entity.ToTable("inspector", "rental_schema");

            entity.Property(e => e.InspectorId).HasColumnName("inspector_id");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .HasColumnName("firstname");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(50)
                .HasColumnName("patronymic");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("location_pkey");

            entity.ToTable("location", "rental_schema");

            entity.HasIndex(e => e.Phone, "location_phone_key").IsUnique();

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.WorkingHours)
                .HasMaxLength(50)
                .HasColumnName("working_hours");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("service_pkey");

            entity.ToTable("service", "rental_schema");

            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    // Конвертер для DateTime
    private class NpgsqlDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public NpgsqlDateTimeConverter()
            : base(
                v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v,
                v => v,
                null)
        {
        }
    }
}