using System.Linq;
using Backend.Object.Management;
using Backend.Object.UI;
using Backend.Util.Data;
using Backend.Util.Debug;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Backend.Object.Process
{
    public class BindingUserInterfaceProcess : IProcessable
    {
        public void Run()
        {
            Debugger.LogProgress();

            UIManager.Clear();

            var currentSceneName = SceneManager.GetActiveScene().name;
            var names = AddressData.Groups[AddressData.Group.UI][currentSceneName].ToArray();

            for (var i = 0; i < names.Length; i++)
            {
                var asset = Util.Management.ResourceManager.GetUIAsset<GameObject>(names[i]);
                UIManager.Add(names[i], asset.GetComponent<Window>());
            }
        }
    }
}
