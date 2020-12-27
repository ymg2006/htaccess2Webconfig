using System;
using System.Xml;

namespace WebConfig
{
    internal sealed class CommentEntry : Entry, IWrapper
    {
        public CommentEntry(Parser.RawComment raw) : base(raw)
        {
            Text = raw.Text;
        }

        public string Text { get; set; }

        protected override void WriteDisplayString(XmlWriter writer)
        {
            writer.WriteComment(Text);
        }

        public override void WriteTo(XmlWriter writer)
        {
            WriteDisplayString(writer);
        }

        public object GetData()
        {
            return Text;
        }

        public void SetData(object o)
        {
            Text = (string)o;
        }
    }

}
