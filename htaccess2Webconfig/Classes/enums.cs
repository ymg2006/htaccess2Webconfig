using System;

namespace WebConfig
{
    internal enum LogicalGrouping
    {
        MatchAll,
        MatchAny
    }

    internal enum ActionType
    {
        None,
        Rewrite,
        Redirect,
        CustomResponse,
        AbortRequest
    }

    internal enum RedirectType
    {
        Permanent = 301,
        Found,
        SeeOther,
        Temporary = 307
    }

    internal enum MatchType
    {
        Pattern,
        IsFile,
        IsDirectory
    }

}
