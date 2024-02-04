using System;
using System.Collections.Generic;

namespace FlightBooking.Models;

public partial class AryanFlight
{
    public int FlightId { get; set; }

    public int? DepartureAirportId { get; set; }

    public int? ArrivalAirportId { get; set; }

    public DateTime? DepartureTime { get; set; }

    public DateTime? ArrivalTime { get; set; }

    public string? AirlineName { get; set; }

    public virtual AryanAirport? ArrivalAirport { get; set; }

    public virtual ICollection<AryanBooking> AryanBookings { get; set; } = new List<AryanBooking>();

    public virtual AryanAirport? DepartureAirport { get; set; }
}
public class FlightSearchViewModel
{
    public int SourceAirportId { get; set; }
    public int DestinationAirportId { get; set; }
}
