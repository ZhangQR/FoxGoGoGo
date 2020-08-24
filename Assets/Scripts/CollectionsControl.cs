using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CollectionType
{
    Cherry,
    Gem,
    MaxCollectionType
}
public class CollectionsControl : MonoBehaviour
{
    public CollectionType type;
    private GameObject player;
    public AudioClip coinClip;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.clip = coinClip;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ConstNames.PlayerTag))
        {
            GetComponent<Animator>().SetBool(ConstNames.IsBoom, true);
            player = collision.gameObject;
        }
    }

    /// <summary>
    /// 在 Boom 动画结束之后调用，因为人物有两个碰撞体，这样可以防止速度过快导致的触发两次 Score++
    /// </summary>
    private void Death()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(ConstNames.PlayerTag);
        }
        player.GetComponent<PlayerController>().GetCollections(type);
        Destroy(gameObject);
    }

    private void PlaySound()
    {
        audioSource.Play();
    }
}
