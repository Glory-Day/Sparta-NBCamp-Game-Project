//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.Serialization;
//using Backend.Object.Character.Enemy.Boss.Skill;
//using Backend.Util;
//using Backend.Util.Data.ActionDatas;
//using Backend.Util.Debug;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//namespace Backend.Object.Character.Enemy.Boss
//{
//    public partial class NineTailBeast : BehaviourTreeRunner
//    {
//        //디버깅용
//        private enum States
//        {
//            Idle,
//            Attack,
//            Move,
//            Grap,
//            EnergyExplose,
//        }

//        private GameObject _target;
//        private BossEnemyStatus _status;
//        private AnimationController _animationController;
//        private BossMovementController _movementController;

//        private readonly Dictionary<string, CoolDownTimer> _cooltimer = new(); // 스킬 쿨다운 타이머 딕셔너리
//        private readonly Dictionary<string, ActionBossData> _skillCache = new(); // 가지고 있는 스킬 딕셔너리
//        [SerializeField] private List<ActionBossData> _usableSkills = new(); // 쿨다운이 끝난 스킬들

//        // BT 노드 간 데이터 전달을 위한 멤버 변수
//        private ActionBossData _selectedSkill;
//        private bool _isSkillExecuting; // 스킬 실행 중 여부
//        private Coroutine _skillCoroutine; // 스킬 실행 코루틴 참조

//        private BossSkillBase[] _bossSkills; // 모든 스킬 컴포넌트 참조

//        // 스트레이핑 사용 여부
//        private bool _useStrafeing = false;

//        private void OnDrawGizmosSelected()
//        {
//            if (_status.BossStatus == null || _status.BossStatus.AttackRange == null)
//            {
//                return;
//            }

//            // 기즈모 색상을 빨간색으로 설정합니다.
//            Gizmos.color = Color.red;

//            // AttackRange 배열에 정의된 각 사거리에 대해 원을 그립니다.
//            foreach (var range in _status.BossStatus.AttackRange)
//            {
//                // 3D 공간에서 XZ 평면을 기준으로 원형 선을 그립니다.
//                Gizmos.DrawWireSphere(transform.position, range);
//            }
//        }

//        protected override void Awake()
//        {
//            base.Awake();
//            _status = GetComponent<BossEnemyStatus>();
//            _animationController = GetComponent<AnimationController>();
//            _movementController = GetComponent<BossMovementController>();
//            _bossSkills = GetComponents<BossSkillBase>();
//            _isSkillExecuting = false;
//        }

//        private void Start()
//        {
//            SkillInit();
//            _movementController.Init(_status.BossStatus.Speed, _status.BossStatus.TurnSpeed);
//        }

//        protected override void Update()
//        {
//            base.Update();
//            //test
//            Debugger.LogSuccess("동작중?");
//        }

//        private void SkillInit()
//        {
//            for (int i = 0; i < _bossSkills.Length; i++)
//            {
//                _skillCache.Add(_bossSkills[i].SkillData.ID, _bossSkills[i].SkillData); //id, skill
//                _cooltimer.Add(_bossSkills[i].SkillData.ID, new CoolDownTimer(_bossSkills[i].SkillData.CoolDown)); // id, coolTime

//                _bossSkills[i].OnSkillEnd += () =>
//                {
//                    _isSkillExecuting = false;
//                };
//            }
//        }
//    }
//}
