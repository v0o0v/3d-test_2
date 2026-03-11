using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyState: EnemyState, ICharacterState
{
    public PatrolEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent) 
        : base(enemyController, animator, navMeshAgent) { }

    public void Enter()
    {
        _animator.SetBool(EnemyController.EnemyAniParamPatrol, true);
    }

    public void Update()
    {
        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance){
            _enemyController.SetState((EnemyController.EEnemyState.Idle));
        }
        
    }

    public void Exit()
    {
        _animator.SetBool(EnemyController.EnemyAniParamPatrol, false);
    }
}