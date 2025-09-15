using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Util.Debug;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Backend.Util.Management
{
    public static class LabelAssets
    {
        public static List<string> GetAssetsByLabel(string label)
        {
            List<string> assetNames = new List<string>();
            if (label == "aaa")
            {
                assetNames.Add("Cube");
                assetNames.Add("Assets/Script/Test/Cube 1.prefab");
                assetNames.Add("Assets/Script/Test/Cube 2.prefab");
                assetNames.Add("Assets/Script/Test/Cube 3.prefab");
                assetNames.Add("Sliding");
                assetNames.Add("Square");
            }
            else if (label == "bbb")
            {
                assetNames.Add("Cube");
            }
            return assetNames;
        }
    }

    public class ResourceManager : Singleton<ResourceManager>
    {
        private ResourceManager()
        {
        }

        private Dictionary<string, UnityEngine.Object> _assets = new();
        private List<string> _list = new();
        private List<Task> _tasks = new();

        private void LoadAssetsByLabelAsync_Internal(string label)
        {
            // 1. Label에 맞는 Asset 주소 목록을 _list로 가져오기
            List<string> _list = LabelAssets.GetAssetsByLabel(label);

            // 2. _assets에 이미 포함된 것은 _list에서 제거
            // 3. Label 목록에 포함이 안되어 있으면 UnLoad 한다.
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (_assets.ContainsKey(_list[i]))
                {
                    _list.RemoveAt(i);
                }
                else
                {
                    try
                    {
                        Addressables.Release(_assets[_list[i]]);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            // 4. _list 순회하며 비동기 Load 후 _assets에 추가
            for (int i = 0; i < _list.Count; i++)
            {
                AsyncOperationHandle<UnityEngine.Object> handle = Addressables.LoadAssetAsync<UnityEngine.Object>(_list[i]);

                Task task = SetTask(handle, _list[i]); //핸들 작업을 테스크로 바꾸기
                _tasks.Add(task);   //리스트에 담기
            }
        }

        // 각 핸들의 진행 상태가 완료 되면 _assets의 추가 및 진행률 셋팅
        private async Task SetTask(AsyncOperationHandle<UnityEngine.Object> handle, string key)
        {
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GetProgress_Internal();
                _assets[key] = handle.Result;
            }
        }

        // 진행률 셋팅 하는 함수
        private float GetProgress_Internal()
        {
            // 진행률을 전달해야함. Task를 이용할 것
            // 업데이트에서 돌리는 게 아님. 가능한가? 이게 async로 계속 반환 해줘야 하나?
            // 그럼 async Task<float> 로 해야 하나?
            // 여기서 for문을 돌리나? awit를 써서? 그럼 여기서 반환 되는 값은 하나 아닌가?

            //이미 로딩이 다 되어있으면 바로 1 반환
            if (_tasks.Count == 0)
            {
                return 1f;
            }

            int completed = 0;

            // 로딩 중이라면 List 내부의 Task를 순회 하며 로딩 완료 된 Task 개수를 확인
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].IsCompleted)
                {
                    completed++;
                }
            }

            // 로딩 완료된 갯수 / 총 갯수 를 나눠 반환 (퍼센트 구하는 공식)
            Debugger.LogMessage(((float)completed / _tasks.Count).ToString());
            return ((float)completed / _tasks.Count);
        }

        private T GetAsset_Internal<T>(string key) where T : UnityEngine.Object
        {
            if (_assets.TryGetValue(key, out UnityEngine.Object obj))
            {
                try
                {
                    return (T)obj;  //변환 해보고 안되면 catch
                }
                catch
                {
                    Debugger.LogMessage($"{key} 주소를 가진 에셋을 {typeof(T).Name} 타입으로 변환할 수 없습니다.");
                    return null;
                }
            }
            else
            {
                Debugger.LogMessage($"{key} 주소를 가진 에셋이 없습니다.");
                return null;
            }
        }

        public static void LoadAssetsByLabelAsync(string label)
        {
            Instance.LoadAssetsByLabelAsync_Internal(label);
        }

        public static float GetProgress()
        {
            return Instance.GetProgress_Internal();
        }

        public static T GetAsset<T>(string key) where T : UnityEngine.Object
        {
            return Instance.GetAsset_Internal<T>(key);
        }
    }
}
