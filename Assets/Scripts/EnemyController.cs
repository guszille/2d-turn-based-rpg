using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BattleAgent
{
    private float maxTimeToProjectAnAttack = 1f;
    private float timeProjectingAnAttack;

    private void Awake()
    {
        animationState = AnimationState.IDLE;

        pathToFollow = new List<(Vector3, int)>();
        nextPositionToReach = transform.position;
        hasPathToFollow = false;
        navigatedDistance = 0;

        isInBattle = false;
        agentType = AgentType.ENEMY;
        battleAction = BattleAction.ON_HOLD;
        mainActionType = MainActionType.NONE;
        mainActionState = MainActionState.ON_HOLD;

        hitPoints = maxHitPoints;

        timeProjectingAnAttack = maxTimeToProjectAnAttack;
    }

    private void Update()
    {
        if (isInBattle)
        {
            if (battleAction == BattleAction.MAIN)
            {
                if (mainActionState == MainActionState.PROJECTING)
                {
                    if (mainActionType == MainActionType.ATTACK)
                    {
                        List<Vector2Int> projectedArea = new List<Vector2Int>();
                        Vector2Int centerCellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);

                        projectedArea.Add(centerCellPosition + new Vector2Int(-1,  1));
                        projectedArea.Add(centerCellPosition + new Vector2Int( 0,  1));
                        projectedArea.Add(centerCellPosition + new Vector2Int( 1,  1));
                        projectedArea.Add(centerCellPosition + new Vector2Int(-1,  0));

                        projectedArea.Add(centerCellPosition + new Vector2Int( 1,  0));
                        projectedArea.Add(centerCellPosition + new Vector2Int(-1, -1));
                        projectedArea.Add(centerCellPosition + new Vector2Int( 0, -1));
                        projectedArea.Add(centerCellPosition + new Vector2Int( 1, -1));

                        NavigationManager.Instance.MarkPath(projectedArea, new Color(1f, 0f, 1f, 0.125f));

                        timeProjectingAnAttack = timeProjectingAnAttack - Time.deltaTime;

                        if (timeProjectingAnAttack <= 0f)
                        {
                            mainActionState = MainActionState.PERFORMING;

                            timeProjectingAnAttack = maxTimeToProjectAnAttack;

                            // TODO: trigger attack animation.
                        }
                    }
                }

                if (mainActionState == MainActionState.PERFORMING)
                {
                    if (mainActionType == MainActionType.ATTACK)
                    { 
                        List<Vector2Int> hittedArea = new List<Vector2Int>();
                        Vector2Int centerCellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);

                        hittedArea.Add(centerCellPosition + new Vector2Int(-1,  1));
                        hittedArea.Add(centerCellPosition + new Vector2Int( 0,  1));
                        hittedArea.Add(centerCellPosition + new Vector2Int( 1,  1));
                        hittedArea.Add(centerCellPosition + new Vector2Int(-1,  0));

                        hittedArea.Add(centerCellPosition + new Vector2Int( 1,  0));
                        hittedArea.Add(centerCellPosition + new Vector2Int(-1, -1));
                        hittedArea.Add(centerCellPosition + new Vector2Int( 0, -1));
                        hittedArea.Add(centerCellPosition + new Vector2Int( 1, -1));

                        DoBasicAttack(hittedArea);

                        EndAttackAction();
                    }
                }

                if (mainActionState == MainActionState.ON_HOLD)
                {
                    // Awaiting or doing something...
                }
            }

            if (battleAction == BattleAction.MOVEMENT)
            {
                if (hasPathToFollow)
                {
                    FollowPathFound(movementSpeed * Time.deltaTime);

                    if (!hasPathToFollow)
                    {
                        SetNextAnimation(AnimationState.IDLE);

                        SetNextAction(BattleAction.MAIN);

                        StartAttackAction();
                    }
                }
                else
                {
                    List<(Vector2Int, int)> pathFound = FindPathToFollow(MainCharacterController.Instance.transform.position);

                    if (pathFound.Count > 0)
                    {
                        pathFound.RemoveAt(pathFound.Count - 1); // Removing the last element prevents the enemy from reaching the same position as the target.

                        SetNextAnimation(AnimationState.RUNNING);

                        SetPathToFollow(NavigationManager.Instance.ConvertToWorldPosition(pathFound));
                    }
                    else
                    {
                        SetNextAction(BattleAction.MAIN);

                        StartAttackAction();
                    }
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, MainCharacterController.Instance.transform.position) <= visionRange)
            {
                LookAt(MainCharacterController.Instance.transform.position.x);

                BattleManager.Instance.RequestBattleToStart();
            }
        }
    }

    private void StartAttackAction()
    {
        mainActionState = MainActionState.PROJECTING;
        mainActionType = MainActionType.ATTACK;
    }

    private void EndAttackAction()
    {
        mainActionState = MainActionState.ON_HOLD;
        mainActionType = MainActionType.NONE;

        EndTurn();
    }

    private void DoBasicAttack(List<Vector2Int> hittedCellPositions)
    {
        Vector2Int mainCharacterCellPosition = NavigationManager.Instance.ConvertToCellPosition(MainCharacterController.Instance.transform.position);

        foreach (Vector2Int cellPosition in hittedCellPositions)
        {
            if (cellPosition == mainCharacterCellPosition)
            {
                MainCharacterController.Instance.TakeDamage(damage);
            }
        }
    }
}
