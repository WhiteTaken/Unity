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

        private IRepositoryInfoCache repositoryInfoCache;
        public IRepositoryInfoCache RepositoryInfoCache
        {
            get { return repositoryInfoCache; }
            set
            {
                if (repositoryInfoCache == null)
                    repositoryInfoCache = value;
            }
        }

        private IGitStatusCache gitStatusCache;
        public IGitStatusCache GitStatusCache
        {
            get { return gitStatusCache; }
            set
            {
                if (gitStatusCache == null)
                    gitStatusCache = value;
            }
        }

        private IGitLocksCache gitLocksCache;
        public IGitLocksCache GitLocksCache
        {
            get { return gitLocksCache; }
            set
            {
                if (gitLocksCache == null)
                    gitLocksCache = value;
            }
        }

        private IGitUserCache gitUserCache;
        public IGitUserCache GitUserCache
        {
            get { return gitUserCache; }
            set
            {
                if (gitUserCache == null)
                    gitUserCache = value;
            }
        }
    }

    public interface ICacheContainer
    {
        IBranchCache BranchCache { get; }
        IGitLogCache GitLogCache { get; }
        IRepositoryInfoCache RepositoryInfoCache { get; }
        IGitStatusCache GitStatusCache { get; }
        IGitLocksCache GitLocksCache { get; }
        IGitUserCache GitUserCache { get; }
    }

    public interface IManagedCache
    {
        DateTime LastUpdatedAt { get; }
        DateTime LastVerifiedAt { get; }

        event Action<DateTime> CacheUpdated;
        event Action CacheInvalidated;
    }

    public interface IGitLocksCache : IManagedCache
    {
        List<GitLock> GitLocks { get; }
    }

    public interface IGitUserCache : IManagedCache
    {
        User User { get; }
    }

    public interface IGitStatusCache : IManagedCache
    {
        GitStatus GitStatus { get; }
    }

    public interface IRepositoryInfoCache : IManagedCache
    {
        string Name { get; }
        GitRemote? CurrentRemote { get; }
        GitBranch? CurentBranch { get; }
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