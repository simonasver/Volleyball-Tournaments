﻿namespace Backend.Data.Dtos.Team;

public class AddTeamDto
{
    public string Title { get; set; }
    public string? PictureUrl { get; set; }
    public string? Description { get; set; }
}