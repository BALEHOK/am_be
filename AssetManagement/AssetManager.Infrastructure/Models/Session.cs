using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AssetManager.Infrastructure.Models
{
    public class Session
    {
        public bool Authenticated { get; set; }
        public User User { get; set; }
        public string Csrf { get; set; }
    }
}