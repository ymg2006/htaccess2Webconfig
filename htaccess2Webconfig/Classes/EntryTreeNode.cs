using System;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace WebConfig
{
    public sealed class EntryTreeNode : TreeNode, IEntryNode
    {
        internal EntryTreeNode(Entry entry, string text)
        {
            Entry = entry;
            Initialize(text);
        }

        private EntryTreeNode(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Entry Entry { get; }
        void IEntryNode.AddChild(IEntryNode node)
        {
            base.Nodes.Add((EntryTreeNode)node);
        }

        void IEntryNode.Expand()
        {
            base.Expand();
        }

        public void SetImageSelected(int index)
        {
            base.ImageIndex = index;
            base.SelectedImageIndex = index;
        }

        public void SetText(string text)
        {
            base.Text = text;
        }

        private void Initialize(string text)
        {
            base.Text = text;
            if (Entry.ErrorInfo != null)
            {
                if (Entry.ErrorInfo.IsWarning)
                {
                    base.ImageIndex = 2;
                    base.SelectedImageIndex = 2;
                    return;
                }
                base.ImageIndex = 1;
                base.SelectedImageIndex = 1;
            }
        }
    }

}
