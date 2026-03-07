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

    // what level this ui pannel sit at
    public int level { get; private set; }


    private void Awake()
    {
        directionTriangle.SetActive(false);
    }

    private void OnEnable()
    {
        ElevatorManager.OnElevatorCalledUp += ElevatorCalledUp;
        ElevatorManager.OnElevatorCalledDown += ElevatorCalledDown;
        ElevatorManager.OnElevatorReachedUp += ElevatorReachedUp;
        ElevatorManager.OnElevatorReachedDown += ElevatorReachedDown;

        callUpButton.onClick.AddListener(CallUp);
        callDownButton.onClick.AddListener(CallDown);
    }

    private void OnDisable()
    {
        ElevatorManager.OnElevatorCalledUp -= ElevatorCalledUp;
        ElevatorManager.OnElevatorCalledDown -= ElevatorCalledDown;
        ElevatorManager.OnElevatorReachedUp -= ElevatorReachedUp;
        ElevatorManager.OnElevatorReachedDown -= ElevatorReachedDown;

        callUpButton.onClick.RemoveAllListeners();
        callDownButton.onClick.RemoveAllListeners();
    }


    public void ElevatorCalledDown(int level)
    {
        if (level != this.level) return;
        callDownButton.interactable = false;
    }

    public void ElevatorCalledUp(int level)
    {
        if (level != this.level) return;
        callUpButton.interactable = false;
    }

    public void ElevatorReachedUp(int level)
    {
        if (level != this.level) return;
        callUpButton.interactable = true;
    }

    public void ElevatorReachedDown(int level)
    {
        if (level != this.level) return;
        callDownButton.interactable = true;
    }

    public void SetLevel(int level)
    {
        if (level > 0)
            levelText.text = $"{level}";
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

    public void SetUILevel(int level)
    {
        this.level = level;
    }

    private void CallUp()
    {
        ElevatorManager.CallUp(level);
    }

    private void CallDown()
    {
        ElevatorManager.CallDown(level);
    }

}
