using System;
using System.Collections.Generic;
using Backend.Object.Management;
using Backend.Object.NPC;
using Backend.Util.Debug;
using Script.Object.Character.Player;
using Script.Util.Extension;
using UnityEngine;

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

        public async void Run(int id)
        {
            try
            {
                Debugger.LogMessage("Starting bootstrap...");

                IsDone = false;

                ObjectPoolManager.ReleaseAll();

                Debugger.LogMessage("All pooling objects are released.");
                Debugger.LogMessage("All processes are running.");

                await _progresses[0].Running().AsTask(this);

                Debugger.LogMessage("Binding user interface process is completed.");

                var hub = FindAnyObjectByType<PlayerCharacterSpawnerHub>();
                var data = hub.GetSpawnData(id);

                ((BindingPlayerCharacterProcess)_progresses[1]).Data = data;

                await _progresses[1].Running().AsTask(this);

                var target = ((BindingPlayerCharacterProcess)_progresses[1]).Target;
                var composer = target.GetComponent<PlayerCharacterComposer>();
                hub.Bind(composer);

                Debugger.LogMessage("Binding player character process is completed.");

                await _progresses[2].Running().AsTask(this);

                Debugger.LogMessage("Binding enemy character process is completed.");

                ((BindingBossEnemyCharacterProcess)_progresses[3]).Target = target;
                await _progresses[3].Running().AsTask(this);

                Debugger.LogSuccess("All processes are completed.");

                IsDone = true;
            }
            catch (Exception exception)
            {
                Debugger.LogError(exception.Message);
            }
        }

        public bool IsDone { get; private set; }
    }
}
