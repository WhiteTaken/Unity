using System;
using System.Collections.Generic;
using System.Linq;

namespace GitHub.Unity
{
    public class CacheContainer: ICacheContainer
    {
        private IBranchCache branchCache;
        public IBranchCache BranchCache
        {
            get { return branchCache; }
            set
            {
                if (branchCache == null)
                    branchCache = value;
            }
        }

        private IGitLogCache gitLogCache;
        public IGitLogCache GitLogCache
        {
            get { return gitLogCache; }
            set
            {
                if (gitLogCache == null)
                    gitLogCache = value;
            }
        }
    }

    public interface ICacheContainer
    {
        IBranchCache BranchCache { get; set; }
        IGitLogCache GitLogCache { get; set; }
    }

    public interface IManagedCache
    {
        DateTime LastUpdatedAt { get; }
        DateTime LastVerifiedAt { get; }

        event Action<DateTime> CacheUpdated;
        event Action CacheInvalidated;
    }

    public interface IBranchCache: IManagedCache
    {
        List<GitBranch> LocalBranches { get; }
        List<GitBranch> RemoteBranches { get; }
        void Update(List<GitBranch> localBranchUpdate, List<GitBranch> remoteBranchUpdate);
    }

    public interface IGitLogCache : IManagedCache
    {
        List<GitLogEntry> Log { get; }
        void Update(List<GitLogEntry> logUpdate);
    }
}