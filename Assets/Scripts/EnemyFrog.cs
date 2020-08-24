using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrog : MonoBehaviour
{
    public float JumpForce = 10; // 跳跃的力度
    public float JumpUpForce = 10; // 跳跃的力度
    public LayerMask GroundMask;

    [SerializeField] private int IsFaceToRight = 1; // 当前的朝向, -1 向右， 1 向左
    [SerializeField] private bool IsJumping; // 是否处于跳跃中

    private Vector3 origin_position; // 记录原始的位置，便于后续再次生成。
    private Rigidbody2D rigidbd;
    private Animator animator;
    public BoxCollider2D coll;
    private Coroutine waitIdle;

    private GameManager gameManager;

    //temp
    public bool isOnGround;

    private void Awake()
    {
        rigidbd = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        origin_position = transform.position;
        gameManager = GameManager.Instance();
        //coll = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // rigidbd.AddForce(new Vector2(JumpForce * IsFaceToRight, JumpUpForce));
    }

    private void FixedUpdate()
    {
        if (IsOnGround())
        {
            IsJumping = false;
            if (waitIdle ==null)
            {
                waitIdle = StartCoroutine(WaitTime(Random.Range(1.0f, 4.0f)));
            }
        }
    }

    private bool IsOnGround()
    {
        Vector2 collider_center = (Vector2)transform.position + coll.offset;
        float ray_length = coll.size.y / 2 + 0.12f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, ray_length, GroundMask.value);
        return hit.collider!=null;
    }

    private void Update()
    {
        transform.localScale = new Vector3(-IsFaceToRight, transform.localScale.y, transform.localScale.z);
    }

    private void LateUpdate()
    {
        CheckAnimationChange();
    }

    private void CheckAnimationChange()
    {
        if (IsOnGround())
        {
            animator.SetBool(ConstNames.FrogIsIdle, true);
            animator.SetBool(ConstNames.FrogIsFall, false);
            animator.SetBool(ConstNames.FrogIsJump, false);
        }
        else
        {
            if (rigidbd.velocity.y >= 0)
            {
                animator.SetBool(ConstNames.FrogIsIdle, false);
                animator.SetBool(ConstNames.FrogIsFall, false);
                animator.SetBool(ConstNames.FrogIsJump, true);
            }
            else
            {
                animator.SetBool(ConstNames.FrogIsIdle, false);
                animator.SetBool(ConstNames.FrogIsFall, true);
                animator.SetBool(ConstNames.FrogIsJump, false);
            }
        }
    }

    private void Jump()
    {
        rigidbd.AddForce(new Vector2(JumpForce * IsFaceToRight, JumpUpForce));
        StopCoroutine(waitIdle);
        waitIdle = null;
    }

    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);
        IsJumping = true;
        Jump();
    }

    //private void OnDrawGizmos()
    //{
    //    Vector2 collider_center = (Vector2)transform.position + coll.offset;
    //    float ray_length = coll.size.y / 2 + 0.2f;
    //    Gizmos.DrawLine(new Vector3(collider_center.x, collider_center.y, transform.position.z),
    //        new Vector3(collider_center.x, collider_center.y - ray_length, transform.position.z));
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer_value = (int)Mathf.Pow(2, collision.gameObject.layer);
        int mask_int = LayerMask.GetMask(ConstNames.GroundLayer, ConstNames.EnemiesLayer);
        if((layer_value & mask_int) != 0 || collision.gameObject.CompareTag(ConstNames.PlayerTag))
        {
            IsFaceToRight = -IsFaceToRight;
        }
    }

    private void OnDestroy()
    {
        if(gameManager != null)
        {
            //GameManager.SpawnEnemy(origin_position, EnemyType.Frog, transform.GetComponentInParent<Transform>());
            gameManager.SpawnEnemy(origin_position, EnemyType.Frog, transform.GetComponentInParent<Transform>(), 3.0f);

        }
    }
}
