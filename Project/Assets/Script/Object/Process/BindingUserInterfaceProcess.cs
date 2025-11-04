using System.Linq;
using System.Collections;
using Backend.Object.Management;
using Backend.Object.UI;
using Backend.Util.Data;
using Backend.Util.Debug;
using UnityEngine;

namespace Backend.Object.Process
{
    public class BindingUserInterfaceProcess : IProcessable
    {
        public IEnumerator Running()
        {
            Debugger.LogProgress();

            var currentSceneName = SceneManager.CurrentSceneName;
            var names = AddressData.Groups[AddressData.Group.UI][currentSceneName].ToArray();

            for (var i = 0; i < names.Length; i++)
            {
                var asset = Util.Management.ResourceManager.GetUIAsset<GameObject>(names[i]);
                UIManager.Add(names[i], asset.GetComponent<Window>());
            }

            yield return null;
        }
    }
}
