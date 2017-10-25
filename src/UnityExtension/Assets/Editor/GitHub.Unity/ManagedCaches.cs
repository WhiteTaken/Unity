using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GitHub.Unity
{
    [Location("cache/repoinfo.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class RepositoryInfoCache : ScriptObjectSingleton<RepositoryInfoCache>, IRepositoryInfoCache
    {
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

        public void Update(string repositoryNameUpdate, GitRemote? gitRemoteUpdate, GitBranch? gitBranchUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

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
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
        }

        public void Validate()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                Invalidate();
            }
        }

        public void Invalidate()
        {
            CacheInvalidated.SafeInvoke();
            Update(null, null, null);
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
                Validate();
                return repositoryName;
            }
        }

        public GitRemote? CurrentRemote
        {
            get
            {
                Validate();
                return gitRemote;
            }
        }

        public GitBranch? CurentBranch
        {
            get
            {
                Validate();
                return gitBranch;
            }
        }
    }

    [Location("cache/branches.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class BranchCache : ScriptObjectSingleton<BranchCache>, IBranchCache
    {
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private List<GitBranch> localBranches = new List<GitBranch>();
        [SerializeField] private List<GitBranch> remoteBranches = new List<GitBranch>();

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public BranchCache()
        { }

        public void Update(List<GitBranch> localBranchUpdate, List<GitBranch> remoteBranchUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

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
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
        }

        public void Validate()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                Invalidate();
            }
        }

        public void Invalidate()
        {
            CacheInvalidated.SafeInvoke();
            Update(new List<GitBranch>(), new List<GitBranch>());
        }

        public List<GitBranch> LocalBranches
        {
            get
            {
                Validate();
                return localBranches;
            }
        }

        public List<GitBranch> RemoteBranches
        {
            get
            {
                Validate();
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
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private List<GitLogEntry> log = new List<GitLogEntry>();

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitLogCache()
        { }

        public void Update(List<GitLogEntry> logUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

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
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
        }

        public List<GitLogEntry> Log
        {
            get
            {
                Validate();
                return log;
            }
        }

        public void Validate()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                Invalidate();
            }
        }

        public void Invalidate()
        {
            CacheInvalidated.SafeInvoke();
            Update(new List<GitLogEntry>());
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
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private GitStatus status;

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitStatusCache()
        { }

        public void Update(GitStatus statusUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

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
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
        }

        public GitStatus GitStatus
        {
            get
            {
                Validate();
                return status;
            }
        }

        public void Validate()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                Invalidate();
            }
        }

        public void Invalidate()
        {
            CacheInvalidated.SafeInvoke();
            Update(new GitStatus());
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
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private List<GitLock> locks;

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitLocksCache()
        { }

        public void Update(List<GitLock> locksUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

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
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
        }

        public List<GitLock> GitLocks
        {
            get
            {
                Validate();
                return locks;
            }
        }

        public void Validate()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                Invalidate();
            }
        }

        public void Invalidate()
        {
            CacheInvalidated.SafeInvoke();
            Update(null);
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
        private static readonly TimeSpan DataTimeout = TimeSpan.FromSeconds(0.5);

        [SerializeField] private DateTime lastUpdatedAt;
        [SerializeField] private DateTime lastVerifiedAt;
        [SerializeField] private User user;

        public event Action CacheInvalidated;
        public event Action<DateTime> CacheUpdated;

        public GitUserCache()
        { }

        public void Update(User userUpdate)
        {
            var now = DateTime.Now;
            var isUpdated = false;

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
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
        }

        public User User
        {
            get
            {
                Validate();
                return user;
            }
        }

        public void Validate()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                Invalidate();
            }
        }

        public void Invalidate()
        {
            CacheInvalidated.SafeInvoke();
            Update(null);
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
