using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace WebConfig
{
    public abstract class Entry
    {
        public Entry() : this(null)
        {
        }

        internal Entry(Parser.RawEntry rawEntry)
        {
            _entryNodeFactory = new EntryTreeNodeFactory();
            Raw = rawEntry;
        }

        internal List<CommentEntry> Comments
        {
            get
            {
                if (_comments == null)
                {
                    _comments = new List<CommentEntry>();
                }
                return _comments;
            }
        }

        public string DisplayString
        {
            get
            {
                string text = null;
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    using (XmlWriter xmlWriter = Entry.CreateWriter(stringWriter))
                    {
                        WriteDisplayString(xmlWriter);
                        xmlWriter.Close();
                        text = stringWriter.GetStringBuilder().ToString();
                    }
                }
                return text.Replace(" />", ">");
            }
        }

        public bool HasErrorInfo
        {
            get
            {
                return ErrorInfo != null && !ErrorInfo.IsWarning;
            }
        }

        public bool HasWarningInfo
        {
            get
            {
                return ErrorInfo != null && ErrorInfo.IsWarning;
            }
        }

        internal Parser.RawEntry Raw { get; set; }

        protected static XmlWriter CreateWriter(TextWriter writer)
        {
            return XmlWriter.Create(writer, new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                OmitXmlDeclaration = true
            });
        }

        internal IEntryNode CreateEntryNode(Entry entry)
        {
            return CreateEntryNode(entry, entry.DisplayString);
        }

        internal IEntryNode CreateEntryNode(Entry entry, string text)
        {
            return _entryNodeFactory.CreateEntryNode(entry, text);
        }

        protected void WriteComments(XmlWriter writer)
        {
            if (_comments != null)
            {
                foreach (CommentEntry commentEntry in _comments)
                {
                    writer.WriteComment(commentEntry.Text);
                }
            }
        }

        protected abstract void WriteDisplayString(XmlWriter writer);

        public abstract void WriteTo(XmlWriter writer);

        private readonly IEntryNodeFactory _entryNodeFactory;
        private List<CommentEntry> _comments;

        internal ErrorInfo ErrorInfo;
    }

}
