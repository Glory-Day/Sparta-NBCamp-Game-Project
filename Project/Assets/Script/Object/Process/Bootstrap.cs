using System;
using System.Collections;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Util.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = Backend.Object.Management.SceneManager;

namespace Backend.Object.Process
{
    public class Bootstrap : MonoBehaviour
    {
        private readonly List<IProcessable> _progresses = new ()
        {
            new BindingUserInterfaceProcess(),
            new BindingPlayerCharacterProcess(),
            new BindingEnemyCharacterProcess(),
            new BindingBossEnemyCharacterProcess()
        };

        public void Run()
        {
            StartCoroutine(Booting());
        }

        private IEnumerator Booting()
        {
            IsDone = false;

            ObjectPoolManager.ReleaseAll();

            _progresses[0].Run();

            yield return null;

            _progresses[1].Run();
            var target = ((BindingPlayerCharacterProcess)_progresses[1]).Target;

            yield return null;

            _progresses[2].Run();

            yield return null;

            ((BindingBossEnemyCharacterProcess)_progresses[3]).Target = target;
            _progresses[3].Run();

            IsDone = true;
        }

        public bool IsDone { get; private set; }
    }
}
