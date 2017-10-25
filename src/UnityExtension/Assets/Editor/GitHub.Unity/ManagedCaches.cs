using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GitHub.Unity
{
    [Location("cache/repoinfo.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class RepositoryInfoCache : ScriptObjectSingleton<RepositoryInfoCache>, IRepositoryInfoCache
    {
        private static ILogging Logger = Logging.GetLogger<RepositoryInfoCache>();
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;

        [SerializeField] private string repositoryName;
        [SerializeField] private GitRemote? gitRemote;
        [SerializeField] private GitBranch? gitBranch;

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public RepositoryInfoCache()
        { }

        public void UpdateData(string repositoryNameUpdate, GitRemote? gitRemoteUpdate, GitBranch? gitBranchUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

            Logger.Trace("Processing Update: {0}", now);

            if (repositoryName != repositoryNameUpdate)
            {
                repositoryName = repositoryNameUpdate;
                isUpdated = true;
            }

            if (Nullable.Equals(gitRemote, gitRemoteUpdate))
            {
                gitRemote = gitRemoteUpdate;
                isUpdated = true;
            }

            if (Nullable.Equals(gitBranch, gitBranchUpdate))
            {
                gitBranch = gitBranchUpdate;
                isUpdated = true;
            }

            if (isUpdated)
            {
                lastUpdatedAt = now;
            }

            lastVerifiedAt = now;
            Save(true);

            if (isUpdated)
            {
                Logger.Trace("Updated: {0}", now);
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
            else
            {
                Logger.Trace("Verified: {0}", now);
            }
        }

        public void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                InvalidateData();
            }
        }

        public void InvalidateData()
        {
            Logger.Trace("Invalidated");
            CacheInvalidated.SafeInvoke();
            UpdateData(null, null, null);
        }

        public DateTime LastUpdatedAt
        {
            get { return lastUpdatedAt; }
        }

        public DateTime LastVerifiedAt
        {
            get { return lastVerifiedAt; }
        }

        public string RepositoryName
        {
            get
            {
                ValidateData();
                return repositoryName;
            }
        }

        public GitRemote? CurrentRemote
        {
            get
            {
                ValidateData();
                return gitRemote;
            }
        }

        public GitBranch? CurentBranch
        {
            get
            {
                ValidateData();
                return gitBranch;
            }
        }
    }

    [Location("cache/branches.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class BranchCache : ScriptObjectSingleton<BranchCache>, IBranchCache
    {
        private static ILogging Logger = Logging.GetLogger<RepositoryInfoCache>();
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private List<GitBranch> localBranches = new List<GitBranch>();
        [SerializeField] private List<GitBranch> remoteBranches = new List<GitBranch>();

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public BranchCache()
        { }

        public void UpdateData(List<GitBranch> localBranchUpdate, List<GitBranch> remoteBranchUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

            Logger.Trace("Processing Update: {0}", now);

            var localBranchesIsNull = localBranches == null;
            var localBranchUpdateIsNull = localBranchUpdate == null;

            if (localBranchesIsNull != localBranchUpdateIsNull
                || !localBranchesIsNull && !localBranches.SequenceEqual(localBranchUpdate))
            {
                localBranches = localBranchUpdate;
                isUpdated = true;
            }

            var remoteBranchesIsNull = remoteBranches == null;
            var remoteBranchUpdateIsNull = remoteBranchUpdate == null;

            if (remoteBranchesIsNull != remoteBranchUpdateIsNull 
                || !remoteBranchesIsNull && !remoteBranches.SequenceEqual(remoteBranchUpdate))
            {
                remoteBranches = remoteBranchUpdate;
                isUpdated = true;
            }

            if(isUpdated)
            {
                lastUpdatedAt = now;
            }

            lastVerifiedAt = now;
            Save(true);

            if (isUpdated)
            {
                Logger.Trace("Updated: {0}", now);
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
            else
            {
                Logger.Trace("Verified: {0}", now);
            }
        }

        public void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                InvalidateData();
            }
        }

        public void InvalidateData()
        {
            Logger.Trace("Invalidated");
            CacheInvalidated.SafeInvoke();
            UpdateData(new List<GitBranch>(), new List<GitBranch>());
        }

        public List<GitBranch> LocalBranches
        {
            get
            {
                ValidateData();
                return localBranches;
            }
        }

        public List<GitBranch> RemoteBranches
        {
            get
            {
                ValidateData();
                return remoteBranches;
            }
        }

        public DateTime LastUpdatedAt
        {
            get { return lastUpdatedAt; }
        }

        public DateTime LastVerifiedAt
        {
            get { return lastVerifiedAt; }
        }
    }

    [Location("cache/gitlog.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class GitLogCache : ScriptObjectSingleton<GitLogCache>, IGitLogCache
    {
        private static ILogging Logger = Logging.GetLogger<RepositoryInfoCache>();
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private List<GitLogEntry> log = new List<GitLogEntry>();

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitLogCache()
        { }

        public void UpdateData(List<GitLogEntry> logUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

            Logger.Trace("Processing Update: {0}", now);

            var logIsNull = log == null;
            var updateIsNull = logUpdate == null;
            if (logIsNull != updateIsNull ||
                !logIsNull && !log.SequenceEqual(logUpdate))
            {
                log = logUpdate;
                lastUpdatedAt = now;
                isUpdated = true;
            }

            lastVerifiedAt = now;
            Save(true);

            if (isUpdated)
            {
                Logger.Trace("Updated: {0}", now);
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
            else
            {
                Logger.Trace("Verified: {0}", now);
            }
        }

        public List<GitLogEntry> Log
        {
            get
            {
                ValidateData();
                return log;
            }
        }

        public void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                InvalidateData();
            }
        }

        public void InvalidateData()
        {
            Logger.Trace("Invalidated");
            CacheInvalidated.SafeInvoke();
            UpdateData(new List<GitLogEntry>());
        }

        public DateTime LastUpdatedAt
        {
            get { return lastUpdatedAt; }
        }

        public DateTime LastVerifiedAt
        {
            get { return lastVerifiedAt; }
        }
    }

    [Location("cache/gitstatus.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class GitStatusCache : ScriptObjectSingleton<GitStatusCache>, IGitStatusCache
    {
        private static ILogging Logger = Logging.GetLogger<RepositoryInfoCache>();
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private GitStatus status;

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitStatusCache()
        { }

        public void UpdateData(GitStatus statusUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

            Logger.Trace("Processing Update: {0}", now);

            if (!status.Equals(statusUpdate))
            {
                status = statusUpdate;
                lastUpdatedAt = now;
                isUpdated = true;
            }

            lastVerifiedAt = now;
            Save(true);

            if (isUpdated)
            {
                Logger.Trace("Updated: {0}", now);
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
            else
            {
                Logger.Trace("Verified: {0}", now);
            }
        }

        public GitStatus GitStatus
        {
            get
            {
                ValidateData();
                return status;
            }
        }

        public void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                InvalidateData();
            }
        }

        public void InvalidateData()
        {
            Logger.Trace("Invalidated");
            CacheInvalidated.SafeInvoke();
            UpdateData(new GitStatus());
        }

        public DateTime LastUpdatedAt
        {
            get { return lastUpdatedAt; }
        }

        public DateTime LastVerifiedAt
        {
            get { return lastVerifiedAt; }
        }
    }

    [Location("cache/gitlocks.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class GitLocksCache : ScriptObjectSingleton<GitLocksCache>, IGitLocksCache
    {
        private static ILogging Logger = Logging.GetLogger<RepositoryInfoCache>();
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private List<GitLock> locks;

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitLocksCache()
        { }

        public void UpdateData(List<GitLock> locksUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

            Logger.Trace("Processing Update: {0}", now);

            var locksIsNull = locks == null;
            var locksUpdateIsNull = locksUpdate == null;

            if (locksIsNull != locksUpdateIsNull
                || !locksIsNull && !locks.SequenceEqual(locksUpdate))
            {
                locks = locksUpdate;
                isUpdated = true;
                lastUpdatedAt = now;
            }

            lastVerifiedAt = now;
            Save(true);

            if (isUpdated)
            {
                Logger.Trace("Updated: {0}", now);
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
            else
            {
                Logger.Trace("Verified: {0}", now);
            }
        }

        public List<GitLock> GitLocks
        {
            get
            {
                ValidateData();
                return locks;
            }
        }

        public void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                InvalidateData();
            }
        }

        public void InvalidateData()
        {
            Logger.Trace("Invalidated");
            CacheInvalidated.SafeInvoke();
            UpdateData(null);
        }

        public DateTime LastUpdatedAt
        {
            get { return lastUpdatedAt; }
        }

        public DateTime LastVerifiedAt
        {
            get { return lastVerifiedAt; }
        }
    }

    [Location("cache/gituser.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class GitUserCache : ScriptObjectSingleton<GitUserCache>, IGitUserCache
    {
        private static ILogging Logger = Logging.GetLogger<RepositoryInfoCache>();
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private User user;

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitUserCache()
        { }

        public void UpdateData(User userUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

            Logger.Trace("Processing Update: {0}", now);

            if (user != userUpdate)
            {
                user = userUpdate;
                isUpdated = true;
                lastUpdatedAt = now;
            }

            lastVerifiedAt = now;
            Save(true);

            if (isUpdated)
            {
                Logger.Trace("Updated: {0}", now);
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
            else
            {
                Logger.Trace("Verified: {0}", now);
            }
        }

        public User User
        {
            get
            {
                ValidateData();
                return user;
            }
        }

        public void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                InvalidateData();
            }
        }

        public void InvalidateData()
        {
            Logger.Trace("Invalidated");
            CacheInvalidated.SafeInvoke();
            UpdateData(null);
        }

        public DateTime LastUpdatedAt
        {
            get { return lastUpdatedAt; }
        }

        public DateTime LastVerifiedAt
        {
            get { return lastVerifiedAt; }
        }
    }
}
