#pragma warning disable 649

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GitHub.Unity
{
    [Serializable]
    class InitProjectView : Subview
    {
        private const string NoRepoTitle = "To begin using GitHub, initialize a git repository";

        [NonSerialized] private bool isBusy;
        [SerializeField] private bool isPublished;

        public override void OnDataUpdate()
        {
            base.OnDataUpdate();
            MaybeUpdateData();
        }

        public override void OnRepositoryChanged(IRepository oldRepository)
        {
            base.OnRepositoryChanged(oldRepository);
            Refresh();
        }

        public override void OnGUI()
        {
            GUILayout.BeginVertical(Styles.GenericBoxStyle);
            {
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                {
                  GUILayout.FlexibleSpace();
                  GUILayout.Label(Styles.EmptyStateInit, GUILayout.MaxWidth(265), GUILayout.MaxHeight(136));
                  GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();

                GUILayout.Label(NoRepoTitle, Styles.BoldCenteredLabel);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                EditorGUI.BeginDisabledGroup(isBusy);
                {
                    if (GUILayout.Button(Localization.InitializeRepositoryButtonText, "Button"))
                    {
                        isBusy = true;
                        Manager.InitializeRepository()
                               .FinallyInUI(() => isBusy = false)
                               .Start();
                    }
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //EditorGUILayout.HelpBox("There was an error initializing a repository.", MessageType.Error);

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }

        private void MaybeUpdateData()
        {
            isPublished = Repository != null && Repository.CurrentRemote.HasValue;
        }

        public override bool IsBusy
        {
            get { return isBusy; }
        }
    }
}
