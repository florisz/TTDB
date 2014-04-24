using System;

namespace TimeTraveller.General.Patterns.Range
{
    /// <summary>
    /// Contains a range between two TimePoints
    /// </summary>
    public class TimePointRange
    {
        #region Public statics
        /// <summary>
        /// Represents an empty range in which the start is after the end of the range
        /// </summary>
        public static readonly TimePointRange EMPTY = new TimePointRange(new TimePoint(1972, 2, 2, 0, 0, 0), new TimePoint(1971, 1, 1, 0, 0, 0));

        /// <summary>
        /// Returns a range from start of time untill and including the specified TimePoint
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public static TimePointRange UpTo(TimePoint end)
        {
            return new TimePointRange(TimePoint.Past, end);
        }

        /// <summary>
        /// Returns a range from (and including) the specified TimePoint untill the end of time 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static TimePointRange StartingOn(TimePoint start)
        {
            return new TimePointRange(start, TimePoint.Future);
        }
        #endregion

        #region Private variables
        private TimePoint _start;
        private TimePoint _end;
        #endregion

        #region Constructors/Destructors
        /// <summary>
        /// Creates a range between start and end. 
        /// The created range includes the specified start and end dates.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TimePointRange(DateTime start, DateTime end)
        {
            this._start = new TimePoint(start);
            this._end = new TimePoint(end);
        }

        /// <summary>
        /// Creates a range between start and end. 
        /// The created range includes the specified start and end Timepoints.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public TimePointRange(TimePoint start, TimePoint end)
        {
            this._start = start;
            this._end = end;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// retruns the start of the range as a TimePoint.
        /// </summary>
        public TimePoint Start
        {
            get { return _start; }
            set
            {
                _start = value;
                if (_start == null)
                    _start = TimePoint.Past;
            }
        }

        /// <summary>
        /// Returns the end of the range as a TimePoint
        /// </summary>
        public TimePoint End
        {
            get { return _end; }
            set 
            { 
                _end = value;
                if (_end == null)
                    _end = TimePoint.Future;
            }
        }

        #endregion

        #region Public Functions
        /// <summary>
        /// Returns the range as an equivalent string representation. Both
        /// start and end are formatted using the short DateTime format. 
        /// </summary>
        /// <returns></returns>
        public String ToShortDateString()
        {
            // local international settings dependent
            if (this.IsEmpty())
            {
                return "Empty Date Range";
            }
            else
            {
                return _start.ToShortDateString() + " - " + _end.ToShortDateString();
            }
        }

        /// <summary>
        /// Returns the range as an equivalent string representation. Both 
        /// start and end are formatted using the specified DateTime format string.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public String ToString(string format)
        {
            if (this.IsEmpty())
            {
                return "Empty Date Range";
            }
            else
            {
                return _start.ToString(format) + " - " + _end.ToString(format);
            }
        }

        /// <summary>
        /// Returns true if the Range is empty, false otherwise.
        /// </summary>
        /// <returns>true if the Range is empty, false otherwise.</returns>
        public bool IsEmpty()
        {
            return _start.After(_end);
        }

        /// <summary>
        /// Returns true if the range includes the argument Timepoint, false otherwise. 
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool Includes(TimePoint argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            return !argument.Before(_start) && !argument.After(_end);
        }

        /// <summary>
        /// Returns true if the range equals the specified argument, false otherwise.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            TimePointRange other = obj as TimePointRange;
            if (other != null)
            {
                return _start.Equals(other.Start) && _end.Equals(other.End);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the hashcode of the base object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _start.GetHashCode();
        }

        /// <summary>
        /// Returns true of this range overlaps with the argument range, false otherwise.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool Overlaps(TimePointRange argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            return argument.Includes(_start) || argument.Includes(_end) || this.Includes(argument);
        }
        
        /// <summary>
        /// Returns true if the argument range is included in this range, false otherwise
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool Includes(TimePointRange argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            return this.Includes(argument.Start) && this.Includes(argument.End);
        }

        /// <summary>
        /// Returns a range with falss exactly between this range and the argument range,
        /// if it is not possible to construct such a range null is returned
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public TimePointRange Gap(TimePointRange argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            if (this.Overlaps(argument))
            {
                return TimePointRange.EMPTY;
            }
            
            TimePointRange lower, higher;
            
            if (this.CompareTo(argument) < 0)
            {
                lower = this;
                higher = argument;
            }
            else
            {
                lower = argument;
                higher = this;
            }
            return new TimePointRange(lower.End.AddSeconds(1), higher.Start.AddSeconds(-1));
        }
        
        /// <summary>
        /// Compare this range with the argument range and return 0 if the range value
        /// is equal to the argument value, -1 if the start of this range
        /// lies before the argument and 1 if this range lies after the argument range
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public int CompareTo(Object argument)
        {
            TimePointRange other = argument as TimePointRange;
            if (other == null)
            {
                return -1;
            }
            else if (!_start.Equals(other.Start))
            {
                return _start.CompareTo(other.Start);
            }
            else
            {
                return _end.CompareTo(other.End);
            }
        }

        /// <summary>
        /// Returns true if the argument range exactly connects with this range and
        /// false otherwise.
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool Connecting(TimePointRange argument)
        {
            return !this.Overlaps(argument) && this.Gap(argument).IsEmpty();
        }

        /// <summary>
        /// Returns true if this range is exactly partitioned by the argument ranges and
        /// false otherwise.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool PartitionedBy(TimePointRange[] args)
        {
            if (!IsContiguous(args)) 
                return false;
            
            return this.Equals(TimePointRange.Combination(args));
        }

        /// <summary>
        /// Return one range which is the combination of the consecutive ordered
        /// argument range collection.
        /// If no arguments are specified or if the argument collection does not contain a 
        /// consecutive collection of ranges an exception is thrown.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static TimePointRange Combination(TimePointRange[] args)
        {
            // test if the array is contiguous, as a side effect the  array will get sorted
            if (args == null)
            {
                throw new ArgumentNullException("Range of arguments can not be null");
            }
            if (!IsContiguous(args))
            {
                throw new ArgumentException("Unable to combine date ranges");
            }

            return new TimePointRange(args[0].Start, args[args.Length - 1].End);
        }
        
        /// <summary>
        /// Returns true if the collection of ranges specified in the argument contains
        /// an ordered consecutive set of ranges.
        /// Returns false in all other cases.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsContiguous(TimePointRange[] args)
        {
            Array.Sort<TimePointRange>(args, Comparison);

            for (int i = 0; i < args.Length - 1; i++)
            {
                if (!args[i].Connecting(args[i + 1])) return false;
            }
            return true;
        }
        #endregion
  
        #region Event Handling Functions
        #endregion

        #region Private Functions
        private static int Comparison(TimePointRange x, TimePointRange y)
        {
            return x.CompareTo(y);
        }

        #endregion
    }
}
