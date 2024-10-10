using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KabreetGames.SceneManagement
{
    public static class SceneLoadingManager
    {
        public static SceneGroup ActiveSceneGroup { get; private set; } =
            (GroupsData == null || GroupsData.sceneGroups.Length == 0) ? null : GroupsData.sceneGroups[0];

        private static SceneGroupsData groupsData;
        private static OperationCanceledException cancellationToken = new();

        public static void SetGroupsData(SceneGroupsData sceneGroupsData)
        {
            groupsData = sceneGroupsData;
        }
        public static SceneGroupsData GroupsData => groupsData ??= Resources.Load<SceneGroupsData>("SceneGroups");
        public static async UniTask ReplaceScene(SceneGroupNames loadSceneGroupName,
            SceneGroupNames unloadSceneGroupName, ReplaceSceneMode replaceSceneMode = ReplaceSceneMode.LoadUnload,
            IProgress<float> progress = default, bool reloadDupScenes = false,
            LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            switch (replaceSceneMode)
            {
                case ReplaceSceneMode.LoadUnload:
                    await LoadScene(GroupsData.GetGroupByName(loadSceneGroupName), progress, reloadDupScenes,
                        loadSceneMode);
                    await UnloadScene(unloadSceneGroupName);
                    break;
                case ReplaceSceneMode.UnLoadLoad:
                    await UnloadScene(unloadSceneGroupName);
                    await LoadScene(GroupsData.GetGroupByName(loadSceneGroupName), progress, reloadDupScenes,
                        loadSceneMode);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(replaceSceneMode), replaceSceneMode, null);
            }
        }

        public static async UniTask Restart(IProgress<float> progress = default,
            bool reloadDupScenes = false, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            await UnloadScene(ActiveSceneGroup);
            await LoadScene(ActiveSceneGroup, progress, reloadDupScenes, loadSceneMode);
        }

        public static async UniTask Restart(SceneGroupNames restartSceneGroupName,
            IProgress<float> progress = default,
            bool reloadDupScenes = false, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            await UnloadScene(restartSceneGroupName);
            await LoadScene(restartSceneGroupName, progress, reloadDupScenes, loadSceneMode);
        }

        public static async UniTask LoadScene(SceneGroupNames sceneGroupName,
            IProgress<float> progress = default, bool reloadDupScenes = false,
            LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            await LoadScene(GroupsData.GetGroupByName(sceneGroupName), progress, reloadDupScenes, loadSceneMode);
        }

        public static async UniTask LoadScene(int sceneGroupIndex, IProgress<float> progress = default,
            bool reloadDupScenes = false, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            await LoadScene(GroupsData.sceneGroups[sceneGroupIndex], progress, reloadDupScenes, loadSceneMode);
        }

        public static async UniTask LoadScene(SceneGroup sceneGroup, IProgress<float> progress,
            bool reloadDupScenes = false, LoadSceneMode loadSceneMode = LoadSceneMode.Additive)
        {
            ActiveSceneGroup = sceneGroup;
            var totalSceneCount = ActiveSceneGroup.scenes.Count;
            var operationGroup = new AsyncOperationGroup();
            for (var i = 0; i < totalSceneCount; i++)
            {
                var sceneData = sceneGroup.scenes[i];
                if (!reloadDupScenes && sceneData.reference.LoadedScene.IsValid()) continue;
                var operation = SceneManager
                    .LoadSceneAsync(sceneData.reference.Path, loadSceneMode);
                operationGroup.operations.Add(operation);
            }

            progress?.Report(0f);
            while (!operationGroup.IsDone)
            {
                progress?.Report(operationGroup.Progress);
                await UniTask.Delay(1);
            }

            progress?.Report(1f);
            var activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.GetSceneByName(SceneType.ActiveScene));
            if (activeScene.IsValid()) SceneManager.SetActiveScene(activeScene);
        }

        public static async UniTask UnloadAllScene()
        {
            var sceneCount = SceneManager.sceneCount;
            var operationGroup = new AsyncOperationGroup();
            if (!SceneManager.GetSceneByName("DummyScene").IsValid())
                SceneManager.CreateScene("DummyScene", new CreateSceneParameters(LocalPhysicsMode.None));
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.IsValid() || scene.name == "DummyScene") continue;
                var operation = SceneManager.UnloadSceneAsync(scene);
                operationGroup.operations.Add(operation);
            }

            while (!operationGroup.IsDone)
            {
                await UniTask.Delay(1);
            }

            var clean = Resources.UnloadUnusedAssets();

            while (!clean.isDone)
            {
                await UniTask.Delay(1);
            }
        }

        public static async UniTask UnloadScene(SceneGroupNames sceneGroupName)
        {
            await UnloadScene(GroupsData.GetGroupByName(sceneGroupName));
        }

        public static async UniTask UnloadScene(int sceneGroupIndex)
        {
            await UnloadScene(GroupsData.sceneGroups[sceneGroupIndex]);
        }

        public static async UniTask UnloadScene(SceneGroup sceneGroup)
        {
            var operationGroup = new AsyncOperationGroup();
            foreach (var scene in sceneGroup.scenes.Where(scene => scene.reference.LoadedScene.IsValid()))
            {
                if (SceneManager.loadedSceneCount == 1)
                {
                    SceneManager.CreateScene("DummyScene", new CreateSceneParameters(LocalPhysicsMode.None));
                }

                var operation = SceneManager.UnloadSceneAsync(scene.reference.LoadedScene);
                operationGroup.operations.Add(operation);
            }

            while (!operationGroup.IsDone)
            {
                await UniTask.Delay(1);
            }

            var clean = Resources.UnloadUnusedAssets();
            while (!clean.isDone)
            {
                await UniTask.Delay(1);
            }
        }

        private sealed class AsyncOperationGroup
        {
            public readonly List<AsyncOperation> operations = new();
            public float Progress => operations.Count > 0 ? operations.Average(x => x.progress) : 0f;

            public bool IsDone => operations.Count == 0 || operations.Where(o => o != null).All(o => o.isDone);
        }

        public class LoadProgress : IProgress<float>
        {
            public event Action<float> OnProgressChanged = delegate { };
            private const float Ratio = 1f;

            public void Report(float value)
            {
                OnProgressChanged?.Invoke(value / Ratio);
            }
        }
    }
}