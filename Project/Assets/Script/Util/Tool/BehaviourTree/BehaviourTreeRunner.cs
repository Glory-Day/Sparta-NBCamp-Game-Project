using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        private float _hitStunTime;
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

            component.Status.OnEnemyHit += EnemyHit;
            component.Status.OnEnemyHit += component.AnimationController.PlayHitAnimation;

            _hitStunTime = component.Status.HitStunTime;

            if (component.MovementController.IsGetUp)
            {
                StartCoroutine(GetUpAfterDelay(0.1f));
            }
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

        public void EnemyHit()
        {
            stop = true;
            GetComponent<NavMeshAgent>().isStopped = true;
            StartCoroutine(ResumeBehaviourTreeAfterDelay(_hitStunTime));
        }
        private IEnumerator ResumeBehaviourTreeAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            stop = false;
            GetComponent<NavMeshAgent>().isStopped = false;
        }

        private IEnumerator GetUpAfterDelay(float delay)
        {
            WaitForSeconds wait = new WaitForSeconds(delay);

            stop = true;
            yield return wait;
            EnemyAnimationController animationController = GetComponent<EnemyAnimationController>();
            EnemyMovementController movementController = GetComponent<EnemyMovementController>();
            int SkillName = Animator.StringToHash("GetUp");

            animationController.SetCrossFadeInFixedTime(SkillName, 0f); // 공격 애니메이션 재생
            animationController.SetAnimatorSpeed(0f);

            while (movementController.Target == null)
            {
                yield return wait;
            }
            animationController.SetAnimatorSpeed(1f);
        }

        // GetUp 애니메이션이 끝난 후 호출되는 이벤트 메서드
        public void OnGetUpAnimationEnd()
        {
            stop = false;
        }
    }
}
