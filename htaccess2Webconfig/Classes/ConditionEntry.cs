using System;
using System.Xml;

namespace WebConfig
{
    internal sealed class ConditionEntry : Entry, IWrapper
    {
        public ConditionEntry(Parser.RawEntry rawEntry) : base(rawEntry)
        {
        }

        internal IEntryNode TreeNode
        {
            get
            {
                if (_treeNode == null)
                {
                    _treeNode = base.CreateEntryNode(this);
                }
                return _treeNode;
            }
        }

        protected override void WriteDisplayString(XmlWriter writer)
        {
            if (base.HasErrorInfo)
            {
                writer.WriteComment(ErrorInfo.Message);
                return;
            }
            if (base.HasWarningInfo)
            {
                writer.WriteComment(ErrorInfo.Message);
            }
            writer.WriteStartElement("add");
            if (!string.IsNullOrEmpty(Input))
            {
                writer.WriteAttributeString("input", Input);
            }
            if (!string.IsNullOrEmpty(Pattern))
            {
                writer.WriteAttributeString("pattern", Pattern);
            }
            if (MatchType != MatchType.Pattern)
            {
                writer.WriteAttributeString("matchType", MatchType.ToString());
            }
            if (!IgnoreCase)
            {
                writer.WriteAttributeString("ignoreCase", "false");
            }
            if (Negate)
            {
                writer.WriteAttributeString("negate", "true");
            }
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (base.HasErrorInfo)
            {
                WriteDisplayString(writer);
                return;
            }
            if (base.HasWarningInfo)
            {
                writer.WriteComment(ErrorInfo.Message);
            }
            base.WriteComments(writer);
            WriteDisplayString(writer);
            writer.WriteEndElement();
        }

        public ConditionEntry()
        {
        }

        private object[] Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new object[6];
                    _data[1] = false;
                    _data[2] = false;
                    _data[3] = false;
                    _data[5] = 0;
                }
                return _data;
            }
        }

        public bool IgnoreCase
        {
            get
            {
                return (bool)Data[1];
            }
            set
            {
                Data[1] = value;
            }
        }

        public string Input
        {
            get
            {
                return (string)Data[0];
            }
            set
            {
                Data[0] = value;
            }
        }

        public bool IsOr
        {
            get
            {
                return (bool)Data[3];
            }
            set
            {
                Data[3] = value;
            }
        }

        public MatchType MatchType
        {
            get
            {
                return (MatchType)Data[5];
            }
            set
            {
                Data[5] = (int)value;
            }
        }

        public bool Negate
        {
            get
            {
                return (bool)Data[2];
            }
            set
            {
                Data[2] = value;
            }
        }

        public string Pattern
        {
            get
            {
                return (string)Data[4];
            }
            set
            {
                Data[4] = value;
            }
        }

        public object GetData()
        {
            return _data;
        }

        public void SetData(object o)
        {
            _data = (object[])o;
        }

        //private const int IndexInput = 0;

        //private const int IndexIgnoreCase = 1;

        //private const int IndexNegate = 2;

        //private const int IndexIsOr = 3;

        //private const int IndexPattern = 4;

        //private const int IndexMatchType = 5;

        private IEntryNode _treeNode;

        private object[] _data;
    }

}
