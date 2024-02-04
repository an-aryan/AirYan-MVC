using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Models;

public partial class AryanUser
{
    public int UserId { get; set; }
    [Required(ErrorMessage = "Please Enter a Username to proceed")]
    public string? Username { get; set; }
    [Required(ErrorMessage ="Please use a password")]
    public string? Password { get; set; }
    [NotMapped]
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public string?ConfirmPassword { get; set; }
}
public class ApiResponse
{
    public bool Success { get; set; }

}