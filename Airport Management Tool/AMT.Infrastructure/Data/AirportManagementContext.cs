using System;
using System.Collections.Generic;
using AMT.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AMT.Infrastructure.Data;

public partial class AirportManagementContext : IdentityDbContext<IdentityUser>
{
    public AirportManagementContext()
    {
    }

    public AirportManagementContext(DbContextOptions<AirportManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aircraft> Aircraft { get; set; }

    public virtual DbSet<Airline> Airlines { get; set; }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<FlightSchedule> FlightSchedules { get; set; }

    public virtual DbSet<Gate> Gates { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Aircraft>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Aircraft__3214EC07778B447D");

            entity.HasIndex(e => e.TailNumber, "UQ__Aircraft__3F41D11B72565A18").IsUnique();

            entity.Property(e => e.Model).HasMaxLength(60);
            entity.Property(e => e.TailNumber).HasMaxLength(10);

            entity.HasOne(d => d.OwnedByAirline).WithMany(p => p.Aircraft)
                .HasForeignKey(d => d.OwnedByAirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Aircraft__OwnedB__403A8C7D");
        });

        modelBuilder.Entity<Airline>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Airline__3214EC07EF6E5C3F");

            entity.ToTable("Airline");

            entity.HasIndex(e => e.Iatacode, "UQ__Airline__EFD6F5BEFD85E894").IsUnique();

            entity.Property(e => e.Iatacode)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("IATACode");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Airport__3214EC076B71CB82");

            entity.ToTable("Airport");

            entity.HasIndex(e => e.Iatacode, "UQ__Airport__EFD6F5BEF98086B4").IsUnique();

            entity.Property(e => e.City).HasMaxLength(80);
            entity.Property(e => e.Country).HasMaxLength(80);
            entity.Property(e => e.Iatacode)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("IATACode");
            entity.Property(e => e.Name).HasMaxLength(120);
            entity.Property(e => e.Timezone).HasMaxLength(64);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Booking__3214EC07CA100208");

            entity.ToTable("Booking");

            entity.HasIndex(e => e.ConfirmationCode, "UQ__Booking__1968308669084D11").IsUnique();

            entity.Property(e => e.ConfirmationCode).HasMaxLength(8);
            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PassagerEmail).HasMaxLength(120);
            entity.Property(e => e.PassagerFullName).HasMaxLength(120);

            entity.HasOne(d => d.Flight).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__FlightI__4F7CD00D");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Booking__TicketI__5070F446");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Flight__3214EC0713B6FD16");

            entity.ToTable("Flight");

            entity.Property(e => e.FlightNumber).HasMaxLength(8);
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Airline).WithMany(p => p.Flights)
                .HasForeignKey(d => d.AirlineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__AirlineI__440B1D61");

            entity.HasOne(d => d.DefaultAircraft).WithMany(p => p.Flights)
                .HasForeignKey(d => d.DefaultAircraftId)
                .HasConstraintName("FK__Flight__DefaultA__44FF419A");

            entity.HasOne(d => d.DestinationAirport).WithMany(p => p.FlightDestinationAirports)
                .HasForeignKey(d => d.DestinationAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__Destinat__46E78A0C");

            entity.HasOne(d => d.OriginAirport).WithMany(p => p.FlightOriginAirports)
                .HasForeignKey(d => d.OriginAirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Flight__OriginAi__45F365D3");
        });

        modelBuilder.Entity<FlightSchedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__FlightSc__3214EC0727117A60");

            entity.ToTable("FlightSchedule");

            entity.HasOne(d => d.AssignedAircraft).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.AssignedAircraftId)
                .HasConstraintName("FK__FlightSch__Assig__5535A963");

            entity.HasOne(d => d.Flight).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FlightSch__Fligh__534D60F1");

            entity.HasOne(d => d.Gate).WithMany(p => p.FlightSchedules)
                .HasForeignKey(d => d.GateId)
                .HasConstraintName("FK__FlightSch__GateI__5441852A");
        });

        modelBuilder.Entity<Gate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Gate__3214EC07CE6081CF");

            entity.ToTable("Gate");

            entity.Property(e => e.Code).HasMaxLength(10);

            entity.HasOne(d => d.Airport).WithMany(p => p.Gates)
                .HasForeignKey(d => d.AirportId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Gate__AirportId__398D8EEE");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ticket__3214EC079AEE68C4");

            entity.ToTable("Ticket");

            entity.Property(e => e.BasePrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.Property(e => e.FareClass).HasMaxLength(2);
            entity.Property(e => e.IsRefundable).HasDefaultValue(false);
            entity.Property(e => e.Taxes).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([BasePrice]+[Taxes])", true)
                .HasColumnType("decimal(11, 2)");

            entity.HasOne(d => d.Flight).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.FlightId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__FlightId__4AB81AF0");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
