using UnityEngine;

public class ElevatorDoors : MonoBehaviour
{

    [Header("Door GameObjects")]
    [SerializeField]
    private Transform leftDoor;
    [SerializeField]
    private Transform rightDoor;

    [Header("Movement Parameters")]
    [SerializeField, Tooltip("Time it takes to make the doors open and close")]
    private float doorAnimationDuration;
    [SerializeField, Tooltip("Local x position of right door's open position")]
    private float openPositionX;
    [SerializeField, Tooltip("Local x position of right door's close position")]
    private float closePositionX;

    [Header("Scale Parameters")]
    [SerializeField, Tooltip("Local x scale of door when closed")]
    private float doorScaleX;
    [SerializeField, Tooltip("Local x scale of right frame")]
    private float frameScaleX;

    [Header("Testing")]
    [SerializeField]
    private bool openDoor;
    [SerializeField]
    private bool closeDoor;
    [SerializeField]
    private bool switchState;

    [HideInInspector]
    public bool doorOpened { get; private set; }
    [HideInInspector]
    public bool isSwitchingState { get; private set; }


    private void Start()
    {
        _ = CloseDoor();
    }

#if UNITY_EDITOR
    // Only for editor testing
    private void Update()
    {
        if (isSwitchingState) return;

        if (openDoor && !doorOpened)
        {
            openDoor = false;
            SwitchState();
            return;
        }

        if (closeDoor && doorOpened)
        {
            closeDoor = false;
            SwitchState();
            return;
        }

        if (switchState)
        {
            switchState = false;
            SwitchState();
        }
    }
#endif

    private async Awaitable OpenDoor()
    {
        float timeTaken = 0;
        Vector3 currentPosition = Vector3.zero;
        Vector3 currentScale = leftDoor.localScale;
        
        while (timeTaken <= doorAnimationDuration)
        {
            timeTaken += Time.deltaTime;
            
            currentScale.x = Mathf.Lerp(doorScaleX, frameScaleX, Mathf.Clamp01(timeTaken / doorAnimationDuration));
            rightDoor.localScale = currentScale;
            leftDoor.localScale = currentScale;
            
            currentPosition.x = Mathf.Lerp(closePositionX, openPositionX, Mathf.Clamp01(timeTaken/doorAnimationDuration));
            rightDoor.localPosition = currentPosition;
            leftDoor.localPosition = -currentPosition;
            
            await Awaitable.NextFrameAsync();
        }
        
        // Door is only considered open once animation completes
        doorOpened = true;
    }

    private async Awaitable CloseDoor()
    {
        // Door is considered closed as soon as it starts moving
        doorOpened = false;
        
        float timeTaken = 0;
        Vector3 currentPosition = Vector3.zero;
        Vector3 currentScale = leftDoor.localScale;
        
        while (timeTaken <= doorAnimationDuration)
        {
            timeTaken += Time.deltaTime;
            
            currentScale.x = Mathf.Lerp(frameScaleX, doorScaleX, Mathf.Clamp01(timeTaken / doorAnimationDuration));
            rightDoor.localScale = currentScale;
            leftDoor.localScale = currentScale;

            currentPosition.x = Mathf.Lerp(openPositionX, closePositionX, Mathf.Clamp01(timeTaken/doorAnimationDuration));
            rightDoor.localPosition = currentPosition;
            leftDoor.localPosition = -currentPosition;
            
            await Awaitable.NextFrameAsync();
        }
    }
    
    public async void SwitchState()
    {
        // Checking isSwitchingState as it will be called by other scripts
        if (isSwitchingState) return;
        isSwitchingState = true;
        if (doorOpened) await CloseDoor();
        else await OpenDoor();
        isSwitchingState = false;
    }

}
