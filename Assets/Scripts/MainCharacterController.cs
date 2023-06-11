using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : BattleAgent
{
    public static MainCharacterController Instance { get; private set; }

    public enum AnimationState { IDLE, RUNNING }

    public class OnAnimationStateChangedEventArgs : EventArgs { public AnimationState state; }
    public event EventHandler<OnAnimationStateChangedEventArgs> OnAnimationStateChanged;

    private AnimationState animationState;

    private const float FREE_MOVE_SPEED_MULTIPLIER = 2f;

    private void Awake()
    {
        Instance = this;

        animationState = AnimationState.IDLE;

        pathToFollow = new List<Vector3>();
        nextPositionToReach = transform.position;
        hasPathToFollow = false;

        isInBattle = false;
        agentType = AgentType.PLAYER;
        battleAction = BattleAction.ON_HOLD;

        hitPoints = maxHitPoints;
    }

    private void Update()
    {
        NavigationManager.Instance.ClearMarkedTilemap();

        if (isInBattle)
        {
            if (battleAction == BattleAction.MAIN)
            {
                // TODO: do attack action.

                EndTurn();
            }

            if (battleAction == BattleAction.MOVEMENT)
            {
                if (hasPathToFollow)
                {
                    FollowPathFound(movementSpeed * Time.deltaTime);

                    if (!hasPathToFollow)
                    {
                        animationState = AnimationState.IDLE;

                        OnAnimationStateChanged?.Invoke(this, new OnAnimationStateChangedEventArgs { state = animationState });

                        SetNextAction(BattleAction.MAIN);
                    }
                }
                else
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    List<Vector2Int> pathFound = FindPathToFollow(mouseWorldPosition);

                    if (pathFound != null && pathFound.Count > 0)
                    {
                        NavigationManager.Instance.MarkPath(pathFound);

                        if (Input.GetMouseButtonDown(0)) // LEFT MOUSE CLICK.
                        {
                            animationState = AnimationState.RUNNING;

                            OnAnimationStateChanged?.Invoke(this, new OnAnimationStateChangedEventArgs { state = animationState });

                            SetPathToFollow(NavigationManager.Instance.ConvertToWorldPosition(pathFound));
                        }
                    }
                }
            }
        }
        else
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            List<Vector2Int> pathFound = FindPathToFollow(mouseWorldPosition);

            if (pathFound != null && pathFound.Count > 0)
            {
                NavigationManager.Instance.MarkPosition(NavigationManager.Instance.ConvertToCellPosition(mouseWorldPosition));

                if (Input.GetMouseButtonDown(0)) // LEFT MOUSE CLICK.
                {
                    SetPathToFollow(NavigationManager.Instance.ConvertToWorldPosition(pathFound));
                }
            }

            if (hasPathToFollow)
            {
                FollowPathFound(movementSpeed * FREE_MOVE_SPEED_MULTIPLIER * Time.deltaTime);
            }
        }
    }
}
