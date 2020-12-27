using System;

namespace WebConfig
{
    internal sealed class EntryTreeNodeFactory : IEntryNodeFactory
    {
        public IEntryNode CreateEntryNode(Entry entry, string text)
        {
            return new EntryTreeNode(entry, text);
        }
    }
}
