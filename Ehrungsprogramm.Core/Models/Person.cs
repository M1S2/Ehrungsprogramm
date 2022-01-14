using System;
using System.Collections.Generic;
using System.Text;

namespace Ehrungsprogramm.Core.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string FirstName { get; set; }

        public DateTime EntryDate { get; set; }

        public int MembershipYears => (int)Math.Ceiling((DateTime.Now - EntryDate).TotalDays / 365);

        public int ScoreBLSV => CalculateScoreBLSV();

        public int ScoreTSV { get; set; }

        private int CalculateScoreBLSV()
        {
            return MembershipYears * 1;
        }
    }
}
