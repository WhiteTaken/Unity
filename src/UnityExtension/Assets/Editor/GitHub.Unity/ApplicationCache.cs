using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace GitHub.Unity
{
    sealed class ApplicationCache : ScriptObjectSingleton<ApplicationCache>
    {
        [SerializeField] private bool firstRun = true;

        [NonSerialized] private bool? val;

        public bool FirstRun
        {
            get
            {
                if (!val.HasValue)
                {
                    val = firstRun;
                }

                if (firstRun)
                {
                    firstRun = false;
                    Save(true);
                }

                return val.Value;
            }
        }
    }

    sealed class EnvironmentCache : ScriptObjectSingleton<EnvironmentCache>
    {
        [SerializeField] private string repositoryPath;
        [SerializeField] private string unityApplication;
        [SerializeField] private string unityAssetsPath;
        [SerializeField] private string extensionInstallPath;
        [SerializeField] private string unityVersion;

        [NonSerialized] private IEnvironment environment;
        public IEnvironment Environment
        {
            get
            {
                if (environment == null)
                {
                    environment = new DefaultEnvironment();
                    if (unityApplication == null)
                    {
                        unityAssetsPath = Application.dataPath;
                        unityApplication = EditorApplication.applicationPath;
                        extensionInstallPath = DetermineInstallationPath();
                        unityVersion = Application.unityVersion;
                    }
                    environment.Initialize(unityVersion, extensionInstallPath.ToNPath(), unityApplication.ToNPath(), unityAssetsPath.ToNPath());
                    environment.InitializeRepository(!String.IsNullOrEmpty(repositoryPath) ? repositoryPath.ToNPath() : null);
                    Flush();
                }
                return environment;
            }
        }

        private NPath DetermineInstallationPath()
        {
            // Juggling to find out where we got installed
            var shim = ScriptableObject.CreateInstance<RunLocationShim>();
            var script = MonoScript.FromScriptableObject(shim);
            var scriptPath = AssetDatabase.GetAssetPath(script).ToNPath();
            ScriptableObject.DestroyImmediate(shim);
            return scriptPath.Parent;
        }

        public void Flush()
        {
            repositoryPath = Environment.RepositoryPath;
            unityApplication = Environment.UnityApplication;
            unityAssetsPath = Environment.UnityAssetsPath;
            extensionInstallPath = Environment.ExtensionInstallPath;
            Save(true);
        }
    }

    [Location("views/branches.yaml", LocationAttribute.Location.LibraryFolder)]
    sealed class Favorites : ScriptObjectSingleton<Favorites>
    {
        [SerializeField] private List<string> favoriteBranches;
        public List<string> FavoriteBranches
        {
            get
            {
                if (favoriteBranches == null)
                    FavoriteBranches = new List<string>();
                return favoriteBranches;
            }
            set
            {
                favoriteBranches = value;
                Save(true);
            }
        }

        public void SetFavorite(string branchName)
        {
            if (FavoriteBranches.Contains(branchName))
                return;
            FavoriteBranches.Add(branchName);
            Save(true);
        }

        public void UnsetFavorite(string branchName)
        {
            if (!FavoriteBranches.Contains(branchName))
                return;
            FavoriteBranches.Remove(branchName);
            Save(true);
        }

        public void ToggleFavorite(string branchName)
        {
            if (FavoriteBranches.Contains(branchName))
                FavoriteBranches.Remove(branchName);
            else
                FavoriteBranches.Add(branchName);
            Save(true);
        }

        public bool IsFavorite(string branchName)
        {
            return FavoriteBranches.Contains(branchName);
        }
    }
}
