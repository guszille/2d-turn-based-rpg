using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisualController : MonoBehaviour
{
    private const string IS_RUNNING_PARAM = "IsRunning";

    [SerializeField] private EnemyController enemyController;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        enemyController.OnAnimationStateChanged += EnemyController_OnAnimationStateChanged;
    }

    private void EnemyController_OnAnimationStateChanged(object sender, BattleAgent.OnAnimationStateChangedEventArgs e)
    {
        bool isRunning = e.state == BattleAgent.AnimationState.RUNNING;

        animator.SetBool(IS_RUNNING_PARAM, isRunning);
    }
}
