using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : BattleAgent
{
    public event EventHandler OnAttackActionStarted;
    public event EventHandler OnAttackActionEnded;

    [SerializeField] private int roomNumber;

    private float maxTimeProjectingAnAttack = 1f;
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
        armorPoints = maxArmorPoints;

        timeProjectingAnAttack = maxTimeProjectingAnAttack;
    }

    private void Update()
    {
        if (hitPoints == 0f)
        {
            return;
        }

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

                            timeProjectingAnAttack = maxTimeProjectingAnAttack;
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

                        if (Vector3.Distance(transform.position, MainCharacterController.Instance.transform.position) <= rangeToAttack)
                        {
                            SetNextAction(BattleAction.MAIN);

                            StartAttackAction();
                        }
                        else
                        {
                            EndTurn();
                        }
                    }
                }
                else
                {
                    Vector2Int cellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);
                    List<Vector2Int> blackList = BattleManager.Instance.GetAllEnemiesCellPositions(false);

                    blackList.Remove(cellPosition); // Removes own cell position from the balck list.

                    List<(Vector2Int, int)> pathFound = FindPathToFollow(MainCharacterController.Instance.transform.position, blackList);

                    if (pathFound.Count > 0)
                    {
                        Vector2Int mainCharacterCellPosition = NavigationManager.Instance.ConvertToCellPosition(MainCharacterController.Instance.transform.position);

                        if (pathFound[^1].Item1 == mainCharacterCellPosition)
                        {
                            pathFound.Remove(pathFound[^1]); // Removes the last element to prevent the enemy from reaching the same position as the target.
                        }

                        SetNextAnimation(AnimationState.RUNNING);

                        SetPathToFollow(NavigationManager.Instance.ConvertToWorldPosition(pathFound));
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, MainCharacterController.Instance.transform.position) <= rangeToAttack)
                        {
                            SetNextAction(BattleAction.MAIN);

                            StartAttackAction();
                        }
                    }
                }
            }

            if (battleAction == BattleAction.ON_HOLD)
            {
                // Awaiting or doing something...
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, MainCharacterController.Instance.transform.position) <= visionRange)
            {
                Vector2Int cellPosition = NavigationManager.Instance.ConvertToCellPosition(transform.position);
                List<Vector2Int> blackList = BattleManager.Instance.GetAllEnemiesCellPositions(false);

                blackList.Remove(cellPosition); // Removes own cell position from the balck list.

                List<(Vector2Int, int)> pathFound = FindPathToFollow(MainCharacterController.Instance.transform.position, blackList);

                if (pathFound.Count > 0) // Needs to see a clear path between itself and the palyer.
                {
                    LookAt(MainCharacterController.Instance.transform.position.x);

                    BattleManager.Instance.RequestBattleToStart(roomNumber);
                }
            }
        }
    }

    private void StartAttackAction()
    {
        LookAt(MainCharacterController.Instance.transform.position.x);

        mainActionState = MainActionState.PROJECTING;
        mainActionType = MainActionType.ATTACK;

        OnAttackActionStarted?.Invoke(this, EventArgs.Empty);
    }

    private void EndAttackAction()
    {
        mainActionState = MainActionState.ON_HOLD;
        mainActionType = MainActionType.NONE;

        OnAttackActionEnded?.Invoke(this, EventArgs.Empty);

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
