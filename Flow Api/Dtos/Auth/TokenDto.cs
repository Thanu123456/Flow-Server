using Flow_Api.Models.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Flow_Api.Models.Entities;

namespace Flow_Api.Dtos.Auth

{
    public class TokenDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public required UserDto User { get; set; }
    }

    public class UserDto
    {
        public required string Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public System.Collections.Generic.List<string> Roles { get; set; } = new();
        public System.Collections.Generic.List<string> Permissions { get; set; } = new();
    }
}
