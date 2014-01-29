using System;
using System.Net;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace StarNet.Database
{
    public class DatabaseServer
    {
        public virtual int Id { get; protected set; }
        public virtual Guid UUID { get; protected set; }
        public virtual uint IPv4Address { get; set; }
        public virtual ushort Port { get; set; }
        public virtual IList<DatabaseCoordinates> Coordinates { get; protected set; }
        public virtual string Password { get; set; }

        public DatabaseServer()
        {
            Coordinates = new List<DatabaseCoordinates>();
            UUID = Guid.NewGuid();
        }

        public DatabaseServer(Guid uuid, IPAddress ipAddress, ushort port) : this()
        {
            UUID = uuid;
            IPv4Address = BitConverter.ToUInt32(ipAddress.GetAddressBytes(), 0);
            Port = port;
        }
    }
}