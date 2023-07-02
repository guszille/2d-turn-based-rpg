using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : BattleAgent
{
    public static MainCharacterController Instance { get; private set; }

    private const float FREE_MOVE_SPEED_MULTIPLIER = 1.5f;

    public event EventHandler OnAttackPerformanceStarted;

    private void Awake()
    {
        Instance = this;

        animationState = AnimationState.IDLE;

        pathToFollow = new List<(Vector3, int)>();
        nextPositionToReach = transform.position;
        hasPathToFollow = false;
        navigatedDistance = 0;

        isInBattle = false;
        agentType = AgentType.PLAYER;
        battleAction = BattleAction.ON_HOLD;
        mainActionType = MainActionType.NONE;
        mainActionState = MainActionState.ON_HOLD;

        hitPoints = maxHitPoints;
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

                        NavigationManager.Instance.MarkPath(projectedArea, new Color(0f, 0f, 1f, 0.125f));

                        if (Input.GetMouseButtonDown(0)) // LEFT MOUSE CLICK.
                        {
                            mainActionState = MainActionState.PERFORMING;

                            OnAttackPerformanceStarted?.Invoke(this, EventArgs.Empty);
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

                        mainActionState = MainActionState.ON_HOLD;
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
                    }
                }
                else
                {
                    Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    List<(Vector2Int, int)> pathFound = FindPathToFollow(mouseWorldPosition);

                    if (pathFound.Count > 0)
                    {
                        (List<Vector2Int> pathToMark, int pathCost) = NavigationManager.Instance.GetCostFromPath(pathFound);

                        if (pathCost > maxMovementAmount || BattleManager.Instance.HasEnemyOnPosition(mouseWorldPosition, false))
                        {
                            NavigationManager.Instance.MarkPath(pathToMark, new Color(1f, 0f, 0f, 0.125f));
                        }
                        else
                        {
                            NavigationManager.Instance.MarkPath(pathToMark, new Color(1f, 1f, 1f, 0.125f));

                            if (Input.GetMouseButtonDown(0)) // LEFT MOUSE CLICK.
                            {
                                SetNextAnimation(AnimationState.RUNNING);

                                SetPathToFollow(NavigationManager.Instance.ConvertToWorldPosition(pathFound));
                            }
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
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            List<(Vector2Int, int)> pathFound = FindPathToFollow(mouseWorldPosition);

            if (pathFound.Count > 0)
            {
                if (BattleManager.Instance.HasEnemyOnPosition(mouseWorldPosition, false))
                {
                    NavigationManager.Instance.MarkPosition(NavigationManager.Instance.ConvertToCellPosition(mouseWorldPosition), new Color(1f, 0f, 0f, 0.125f));
                }
                else
                {
                    NavigationManager.Instance.MarkPosition(NavigationManager.Instance.ConvertToCellPosition(mouseWorldPosition), new Color(1f, 1f, 1f, 0.125f));

                    if (Input.GetMouseButtonDown(0)) // LEFT MOUSE CLICK.
                    {
                        SetNextAnimation(AnimationState.RUNNING);

                        SetPathToFollow(NavigationManager.Instance.ConvertToWorldPosition(pathFound));
                    }
                }
            }

            if (hasPathToFollow)
            {
                FollowPathFound(movementSpeed * FREE_MOVE_SPEED_MULTIPLIER * Time.deltaTime, true);
            }
        }
    }

    private void DoBasicAttack(List<Vector2Int> hittedCellPositions)
    {
        foreach (Vector2Int cellPosition in hittedCellPositions)
        {
            BattleAgent enemy = BattleManager.Instance.GetEnemyOnCellPosition(cellPosition, false);

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public void StartAttackAction()
    {
        mainActionState = MainActionState.PROJECTING;
        mainActionType = MainActionType.ATTACK;
    }

    public void CancelAttackAction()
    {
        mainActionState = MainActionState.ON_HOLD;
        mainActionType = MainActionType.NONE;
    }

    public void EndAttackAction()
    {
        mainActionState = MainActionState.ON_HOLD;
        mainActionType = MainActionType.NONE;

        EndTurn();
    }
}
