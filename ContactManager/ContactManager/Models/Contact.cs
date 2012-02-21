﻿using System;
using ProtoBuf;

namespace ContactManager.Models
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string Twitter { get; set; }
        public DateTime Birthday { get; set; }        
    }
}