using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOposssum : MonoBehaviour
{
    // 配置
    public float Move_Speed;
    public float IsFaceToRight = 1;

    // Debug

    // private
    private Rigidbody2D rigidbd;
    // [SerializeField] private Collider2D coll;
    private GameManager gameManager;
    private Vector3 origin_position; // 记录原始的位置，便于后续再次生成。

    private void Awake()
    {
        rigidbd = GetComponent<Rigidbody2D>();
        gameManager = GameManager.Instance();
        origin_position = transform.position;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rigidbd.velocity = new Vector2(Move_Speed * IsFaceToRight, rigidbd.velocity.y);
    }

    private void Update()
    {
        transform.localScale = new Vector3(-IsFaceToRight, transform.localScale.y, transform.localScale.z);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layer_value = (int)Mathf.Pow(2, collision.gameObject.layer);
        int mask_int = LayerMask.GetMask(ConstNames.GroundLayer, ConstNames.EnemiesLayer);
        if ((layer_value & mask_int) != 0 || 
            (collision.gameObject.CompareTag(ConstNames.PlayerTag) && collision.GetType() == typeof(CapsuleCollider2D)))
        {
            IsFaceToRight = -IsFaceToRight;
        }
    }
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            //GameManager.SpawnEnemy(origin_position, EnemyType.Frog, transform.GetComponentInParent<Transform>());
            gameManager.SpawnEnemy(origin_position, EnemyType.Opossum, transform.GetComponentInParent<Transform>(), 3.0f);
        }
    }

}
