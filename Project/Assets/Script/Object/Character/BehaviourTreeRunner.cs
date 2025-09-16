using UnityEngine;
using Ironcow.Synapse.BT;

public partial class BehaviourTreeRunner : BTActionsBase
{
    [Header("Behaviour Tree Reference")]
    [SerializeField] private BTRunner runner;

    protected virtual void Awake()
    {
        runner.SetRoot(runner.lastData);
        runner.SetActions(this);
    }

    protected virtual void Update()
    {
        runner.Operate();
    }
}
