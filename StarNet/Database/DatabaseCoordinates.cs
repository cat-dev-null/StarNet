using System;

namespace StarNet.Database
{
    /// <summary>
    /// Coordinates owned by a server. Only defines sector and X/Y, doesn't care about planetary systems.
    /// </summary>
    public class DatabaseCoordinates
    {
        public virtual int Id { get; set; }
        public virtual int X { get; set; }
        public virtual int Y { get; set; }
        public virtual StarboundSector Sector { get; set; }
        public virtual DatabaseServer Server { get; set; }
    }
}