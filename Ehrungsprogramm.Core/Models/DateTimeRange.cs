using System;
using System.Collections.Generic;
using System.Text;

namespace Ehrungsprogramm.Core.Models
{
    /// <summary>
    /// Class describing a Range between two dates
    /// </summary>
    public class DateTimeRange
    {
        /// <summary>
        /// Start of the range
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// End of the range
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Duration of the range (End - Start)
        /// </summary>
        public TimeSpan Duration => End - Start;

        /// <summary>
        /// Empty constructor of the DateTimeRange
        /// </summary>
        public DateTimeRange()
        {
        }

        /// <summary>
        /// Constructor of the DateTimeRange
        /// </summary>
        /// <param name="start">Start of the range</param>
        /// <param name="end">End of the range</param>
        public DateTimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Subtract DateTimeRange a from b and return the remaining part of b.
        /// !!! Caution: For the case "a completely part of b", this operator only returns the first range. The second range is not returned due to the nature of the operator that can only return one parameter.
        /// </summary>
        /// <param name="b">DateTimeRange from which a is subtracted</param>
        /// <param name="a">DateTimeRange that is subtracted from b</param>
        /// <returns>Remaining part of b after subtracting a from b</returns>
        public static DateTimeRange operator -(DateTimeRange b, DateTimeRange a)
        {
            return b.Subtract(a, out _);
        }

        /// <summary>
        /// Subtract DateTimeRange a from this range and return the remaining part of this range.
        /// </summary>
        /// <param name="a">DateTimeRange that is subtracted from this range</param>
        /// <param name="additionalRangeAfterSubtraction">Additional range after subtraction. Most of the time this is null. Only for "a completely part of this range" case it is not null.</param>
        /// <returns>Remaining part of this range after subtracting a from this range</returns>
        public DateTimeRange Subtract(DateTimeRange a, out DateTimeRange additionalRangeAfterSubtraction)
        {
            additionalRangeAfterSubtraction = null;
            if (a == null || this == null) { return null; }

            if(a.Start >= this.End)                                                     // Not overlapping, a after this range
            {
                // a            |------|
                // this |-----|
                return this;
            }
            else if(a.End <= this.Start)                                                // Not overlapping, this range after a
            {
                // a    |-----|
                // this         |------|
                return this;
            }
            else if (a.Start > this.Start && a.End >= this.End && a.Start < this.End)   // Overlapping, a after this range
            {
                // a        |------|
                // this |------|
                return new DateTimeRange(this.Start, a.Start);
            }
            else if (a.Start <= this.Start && a.End < this.End && a.End > this.Start)   // Overlapping, this range after a
            {
                // a    |------|
                // this     |------|
                return new DateTimeRange(a.End, this.End);
            }
            else if (a.Start <= this.Start && a.End >= this.End)                        // this range completely part of a
            {
                // a    |------------|
                // this    |------|
                return null;
            }
            else if (a.Start > this.Start && a.End < this.End)                          // a completely part of this range
            {
                // a       |------|
                // this |------------|
                additionalRangeAfterSubtraction = new DateTimeRange(a.End, this.End);
                return new DateTimeRange(this.Start, a.Start);
            }

            return null;
        }
    }
}
