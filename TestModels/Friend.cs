namespace TestModels
{
    using System;
    using System.Collections.Generic;

    public class Friend
    {
        public Address Address { get; set; } = new();
        public DateTime? Birthday { get; set; }
        public List<FriendEmail> Emails { get; set; } = new();
        public string FirstName { get; set; } = string.Empty;
        public int FriendGroupId { get; set; }
        public int Id { get; set; }
        public bool IsDeveloper { get; set; }
        public string LastName { get; set; } = string.Empty;
    }
}