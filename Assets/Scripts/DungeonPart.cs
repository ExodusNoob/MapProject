using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonPart : MonoBehaviour
{
    public enum DungeonPartType
    {
        Room,
        Hallway
    }
    [SerializeField]
    private LayerMask roomLayermask;

    [SerializeField]
    private DungeonPartType dungeonPartType;

    [SerializeField]
    private GameObject fillerWall;

    public List<Transform> entrypoints;

    //public new BoxCollider2D collider2D;

    public bool HasAvailableEntrypoint(out Transform entrypoint)
    {
        Transform resultingEntry = null;
        bool result = false;

        int totalRetries = 75;
        int retryIndex = 0;

        if (entrypoints.Count == 1)
        {
            Transform entry = entrypoints[0];

            Debug.Log("1 punto en " + entry.name);
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint res))
            {
                if (res.IsOccupied())
                {
                    Debug.Log("ta ocupao el entry " + entry.name);
                    result = false;
                    resultingEntry = null;
                }
                else
                {
                    Debug.Log("se ocupo el entry " + entry.name);
                    result = true;
                    resultingEntry = entry;
                    res.SetOccupied();
                }
                entrypoint = resultingEntry;
                return result;
            }
        }
        while (resultingEntry == null && retryIndex < totalRetries)
        {
            int randomEntryIndex = Random.Range(0, entrypoints.Count); //LA ELECCION DEL ENTRYPOINT ES RANDOM, POR LO QUE PUEDE GENERAR PROBLEMAS CON CUARTOS QUE NO VAN

            Transform entry = entrypoints[randomEntryIndex];
            Debug.Log("el entry es " + entry.name);
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    resultingEntry = entry;
                    result = true;
                    entryPoint.SetOccupied();
                    Debug.Log("se ocupo el entry " + entryPoint.name);
                    break;
                }
            }
            retryIndex++;
        }
        //if devolviendo el entrypoint result como un Null
        entrypoint = resultingEntry;
        if (resultingEntry == null)
        {
            Debug.Log("solto un NULL, o sea, no encontro entrypoints");
        }
        else
        {
            Debug.Log("elegio el entry " + entrypoint.name);

        }
        return result;
    }

    public void UnuseEntrypoint(Transform entrypoint)
    {
        if (entrypoint.TryGetComponent<EntryPoint>(out EntryPoint entry))
        {
            Debug.Log("desocupamos el entry");
            entry.SetOccupied(false);
            Debug.Log("es false ahora?" + entry.IsOccupied());
        }
    }

    public void FillEmptyDoors()
    {
        entrypoints.ForEach((entry) =>
        {
            if (entry.TryGetComponent(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    GameObject wall = Instantiate(fillerWall);
                    wall.transform.position = entry.transform.position;
                    wall.transform.rotation = entry.transform.rotation;
                }
            }
        });
    }
    public void DestroyRoom()
    {
        //posible codigo para eliminar el gameobject o no
    }
    private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.red;
    Gizmos.DrawWireCube(
        GetComponent<Collider2D>().bounds.center,
        GetComponent<Collider2D>().bounds.size
    );
}
}
