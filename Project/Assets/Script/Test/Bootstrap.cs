using System.Collections.Generic;
using UnityEngine;

namespace Script.Test
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeReference] private List<Progress> progresses;

        private void Awake()
        {
            var count = progresses.Count;
            for (var i = 0; i < count; i++)
            {
                progresses[i].Boot();
            }
        }
    }
}
