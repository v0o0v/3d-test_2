using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.State;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct EnemyStatus {

    public int maxHp;
    public int hp;

}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour {

    // AI 관련
    [SerializeField] private float patrolDetectionDistance = 10f; // 정찰 범위
    [SerializeField] private float patrolWaitTime = 1f; // 정찰 대기 시간
    [SerializeField] private float patrolChance = 30f; // 정찰 확률
    [SerializeField] private LayerMask detectionTargetLayerMask; // 추적 대상 레이어마스크
    [SerializeField] private float detectionSightAngle = 30f; // 디텍션 시야 반각(전체 보는 각은 결국 60도로 계산해야함)
    [SerializeField] private float minimumRunDistance = 5f;
    [Header("Status")] [SerializeField] private EnemyStatus enemyStatus;

    public float PatrolDetectionDistance => patrolDetectionDistance;
    public float PatrolWaitTime => patrolWaitTime;
    public float PatrolChance => patrolChance;
    public float DetectionSightAngle => detectionSightAngle;
    public float MinimumRunDistance => minimumRunDistance;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private HPBarController _hpBarController;

    // 상태
    public enum EEnemyState {

        None, Idle, Patrol, Chase, Attack,
        Hit, Dead

    }

    public EEnemyState State{ get; private set; }
    private Dictionary<EEnemyState, ICharacterState> _states;

    // 애니메이터 파라미터
    public static readonly int EnemyAniParamIdle = Animator.StringToHash("idle");
    public static readonly int EnemyAniParamPatrol = Animator.StringToHash("patrol");
    public static readonly int EnemyAniParamChase = Animator.StringToHash("chase");
    public static readonly int EnemyAniParamAttack = Animator.StringToHash("attack");
    public static readonly int EnemyAniParamHit = Animator.StringToHash("hit");
    public static readonly int EnemyAniParamDead = Animator.StringToHash("dead");
    public static readonly int EnemyAniParamMoveSpeed = Animator.StringToHash("move_speed");

    //추적 대상
    private Transform _targetTransform;
    private Collider[] _detectionResults = new Collider[1];

    private void Awake(){
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        // NavMesh Agent 설정
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = true;

        _states = new Dictionary<EEnemyState, ICharacterState>{
            { EEnemyState.Idle, new IdleEnemyState(this, _animator, _navMeshAgent) },
            { EEnemyState.Patrol, new PatrolEnemyState(this, _animator, _navMeshAgent) },
            { EEnemyState.Chase, new ChaseEnemyState(this, _animator, _navMeshAgent) },
            { EEnemyState.Attack, new AttackEnemyState(this, _animator, _navMeshAgent) },
            { EEnemyState.Hit, new HitEnemyState(this, _animator, _navMeshAgent) },
            { EEnemyState.Dead, new DeadEnemyState(this, _animator, _navMeshAgent) },
        };
        SetState(EEnemyState.Idle);

        _targetTransform = null;
        _hpBarController = GetComponent<HPBarController>();
    }

    private void Update(){
        if (State != EEnemyState.Dead && State != EEnemyState.None)
            _states[State].Update();
    }

    /// <summary>
    /// 애니메이터의 루트 모션이 처리된 후 호출되는 이벤트 함수입니다.
    /// 루트 모션에 의한 위치 변화를 NavMeshAgent와 게임 오브젝트의 트랜스폼에 동기화합니다.
    /// </summary>
    private void OnAnimatorMove(){
        // 애니메이터의 계산된 루트 모션 위치를 가져옵니다.
        var position = _animator.rootPosition;

        // NavMeshAgent의 다음 위치를 애니메이터의 루트 위치로 업데이트하여 에이전트가 캐릭터를 따라오게 합니다.
        _navMeshAgent.nextPosition = position;

        // 실제 게임 오브젝트의 위치를 루트 모션 위치로 적용합니다.
        transform.position = position;
    }

    public void SetState(EEnemyState state){
        if (State == state) return;

        if (State != EEnemyState.None) _states[State].Exit();
        State = state;
        if (State != EEnemyState.None) _states[State].Enter();
    }

    // 일정 거리 안에 Player가 있는지 확인 후 있으면 Player의 Transform 정보를 반환
    public Transform DetectionTargetInCircle(){
        if (!_targetTransform){
            // NonAlloc은 새로운 배열을 생성하지 않기 때문에 메모리 사용 효율적.
            Physics.OverlapSphereNonAlloc(transform.position, patrolDetectionDistance, _detectionResults,
                detectionTargetLayerMask);
            _targetTransform = _detectionResults[0]?.transform;
        }
        else{
            // 대상과의 거리를 계산해서 거리가 벗어나면 정보 초기화
            float playerDistance = Vector3.Distance(transform.position, _targetTransform.position);
            if (playerDistance > patrolDetectionDistance){
                _targetTransform = null;
                _detectionResults[0] = null;
            }
        }

        return _targetTransform;
    }

    public void SetHit(int damage, Vector3 attackDirection){
        enemyStatus.hp -= damage;
        _hpBarController.SetHp((float)enemyStatus.hp / enemyStatus.maxHp);

        if (enemyStatus.hp <= 0){
            SetState(EEnemyState.Dead);
        }
        else{
            SetState(EEnemyState.Hit);
            StartCoroutine(Knockback(attackDirection));
        }
    }

    private IEnumerator Knockback(Vector3 direction){
        var kDir = direction;
        float kDistance = 1f;
        float kDuration = 0.2f;
        float elapse = 0f;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + kDir * kDistance;
        targetPos.y = transform.position.y;

        while (elapse < kDuration){
            Vector3 lerpPos = Vector3.Lerp(startPos, targetPos, elapse / kDuration);
            lerpPos.y = startPos.y;
            transform.position = lerpPos;
            elapse += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }

    private void OnDrawGizmos(){
        // 감지 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolDetectionDistance);

        // 시야각
        Gizmos.color = Color.red;
        Vector3 rightDirection = Quaternion.Euler(0, detectionSightAngle, 0) * transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -detectionSightAngle, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, rightDirection * patrolDetectionDistance);
        Gizmos.DrawRay(transform.position, leftDirection * patrolDetectionDistance);
        Gizmos.DrawRay(transform.position, transform.forward * patrolDetectionDistance);

        // Agent 목적지
        if (_navMeshAgent && _navMeshAgent.hasPath){
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_navMeshAgent.destination, 0.5f);
            Gizmos.DrawLine(transform.position, _navMeshAgent.destination);
        }
    }

}