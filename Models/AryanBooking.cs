using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models;

public partial class AryanBooking
{
    public int BookingId { get; set; }

    public int? FlightId { get; set; }

    public int PassengerId { get; set; }

    public DateTime? BookingDate { get; set; }

    public virtual AryanFlight? Flight { get; set; }

    public virtual AryanPassenger? Passenger { get; set; }
}
public class BookingViewModel
{
    public int SelectedFlightId { get; set; }
    public List<AryanFlight> AvailableFlights { get; set; }
    public AryanBooking BookingDetails { get; set; }
}
public class ConfirmationViewModel
{
    public AryanFlight? FlightDetails { get; set; }
}
public class DetailedViewModel
{
    public AryanBooking? DetailBooking { get; set; }
}
public class CustomerBookingsViewModel
{
    [Required(ErrorMessage = "Please enter a valid email address.")]
    public string EmailAddress { get; set; }
    public List<AryanPassenger> Passengers { get; set; }
    public List<AryanBooking> Bookings { get; set; }
}
