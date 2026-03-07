using UnityEngine;
using System.Collections.Generic;

public class Elevator : MonoBehaviour
{

    [SerializeField]
    private float moveTime;
    [SerializeField, Tooltip("Enter frames in order of the level, i.e. 0->G, 1->1, etc")]
    private List<ElevatorUI> frames;

    public ElevatorDirection moveDirection { get; private set; }
    public bool isMoving { get; private set; }
    // tracks current level from the platform
    public int level
    {
        get => platform.currentLevel;
    }

    private bool[] upRequestedLevels = new bool[4];
    private bool[] downRequestedLevels = new bool[4];
    private ElevatorPlatform platform;


    private void Awake()
    {
        platform = GetComponentInChildren<ElevatorPlatform>();
    }

    private void OnEnable()
    {
        AssignLevelToFrames();
    }

    private void Update()
    {
        if (isMoving) return;
        if (areLevelsRequested())
        {
            _ = MovePlatform();
            return;
        }
    }

    public async Awaitable MovePlatform()
    {
        isMoving = true;

        // Determines the nearest request direction
        ElevatorDirection requestDir = ElevatorDirection.None;

        // Determine initial direction by finding nearest request
        moveDirection = ElevatorDirection.None;
        int bestDiff = int.MaxValue;
        for (int i = 0; i < upRequestedLevels.Length; i++)
        {
            if (!upRequestedLevels[i] && !downRequestedLevels[i]) continue;
            int diff = Mathf.Abs(i - level);
            if (diff < bestDiff)
            {
                bestDiff = diff; 
                moveDirection = i >= level ? ElevatorDirection.Up : ElevatorDirection.Down;
                if (upRequestedLevels[i] && downRequestedLevels[i])
                    continue;
                else if (upRequestedLevels[i])
                    requestDir = ElevatorDirection.Up;
                else requestDir = ElevatorDirection.Down;
            }
        }

        while (areLevelsRequested())
        {
            int? toLevel = null;
            int lastLevel = level;

            // Find next stop in the travel direction
            if (moveDirection == ElevatorDirection.Up)
            {
                for (int i = 0; i < upRequestedLevels.Length; i++)
                {
                    if (!upRequestedLevels[i]) continue;
                    toLevel = i;
                    break;
                }
            }
            else
            {
                for (int i = upRequestedLevels.Length-1; i >= 0; i--)
                {
                    if (!downRequestedLevels[i]) continue;
                    toLevel = i;
                    break;
                }
            }

            // runs if no request in current direction
            if (toLevel == null)
            {
                for (int i = upRequestedLevels.Length - 1; i >= 0; i--)
                {
                    if (!downRequestedLevels[i] && !upRequestedLevels[i]) continue;
                    toLevel = i;
                    break;
                }
            }
            if (toLevel == null) break;

            SetDirectionUI();
            platform.SetTargetLevel(toLevel.Value);

            // waits for platform to reach level
            while (level != toLevel.Value)
            {
                await Awaitable.EndOfFrameAsync();
                SetLevelUI();
                if (lastLevel == level) continue;
                
                // clears any requests in the same direction in the moveDirection
                if (moveDirection == ElevatorDirection.Up)
                {
                    upRequestedLevels[level] = false;
                    ElevatorManager.ReachUp(level);
                }
                else if (moveDirection == ElevatorDirection.Down)
                {
                    downRequestedLevels[level] = false;
                    ElevatorManager.ReachDown(level);
                }
                lastLevel = level;
            }

            // Clears request at the final stop
            if (requestDir != ElevatorDirection.Down)
            {
                upRequestedLevels[level] = false;
                ElevatorManager.ReachUp(level);
            }
            else if (requestDir != ElevatorDirection.Up)
            {
                downRequestedLevels[level] = false;
                ElevatorManager.ReachDown(level);
            }
        }

        isMoving = false;
        moveDirection = ElevatorDirection.None;
        SetDirectionUI();
    }

    private void AssignLevelToFrames()
    {
        for (int i=0; i<frames.Count; i++)
            frames[i].SetUILevel(i);
    }

    private bool areLevelsRequested()
    {
        foreach (bool requested in upRequestedLevels) 
            if (requested) return true;
        foreach (bool requested in downRequestedLevels) 
            if (requested) return true;
        return false;
    }

    public void RequestUpLevels(int level)
    {
        if (level < 0 || level >= upRequestedLevels.Length) return;
        upRequestedLevels[level] = true;
    }
    public void RequestDownLevels(int level)
    {
        if (level < 0 || level >= downRequestedLevels.Length) return;
        downRequestedLevels[level] = true;
    }

    private void SetLevelUI()
    {
        foreach (ElevatorUI ui in frames)
        {
            ui.SetLevel(level);
        }
    }

    private void SetDirectionUI()
    {
        foreach (ElevatorUI ui in frames)
        {
            ui.SetDirection(moveDirection);
        }
    }

}
