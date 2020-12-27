using System;

namespace WebConfig
{
    internal interface IEntryNodeFactory
    {
        IEntryNode CreateEntryNode(Entry entry, string text);
    }
}
