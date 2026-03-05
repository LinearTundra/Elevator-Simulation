using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ElevatorUI : MonoBehaviour
{

    [Header("Level and Direction Display")]
    [SerializeField]
    private GameObject directionTriangle;
    [SerializeField]
    private TMP_Text levelText;

    [Header("Call Buttons")]
    [SerializeField]
    private Button callUpButton;
    [SerializeField]
    private Button callDownButton;

    [Header("Testing")]
    [SerializeField]
    private int level;
    [SerializeField]
    private ElevatorDirection direction;
    [SerializeField]
    private bool callElevatorDown;
    [SerializeField]
    private bool callElevatorUp;
    [SerializeField]
    private bool reachElevatorDown;
    [SerializeField]
    private bool reachElevatorUp;


#if UNITY_EDITOR
    private void Update()
    {
        SetLevel(level);
        SetDirection(direction);
        if (callElevatorDown)
        {
            callElevatorDown = false;
            ElevatorCalledDown();
        }
        if (callElevatorUp)
        {
            callElevatorUp = false;
            ElevatorCalledUp();
        }
        if (reachElevatorDown)
        {
            reachElevatorDown = false;
            ElevatorReachedDown();
        }
        if (reachElevatorUp)
        {
            reachElevatorUp = false;
            ElevatorReachedUp();
        }
    }
#endif

    public void ElevatorCalledDown()
    {
        callDownButton.interactable = false;
    }

    public void ElevatorCalledUp()
    {
        callUpButton.interactable = false;
    }

    public void ElevatorReachedUp()
    {
        callUpButton.interactable = true;
    }

    public void ElevatorReachedDown()
    {
        callDownButton.interactable = true;
    }

    public void SetLevel(int level)
    {
        if (level > 0)
            levelText.text = level.ToString();
        else if (level == 0)
            levelText.text = "G";
        else levelText.text = "B"+Mathf.Abs(level);
    }

    public void SetDirection(ElevatorDirection direction)
    {
        if (direction == ElevatorDirection.None)
        {
            directionTriangle.SetActive(false);
            return;
        }

        directionTriangle.SetActive(true);
        directionTriangle.transform.localScale = Vector3.one;

        if (direction == ElevatorDirection.Down)
        {
            directionTriangle.transform.localScale = new Vector3(1, -1, 1);
        }

    }

}
