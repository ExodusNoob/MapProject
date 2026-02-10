using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    [SerializeField]
    private bool isOccupied = false;
    public void SetOccupied(bool value = true)
    {
        isOccupied = value;
    }
    public bool IsOccupied()
    {
        return isOccupied;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
