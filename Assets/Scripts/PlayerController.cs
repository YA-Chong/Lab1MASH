using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 4f; // 默认移动速度
    public int carryCount = 0; // 当前载人数
    public int maxCarry = 3; // 最大载人数

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private Vector3 startPosition;

    // 屏幕边界数值（自动计算）
    private float minX,
        maxX,
        minY,
        maxY;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // 立即记录初始位置，防止被重置为 (0,0,0)
        startPosition = transform.position;
    }

    void Start()
    {
        /* rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // 记录初始位置，方便重开时瞬移回来
        startPosition = transform.position; */
        // 自动计算当前摄像机能看到的边界
        CalculateBoundaries();
    }

    void Update()
    {
        // 获取输入
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;

        // 处理图片翻转：按左变左，按右变右，上下不改
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
        // 将屏幕左下角(0,0)和右上角(1,1)转换为游戏世界坐标
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // 获取直升机图片的半径，防止边缘“露出一半”
        float spriteHalfWidth = sr.bounds.extents.x;
        float spriteHalfHeight = sr.bounds.extents.y;

        // 设置边界范围
        minX = bottomLeft.x + spriteHalfWidth;
        maxX = topRight.x - spriteHalfWidth;
        minY = bottomLeft.y + spriteHalfHeight;
        maxY = topRight.y - spriteHalfHeight;
    }

    // 在每一帧移动完成后，强行修正坐标，实现“空气墙”
    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void FixedUpdate()
    {
        // 速度逻辑：每承载一个伤员速度 -1，最低速度保留 1
        float currentSpeed = Mathf.Max(0.5f, baseSpeed - carryCount);
        rb.linearVelocity = moveInput * currentSpeed;
    }

    // 重置直升机状态的方法
    public void ResetPlayer()
    {
        transform.position = startPosition;
        carryCount = 0;
        rb.linearVelocity = Vector2.zero;
        sr.flipX = false; // 恢复初始朝右
    }

    // 触发检测：直升机碰到 Trigger 类型的物体时自动执行

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.Instance.isGameActive)
            return; // 游戏结束了就不再处理

        // 1. 碰到伤员
        if (other.CompareTag("Soldier") && carryCount < maxCarry)
        {
            carryCount++;
            Destroy(other.gameObject);
            // 播放拾取音效
            SoundManager.Instance.PlayPick();
        }

        // 2. 碰到医院
        if (other.CompareTag("Hospital") && carryCount > 0)
        {
            // 先把要交的人数存下来
            int deliveredAmount = carryCount;

            // 步骤 A：先清空直升机自身的负重（确保数据先更新）
            carryCount = 0;

            // 步骤 B：再把人数报给 GameManager 判定胜负
            GameManager.Instance.AddSavedSoldiers(deliveredAmount);

            // 播放音效
            SoundManager.Instance.PlayDrop();
        }

        // 3. 碰到树（坠机失败） [cite: 22, 56]
        if (other.CompareTag("Tree"))
        {
            // 播放死亡音效
            SoundManager.Instance.PlayDie();
            GameManager.Instance.GameOver();
        }
    }
}
