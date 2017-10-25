using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace GitHub.Unity
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    class Repository : IEquatable<Repository>, IRepository
    {
        private IRepositoryManager repositoryManager;
        public event Action<string> OnCurrentBranchChanged;
        public event Action<string> OnCurrentRemoteChanged;
        public event Action OnCurrentBranchUpdated;
        public event Action OnLocalBranchListChanged;
        public event Action<IEnumerable<GitLock>> OnLocksChanged;
        public event Action OnRemoteBranchListChanged;
        public event Action OnRepositoryInfoChanged;

        public event Action<GitStatus> OnStatusChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository"/> class.
        /// </summary>
        /// <param name="localPath"></param>
        public Repository(NPath localPath)
        {
            Guard.ArgumentNotNull(localPath, nameof(localPath));
            LocalPath = localPath;
        }

        public void Initialize(IRepositoryManager repositoryManager)
        {
            Guard.ArgumentNotNull(repositoryManager, nameof(repositoryManager));

            this.repositoryManager = repositoryManager;
        }

        public void Refresh()
        {
            repositoryManager?.Refresh();
        }

        public ITask SetupRemote(string remote, string remoteUrl)
        {
            Guard.ArgumentNotNullOrWhiteSpace(remote, "remote");
            Guard.ArgumentNotNullOrWhiteSpace(remoteUrl, "remoteUrl");
            if (!CurrentRemote.HasValue || String.IsNullOrEmpty(CurrentRemote.Value.Name)) // there's no remote at all
            {
                return repositoryManager.RemoteAdd(remote, remoteUrl);
            }
            else
            {
                return repositoryManager.RemoteChange(remote, remoteUrl);
            }
        }

        public ITask CommitAllFiles(string message, string body)
        {
            return repositoryManager.CommitAllFiles(message, body);
        }

        public ITask CommitFiles(List<string> files, string message, string body)
        {
            return repositoryManager.CommitFiles(files, message, body);
        }

        public ITask Pull()
        {
            return repositoryManager.Pull(CurrentRemote.Value.Name, CurrentBranch?.Name);
        }

        public ITask Push()
        {
            return repositoryManager.Push(CurrentRemote.Value.Name, CurrentBranch?.Name);
        }

        public ITask Fetch()
        {
            return repositoryManager.Fetch(CurrentRemote.Value.Name);
        }

        public ITask Revert(string changeset)
        {
            return repositoryManager.Revert(changeset);
        }

        public ITask RequestLock(string file)
        {
            return repositoryManager.LockFile(file);
        }

        public ITask ReleaseLock(string file, bool force)
        {
            return repositoryManager.UnlockFile(file, force);
        }

        /// <summary>
        /// Note: We don't consider CloneUrl a part of the hash code because it can change during the lifetime
        /// of a repository. Equals takes care of any hash collisions because of this
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return LocalPath.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            var other = obj as Repository;
            return Equals(other);
        }

        public bool Equals(Repository other)
        {
            return Equals((IRepository)other);
        }

        public bool Equals(IRepository other)
        {
            if (ReferenceEquals(this, other))
                return true;
            return other != null &&
                object.Equals(LocalPath, other.LocalPath);
        }

        public GitBranch? CurrentBranch => repositoryManager.CacheContainer.RepositoryInfoCache.CurentBranch;

        public GitRemote? CurrentRemote => repositoryManager.CacheContainer.RepositoryInfoCache.CurrentRemote;

        public UriString CloneUrl => CurrentRemote.HasValue ? new UriString(CurrentRemote.Value.Url) : null;

        public string Name => repositoryManager.CacheContainer.RepositoryInfoCache.RepositoryName;

        public List<GitBranch> LocalBranches => repositoryManager.CacheContainer.BranchCache.LocalBranches;

        public List<GitBranch> RemoteBranches => repositoryManager.CacheContainer.BranchCache.RemoteBranches;

        public GitStatus CurrentStatus => repositoryManager.CacheContainer.GitStatusCache.GitStatus;

        public List<GitLock> CurrentLocks => repositoryManager.CacheContainer.GitLocksCache.GitLocks;

        public List<GitLogEntry> Log => repositoryManager.CacheContainer.GitLogCache.Log;

        public User User => repositoryManager.CacheContainer.GitUserCache.User;

        public NPath LocalPath { get; }

        public bool IsGitHub => HostAddress.IsGitHubDotCom(CloneUrl);

        internal string DebuggerDisplay => String.Format(
            CultureInfo.InvariantCulture,
            "{0} Name: {1} CloneUrl: {2} LocalPath: {3} Branch: {4} Remote: {5}",
            GetHashCode(),
            Name,
            CloneUrl,
            LocalPath,
            CurrentBranch,
            CurrentRemote);

        protected static ILogging Logger { get; } = Logging.GetLogger<Repository>();
    }

    public interface IUser
    {
        string Name { get; set; }
        string Email { get; set; }
    }

    [Serializable]
    public class User : IUser
    {
        public override string ToString()
        {
            return String.Format("Name: {0} Email: {1}", Name, Email);
        }

        public string Name { get; set; }
        public string Email { get; set; }
    }
}