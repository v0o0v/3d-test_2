using UnityEngine;
using UnityEngine.AI;

public class ChaseEnemyState : EnemyState, ICharacterState {

    public ChaseEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
        : base(enemyController, animator, navMeshAgent){ }

    public void Enter(){
        _animator.SetBool(EnemyController.EnemyAniParamChase, true);
    }

    public void Update(){
        var detectionTargetTransform = _enemyController.DetectionTargetInCircle();
        if (detectionTargetTransform){
            //attack 여부 판단

            // 달리기 여부 판단
            if (DetectionTargetInSight(detectionTargetTransform.position)
                && _navMeshAgent.remainingDistance > _enemyController.MinimumRunDistance){
                _animator.SetFloat(EnemyController.EnemyAniParamMoveSpeed, 1);
            }
            else{
                _animator.SetFloat(EnemyController.EnemyAniParamMoveSpeed, 0);
            }

            _navMeshAgent.SetDestination(detectionTargetTransform.position);
        }
        else{
            _enemyController.SetState(EnemyController.EEnemyState.Idle);
        }
    }

    public void Exit(){
        _animator.SetBool(EnemyController.EnemyAniParamChase, false);
    }

    private bool DetectionTargetInSight(Vector3 position){
        var direction = (position - _enemyController.transform.position).normalized;
        //정면 기준으로 시야각 시계방향이나 반시계방향으로 들어오는지 확인. 그래서 반각을 사용함.
        return Vector3.Angle(_enemyController.transform.forward, direction) < _enemyController.DetectionSightAngle;
    }

}