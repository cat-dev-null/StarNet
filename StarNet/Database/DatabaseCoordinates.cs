using System;

namespace StarNet.Database
{
    public class DatabaseCoordinates
    {
        public virtual int Id { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public virtual DatabaseServer Server { get; set; }
    }
}