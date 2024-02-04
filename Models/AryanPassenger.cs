using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FlightBooking.Models;

public partial class AryanPassenger
{
    
    public int PassengerId { get; set; }
    [Required(ErrorMessage = "Please enter your first name")]
    public string? FirstName { get; set; }
    [Required(ErrorMessage ="Please enter your last name")]
    public string? LastName { get; set; }
    [Required(ErrorMessage ="Please enter your Email")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    public virtual ICollection<AryanBooking> AryanBookings { get; set; } = new List<AryanBooking>();
}
