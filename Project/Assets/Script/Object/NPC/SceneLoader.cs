using Backend.Object.Management;
using UnityEngine;

namespace Backend.Object.NPC
{
    public class SceneLoader : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private int sceneIndex;

        public void Load()
        {
            SceneManager.LoadSceneByIndex(sceneIndex);
        }
    }
}
