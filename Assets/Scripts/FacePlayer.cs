using UnityEditor.Tilemaps;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private bool isFacingRight = true;

    void Update()
    {
        bool isPlayerRight = transform.position.x < player.transform.position.x;
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
