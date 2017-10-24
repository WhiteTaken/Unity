using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GitHub.Unity
{
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
            var isUpdated = false;

            var localBranchesIsNull = localBranches == null;
            var localBranchUpdateIsNull = localBranchUpdate == null;

            var remoteBranchesIsNull = remoteBranches == null;
            var remoteBranchUpdateIsNull = remoteBranchUpdate == null;

            if (localBranchesIsNull != localBranchUpdateIsNull
                || !localBranchesIsNull && !localBranches.SequenceEqual(localBranchUpdate) 
                || remoteBranchesIsNull != remoteBranchUpdateIsNull 
                || !remoteBranchesIsNull && !remoteBranches.SequenceEqual(remoteBranchUpdate))
            {
                localBranches = localBranchUpdate;
                remoteBranches = remoteBranchUpdate;
                lastUpdatedAt = DateTime.Now;
                isUpdated = true;
            }

            lastVerifiedAt = DateTime.Now;
            Save(true);

            if (isUpdated)
            {
                CacheUpdated.SafeInvoke(lastUpdatedAt);
            }
        }

        private void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                CacheInvalidated.SafeInvoke();
                Update(new List<GitBranch>(), new List<GitBranch>());
            }
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
            var isUpdated = false;

            var logIsNull = log == null;
            var updateIsNull = logUpdate == null;
            if (logIsNull != updateIsNull ||
                !logIsNull && !log.SequenceEqual(logUpdate))
            {
                log = logUpdate;
                lastUpdatedAt = DateTime.Now;
                isUpdated = true;
            }

            lastVerifiedAt = DateTime.Now;
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
                ValidateData();
                return log;
            }
        }

        private void ValidateData()
        {
            if (DateTime.Now - lastUpdatedAt > DataTimeout)
            {
                CacheInvalidated.SafeInvoke();
                Update(new List<GitLogEntry>());
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
}
