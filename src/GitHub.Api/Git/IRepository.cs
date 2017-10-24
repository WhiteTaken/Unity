using System;
using System.Collections.Generic;

namespace GitHub.Unity
{
    /// <summary>
    /// Represents a repository, either local or retreived via the GitHub API.
    /// </summary>
    public interface IRepository : IEquatable<IRepository>
    {
        void Initialize(IRepositoryManager repositoryManager);
        void Refresh();
        ITask CommitAllFiles(string message, string body);
        ITask CommitFiles(List<string> files, string message, string body);
        ITask SetupRemote(string remoteName, string remoteUrl);
        ITask Pull();
        ITask Push();
        ITask Fetch();
        ITask Revert(string changeset);
        ITask RequestLock(string file);
        ITask ReleaseLock(string file, bool force);

        NPath LocalPath { get; }
        string Name { get; }
        UriString CloneUrl { get; }
        GitRemote? CurrentRemote { get; }
        GitBranch? CurrentBranch { get; }
        List<GitBranch> LocalBranches { get; }
        List<GitBranch> RemoteBranches { get; }
        GitStatus CurrentStatus { get; }
        List<GitLock> CurrentLocks { get; }
        List<GitLogEntry> Log { get; }
        User User { get; }

        event Action<GitStatus> OnStatusChanged;
        event Action<string> OnCurrentBranchChanged;
        event Action<string> OnCurrentRemoteChanged;
        event Action OnLocalBranchListChanged;
        event Action OnCurrentBranchUpdated;
        event Action<IEnumerable<GitLock>> OnLocksChanged;
        event Action OnRepositoryInfoChanged;
        event Action OnRemoteBranchListChanged;
    }
}