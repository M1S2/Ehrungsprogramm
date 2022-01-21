using System;
using System.Collections.Generic;
using System.Text;

namespace Ehrungsprogramm.Core.Models
{
    public class Function
    {
        public int Id { get; set; }

        public FunctionType Type { get; set; }

        public string Description { get; set; }

        public DateTimeRange TimePeriod { get; set; }

        public int FunctionYears => (int)Math.Ceiling(TimePeriod.Duration.TotalDays / 365);

        public Function()
        {
            TimePeriod = new DateTimeRange();
        }
    }
}
