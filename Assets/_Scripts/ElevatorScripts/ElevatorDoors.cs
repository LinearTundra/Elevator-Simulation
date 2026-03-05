using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    private async Awaitable OpenDoor()
    {
        float timeTaken = 0;
        Vector3 currentPosition = Vector3.zero;
        while (timeTaken <= doorAnimationDuration)
        {
            timeTaken += Time.deltaTime;
            currentPosition.x = Mathf.Lerp(closePositionX, openPositionX, Mathf.Clamp01(timeTaken/doorAnimationDuration));
            rightDoor.localPosition = currentPosition;
            leftDoor.localPosition = -currentPosition;
            await Awaitable.NextFrameAsync();
        }
        doorOpened = true;
    }

    private async Awaitable CloseDoor()
    {
        doorOpened = false;
        float timeTaken = 0;
        Vector3 currentPosition = Vector3.zero;
        while (timeTaken <= doorAnimationDuration)
        {
            timeTaken += Time.deltaTime;
            currentPosition.x = Mathf.Lerp(openPositionX, closePositionX, Mathf.Clamp01(timeTaken/doorAnimationDuration));
            rightDoor.localPosition = currentPosition;
            leftDoor.localPosition = -currentPosition;
            await Awaitable.NextFrameAsync();
        }
    }
    
    public async void SwitchState()
    {
        if (isSwitchingState) return;
        isSwitchingState = true;
        if (doorOpened) await CloseDoor();
        else await OpenDoor();
        isSwitchingState = false;
    }

}
