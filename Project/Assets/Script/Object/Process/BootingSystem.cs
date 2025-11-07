using System.Collections;
using System.Linq;
using Backend.Object.Management;
using Backend.Object.UI;
using Backend.Util.Data;
using Backend.Util.Debug;
using Backend.Util.Management;
using Unity.VisualScripting;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Backend.Object.Process
{
    public class BootingSystem : MonoBehaviour
    {
        private void Awake()
        {
            Debugger.LogProgress();

            StartCoroutine(Booting());
        }

        private IEnumerator Booting()
        {
            if (DataManager.IsSettingDataExisted == false)
            {
                DataManager.ResetSettingData();
            }

            var currentSceneName = SceneManager.GetActiveScene().name;
            ResourceManager.LoadAssetsByLabelAsync(currentSceneName);

            while (ResourceManager.IsLoadedDone == false)
            {
                yield return null;
            }

            var names = AddressData.Groups[AddressData.Group.UI][currentSceneName].ToArray();
            for (var i = 0; i < names.Length; i++)
            {
                var asset = ResourceManager.GetUIAsset<GameObject>(names[i]);
                UIManager.AddAsDefault(names[i], asset.GetComponent<Window>());
            }

            var value = UIManager.GetDefaultWindow(AddressData.Assets_Prefab_UI_Main_Window_Prefab);
            var window = value.GetComponent<MainWindow>();

            window.StartButton.onClick.AddListener(LoadNewData);
            window.LoadButton.onClick.AddListener(LoadData);
            window.SetLoadButtonInteractable(DataManager.IsUserDataExisted);
            window.Open();
        }

        private void LoadNewData()
        {
            DataManager.ResetUserData();
            DataManager.ResetStatusData();
        }

        private void LoadData()
        {
            DataManager.LoadUserData();
            DataManager.LoadStatusData();
        }
    }
}
