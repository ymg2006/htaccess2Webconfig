using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace WebConfig
{
    public sealed class RuleEntry : Entry, IWrapper
    {
        internal RuleEntry(Parser.RawEntry rawEntry) : base(rawEntry)
        {
        }

        private string ActionString
        {
            get
            {
                string text = null;
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    using (XmlWriter xmlWriter = Entry.CreateWriter(stringWriter))
                    {
                        WriteAction(xmlWriter);
                        xmlWriter.Close();
                        text = stringWriter.GetStringBuilder().ToString();
                    }
                }
                return text.Replace(" />", ">");
            }
        }

        private string ConditionsString
        {
            get
            {
                string text = null;
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    using (XmlWriter xmlWriter = Entry.CreateWriter(stringWriter))
                    {
                        WriteConditionsStartElement(xmlWriter);
                        xmlWriter.Close();
                        text = stringWriter.GetStringBuilder().ToString();
                    }
                }
                return text.Replace(" />", ">");
            }
        }

        private string MatchString
        {
            get
            {
                string text = null;
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    using (XmlWriter xmlWriter = Entry.CreateWriter(stringWriter))
                    {
                        WriteMatch(xmlWriter);
                        xmlWriter.Close();
                        text = stringWriter.GetStringBuilder().ToString();
                    }
                }
                return text.Replace(" />", ">");
            }
        }

        public IEntryNode TreeNode
        {
            get
            {
                if (_treeNode == null)
                {
                    _treeNode = base.CreateEntryNode(this);
                    if (!base.HasErrorInfo)
                    {
                        IEntryNode node = base.CreateEntryNode(this, MatchString);
                        _treeNode.AddChild(node);
                        if (HasConditions)
                        {
                            IEntryNode entryNode = base.CreateEntryNode(this, ConditionsString);
                            _treeNode.AddChild(entryNode);
                            foreach (ConditionEntry conditionEntry in Conditions)
                            {
                                entryNode.AddChild(conditionEntry.TreeNode);
                            }
                            if (HasErrorInConditions)
                            {
                                entryNode.SetImageSelected(1);
                                _treeNode.SetImageSelected(1);
                            }
                        }
                        IEntryNode entryNode2 = base.CreateEntryNode(this);
                        entryNode2.SetText(ActionString);
                        _treeNode.AddChild(entryNode2);
                    }
                    _treeNode.Expand();
                }
                return _treeNode;
            }
        }

        internal void UpdateTreeNode()
        {
            TreeNode.SetText(base.DisplayString);
        }

        private void WriteAction(XmlWriter writer)
        {
            writer.WriteStartElement("action");
            writer.WriteAttributeString("type", ActionType.ToString());
            if (ActionType != ActionType.None)
            {
                if (ActionType == ActionType.Redirect)
                {
                    writer.WriteAttributeString("redirectType", ActionRedirectType.ToString());
                }
                if (!string.IsNullOrEmpty(ActionUrl))
                {
                    writer.WriteAttributeString("url", ActionUrl);
                }
                if (ActionAppendQueryStringSet)
                {
                    writer.WriteAttributeString("appendQueryString", ActionAppendQueryString ? "true" : "false");
                }
                if (StatusCode != 0L)
                {
                    writer.WriteAttributeString("statusCode", StatusCode.ToString(CultureInfo.InvariantCulture));
                }
                if (SubStatusCode != 0L)
                {
                    writer.WriteAttributeString("subStatusCode", SubStatusCode.ToString(CultureInfo.InvariantCulture));
                }
                if (StatusReason != null)
                {
                    writer.WriteAttributeString("statusReason", StatusReason);
                }
                if (StatusDescription != null)
                {
                    writer.WriteAttributeString("statusDescription", StatusDescription);
                }
            }
            writer.WriteEndElement();
        }

        private void WriteConditionsStartElement(XmlWriter writer)
        {
            writer.WriteStartElement("conditions");
            if (ConditionsLogicalGrouping != LogicalGrouping.MatchAll)
            {
                writer.WriteAttributeString("logicalGrouping", ConditionsLogicalGrouping.ToString());
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
            writer.WriteStartElement("rule");
            writer.WriteAttributeString("name", Name);
            if (StopProcessing)
            {
                writer.WriteAttributeString("stopProcessing", "true");
            }
            if (!Enabled)
            {
                writer.WriteAttributeString("enabled", "false");
            }
        }

        private void WriteMatch(XmlWriter writer)
        {
            writer.WriteStartElement("match");
            if (!string.IsNullOrEmpty(MatchUrl))
            {
                writer.WriteAttributeString("url", MatchUrl);
            }
            if (!MatchIgnoreCase)
            {
                writer.WriteAttributeString("ignoreCase", MatchIgnoreCase ? "true" : "false");
            }
            if (MatchNegate)
            {
                writer.WriteAttributeString("negate", "true");
            }
            writer.WriteEndElement();
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (base.HasErrorInfo)
            {
                WriteDisplayString(writer);
                return;
            }
            base.WriteComments(writer);
            WriteDisplayString(writer);
            WriteMatch(writer);
            if (HasConditions)
            {
                WriteConditionsStartElement(writer);
                foreach (ConditionEntry conditionEntry in Conditions)
                {
                    conditionEntry.WriteTo(writer);
                }
                writer.WriteEndElement();
            }
            WriteAction(writer);
            writer.WriteEndElement();
        }

        public RuleEntry()
        {
        }

        public bool ActionAppendQueryString
        {
            get
            {
                return (bool)Data[4];
            }
            set
            {
                Data[4] = value;
                ActionAppendQueryStringSet = true;
            }
        }

        public bool ActionAppendQueryStringSet
        {
            get
            {
                return (bool)Data[5];
            }
            private set
            {
                Data[5] = value;
            }
        }

        internal RedirectType ActionRedirectType
        {
            get
            {
                return (RedirectType)Data[13];
            }
            set
            {
                Data[13] = (int)value;
            }
        }

        internal ActionType ActionType
        {
            get
            {
                return (ActionType)Data[12];
            }
            set
            {
                Data[12] = (int)value;
            }
        }

        public string ActionUrl
        {
            get
            {
                return (string)Data[3];
            }
            set
            {
                Data[3] = value;
            }
        }

        internal IList<ConditionEntry> Conditions
        {
            get
            {
                if (_conditions == null)
                {
                    _conditions = new CollectionWrapper<ConditionEntry>((ArrayList)Data[15]);
                }
                return _conditions;
            }
        }

        internal LogicalGrouping ConditionsLogicalGrouping
        {
            get
            {
                return (LogicalGrouping)Data[14];
            }
            set
            {
                Data[14] = (int)value;
            }
        }

        private object[] Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new object[17];
                    _data[1] = false;
                    _data[2] = false;
                    _data[4] = false;
                    _data[5] = false;
                    _data[6] = false;
                    _data[12] = 0;
                    _data[13] = 302;
                    _data[14] = 0;
                    _data[8] = 0L;
                    _data[9] = 0L;
                    _data[16] = true;
                }
                return _data;
            }
        }

        public bool Enabled
        {
            get
            {
                return (bool)Data[16];
            }
            set
            {
                Data[16] = value;
            }
        }

        public bool HasConditions
        {
            get
            {
                return Conditions.Count > 0;
            }
        }

        public bool MatchIgnoreCase
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

        public bool MatchNegate
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

        public string MatchUrl
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

        public string Name
        {
            get
            {
                return (string)Data[7];
            }
            set
            {
                Data[7] = value;
            }
        }

        public long StatusCode
        {
            get
            {
                return (long)Data[8];
            }
            set
            {
                Data[8] = value;
            }
        }

        public string StatusDescription
        {
            get
            {
                return (string)Data[11];
            }
            set
            {
                Data[11] = value;
            }
        }

        public string StatusReason
        {
            get
            {
                return (string)Data[10];
            }
            set
            {
                Data[10] = value;
            }
        }

        public bool StopProcessing
        {
            get
            {
                return (bool)Data[6];
            }
            set
            {
                Data[6] = value;
            }
        }

        public long SubStatusCode
        {
            get
            {
                return (long)Data[9];
            }
            set
            {
                Data[9] = value;
            }
        }

        public object GetData()
        {
            if (_conditions != null)
            {
                _data[15] = _conditions.GetData();
            }
            return _data;
        }

        public void SetData(object o)
        {
            _data = (object[])o;
        }

        //private const int IndexUrl = 0;

        //private const int IndexIgnoreCase = 1;

        //private const int IndexNegate = 2;

        //private const int IndexActionUrl = 3;

        //private const int IndexActionAppendQueryString = 4;

        //private const int IndexActionAppendQueryStringSet = 5;

        //private const int IndexStopProcessing = 6;

        //private const int IndexName = 7;

        //private const int IndexStatusCode = 8;

        //private const int IndexSubStatusCode = 9;

        //private const int IndexStatusReason = 10;

        //private const int IndexStatusDescription = 11;

        //private const int IndexActionType = 12;

        //private const int IndexActionRedirectType = 13;

        //private const int IndexLogicalGrouping = 14;

        //private const int IndexConditions = 15;

        //private const int IndexEnabled = 16;

        private IEntryNode _treeNode;

        public bool IsChained;

        public bool HasErrorInConditions;

        private object[] _data;

        private CollectionWrapper<ConditionEntry> _conditions;
    }

}
