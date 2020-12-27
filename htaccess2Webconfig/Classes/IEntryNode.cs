using System;

namespace WebConfig
{
    public interface IEntryNode
    {
        void AddChild(IEntryNode node);

        void Expand();

        void SetImageSelected(int index);

        void SetText(string text);
    }
}
