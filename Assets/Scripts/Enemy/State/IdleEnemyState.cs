using UnityEngine;
using UnityEngine.AI;

public class IdleEnemyState : EnemyState, ICharacterState {

    private float _waitTime;

    public IdleEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
        : base(enemyController, animator, navMeshAgent){ }

    public void Enter(){
        _waitTime = 0f;
        _animator.SetBool(EnemyController.EnemyAniParamIdle, true);
    }

    public void Update(){
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();

        //chase 체크
        if (detectionTargetTransform){
            _navMeshAgent.SetDestination((detectionTargetTransform.position));
            _enemyController.SetState(EnemyController.EEnemyState.Chase);
        }

        //patrol 체크
        else if (_waitTime > _enemyController.PatrolWaitTime){
            var randomValue = Random.Range(0, 100);
            if (randomValue < _enemyController.PatrolChance){
                // 정찰 시작
                var patrolPosition = FindRandomPatrolPosition();
                // 정찰 위치가 현 위치에서 2unit 이상일 경우에만 정찰 시작
                // patrolPosition(정찰 목적지 좌표)에서 _enemyController.transform.position(현재 적의 위치 좌표)를 빼서, 현재 위치에서 목적지로 향하는 벡터를 구합니다.
                var realDistance = Vector3.SqrMagnitude(patrolPosition - _enemyController.transform.position);
                var minimumDistance = _navMeshAgent.stoppingDistance + 2;
                if (realDistance > minimumDistance * minimumDistance){
                    _navMeshAgent.SetDestination((patrolPosition));
                    _enemyController.SetState(EnemyController.EEnemyState.Patrol);
                }
            }

            _waitTime = 0;
        }

        _waitTime += Time.deltaTime;
    }

    public void Exit(){
        _animator.SetBool(EnemyController.EnemyAniParamIdle, false);
    }

    // 정찰 목적지를 찾는 함수
    private Vector3 FindRandomPatrolPosition(){
        Vector3 randomDirection = Random.insideUnitSphere * _enemyController.PatrolDetectionDistance;
        randomDirection += _enemyController.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, _enemyController.PatrolDetectionDistance,
                NavMesh.AllAreas)){
            return hit.position;
        }
        else{
            return _enemyController.transform.position;
        }
    }

}