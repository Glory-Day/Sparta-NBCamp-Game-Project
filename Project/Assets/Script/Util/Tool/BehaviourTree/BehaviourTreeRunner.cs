using Backend.Object.Character;
using Backend.Object.Character.Enemy;
using Backend.Object.Character.Enemy.Boss;
using UnityEngine;
using UnityEngine.AI;

namespace GloryDay.BehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree Tree;
        public bool stop;

        private void Awake()
        {
            var component = new EnemyComponent()
            {
                Status = GetComponent<EnemyStatus>(),
                AnimationController = GetComponent<EnemyAnimationController>(),
                MovementController = GetComponent<EnemyMovementController>(),
                CombatController = GetComponent<EnemyCombatController>(),
                NavMeshAgent = GetComponent<NavMeshAgent>()
            };
            Tree = Tree.Clone();
            Tree.Bind(component);

            component.Status.OnEnemyDeath += EnemyDeath;
            component.Status.OnEnemyDeath += component.AnimationController.PlayDeathAnimation;
            component.Status.OnEnemyDeath += component.MovementController.OnEnemyDeath;
        }

        private void Update()
        {
            if (stop)
            {
                return;
            }

            Tree.Update();
        }

        // 몬스터가 죽었을 때 실행 되는 메서드
        public void EnemyDeath()
        {
            stop = true;
            GetComponent<NavMeshAgent>().isStopped = true;
            GetComponent<Collider>().enabled = false;
        }
    }
}
