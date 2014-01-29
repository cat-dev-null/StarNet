using System;
using System.Collections.Generic;

namespace StarNet
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string AccountName { get; set; }
        /// <summary>
        /// Kept in plain text for now, will eventually just use the official system Chucklefish is working on.
        /// </summary>
        public virtual string Password { get; set; }
        public virtual IList<Character> Characters { get; set; }
    }
}