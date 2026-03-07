using System;
using UnityEngine;
using System.Collections.Generic;

public class ElevatorManager : MonoBehaviour
{

    [Header("Floor Info")]
    [SerializeField]
    private int totalLevels;
    [SerializeField]
    private List<Elevator> elevators;

    public static event Action<int> OnElevatorCalledUp;
    public static event Action<int> OnElevatorCalledDown;
    public static event Action<int> OnElevatorReachedUp;
    public static event Action<int> OnElevatorReachedDown;

    // Tracks what direction the elevator was requested at
    private static ElevatorDirection requestDirection = ElevatorDirection.None;


    private void OnEnable()
    {
        OnElevatorCalledUp += CallElevator;
        OnElevatorCalledDown += CallElevator;
    }

    private void OnDisable()
    {
        OnElevatorCalledUp -= CallElevator;
        OnElevatorCalledDown -= CallElevator;
    }

    private async void CallElevator(int targetLevel)
    {
        if (elevators == null) return;
        
        Elevator elevator = FindNearestElevator(targetLevel);
        // If elevator is already at the requested floor, treat it as immediately reached
        if (elevator.level == targetLevel)
        {
            ReachUp(targetLevel);
            ReachDown(targetLevel);
            requestDirection = ElevatorDirection.None;
            return;
        }

        if (requestDirection == ElevatorDirection.Up)
            elevator.RequestUpLevels(targetLevel);
        else elevator.RequestDownLevels(targetLevel);
        requestDirection = ElevatorDirection.None;
    }

    private Elevator FindNearestElevator(int targetLevel)
    {
        Elevator nearest = elevators[0];
        int min = int.MaxValue;
        foreach (Elevator elevator in elevators)
        {
            int diff = targetLevel - elevator.level;
            float dir = Mathf.Sign(diff);
            diff = Mathf.Abs(diff);

            // Skip if not closer, or if already at floor but still moving
            if (diff >= min || (elevator.level == targetLevel && elevator.isMoving)) 
                continue;
            
            if (
                elevator.moveDirection == ElevatorDirection.None ||
                (dir < 0 && elevator.moveDirection == ElevatorDirection.Down) ||
                (dir > 0 && elevator.moveDirection == ElevatorDirection.Up)
                )
            {
                min = diff;
                nearest = elevator;
            }
        }
        return nearest;
    }

    #region Public Action Invokers
    public static void CallUp(int level)
    {
        requestDirection = ElevatorDirection.Up;
        OnElevatorCalledUp?.Invoke(level);
    }

    public static void CallDown(int level)
    {
        requestDirection = ElevatorDirection.Down;
        OnElevatorCalledDown?.Invoke(level);
    }

    public static void ReachUp(int level)
    {
        OnElevatorReachedUp?.Invoke(level);
    }

    public static void ReachDown(int level)
    {
        OnElevatorReachedDown?.Invoke(level);
    }
    #endregion

}

public enum ElevatorDirection { Up, Down, None };