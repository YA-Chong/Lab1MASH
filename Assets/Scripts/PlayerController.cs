using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 2f; //default move speed
    public int carryCount = 0; //current carry count
    public int maxCarry = 3; //max carry count

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private Vector3 startPosition;

    //caculate boundaries values
    private float minX,
        maxX,
        minY,
        maxY;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        startPosition = transform.position;
    }

    void Start()
    {
        /* rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        startPosition = transform.position; */

        CalculateBoundaries();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        if (moveX < 0)
        {
            sr.flipX = true;
        }
        else if (moveX > 0)
        {
            sr.flipX = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UIManager.Instance.OpenRestartMenu();
        }
    }

    void CalculateBoundaries()
    {
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        float spriteHalfWidth = sr.bounds.extents.x;
        float spriteHalfHeight = sr.bounds.extents.y;

        minX = bottomLeft.x + spriteHalfWidth;
        maxX = topRight.x - spriteHalfWidth;
        minY = bottomLeft.y + spriteHalfHeight;
        maxY = topRight.y - spriteHalfHeight;
    }

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void FixedUpdate()
    {
        //Load deceleration logic
        float currentSpeed = Mathf.Max(0.5f, baseSpeed - (carryCount * 0.5f));
        rb.linearVelocity = moveInput * currentSpeed;
    }

    public void ResetPlayer()
    {
        transform.position = startPosition;
        carryCount = 0;
        rb.linearVelocity = Vector2.zero;
        sr.flipX = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.isGameActive)
            return;

        if (other.CompareTag("Soldier") && carryCount < maxCarry)
        {
            carryCount++;
            Destroy(other.gameObject);
            SoundManager.Instance.PlayPick();
        }

        if (other.CompareTag("Hospital") && carryCount > 0)
        {
            int deliveredAmount = carryCount;

            carryCount = 0;

            GameManager.Instance.AddSavedSoldiers(deliveredAmount);

            SoundManager.Instance.PlayDrop();
        }

        if (other.CompareTag("Tree"))
        {
            SoundManager.Instance.PlayDie();
            GameManager.Instance.GameOver();
        }
    }
}
