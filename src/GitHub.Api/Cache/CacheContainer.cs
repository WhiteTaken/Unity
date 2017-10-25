using System;
using System.Collections.Generic;

namespace GitHub.Unity
{
    public class CacheContainer : ICacheContainer
    {
        private IBranchCache branchCache;

        private IGitLocksCache gitLocksCache;

        private IGitLogCache gitLogCache;

        private IGitStatusCache gitStatusCache;

        private IGitUserCache gitUserCache;

        private IRepositoryInfoCache repositoryInfoCache;

        public event Action<CacheType> CacheInvalidated;

        public event Action<CacheType, DateTime> CacheUpdated;

        private IManagedCache GetManagedCache(CacheType cacheType)
        {
            switch (cacheType)
            {
                case CacheType.BranchCache:
                    return BranchCache;

                case CacheType.GitLogCache:
                    return GitLogCache;

                case CacheType.RepositoryInfoCache:
                    return RepositoryInfoCache;

                case CacheType.GitStatusCache:
                    return GitStatusCache;

                case CacheType.GitLocksCache:
                    return GitLocksCache;

                case CacheType.GitUserCache:
                    return GitUserCache;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null);
            }
        }

        public void Validate(CacheType cacheType)
        {
            GetManagedCache(cacheType).Validate();
        }

        public void ValidateAll()
        {
            BranchCache.Validate();
            GitLogCache.Validate();
            RepositoryInfoCache.Validate();
            GitStatusCache.Validate();
            GitLocksCache.Validate();
            GitUserCache.Validate();
        }

        public void Invalidate(CacheType cacheType)
        {
            GetManagedCache(cacheType).Invalidate();
        }

        public void InvalidateAll()
        {
            BranchCache.Invalidate();
            GitLogCache.Invalidate();
            RepositoryInfoCache.Invalidate();
            GitStatusCache.Invalidate();
            GitLocksCache.Invalidate();
            GitUserCache.Invalidate();
        }

        public IBranchCache BranchCache
        {
            get { return branchCache; }
            set
            {
                if (branchCache == null)
                {
                    branchCache = value;
                    branchCache.CacheInvalidated += () => CacheInvalidated(CacheType.BranchCache);
                    branchCache.CacheUpdated += datetime => CacheUpdated(CacheType.BranchCache, datetime);
                }
            }
        }

        public IGitLogCache GitLogCache
        {
            get { return gitLogCache; }
            set
            {
                if (gitLogCache == null)
                {
                    gitLogCache = value;
                    branchCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitLogCache);
                    branchCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitLogCache, datetime);
                }
            }
        }

        public IRepositoryInfoCache RepositoryInfoCache
        {
            get { return repositoryInfoCache; }
            set
            {
                if (repositoryInfoCache == null)
                {
                    repositoryInfoCache = value;
                    branchCache.CacheInvalidated += () => CacheInvalidated(CacheType.RepositoryInfoCache);
                    branchCache.CacheUpdated += datetime => CacheUpdated(CacheType.RepositoryInfoCache, datetime);
                }
            }
        }

        public IGitStatusCache GitStatusCache
        {
            get { return gitStatusCache; }
            set
            {
                if (gitStatusCache == null)
                {
                    gitStatusCache = value;
                    branchCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitStatusCache);
                    branchCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitStatusCache, datetime);
                }
            }
        }

        public IGitLocksCache GitLocksCache
        {
            get { return gitLocksCache; }
            set
            {
                if (gitLocksCache == null)
                {
                    gitLocksCache = value;
                    branchCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitLocksCache);
                    branchCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitLocksCache, datetime);
                }
            }
        }

        public IGitUserCache GitUserCache
        {
            get { return gitUserCache; }
            set
            {
                if (gitUserCache == null)
                {
                    gitUserCache = value;
                    branchCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitUserCache);
                    branchCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitUserCache, datetime);
                }
            }
        }
    }

    public enum CacheType
    {
        BranchCache,
        GitLogCache,
        RepositoryInfoCache,
        GitStatusCache,
        GitLocksCache,
        GitUserCache
    }

    public interface ICacheContainer
    {
        event Action<CacheType> CacheInvalidated;
        event Action<CacheType, DateTime> CacheUpdated;

        IBranchCache BranchCache { get; }
        IGitLogCache GitLogCache { get; }
        IRepositoryInfoCache RepositoryInfoCache { get; }
        IGitStatusCache GitStatusCache { get; }
        IGitLocksCache GitLocksCache { get; }
        IGitUserCache GitUserCache { get; }
        void Validate(CacheType cacheType);
        void ValidateAll();
        void Invalidate(CacheType cacheType);
        void InvalidateAll();
    }

    public interface IManagedCache
    {
        event Action CacheInvalidated;
        event Action<DateTime> CacheUpdated;

        void Validate();
        void Invalidate();

        DateTime LastUpdatedAt { get; }
        DateTime LastVerifiedAt { get; }
    }

    public interface IGitLocks
    {
        List<GitLock> GitLocks { get; }
    }

    public interface IGitLocksCache : IManagedCache, IGitLocks
    { }

    public interface IGitUser
    {
        User User { get; }
    }

    public interface IGitUserCache : IManagedCache, IGitUser
    { }

    public interface IGitStatus
    {
        GitStatus GitStatus { get; }
    }

    public interface IGitStatusCache : IManagedCache, IGitStatus
    { }

    public interface IRepositoryInfo
    {
        string RepositoryName { get; }
        GitRemote? CurrentRemote { get; }
        GitBranch? CurentBranch { get; }
    }

    public interface IRepositoryInfoCache : IManagedCache, IRepositoryInfo
    { }

    public interface IBranch
    {
        void Update(List<GitBranch> localBranchUpdate, List<GitBranch> remoteBranchUpdate);
        List<GitBranch> LocalBranches { get; }
        List<GitBranch> RemoteBranches { get; }
    }

    public interface IBranchCache : IManagedCache, IBranch
    { }

    public interface IGitLogCache : IManagedCache
    {
        List<GitLogEntry> Log { get; }
    }
}
