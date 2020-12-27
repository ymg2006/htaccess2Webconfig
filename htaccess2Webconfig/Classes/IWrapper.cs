using System;

namespace WebConfig
{
    internal interface IWrapper
    {
        object GetData();

        void SetData(object o);
    }
}
