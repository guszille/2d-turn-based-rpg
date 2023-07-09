using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterWeaponVisualController : MonoBehaviour
{
    private const string IS_ATTACKING_PARAM = "IsAttacking";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MainCharacterController.Instance.OnAttackPerformanceStarted += MainCharacter_OnAttackPerformanceStarted; ;
    }

    private void MainCharacter_OnAttackPerformanceStarted(object sender, System.EventArgs e)
    {
        animator.SetTrigger(IS_ATTACKING_PARAM);
    }

    private void OnAttackAnimEnd() // Callback of attack animation.
    {
        MainCharacterController.Instance.EndAttackAction();
    }
}
