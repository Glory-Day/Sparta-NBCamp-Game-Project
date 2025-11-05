using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Backend.Object.Process
{
    public class TestBootLoad : MonoBehaviour
    {
        [SerializeReference] private List<IProcessable> progresses;

        private void Awake()
        {
            var count = progresses.Count;
            for (var i = 0; i < count; i++)
            {
                //progresses[i].Run();
            }
        }
    }
}
