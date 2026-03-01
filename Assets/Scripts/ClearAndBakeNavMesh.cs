using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class ClearAndBakeNavMesh : MonoBehaviour
{

    NavMeshSurface navMeshSurface;
    private void Awake()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }


    public void ClearNavMesh()
    {
        navMeshSurface.RemoveData();
    }
    public void CreateBake()
    {
        Debug.Log("creamos");
        navMeshSurface.BuildNavMesh();
    }
    void Update()
    {
        
    }
}
