using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Projection
{
    public class Promotion
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public HashSet<int> DaysOfWeek { get; set; }

        public HashSet<string> HiddenList { get; set; }

        public HashSet<string> ShownList { get; set; }

        public Promotion()
        {
            Name = "";
            Description = "";
            StartDate = DateTime.UtcNow;
            EndDate = DateTime.UtcNow;
            StartTime = StartDate.TimeOfDay;
            EndTime = EndDate.TimeOfDay;
            DaysOfWeek = new HashSet<int>();
            HiddenList = new HashSet<string>();
            ShownList = new HashSet<string>();
        }
    }
}