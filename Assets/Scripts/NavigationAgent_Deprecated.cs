using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NavigationAgent
{
    [SerializeField] protected float minThresholdToReachPosition = 0.005f;

    protected List<Vector3> commitedPathToFollow;
    protected List<Vector2Int> lastPathFound;
    protected Vector3 nextPositionToReach;
    protected bool hasCommitedPathToFollow;

    protected void SetupNavigationAgent(Vector3 initialPosition)
    {
        commitedPathToFollow = new List<Vector3>();
        lastPathFound = null;
        nextPositionToReach = initialPosition;
        hasCommitedPathToFollow = false;
    }

    protected void FindPathToFollow(Vector3 initialPosition, Vector3 finalPosition)
    {
        Vector2Int initialCellPosition = NavigationManager.Instance.ConvertToCellPosition(initialPosition);
        Vector2Int finalCellPosition = NavigationManager.Instance.ConvertToCellPosition(finalPosition);

        lastPathFound = NavigationManager.Instance.FindPath(initialCellPosition, finalCellPosition);
    }

    protected void CommitPathToFollow()
    {
        if (lastPathFound != null)
        {
            commitedPathToFollow = NavigationManager.Instance.ConvertToWorldPosition(lastPathFound);
            hasCommitedPathToFollow = true;
        }
    }

    protected void FollowPath(Vector3 currentPosition, Vector3 currentScale, float translationSpeed, out Vector3 nextPosition, out Vector3 nextScale)
    {
        nextPosition = currentPosition;
        nextScale = currentScale;

        if (Vector3.Distance(currentPosition, nextPositionToReach) > minThresholdToReachPosition)
        {
            Vector3 nextLerpPosition = Vector3.Lerp(currentPosition, nextPositionToReach, translationSpeed * Time.deltaTime);

            nextPosition = nextLerpPosition;
        }
        else
        {
            if (commitedPathToFollow.Count > 0)
            {
                nextPositionToReach = commitedPathToFollow[0];
                commitedPathToFollow.RemoveAt(0);

                nextScale = new Vector3(currentPosition.x <= nextPositionToReach.x ? 1f : -1f, 1f, 1f);
            }
            else
            {
                hasCommitedPathToFollow = false;
            }
        }
    }

    protected bool FoundPathToFollow()
    {
        return lastPathFound != null && lastPathFound.Count > 0;
    }

    protected bool HasCommitedPathToFollow()
    {
        return hasCommitedPathToFollow;
    }
}
