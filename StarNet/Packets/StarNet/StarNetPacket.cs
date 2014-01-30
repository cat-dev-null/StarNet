using System;
using System.IO;
using System.Net;

namespace StarNet
{
    public abstract class StarNetPacket
    {
        public StarNetPacket()
        {
            AssignTransactionId = true;
            Retries = 0;
        }

        public int Retries { get; set; }
        public IPEndPoint Destination { get; set; }
        public DateTime ScheduledRetry { get; set; }
        public bool AssignTransactionId { get; set; }
        private uint _Transaction;
        public uint Transaction
        {
            get
            {
                return _Transaction;
            }
            set
            {
                AssignTransactionId = false;
                _Transaction = value;
            }
        }
        public abstract MessageFlags Flags { get; }
        public abstract byte PacketId { get; }
        public abstract void Read(BinaryReader stream);
        public abstract void Write(BinaryWriter stream);

        public event EventHandler ConfirmationReceived;
        protected internal virtual void OnConfirmationReceived()
        {
            if (ConfirmationReceived != null)
                ConfirmationReceived(this, null);
        }
    }
}