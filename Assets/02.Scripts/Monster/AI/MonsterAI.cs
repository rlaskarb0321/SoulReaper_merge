using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 몬스터들의 다음 행동 실행을 위해 몬스터 뇌의 욕구를 설정하는 클래스
public class MonsterAI : MonoBehaviour
{

    // 몬스터가 하고자하는 욕구들의 종류
    public enum eMonsterDesires { Patrol, Idle, Trace, Attack, Defense, Dead } 
    [SerializeField] private eMonsterDesires _monsterBrain;
    public eMonsterDesires MonsterBrain 
    {
        get { return _monsterBrain; }
        set // MonsterBrain값을 변경하면서 각 상태의맞는 이동속도값으로 수정
        {
            if (value != eMonsterDesires.Attack && _monsterBase._isActing)
                _monsterBase._isActing = false;

            _monsterBrain = value;
            switch (value)
            {
                case eMonsterDesires.Patrol:
                    _monsterBase._nav.speed = _monsterBase._basicStat._patrolMovSpeed;
                    break;
                case eMonsterDesires.Trace:
                    _monsterBase._nav.speed = _monsterBase._basicStat._traceMovSpeed;
                    break;
                case eMonsterDesires.Attack:
                    break;
                case eMonsterDesires.Defense:
                    _monsterBase._nav.speed = _monsterBase._basicStat._kitingMovSpeed;
                    break;
            }
        }
    }
    [HideInInspector] public bool _isTargetSet;
    [HideInInspector] public Transform _target;
    [HideInInspector] public Vector3 _patrolPos;
    [Range(2.0f, 10.0f)] public float _idleTime; // 행동후 다음행동까지 기다리는 시간값
    [Range(0.0f, 1.0f)]public float _needDefenseHpPercentage; // 해당값 이하일때 방어(카이팅, 가드)가 필요하다고 생각하게되는 hp 퍼센티지


    // Field
    Monster _monsterBase;
    NavMeshAgent _nav;
    int _playerSearchLayer;
    int _soulOrbSearchLayer;
    bool _isFindPatrolPos;
    [SerializeField] bool _needDefense;

    void Awake()
    {
        _monsterBase = GetComponent<Monster>();
        _nav = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _playerSearchLayer = 1 << LayerMask.NameToLayer("PlayerTeam");
        _soulOrbSearchLayer = 1 << LayerMask.NameToLayer("SoulOrb");
        MonsterBrain = eMonsterDesires.Patrol;
        
    }

    void Update()
    {
        if (MonsterBrain == eMonsterDesires.Dead)
            return;

        DetermineDesires();
    }

    // 몬스터가 다음에 행동하고자 할 행동을 실행하기위해 몬스터의 욕구를 결정
    // 욕구결정에 구현해야하는것 : 몹의 선공여부, 몹의 현재체력값, 타 몬스터의 영혼이 필드에있는지
    void DetermineDesires()
    {
        #region 23/04/17 몬스터 Brain 작동방식 전환
        //Collider[] detectedColls;
        //float targetDist;

        //// 타겟을 감지하지 못했을경우에는 Patrol
        //if (!_isTargetSet)
        //{
        //    // 구체형 콜리더와 닿는것들중에 PlayerTeam이라는 레이어값을 가진 요소들만 배열에 추가
        //    detectedColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat._traceRadius, _playerTeamLayer);

        //    if (detectedColls.Length >= 1)
        //    {
        //        MonsterBrain = eMonsterDesires.Trace;
        //        _isTargetSet = true;
        //        _target = detectedColls[0].transform;
        //    }
        //    else
        //    {
        //        MonsterBrain = eMonsterDesires.Patrol;
        //    }
        //}

        //// 타겟을 감지한경우
        //else
        //{
        //    targetDist = Vector3.Distance(transform.position, _target.position);

        //    // 타겟과의 거리에따라 Attack || Trace
        //    if (targetDist <= _monsterBase._basicStat._attakableRadius)
        //        MonsterBrain = eMonsterDesires.Attack;
        //    else
        //        MonsterBrain = eMonsterDesires.Trace;

        //    // 타겟과의 거리에따라 Attack || Trace
        //    //if (_monsterBase._nav.remainingDistance > _monsterBase._basicStat._attakableRadius)
        //    //    MonsterBrain = eMonsterDesires.Trace;
        //    //else
        //    //    print("공격가능");
        //    //    //MonsterBrain = eMonsterDesires.Attack;
        //}
        #endregion 23/04/17 몬스터 Brain 작동방식 전환
        float targetDist;

        switch (MonsterBrain)
        {
            case eMonsterDesires.Idle:
            case eMonsterDesires.Patrol:
                Collider[] detectColls = Physics.OverlapSphere(transform.position, _monsterBase._basicStat._traceRadius,
                    _playerSearchLayer);

                if (detectColls.Length >= 1)
                {
                    MonsterBrain = eMonsterDesires.Trace;
                    _target = detectColls[0].transform;
                }
                else
                {
                    if (!_isFindPatrolPos)
                    {
                        if (SetRandomPoint(transform.position, out _patrolPos, _monsterBase._basicStat._traceRadius))
                        {
                            _isFindPatrolPos = true;
                            MonsterBrain = eMonsterDesires.Patrol;
                        }
                    }
                    else
                    {
                        if (!_nav.pathPending)
                        {
                            if (_nav.remainingDistance <= _nav.stoppingDistance)
                            {
                                if (!_nav.hasPath || _nav.velocity.sqrMagnitude == 0f)
                                {
                                    StartCoroutine(IdlePatrol());
                                }
                            }
                            else
                            {
                                MonsterBrain = eMonsterDesires.Patrol;
                            }
                        }
                    }
                }
                break;

            case eMonsterDesires.Attack:
            case eMonsterDesires.Trace:
                targetDist = Vector3.Distance(transform.position, _target.position);

                if (!_monsterBase._basicStat._isAttackFirst)
                    return;
                if (_monsterBase._isActing)
                    return;
                if (DetermineWhethereNeedDefense(targetDist, (int)_monsterBase._monsterType))
                {
                    MonsterBrain = eMonsterDesires.Defense;
                    break;
                }
                
                if (targetDist <= _monsterBase._basicStat._attakableRadius)
                    MonsterBrain = eMonsterDesires.Attack;
                else
                    MonsterBrain = eMonsterDesires.Trace;
                break;

            case eMonsterDesires.Defense:
                //targetDist = Vector3.Distance(transform.position, _target.position);
                //if (!DetermineWhethereNeedDefense(targetDist, (int)_monsterBase._monsterType))
                //{
                //    MonsterBrain = eMonsterDesires.Trace;
                //    break;
                //}
                break;
            case eMonsterDesires.Dead:
                return;
        }
    }

    bool SetRandomPoint(Vector3 center, out Vector3 destination, float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPos = center + Random.insideUnitSphere * radius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                destination = hit.position;
                return true;
            }
        }
        destination = Vector3.zero;
        return false;
    }

    IEnumerator IdlePatrol()
    {
        if (MonsterBrain == eMonsterDesires.Idle)
            yield break;

        float randomValue = Random.Range(-1.0f, 0.5f);
        WaitForSeconds waitSeconds = new WaitForSeconds(_idleTime + randomValue);
        MonsterBrain = eMonsterDesires.Idle;

        yield return waitSeconds;
        _isFindPatrolPos = false;
    }

    // 몬스터타입별로 상이한 방어모드돌입 조건
    bool DetermineWhethereNeedDefense(float targetDist, int monsterType)
    {
        bool needDefense = false;

        switch ((Monster.eMonsterType)monsterType)
        {
            case Monster.eMonsterType.Melee:
                needDefense = _monsterBase._currHp / _monsterBase._basicStat._health < _needDefenseHpPercentage ? true : false;
                return needDefense;

            case Monster.eMonsterType.Range:
                needDefense = targetDist <= _monsterBase._basicStat._attakableRadius * 0.5f ? true : false;
                return needDefense;

            case Monster.eMonsterType.Charge:
                return needDefense;

            case Monster.eMonsterType.MeleeAndRange:
                return needDefense;
            
            default:
                return needDefense;
        }

    }
}
