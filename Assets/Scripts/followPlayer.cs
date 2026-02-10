using UnityEngine;
using UnityEngine.AI;

public class followPlayer : MonoBehaviour
{
    [SerializeField] private Transform objective;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

    }
    private void Start()
    {
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }
    private void Update() {
    if (navMeshAgent.isOnNavMesh) // Verifica si el agente está en el NavMesh
        {
        navMeshAgent.SetDestination(objective.position);
        }
        else
        {
        Debug.LogWarning("El agente no está en el NavMesh. Verifica la posición inicial del agente.");
        }
    }
}
