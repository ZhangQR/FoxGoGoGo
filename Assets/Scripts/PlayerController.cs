using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 对外暴露的配置
    public float MoveSpeed; // 正常移动时的速度
    public float CrouchSpeed; // 爬行时的速度
    public float JumpSpeed; // 跳跃的力度
    public int MaxJumpTimes; // 最大跳跃次数
    public float ButtomCheckPointRadius = 0.2f; // 主角脚下检测球形半径
    public float HurtTime = 3.0f; // 受伤僵直时间
    public float BounchLevel = 8; // 碰到怪物反弹的力度
    public AudioSource hurtAudio; // 受伤时播放的声音
    public AudioSource explosionAudio; // 消灭敌人时的音效
    
    private Rigidbody2D rigidbd; // 因为只有一个刚体，所以可以直接在 Awake 里面赋值
    private Animator animotor; // 因为只有一个 Animator，所以可以直接在 Awake 里面赋值
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask enemiesMask;
    [SerializeField] private float Speed; // 当前速度，用于观察运行情况

    // [SerializeField]private Collider2D coll; // 下面的球形碰撞体
    [SerializeField] private BoxCollider2D HideColl; // 上面那个矩形碰撞体,因为下面要使用 Size，所以这里必须用 BoxCollider2D

    [SerializeField] private Transform ButtomCheckPoint; // 人物下方监测点
    [SerializeField] private Transform CeilingCheckPoint; // 人物头顶监测点
    
    [SerializeField] private int JumpCndTimes; // 需要执行几次 Jump 操作
    [SerializeField] private int CanJumpTimes; // 当前可跳跃次数

    [SerializeField] private bool IsCrouching = false; // 和 !HideColl.enabled 保持一致
    [SerializeField] private bool NeedStandUp = false; // 抬起时没能站起来，在能站起来的时候立马站起来
    [SerializeField] private bool IsHurt = false; // 是否处于受伤状态
    [SerializeField] private bool IsClimb = false; // 是否处于攀爬的状态

    private float origin_gravity;
    
    // 测试用变量
    // Vector2 CollCentor = new Vector2();
    
    private void Awake()
    {
        rigidbd = this.GetComponent<Rigidbody2D>();
        // rigidbd.velocity = new Vector2(10, 0); // 检测摩擦力的影响

        // coll = this.GetComponent<Collider2D>();
        animotor = this.GetComponent<Animator>();

        CanJumpTimes = MaxJumpTimes;

        Speed = MoveSpeed;

        origin_gravity = rigidbd.gravityScale;
    }

    private void FixedUpdate()
    {
        // 不受伤的时候才可以控制移动
        if (!IsHurt)
        {
            if (!IsClimb)
            {
                NormalMove();
            }
            else
            {
                ClimbMove();
            }
        }

        // 检查是否在地面上
        if (IsOnGround())
        {
            CanJumpTimes = MaxJumpTimes;
        }

        // 设置跳跃
        if ( JumpCndTimes > 0 && CanJump())
        {
            Jump();
        }

        // 设置站立
        if (NeedStandUp && IsCrouching && CanStandUp())
        {
            StandUp();
            NeedStandUp = false;
        }
    }

    private void NormalMove()
    {
        // 设置主角移动，这里不需要加 axis 是否等于 0 的判断
        // if( axis!= 0 ) { rigidbd.velocity = new Vector2(axis * MoveSpeed, rigidbd.velocity.y); }
        // if(Mathf.Abs(axis)>=1e-48f ) { rigidbd.velocity = new Vector2(axis * MoveSpeed, rigidbd.velocity.y); }

        // 我觉得这里也不需要 * Time.Deltatime，因为如果改变的是位移的话，是需要 * 时间的，但这里改变的是速度。
        float axis = Input.GetAxis("Horizontal");
        rigidbd.velocity = new Vector2(axis * Speed, rigidbd.velocity.y);
    }

    private void ClimbMove()
    {
        float axis = Input.GetAxis("Horizontal");
        float yaxis = Input.GetAxis("Vertical");
        rigidbd.velocity = new Vector2(axis * Speed, yaxis * Speed);
    }

    // 如果碰到了一个 layer 为 Enemies 的物体，那么只有销毁或者受伤两种可能。
    private void CheckEnemiesPerform(Collision2D collision)
    {
        bool destory = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(ButtomCheckPoint.position, 0.2f, enemiesMask);
        if (colliders.Length != 0)
        {
            foreach (Collider2D coll in colliders)
            {
                if(coll.gameObject == collision.gameObject)
                {
                    explosionAudio.Play();
                    Transform death_trans = coll.gameObject.transform;
                    Destroy(coll.gameObject);
                    Instantiate(Resources.Load(ConstNames.EnemiesPath), death_trans.position,Quaternion.identity);
                    destory = true;
                    rigidbd.velocity = new Vector2(rigidbd.velocity.x, JumpSpeed); // 这里不能直接调用 Jump，这样可以实现 N+1 段跳。
                }
            }
        }
        if (!destory)
        {
            hurtAudio.Play();
            IsHurt = true;
            StartCoroutine(WaitHurt(HurtTime));
            if(transform.position.x <= collision.transform.position.x)
            {
                // rigidbd.AddForce(new Vector2(-300, 0));
                // 向左弹出去，虽然没有摩擦力了，但其实也不用这样差值运行让人物停下，因为 IsHurt 为 false 时，会自动停下。
                rigidbd.velocity = Vector2.Lerp( new Vector2(-BounchLevel, rigidbd.velocity.y),new Vector2(0,rigidbd.velocity.y),0.5f);
            }
            else
            {
                // rigidbd.AddForce(new Vector2(300, 0));
                rigidbd.velocity = Vector2.Lerp(new Vector2(BounchLevel, rigidbd.velocity.y), new Vector2(0, rigidbd.velocity.y), 0.5f);
            }
        }
    }

    private IEnumerator WaitHurt(float time)
    {
        yield return new WaitForSeconds(time);
        IsHurt = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(Mathf.Pow(2,collision.gameObject.layer) == enemiesMask.value)
        {
            // 检查是否踩到敌人
            CheckEnemiesPerform(collision);
        }
    }

    private void Update()
    {
        float axis_direction = Input.GetAxisRaw("Horizontal");

        // 设置主角方向旋转
        transform.localScale = axis_direction == 0 ? new Vector3(transform.localScale.x,transform.localScale.y,transform.localScale.z) :
            new Vector3(axis_direction,transform.localScale.y,transform.localScale.z);

        // 设置跳跃
        if (Input.GetButtonDown(ConstNames.JumpButton) && CanJump())
        {
            JumpCndTimes++;
        }

        // 设置趴下
        if (Input.GetButton(ConstNames.CrouchButton) && !IsCrouching && !IsClimb)
        {
            Crouch();
        }

        // 从趴下到站起来
        if(Input.GetButtonUp(ConstNames.CrouchButton))
        {
            if(CanStandUp())
            {
                StandUp();
            }
            else
            {
                NeedStandUp = true;
            }
        }

        // 最后根据人物的位置和速度来改变人物的动画状态
        SetAnimatorParam();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void Crouch()
    {
        HideColl.enabled = false;
        IsCrouching = true;
        Speed = CrouchSpeed;
    }

    private void StandUp()
    {
        HideColl.enabled = true;
        IsCrouching = false;
        Speed = MoveSpeed;
    }

    private bool CanStandUp()
    {
        /* if (Physics2D.OverlapCircle(CeilingCheckPoint.position, 0.05f, groundMask) != null)
        {  
            return false;
        }
        return true;*/
        // 我习惯将监测点放的很小，但用这种方法检测，只能在 Ground 边缘处于检测区域的时候有效，所以使用了同样大小的
        // 矩形检测框，感觉这样才是最吼的！
        return Physics2D.OverlapBox(new Vector2(transform.position.x + HideColl.offset.x, transform.position.y + HideColl.offset.y), 
            HideColl.size, 0, groundMask) == null; // offset 用加不用减
    }
    
    private void Jump()
    {
        CanJumpTimes--;
        JumpCndTimes--;
        rigidbd.velocity = new Vector2(rigidbd.velocity.x, JumpSpeed);
    }

    private bool CanJump()
    {
        // 剩余跳跃次数大于0，跳跃指令仍有空余，不能是趴着的状态
        return (CanJumpTimes > 0) && (JumpCndTimes + 1 <= MaxJumpTimes) && !IsCrouching && !IsHurt && !IsClimb; 
    }

    private void SetAnimatorParam()
    {
        animotor.SetBool(ConstNames.IsOnGround, IsOnGround());
        animotor.SetFloat(ConstNames.XaxisSpeed, Mathf.Abs(rigidbd.velocity.x));
        animotor.SetFloat(ConstNames.YaxisSpeed, rigidbd.velocity.y);
        animotor.SetBool(ConstNames.IsNeedCrouch, IsCrouching);
        animotor.SetBool(ConstNames.IsHurt, IsHurt);
        animotor.SetBool(ConstNames.IsClimb, IsClimb);
    }

    private bool IsOnGround()
    {
        return Physics2D.OverlapCircle(ButtomCheckPoint.position, 0.05f, groundMask) != null ||
            Physics2D.OverlapCircle(ButtomCheckPoint.position, 0.05f, enemiesMask) != null;
    }

    public void EnterLadder(float ladder_speed)
    {
        IsClimb = true;
        Speed = ladder_speed;
        rigidbd.gravityScale = 0;
    }

    public void ExitLadder()
    {
        IsClimb = false;
        Speed = MoveSpeed;
        rigidbd.gravityScale = origin_gravity;
    }

    #region Public Functions

    public void GetCollections(CollectionType type)
    {
        switch (type)
        {
            case CollectionType.Cherry:
                GameManager.Instance().CherryNum++;
                break;
            case CollectionType.Gem:
                GameManager.Instance().GemNum++;
                break;
            default:
                break;
        }
    }
    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(ButtomCheckPoint.position, 0.2f);   
        Gizmos.DrawSphere(CeilingCheckPoint.position, 0.05f);
        
        // 这里必须要再写一遍,因为运行之后不会改变值
        Gizmos.DrawCube(new Vector2(transform.position.x + HideColl.offset.x, transform.position.y + HideColl.offset.y), HideColl.size);
    }
#endif
}

