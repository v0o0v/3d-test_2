using UnityEngine;
using UnityEngine.AI;

namespace Enemy.State {

    public class DeadEnemyState : EnemyState, ICharacterState {

        public DeadEnemyState(EnemyController enemyController, Animator animator, NavMeshAgent navMeshAgent)
            : base(enemyController, animator, navMeshAgent){ }

        public void Enter(){
            _animator.SetTrigger(EnemyController.EnemyAniParamDead);
        }

        public void Update(){ }
        public void Exit(){ }

    }

}