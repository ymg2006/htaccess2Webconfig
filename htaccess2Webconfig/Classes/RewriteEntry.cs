using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace WebConfig
{
    public sealed class RewriteEntry : Entry, IWrapper
    {
        internal RewriteEntry(Parser.RawEntry rawEntry, bool isServerLevel) : base(rawEntry)
        {
            _isServerLevel = isServerLevel;
        }

        public bool Enabled
        {
            get
            {
                return (bool)Data[0];
            }
            set
            {
                Data[0] = value;
                EnabledSet = true;
            }
        }

        public bool HasUnsupportedDirectives
        {
            get
            {
                return _unsupportedDirectives != null && _unsupportedDirectives.Count > 0;
            }
        }

        private string RulesNodeName
        {
            get
            {
                if (_isServerLevel)
                {
                    return "globalRules";
                }
                return "rules";
            }
        }

        public IEntryNode TreeNode
        {
            get
            {
                if (_treeNode == null)
                {
                    _treeNode = base.CreateEntryNode(this);
                    InitializeEntryNode(_treeNode);
                }
                return _treeNode;
            }
        }

        private void InitializeEntryNode(IEntryNode treeNode)
        {
            if (HasUnsupportedDirectives)
            {
                foreach (UnsupportedDirectiveEntry unsupportedDirectiveEntry in UnsupportedDirectives)
                {
                    treeNode.AddChild(unsupportedDirectiveEntry.TreeNode);
                }
            }
            if (Rules.Count > 0)
            {
                IEntryNode entryNode = base.CreateEntryNode(this, "<" + RulesNodeName + ">");
                treeNode.AddChild(entryNode);
                foreach (RuleEntry ruleEntry in Rules)
                {
                    entryNode.AddChild(ruleEntry.TreeNode);
                }
                entryNode.Expand();
            }
            treeNode.Expand();
        }

        public string GetXml()
        {
            string result;
            using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                using (XmlWriter xmlWriter = Entry.CreateWriter(stringWriter))
                {
                    WriteTo(xmlWriter);
                    xmlWriter.Close();
                    result = stringWriter.GetStringBuilder().ToString();
                }
            }
            return result;
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
            writer.WriteStartElement("rewrite");
            //bool enabledSet = EnabledSet;
        }

        public override void WriteTo(XmlWriter writer)
        {
            WriteDisplayString(writer);
            if (base.HasErrorInfo)
            {
                return;
            }
            if (base.HasWarningInfo)
            {
                writer.WriteComment(ErrorInfo.Message);
            }
            if (HasUnsupportedDirectives)
            {
                foreach (UnsupportedDirectiveEntry unsupportedDirectiveEntry in UnsupportedDirectives)
                {
                    unsupportedDirectiveEntry.WriteTo(writer);
                }
            }
            if (Rules.Count > 0)
            {
                writer.WriteStartElement(RulesNodeName);
                foreach (Entry entry in Rules)
                {
                    entry.WriteTo(writer);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public RewriteEntry()
        {
        }

        private object[] Data
        {
            get
            {
                if (_data == null)
                {
                    _data = new object[4];
                    _data[0] = false;
                    _data[1] = false;
                }
                return _data;
            }
        }

        public bool EnabledSet
        {
            get
            {
                return (bool)Data[1];
            }
            private set
            {
                Data[1] = value;
            }
        }

        public IList<RuleEntry> Rules
        {
            get
            {
                if (_rules == null)
                {
                    _rules = new CollectionWrapper<RuleEntry>((ArrayList)Data[2]);
                }
                return _rules;
            }
        }

        public IList<UnsupportedDirectiveEntry> UnsupportedDirectives
        {
            get
            {
                if (_unsupportedDirectives == null)
                {
                    _unsupportedDirectives = new CollectionWrapper<UnsupportedDirectiveEntry>((ArrayList)Data[3]);
                }
                return _unsupportedDirectives;
            }
        }

        public object GetData()
        {
            if (_rules != null)
            {
                _data[2] = _rules.GetData();
            }
            if (_unsupportedDirectives != null)
            {
                _data[3] = _unsupportedDirectives.GetData();
            }
            return _data;
        }

        public void SetData(object o)
        {
            _data = (object[])o;
        }

        //private const int IndexEnabled = 0;

        //private const int IndexEnabledSet = 1;

        //private const int IndexRules = 2;

        //private const int IndexUnsupportedDirectives = 3;

        private IEntryNode _treeNode;

        private readonly bool _isServerLevel;

        private object[] _data;

        private CollectionWrapper<RuleEntry> _rules;

        private CollectionWrapper<UnsupportedDirectiveEntry> _unsupportedDirectives;
    }

}
