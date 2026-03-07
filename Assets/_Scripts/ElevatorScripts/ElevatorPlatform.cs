using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed;

    public int currentLevel;

    private Rigidbody2D rb;
    private int? targetLevel;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        if (targetLevel == null) return;
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ElevatorUI ui = other.GetComponentInParent<ElevatorUI>();
        if (ui == null) return; // detect level markers

        OnPlatformReach(other.transform.position, ui.level);
    }

    private async void OnPlatformReach(Vector3 position, int level)
    {
        while (Vector3.Distance(position, transform.position) > .1f)
        {
            // wait till platform close enough to snap to coordinates
            await Awaitable.NextFrameAsync();
        }

        transform.position = position;
        currentLevel = level;
    }

    private void Move()
    {
        if (targetLevel.Value == currentLevel)
        {
            // stop moving once reach target level
            rb.linearVelocityY = 0;
            targetLevel = null;
            return;
        }
        rb.linearVelocityY = Mathf.Sign(targetLevel.Value - currentLevel) * moveSpeed * Time.deltaTime;
    }

    public void SetTargetLevel(int level)
    {
        this.targetLevel = level;
    }

}
