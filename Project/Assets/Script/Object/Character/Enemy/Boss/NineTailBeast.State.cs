//using System.Linq;
//using Backend.Util.Data.ActionDatas;
//using Backend.Util.Debug;
//using Ironcow.Synapse;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//namespace Backend.Object.Character.Enemy.Boss
//{

//    public partial class NineTailBeast
//    {
//        public eNodeState Dead(DataContext context)
//        {
//            if (_status.IsDead())
//            {
//                return eNodeState.success;
//            }
//            return eNodeState.failure;
//        }
//        // 행동이 실행 중인지 여부를 나타내는 노드
//        public eNodeState IsSkillExecuting(DataContext context)
//        {
//            if (_isSkillExecuting || _movementController.IsStrafeing)
//            {
//                return eNodeState.failure;
//            }
//            else
//            {
//                return eNodeState.success;
//            }
//        }

//        // 쿨다운 확인 후 사용 가능한 스킬 리스트에 추가
//        public eNodeState CoolDownCheck(DataContext context) //특수 스킬, 노말 스킬, 10, 20, 0, 0
//        {
//            if (_isSkillExecuting)
//            {

//                return eNodeState.failure;
//            }

//            foreach (string cool in _cooltimer.Keys)
//            {
//                if (!_cooltimer[cool].IsCoolingDown)
//                {
//                    _usableSkills.Add(_skillCache[cool]);
//                }
//            }
//            if (_usableSkills.Count <= 0)
//            {
//                return eNodeState.failure;
//            }
//            else
//            {
//                return eNodeState.success;
//            }
//        }

//        // 거리 확인 후 사용 가능한 스킬 리스트에서 사거리 밖 스킬 제거
//        public eNodeState SkillDistanceCheck(DataContext context)
//        {
//            for (int i = _usableSkills.Count - 1; i >= 0; i--)
//            {
//                ActionBossData skill = _usableSkills[i];

//                int index = (int)skill.ActionRange;

//                float maxDis = _status.BossStatus.AttackRange[index];
//                float minDis = index == 0 ? 0f : _status.BossStatus.AttackRange[index - 1];

//                bool isInRange = _movementController.Distance > minDis && _movementController.Distance <= maxDis;

//                if (!isInRange)
//                {
//                    _usableSkills.RemoveAt(i);
//                }
//            }

//            if (_usableSkills.Count <= 0)
//            {
//                return eNodeState.failure;
//            }
//            else
//            {
//                return eNodeState.success;
//            }
//        }

//        // 우선순위가 가장 높은 스킬 설정
//        public eNodeState SkillPriorityCheck(DataContext context)
//        {
//            // 스킬 리스트 랜덤으로 섞기
//            //_usableSkills = _usableSkills.OrderBy(x => Random.value).ToList();

//            float totalWeight = 0f; // 전체 가중치 합산
//            foreach (var skill in _usableSkills)
//            {
//                if (skill.AttackWeight == 0)
//                {
//                    _selectedSkill = skill;
//                    return eNodeState.success;
//                }

//                totalWeight += skill.AttackWeight;
//            }

//            float randomPoint = Random.Range(0, totalWeight); // 0부터 전체 가중치 사이의 랜덤 값 생성
//            foreach (var skill in _usableSkills)
//            {
//                if (randomPoint <= skill.AttackWeight) // Random < 40
//                {
//                    _selectedSkill = skill;
//                    return eNodeState.success;
//                }
//                randomPoint -= skill.AttackWeight; // 가중치만큼 감소 Random - 40  
//            }

//            return eNodeState.failure;
//        }

//        // 랜덤으로 스킬 선택
//        public eNodeState SelectRandomSkill(DataContext context)
//        {
//            Debug.Log($"스킬 선택: {_selectedSkill.Name} \n플레이어와 거리: {_movementController.Distance}");

//            // 사용후 초기화 및 쿨다운 시작
//            _usableSkills.Clear();
//            if (_selectedSkill.CoolDown > 0)
//            {
//                _cooltimer[_selectedSkill.ID].Start();
//            }
//            return eNodeState.success;
//        }

//        // 선택된 스킬을 실행 
//        public eNodeState ExecuteSkill(DataContext context)
//        {
//            if (_selectedSkill == null)
//            {
//                _usableSkills.Clear();
//                return eNodeState.failure;
//            }

//            if (_skillCoroutine == null)
//            {
//                // _animationController.SetAnimationBoolean("IsStrafing", false); // 스킬 실행 시 스트레이핑 중지

//                _isSkillExecuting = true;
//                foreach (var skill in _bossSkills)
//                {
//                    if (skill.SkillData.ID == _selectedSkill.ID)
//                    {

//                        _skillCoroutine = StartCoroutine(skill.ExecuteSkill(_animationController));
//                        Debug.Log($"스킬 실행: {_selectedSkill.Name}");

//                        _usableSkills.Clear();
//                        if (_selectedSkill.CoolDown > 0)
//                        {
//                            _cooltimer[_selectedSkill.ID].Start();
//                        }
//                        _movementController.StopMovement();
//                        break;
//                    }
//                }
//            }

//            if (_isSkillExecuting)
//            {
//                _usableSkills.Clear();
//                _skillCoroutine = null;
//                _selectedSkill = null;
//                _useStrafeing = false;
//                return eNodeState.success;
//            }
//            Debug.Log("스킬 실행 대기중...");
//            return eNodeState.running;
//        }

//        public eNodeState Distance(DataContext context)
//        {
//            return eNodeState.success;
//        }

//        public eNodeState TurnToPlayer(DataContext context)
//        {
//            Debugger.LogSuccess("회전");
//            //transform.LookAt(new Vector3(TestTarget.transform.position.x, 0, TestTarget.transform.position.z));
//            return eNodeState.success;
//        }

//        public eNodeState Chance(DataContext context)
//        {
//            Debugger.LogSuccess("확률");
//            float rand = Random.Range(0f, 1f);
//            if (rand < 0.3f)
//            {
//                return eNodeState.success;
//            }
//            return eNodeState.failure;
//        }

//        public eNodeState UnderHp(DataContext context)
//        {
//            Debugger.LogSuccess("체력 측정");
//            return eNodeState.success;
//        }

//        public eNodeState Wait(DataContext context)
//        {
//            Debugger.LogSuccess("대기");
//            return eNodeState.success;
//        }

//        public eNodeState DeathState(DataContext context)
//        {
//            Debugger.LogSuccess("사망");
//            //if (CurrentHp <= 0)
//            //{
//            //    return eNodeState.success;
//            //}
//            return eNodeState.failure;
//        }

//        public eNodeState RunToTarget(DataContext context)
//        {
//            // 스킬 실행 중이면 이동 실패
//            if (_isSkillExecuting)
//            {
//                return eNodeState.failure;
//            }

//            // 타겟이 사거리 내에 있으면 이동 실패
//            if (_movementController.Distance <= _status.BossStatus.AttackRange[0])
//            {
//                _movementController.StopMovement();
//                return eNodeState.failure;
//            }

//            _movementController.MoveToTarget();
//            return eNodeState.success;
//        }

//        // 타겟 방향으로 즉시 회전
//        public eNodeState SetRotation(DataContext context)
//        {
//            if (_isSkillExecuting)
//            {
//                return eNodeState.failure;
//            }
//            _movementController.SetRotation();
//            return eNodeState.success;
//        }
//        public bool ShouldStrafe()
//        {
//            return _movementController.Distance < 15f && Random.value < 0.5f;
//        }

//        // 스트레이핑 행동 노드
//        public eNodeState Strafe(DataContext context)
//        {
//            if (!ShouldStrafe() || _useStrafeing)
//            {
//                _useStrafeing = true;
//                return eNodeState.failure;
//            }
//            _useStrafeing = true;
//            _movementController.Strafe(Random.value < 0.5f ? -1 : 1, 1.5f); // 랜덤 방향으로 1.5초간 스트레이핑
//            // 스트레이핑 애니메이션 재생

//            // _animationController.SetAnimationBoolean("IsStrafing", true);

//            // 이는 MovementController에서 코루틴 종료 시 콜백을 호출하거나, Update에서 상태를 체크하여 처리할 수 있습니다.
//            return eNodeState.success;
//        }

//    }
//}
