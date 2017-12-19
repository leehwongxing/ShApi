using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DTO.Databases
{
    public class Promotion : Owned
    {
        [BsonId]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public HashSet<int> DaysOfWeek { get; set; }

        public bool Shown { get; set; }

        public float Score { get { return Scored(); } }

        public Promotion() : base()
        {
            Id = Generator.Id();
            Name = "";
            Description = "";
            StartDate = DateTime.UtcNow;
            EndDate = DateTime.UtcNow;
            StartTime = StartDate.TimeOfDay;
            EndTime = EndDate.TimeOfDay;
            DaysOfWeek = new HashSet<int>();
            Shown = true;
            Group = "PROMOTION";
        }

        protected float Scored()
        {
            var Days = (EndDate - StartDate).Days;
            var TimeDiff = EndTime - StartTime;
            var AffectedTime = TimeDiff.Hours + (TimeDiff.Minutes >= 30 ? 1 : 0);

            if (Days == 0 && AffectedTime == 0)
            {
                return 0;
            }
            return (Shown ? 1 : -1) * 24 / (float)(24 * Days + AffectedTime);
        }
    }
}