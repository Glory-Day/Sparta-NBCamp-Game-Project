using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Util.Debug;
using Backend.Util.Data;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

namespace Backend.Util.Management
{
    /// <summary>
    /// 리소스 기능을 관리 하는 클래스.
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        private readonly Dictionary<string, UnityEngine.Object> _gameObjectAssets = new();
        private readonly Dictionary<string, UnityEngine.Object> _uiAssets = new();
        private readonly Dictionary<string, UnityEngine.Object> _dataAssets = new();

        private readonly List<Task> _tasks = new();

        private ResourceManager() { }

        private void LoadGameObjectAssetsByLabelAsync_Internal(string current)
        {
            var set = AddressData.Groups[AddressData.Group.Game_Object][current].ToList();

            // 로드 목록을 순회하며 비동기 로드 후에 추가한다.
            var count = set.Count;
            for (var i = 0; i < count; i++)
            {
                var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(set[i]);

                //핸들 작업을 테스크로 보관하여 리스트에 추가한다.
                Task task = WaitGameObjectAssetsLoadedDone(handle, set[i]);
                _tasks.Add(task);
            }
        }

        private void LoadUIAssetsByLabelAsync_Internal(string current)
        {
            var set = AddressData.Groups[AddressData.Group.UI][current].ToList();

            // 로드 목록을 순회하며 비동기 로드 후에 추가한다.
            var count = set.Count;
            for (var i = 0; i < count; i++)
            {
                var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(set[i]);

                //핸들 작업을 테스크로 보관하여 리스트에 추가한다.
                Task task = WaitUIAssetsLoadedDone(handle, set[i]);
                _tasks.Add(task);
            }
        }

        private void LoadDataAssetsByLabelAsync_Internal(string current)
        {
            var set = AddressData.Groups[AddressData.Group.Data][current].ToList();

            // 로드 목록을 순회하며 비동기 로드 후에 추가한다.
            var count = set.Count;
            for (var i = 0; i < count; i++)
            {
                var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(set[i]);

                //핸들 작업을 테스크로 보관하여 리스트에 추가한다.
                Task task = WaitDataAssetsLoadedDone(handle, set[i]);
                _tasks.Add(task);
            }
        }

        private void LoadGameObjectAssetsByLabelAsync_Internal(string previous, string next)
        {
            // 로드 된 에셋과 로드 할 에셋의 합집합 차집합을 구한다.
            // 현재 로드된 에셋. 즉, 로드할 에셋의 합집합이다.
            var set = AddressData.Groups[AddressData.Group.Game_Object][previous].Intersect(AddressData.Groups[AddressData.Group.Game_Object][next]).ToList();

            // 로드할 에셋의 차집합. 즉, 로드를 해제할 에셋의 집합이다.
            var a = AddressData.Groups[AddressData.Group.Game_Object][previous].Except(set).ToList();
            // 로드된 에셋의 차집합. 즉, 앞으로 로드를 할 에셋의 집합이다.
            var b = AddressData.Groups[AddressData.Group.Game_Object][next].Except(set).ToList();

            // 언로드 목록에 포함이 되어 있으면 언로드한다.
            var count = a.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                try
                {
                    Addressables.Release(_gameObjectAssets[a[i]]);
                }
                catch
                {
                    Debugger.LogMessage("메모리에 없음");
                }
            }

            // 로드 목록을 순회하며 비동기 로드 후에 추가한다.
            count = b.Count;
            for (var i = 0; i < count; i++)
            {
                var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(b[i]);

                //핸들 작업을 테스크로 보관하여 리스트에 추가한다.
                Task task = WaitGameObjectAssetsLoadedDone(handle, b[i]);
                _tasks.Add(task);
            }
        }

        private void LoadUIAssetsByLabelAsync_Internal(string previous, string next)
        {
            // 로드 된 에셋과 로드 할 에셋의 합집합 차집합을 구한다.
            // 현재 로드된 에셋. 즉, 로드할 에셋의 합집합이다.
            var set = AddressData.Groups[AddressData.Group.UI][previous].Intersect(AddressData.Groups[AddressData.Group.UI][next]).ToList();

            // 로드할 에셋의 차집합. 즉, 로드를 해제할 에셋의 집합이다.
            var a = AddressData.Groups[AddressData.Group.UI][previous].Except(set).ToList();
            // 로드된 에셋의 차집합. 즉, 앞으로 로드를 할 에셋의 집합이다.
            var b = AddressData.Groups[AddressData.Group.UI][next].Except(set).ToList();

            // 언로드 목록에 포함이 되어 있으면 언로드한다.
            var count = a.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                try
                {
                    Addressables.Release(_uiAssets[a[i]]);
                }
                catch
                {
                    Debugger.LogMessage("메모리에 없음");
                }
            }

            // 로드 목록을 순회하며 비동기 로드 후에 추가한다.
            count = b.Count;
            for (var i = 0; i < count; i++)
            {
                var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(b[i]);

                //핸들 작업을 테스크로 보관하여 리스트에 추가한다.
                Task task = WaitUIAssetsLoadedDone(handle, b[i]);
                _tasks.Add(task);
            }
        }

        private void LoadDataAssetsByLabelAsync_Internal(string previous, string next)
        {
            // 로드 된 에셋과 로드 할 에셋의 합집합 차집합을 구한다.
            // 현재 로드된 에셋. 즉, 로드할 에셋의 합집합이다.
            var set = AddressData.Groups[AddressData.Group.Data][previous].Intersect(AddressData.Groups[AddressData.Group.Data][next]).ToList();

            // 로드할 에셋의 차집합. 즉, 로드를 해제할 에셋의 집합이다.
            var a = AddressData.Groups[AddressData.Group.Data][previous].Except(set).ToList();
            // 로드된 에셋의 차집합. 즉, 앞으로 로드를 할 에셋의 집합이다.
            var b = AddressData.Groups[AddressData.Group.Data][next].Except(set).ToList();

            // 언로드 목록에 포함이 되어 있으면 언로드한다.
            var count = a.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                try
                {
                    Addressables.Release(_dataAssets[a[i]]);
                }
                catch
                {
                    Debugger.LogMessage("메모리에 없음");
                }
            }

            // 로드 목록을 순회하며 비동기 로드 후에 추가한다.
            count = b.Count;
            for (var i = 0; i < count; i++)
            {
                var handle = Addressables.LoadAssetAsync<UnityEngine.Object>(b[i]);

                //핸들 작업을 테스크로 보관하여 리스트에 추가한다.
                Task task = WaitDataAssetsLoadedDone(handle, b[i]);
                _tasks.Add(task);
            }
        }

        // 각 핸들의 진행 상태가 완료 되면 _assets의 추가 및 진행률 셋팅
        private async Task WaitGameObjectAssetsLoadedDone(AsyncOperationHandle<UnityEngine.Object> handle, string key)
        {
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GetProgress_Internal();

                _gameObjectAssets[key] = handle.Result;
            }
        }

        private async Task WaitUIAssetsLoadedDone(AsyncOperationHandle<UnityEngine.Object> handle, string key)
        {
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GetProgress_Internal();

                _uiAssets[key] = handle.Result;
            }
        }

        private async Task WaitDataAssetsLoadedDone(AsyncOperationHandle<UnityEngine.Object> handle, string key)
        {
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GetProgress_Internal();

                _dataAssets[key] = handle.Result;
            }
        }

        private float GetProgress_Internal()
        {
            if (_tasks.Count == 0)
            {
                return 1f;
            }

            // 로딩 중이라면 목록 내부의 Task를 순회 하며 로딩 완료 된 Task 개수를 확인한다.
            int completed = _tasks.Count(t => t.IsCompleted);

            // 로딩 완료된 갯수 / 총 갯수 를 나눠 반환 (퍼센트 구하는 공식)
            var percentage = completed / (float)_tasks.Count;
            Debugger.LogMessage($"{percentage:N2}");

            return (float)completed / _tasks.Count;
        }

        private T GetGameObjectAsset_Internal<T>(string key) where T : UnityEngine.Object
        {
            if (_gameObjectAssets.TryGetValue(key, out UnityEngine.Object obj))
            {
                try
                {
                    return (T)obj;
                }
                catch
                {
                    Debugger.LogMessage($"{key} 주소를 가진 에셋을 {typeof(T).Name} 타입으로 변환할 수 없습니다.");

                    return null;
                }
            }

            Debugger.LogMessage($"{key} 주소를 가진 에셋이 없습니다.");

            return null;
        }

        private T GetUIAsset_Internal<T>(string key) where T : UnityEngine.Object
        {
            if (_uiAssets.TryGetValue(key, out UnityEngine.Object obj))
            {
                try
                {
                    return (T)obj;
                }
                catch
                {
                    Debugger.LogMessage($"{key} 주소를 가진 에셋을 {typeof(T).Name} 타입으로 변환할 수 없습니다.");

                    return null;
                }
            }

            Debugger.LogMessage($"{key} 주소를 가진 에셋이 없습니다.");

            return null;
        }

        private T GetDataAsset_Internal<T>(string key) where T : UnityEngine.Object
        {
            if (_dataAssets.TryGetValue(key, out UnityEngine.Object obj))
            {
                try
                {
                    return (T)obj;
                }
                catch
                {
                    Debugger.LogMessage($"{key} 주소를 가진 에셋을 {typeof(T).Name} 타입으로 변환할 수 없습니다.");

                    return null;
                }
            }

            Debugger.LogMessage($"{key} 주소를 가진 에셋이 없습니다.");

            return null;
        }

        public bool IsLoadedDone_Internal => _tasks.All(task => task.IsCompleted);

        public static void LoadAssetsByLabelAsync(string current)
        {
            Instance.LoadGameObjectAssetsByLabelAsync_Internal(current);
            Instance.LoadUIAssetsByLabelAsync_Internal(current);
            Instance.LoadDataAssetsByLabelAsync_Internal(current);
        }

        /// <summary>
        /// 로드 할 씬에 필요한 에셋을 로드 하는 함수.
        /// </summary>
        /// <param name="previous">현재 로드 된 씬의 이름.</param>
        /// <param name="next">로드 할 씬의 이름.</param>
        public static void LoadAssetsByLabelAsync(string previous, string next)
        {
            Instance.LoadGameObjectAssetsByLabelAsync_Internal(previous, next);
            Instance.LoadUIAssetsByLabelAsync_Internal(previous, next);
            Instance.LoadDataAssetsByLabelAsync_Internal(previous, next);
        }

        /// <summary>
        /// 진행률 셋팅 하는 함수.
        /// </summary>
        /// <returns>얼마나 진행 되었는지를 백분율로 반환.</returns>
        public static float GetProgress()
        {
            return Instance.GetProgress_Internal();
        }

        public static bool IsLoadedDone => Instance.IsLoadedDone_Internal;

        /// <summary>
        /// 필요한 에셋을 지정한 타입으로 받아오기 위한 함수.
        /// </summary>
        /// <typeparam name="T">원하는 타입.</typeparam>
        /// <param name="key">원하는 에셋의 주소.</param>
        /// <returns>필요한 에셋을 지정한 타입으로 반환.</returns>
        public static T GetGameObjectAsset<T>(string key) where T : UnityEngine.Object
        {
            return Instance.GetGameObjectAsset_Internal<T>(key);
        }

        public static T GetUIAsset<T>(string key) where T : UnityEngine.Object
        {
            return Instance.GetUIAsset_Internal<T>(key);
        }

        public static T GetDataAsset<T>(string key) where T : UnityEngine.Object
        {
            return Instance.GetDataAsset_Internal<T>(key);
        }
    }
}
