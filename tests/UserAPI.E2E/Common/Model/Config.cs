﻿namespace UserAPI.Host.IntegrationTests.Common.Model
{
    public class Config
    {
        public ApiLink Api { get; set; }
        public TestData TestData { get; set; }
    }
    
    public class ApiLink
    {
        public string UserAPI { get; set; }
    }
    
    public class TestData
    {
        public User ValidUser { get; set; }
        public User InvalidUser { get; set; }
    }
    
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}