using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public static MainCharacterController Instance { get; private set; }
    
    public enum State { IDLE, RUNNING }
    public event EventHandler OnStateChanged;

    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float minThresholdToReachPosition = 0.005f;

    private State state;
    private List<Vector3> pathToFollow;
    private Vector3 nextPositionToReach;
    private bool hasPathToFollow;

    private void Awake()
    {
        Instance = this;

        state = State.IDLE;
        pathToFollow = new List<Vector3>();
        nextPositionToReach = transform.position;
        hasPathToFollow = false;
    }

    private void Update()
    {
        if (hasPathToFollow)
        {
            FollowPath();
        }
    }

    private void FollowPath()
    {
        if (Vector3.Distance(transform.position, nextPositionToReach) > minThresholdToReachPosition)
        {
            Vector3 nextLerpPosition = Vector3.Lerp(transform.position, nextPositionToReach, moveSpeed * Time.deltaTime);

            transform.position = nextLerpPosition;
        }
        else
        {
            if (pathToFollow.Count > 0)
            {
                nextPositionToReach = pathToFollow[0];
                pathToFollow.RemoveAt(0);

                transform.localScale = new Vector3(transform.position.x <= nextPositionToReach.x ? 1f : -1f, 1f, 1f);
            }
            else
            {
                hasPathToFollow = false;
                state = State.IDLE;

                OnStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void SetPathToFollow(List<Vector3> newPathToFollow)
    {
        pathToFollow = newPathToFollow;
        hasPathToFollow = true;
        
        if (state == State.IDLE)
        {
            state = State.RUNNING;

            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public State GetState() {
        return state;
    }
}
