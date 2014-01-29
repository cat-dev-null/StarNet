using System;

namespace StarNet
{
    public class Character
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual Guid UUID { get; set; }
        public virtual User User { get; set; }
    }
}

