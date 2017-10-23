using System;
using System.Collections.Generic;

namespace GitHub.Unity
{
    public interface IGitLogCache
    {
        List<GitLogEntry> Log { get; set; }
        DateTime LastUpdatedAt { get; }
    }
}