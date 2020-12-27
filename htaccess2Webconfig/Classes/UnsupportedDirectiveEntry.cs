using System;
using System.Xml;

namespace WebConfig
{
    public sealed class UnsupportedDirectiveEntry : Entry, IWrapper
    {
        internal UnsupportedDirectiveEntry(Parser.UnsupportedDirective raw) : base(raw)
        {
        }

        public IEntryNode TreeNode
        {
            get
            {
                if (_treeNode == null)
                {
                    _treeNode = base.CreateEntryNode(this);
                    _treeNode.SetImageSelected(1);
                }
                return _treeNode;
            }
        }

        protected override void WriteDisplayString(XmlWriter writer)
        {
            writer.WriteComment(((Parser.UnsupportedDirective)base.Raw).ErrorMessage);
        }

        public override void WriteTo(XmlWriter writer)
        {
            WriteDisplayString(writer);
        }

        public UnsupportedDirectiveEntry()
        {
        }

        public object GetData()
        {
            return null;
        }

        public void SetData(object o)
        {
        }

        private IEntryNode _treeNode;
    }


}
