using UnityEditor.Tilemaps;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private Transform rplayer;

    [SerializeField] private bool isFacingRight = true;

    private void Awake()
    {
        GameObject player = GameObject.Find("soldier");

        if (player != null)
        {
            rplayer = player.transform;
        }
        else
        {
            Debug.LogError("No se encontro el Gameobject llamado Soldier");
        }
    }
    void Update()
    {
        bool isPlayerRight = transform.position.x < rplayer.transform.position.x;
        Flip(isPlayerRight);
    }

    private void Flip(bool isPlayerRight)
    {
        if ((isFacingRight && !isPlayerRight) || (!isFacingRight && isPlayerRight))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }
}
