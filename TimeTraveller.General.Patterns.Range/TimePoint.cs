using System;

namespace TimeTraveller.General.Patterns.Range
{
    /// <summary>
    /// The Timepoint class divides time into discrete units so it can be used
    /// in ranges.
    /// In this implementation the time is divided into seconds.
    /// </summary>
    public class TimePoint
    {
        #region Public statics
        public static readonly TimePoint Past = new TimePoint(DateTime.MinValue);
        public static readonly TimePoint Future = new TimePoint(DateTime.MaxValue);

        public static TimePoint Now
        {
            get { return new TimePoint(DateTime.Now); }
        }
        #endregion

        #region Private variables
        private DateTime _timeValue;
        #endregion

        #region Constructors/Destructors
        /// <summary>
        /// Specify all parameters of the timepoint separately
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        public TimePoint(int year, int month, int day, int hour, int minute, int second)
        {
            _timeValue = TrimToSeconds(new DateTime(year, month, day, hour, minute, second));
        }

        /// <summary>
        /// Specify the current time in the native DateTime format
        /// </summary>
        /// <param name="argument"></param>
        public TimePoint(DateTime argument)
        {
            _timeValue = TrimToSeconds(argument);
        }

        /// <summary>
        /// The current time will be used as value for the TimePoint
        /// </summary>
        public TimePoint()
        {
            _timeValue = TrimToSeconds(DateTime.Now);
        }

        /// <summary>
        /// Specify the time as a string; suported formats are: "d-M-yyyy", "d-M-yy", "dd-MM-yyyy",
        ///                     "dd-MM-yy", "dd MMM yyyy", "d MMM yyyy", "dd MMMM yyyy", "d MMMM yyyy"
        /// </summary>
        /// <param name="dateString"></param>
        public TimePoint(string timeString)
        {
            _timeValue = StringToDate(timeString);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Value of the TimePoint in DateTime format trimmed to seconds
        /// </summary>
        public DateTime TimeValue
        {
            get { return _timeValue; }
            set 
            { 
                _timeValue = value;
                _timeValue = TrimToSeconds(_timeValue); 
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Returns true if the Timepoint is after the specified argument value
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool After(TimePoint argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("Timepoint argument in After method can not be null");
            }
            return (_timeValue > argument.TimeValue);
        }

        /// <summary>
        /// Returns true if the Timepoint is before the specified argument value
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public bool Before(TimePoint argument)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("Timepoint argument in Before method can not be null");
            }
            return (_timeValue < argument.TimeValue);
        }

        /// <summary>
        /// Returns 0 if the Timepoint value is equal to the argument value,
        /// -1 if it lies before the argument and 1 if it lies after the argument
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public int CompareTo(TimePoint argument)
        {
            if (argument != null)
            {
                return (_timeValue.CompareTo(argument.TimeValue));
            }
            else
            {
                return 1;
            }
        }
        
        /// <summary>
        /// Returns true if the Timepoint value contains the same value as the argument
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }

		    // pointing to the same object instance means equal
            if (ReferenceEquals(this, obj)) 
                return true;

            // test if the argument is of the same type
            if (this.GetType() != obj.GetType()) 
                return false;
		    
            TimePoint other = (TimePoint) obj;
            return (other.CompareTo(this) == 0);
	    }

        /// <summary>
        /// Add seconds to the TimePoint specified by the integer argument
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public TimePoint AddSeconds(int argument)
        {
            return new TimePoint(_timeValue.AddSeconds(argument));
        }

        /// <summary>
        /// Subtract seconds from the TimePoint specified by the integer argument
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        public TimePoint MinusSeconds(int argument)
        {
            return this.AddSeconds(-argument);
        }

        /// <summary>
        /// Returns the TimePoint value as its equivalent short string representation
        /// </summary>
        /// <returns></returns>
        public string ToShortDateString()
        {
            return _timeValue.ToShortDateString();
        }

        /// <summary>
        /// Returns the TimePoint value as its equivalent string representation
        /// specified by the format string
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return _timeValue.ToString(format);
        }

        /// <summary>
        /// Returns the hashcode of the base object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region Object methods
        public override string ToString()
        {
            if (TimeValue != null)
            {
                return TimeValue.ToString("O");
            }
            else
            {
                return base.ToString();
            }
        }
        #endregion

        #region Private Functions
        private static DateTime TrimToSeconds(DateTime dateTime)
        {
            DateTime result = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0);
            return result;
        }

        /// <summary>
        /// Convert string to date
        /// </summary>
        /// <param name="dateAsSring">The date to convert.</param>
        /// <returns>The converted string.</returns>
        private static DateTime StringToDate(string dateAsString)
        {
            try
            {
                string[] supportedFormats = {"d-M-yyyy", "d-M-yy", 
                                             "dd-MM-yyyy", "dd-MM-yy", 
                                             "dd MMM yyyy", "d MMM yyyy",
                                             "dd MMMM yyyy", "d MMMM yyyy"
                                             };
                DateTime result;

                switch (dateAsString)
                {
                    case "PAST":
                        result = DateTime.MinValue;
                        break;
                    case "FUTURE":
                        result = DateTime.MaxValue;
                        break;
                    default:
                        result = DateTime.ParseExact(dateAsString,
                                                                        supportedFormats,
                                                                        System.Globalization.CultureInfo.CurrentCulture,
                                                                        System.Globalization.DateTimeStyles.None);
                        break;
                }
                
                return result;
            }
            catch (Exception exception)
            {
                // used to be logging here
                throw exception;
            }
        }

        #endregion
    }
}
