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

        public DateTime BirthDate { get; set; }

        public DateTime EntryDate { get; set; }

        public int MembershipYears => (int)Math.Ceiling((DateTime.Now - EntryDate).TotalDays / 365);

        public int ScoreBLSV => CalculateScoreBLSV();

        public int ScoreTSV { get; private set; }

        private List<Function> _functions;
        public List<Function> Functions 
        {
            get => _functions; 
            set { _functions = value; CalculateScoreTSV(); }
        }



        private int CalculateScoreBLSV()
        {
            return MembershipYears * 1;
        }

        private void CalculateScoreTSV()
        {
            ScoreTSV = 5;


            /*DateTimeRange a = new DateTimeRange(new DateTime(8, 1, 1), new DateTime(16, 1, 1));
            DateTimeRange b = new DateTimeRange(new DateTime(6, 1, 1), new DateTime(12, 1, 1));
            DateTimeRange d;
            DateTimeRange c = b.Subtract(a, out d);
            return;*/
        }
    }
}
