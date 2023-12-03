﻿namespace AuctriaApplication.Services.Membership.Dto;

public record struct RegisterOrLoginDto
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    public string? PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}