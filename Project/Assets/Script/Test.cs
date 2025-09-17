using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend.Util.Management;
using Backend.Util.Data;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        ResourceManager.LoadAssetsByLabelAsync("aaa", "bbb");
    }

    public void OnClick()
    {
        GameObject go = Instantiate(ResourceManager.GetAsset<GameObject>($"{AddressData.Assets_Prefab_Capsule_Prefab}"));
        GameObject go1 = Instantiate(ResourceManager.GetAsset<GameObject>($"{AddressData.Assets_Prefab_Plane_Prefab}"));
        GameObject go2 = Instantiate(ResourceManager.GetAsset<GameObject>($"{AddressData.Assets_Prefab_Sphere_Prefab}"));
        GameObject go3 = Instantiate(ResourceManager.GetAsset<GameObject>($"{AddressData.Assets_Prefab_Quad_Prefab}"));
    }
}
