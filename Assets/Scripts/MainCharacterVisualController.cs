using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterVisualController : MonoBehaviour
{
    private const string IS_RUNNING_PARAM = "IsRunning";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        MainCharacterController.Instance.OnAnimationStateChanged += MainCharacter_OnAnimationStateChanged;
    }

    private void MainCharacter_OnAnimationStateChanged(object sender, MainCharacterController.OnAnimationStateChangedEventArgs e)
    {
        bool isRunning = e.state == MainCharacterController.AnimationState.RUNNING;

        animator.SetBool(IS_RUNNING_PARAM, isRunning);
    }
}
