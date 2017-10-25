﻿using System;
using System.Collections.Generic;

namespace GitHub.Unity
{
    public class CacheContainer : ICacheContainer
    {
        private static ILogging Logger = Logging.GetLogger<CacheContainer>();

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
            GetManagedCache(cacheType).ValidateData();
        }

        public void ValidateAll()
        {
            BranchCache.ValidateData();
            GitLogCache.ValidateData();
            RepositoryInfoCache.ValidateData();
            GitStatusCache.ValidateData();
            GitLocksCache.ValidateData();
            GitUserCache.ValidateData();
        }

        public void Invalidate(CacheType cacheType)
        {
            GetManagedCache(cacheType).InvalidateData();
        }

        public void InvalidateAll()
        {
            BranchCache.InvalidateData();
            GitLogCache.InvalidateData();
            RepositoryInfoCache.InvalidateData();
            GitStatusCache.InvalidateData();
            GitLocksCache.InvalidateData();
            GitUserCache.InvalidateData();
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
                    gitLogCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitLogCache);
                    gitLogCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitLogCache, datetime);
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
                    repositoryInfoCache.CacheInvalidated += () => CacheInvalidated(CacheType.RepositoryInfoCache);
                    repositoryInfoCache.CacheUpdated += datetime => CacheUpdated(CacheType.RepositoryInfoCache, datetime);
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
                    gitStatusCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitStatusCache);
                    gitStatusCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitStatusCache, datetime);
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
                    gitLocksCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitLocksCache);
                    gitLocksCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitLocksCache, datetime);
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
                    gitUserCache.CacheInvalidated += () => CacheInvalidated(CacheType.GitUserCache);
                    gitUserCache.CacheUpdated += datetime => CacheUpdated(CacheType.GitUserCache, datetime);
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

        void ValidateData();
        void InvalidateData();

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
        void UpdateData(List<GitBranch> localBranchUpdate, List<GitBranch> remoteBranchUpdate);
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