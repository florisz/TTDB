using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using TimeTraveller.Services.Data.Interfaces;
using TimeTraveller.General.Patterns.Range;
using TimeTraveller.Services.Interfaces;

namespace TimeTraveller.Services.Items
{
    public abstract class AbstractItem: IItem
    {
        #region Public Properties
        public const string HistoryQueryArgument = "history=true";
        public const string SummaryQueryArgument = "summary=true";
        #endregion

        #region Abstract Members
        [XmlIgnore()]
        public abstract string SelfUri { get; set; }
        #endregion

        #region IItem Members
        [XmlIgnore()]
        public IBaseObjectType BaseObjectType
        {
            get
            {
                return BaseObjectValue.Parent.Type;
            }
        }

        [XmlIgnore()]
        public IBaseObjectValue BaseObjectValue { get; set; }

        [XmlIgnore()]
        public string ExtId
        {
            get
            {
                return BaseObjectValue.Parent.ExtId;
            }
        }

        [XmlIgnore()]
        public Guid Id
        {
            get
            {
                return BaseObjectValue.Id;
            }
        }

        [XmlIgnore()]
        public ItemType Type
        {
            get
            {
                return (ItemType)BaseObjectValue.Parent.Type.Id;
            }
        }

        [XmlIgnore()]
        public int Version
        {
            get
            {
                return BaseObjectValue.Version;
            }
        }

        public string FormatUri(Uri baseUri, params string[] uriParts)
        {
            StringBuilder result = new StringBuilder(baseUri.ToString());
            foreach (string uriPart in uriParts)
            {
                result.Append(uriPart);
            }
            return result.ToString();
        }

        public IJournalEntry[] GetHistory()
        {
            return GetHistory(TimePoint.Past, TimePoint.Future);
        }
        
        public IJournalEntry[] GetHistory(TimePoint from)
        {
            return GetHistory(from, TimePoint.Future);
        }

        public IJournalEntry[] GetHistory(TimePoint from, TimePoint to)
        {
            throw new NotImplementedException();
        }

        public string GetUri(Uri baseUri, UriType uriType, params string[] additionalQueryArguments)
        {
            return GetUri(baseUri, uriType, BaseObjectValue.Version, additionalQueryArguments);
        }

        public string GetUri(Uri baseUri, UriType uriType, int versionNumber, params string[] additionalQueryArguments)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}{1}{2}", baseUri, BaseObjectType.RelativeUri, ExtId);

            List<string> queryArguments = new List<string>();
            switch (uriType)
            {
                case UriType.Exact:
                    queryArguments.Add(string.Format("uuid={0}", BaseObjectValue.Id));
                    break;
                case UriType.TimePoint:
                    queryArguments.Add(string.Format("timepoint={0}", BaseObjectValue.Start.ToString("O")));
                    break;
                case UriType.Version:
                    queryArguments.Add(string.Format("version={0}", versionNumber));
                    break;
            }

            if (additionalQueryArguments != null && additionalQueryArguments.Length > 0)
            {
                queryArguments.AddRange(additionalQueryArguments);
            }
            bool isFirstArgument = true;
            foreach (string queryArgument in queryArguments)
            {
                if (isFirstArgument)
                {
                    result.Append("?");
                }
                else
                {
                    result.Append("&");
                }
                result.Append(queryArgument);
                isFirstArgument = false;
            }

            return result.ToString();
        }
        #endregion
    }
}
