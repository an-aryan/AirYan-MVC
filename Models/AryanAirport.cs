using System;
using System.Collections.Generic;

namespace FlightBooking.Models;

public partial class AryanAirport
{
    public int AirportId { get; set; }

    public string? AirportName { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<AryanFlight> AryanFlightArrivalAirports { get; set; } = new List<AryanFlight>();

    public virtual ICollection<AryanFlight> AryanFlightDepartureAirports { get; set; } = new List<AryanFlight>();
}
