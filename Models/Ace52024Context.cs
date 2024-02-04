using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Models;

public partial class Ace52024Context : DbContext
{
    public Ace52024Context()
    {
    }

    public Ace52024Context(DbContextOptions<Ace52024Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AryanAirport> AryanAirports { get; set; }

    public virtual DbSet<AryanBooking> AryanBookings { get; set; }

    public virtual DbSet<AryanFlight> AryanFlights { get; set; }

    public virtual DbSet<AryanPassenger> AryanPassengers { get; set; }

    public virtual DbSet<AryanUser> AryanUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DEVSQL.Corp.local;Database=ACE 5- 2024;Trusted_Connection=True;encrypt = false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AryanAirport>(entity =>
        {
            entity.HasKey(e => e.AirportId).HasName("PK__AryanAir__E3DBE08A60853A3D");

            entity.Property(e => e.AirportId)
                .ValueGeneratedNever()
                .HasColumnName("AirportID");
            entity.Property(e => e.AirportName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AryanBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__AryanBoo__73951ACD4E310024");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.FlightId).HasColumnName("FlightID");
            entity.Property(e => e.PassengerId).HasColumnName("PassengerID");

            entity.HasOne(d => d.Flight).WithMany(p => p.AryanBookings)
                .HasForeignKey(d => d.FlightId)
                .HasConstraintName("FK__AryanBook__Fligh__2942188C");

            entity.HasOne(d => d.Passenger).WithMany(p => p.AryanBookings)
                .HasForeignKey(d => d.PassengerId)
                .HasConstraintName("FK__AryanBook__Passe__2A363CC5");
        });

        modelBuilder.Entity<AryanFlight>(entity =>
        {
            entity.HasKey(e => e.FlightId).HasName("PK__AryanFli__8A9E148EE5F38784");

            entity.Property(e => e.FlightId)
                .ValueGeneratedNever()
                .HasColumnName("FlightID");
            entity.Property(e => e.AirlineName)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ArrivalAirportId).HasColumnName("ArrivalAirportID");
            entity.Property(e => e.ArrivalTime).HasColumnType("datetime");
            entity.Property(e => e.DepartureAirportId).HasColumnName("DepartureAirportID");
            entity.Property(e => e.DepartureTime).HasColumnType("datetime");

            entity.HasOne(d => d.ArrivalAirport).WithMany(p => p.AryanFlightArrivalAirports)
                .HasForeignKey(d => d.ArrivalAirportId)
                .HasConstraintName("FK__AryanFlig__Arriv__0BE6BFCF");

            entity.HasOne(d => d.DepartureAirport).WithMany(p => p.AryanFlightDepartureAirports)
                .HasForeignKey(d => d.DepartureAirportId)
                .HasConstraintName("FK__AryanFlig__Depar__0AF29B96");
        });

        modelBuilder.Entity<AryanPassenger>(entity =>
        {
            entity.HasKey(e => e.PassengerId).HasName("PK__AryanPas__88915F90ABC9253D");

            entity.Property(e => e.PassengerId).HasColumnName("PassengerID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AryanUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__AryanUse__1788CCACD4F92309");

            entity.ToTable("AryanUser");

            entity.HasIndex(e => e.Username, "UQ__AryanUse__536C85E42B48B093").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
