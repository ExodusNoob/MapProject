using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }

    [SerializeField]
    private List<GameObject> entrance;

    [SerializeField]
    private List<GameObject> rooms;

    [SerializeField]
    private List<GameObject> specialRooms;

    [SerializeField]
    private List<GameObject> alternateEntrances;

    [SerializeField]
    private List<GameObject> hallways;

    [SerializeField]
    private GameObject door;

    [SerializeField]
    private int noOfRooms = 10;

    [SerializeField]
    private LayerMask roomsLayerMask;

    [SerializeField]
    private List<DungeonPart> generatedRooms;

    //private bool isGenerated = false;

    void Start()
    {
        generatedRooms = new List<DungeonPart>();
    }

    public void StartGeneration()
    {
        StartGeneratingServerRPC();
    }

    private void StartGeneratingServerRPC()
    {
        Generate();
        GenerateAlternativeEntrances();
        FillEmptyEntrances();
        //isGenerated = true;
    }

    private void Generate()
    {
        for (int i = 0; i < noOfRooms - alternateEntrances.Count; i++) //un for para crear el numero de cuartos deseados
        {
            if (generatedRooms.Count < 1)
            {
                int randomIndex = Random.Range(0, hallways.Count);
                GameObject generatedRoom = Instantiate(entrance[randomIndex], transform.position, transform.rotation); //se crea el primer cuarto
                if(generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    generatedRooms.Add(dungeonPart);
                }
                Debug.Log("generamos la recepci�n");
            } //si el contador de cuartos es menor que 0, se crea el primer cuarto
            else
            {
                Debug.Log("hay al menos un cuarto y buscamos entrypoints libres");
                bool shouldPlaceHallway = Random.Range(0f, 1f) > 0.5f;
                DungeonPart randomGeneratedRoom = null; 
                Transform room1EntryPoint = null;
                int totalRetries = 100;
                int retryIndex = 0;

                while (randomGeneratedRoom == null && retryIndex < totalRetries)
                {
                    Debug.Log("son " + retryIndex + " en encontrar un entrypoint en el cuarto");
                    int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count); //buscamos en uno de los cuartos ya existentes
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];
                    if (roomToTest.HasAvailableEntrypoint(out room1EntryPoint))
                    {
                        Debug.Log("encontramos entry en el " + roomToTest.name + " y se ocupo");
                        randomGeneratedRoom = roomToTest;
                        break;
                    }
                    retryIndex++;
                }

                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);
                Debug.Log("generamos una puerta, piruri");

                if (shouldPlaceHallway)
                {
                    Debug.Log("vamos a generar un hallway, yupi");

                    int randomIndex = Random.Range(0, hallways.Count);
                    GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);
                    generatedHallway.transform.SetParent(null);
                    if (generatedHallway.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                    {
                        if (dungeonPart.HasAvailableEntrypoint(out Transform room2EntryPoint))
                        {
                            generatedRooms.Add(dungeonPart);
                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            //doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedHallway.transform, room1EntryPoint, room2EntryPoint);
                            Debug.Log("se alineo " + randomGeneratedRoom + " y " + generatedHallway);

                            if (HandleIntersection(dungeonPart))
                            {
                                Debug.Log("si entraste es que es true, papu no");
                                dungeonPart.UnuseEntrypoint(room2EntryPoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                bool success = RetryPlacement(generatedHallway, doorToAlign);

                                if (!success)
                                {
                                    Debug.Log("Reintentamos crear otra pieza");
                                    i--;
                                    continue; // vuelve al for como si nunca pasó
                                }
                            }
                        }
                    }
                }
                else
                {
                    GameObject generatedRoom;

                    Debug.Log("vamos a generar un cuarto, buu");

                    if (specialRooms.Count > 0)
                    {
                        bool shouldPlaceSpecialRoom = Random.Range(0f, 1f) > 0.9f;

                        if (shouldPlaceSpecialRoom)
                        {
                            int randomIndex = Random.Range(0, specialRooms.Count);
                            generatedRoom = Instantiate(specialRooms[randomIndex], transform.position, transform.rotation);
                        }
                        else
                        {
                            int randomIndex = Random.Range(0, rooms.Count);
                            generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        }
                    }
                    else
                    {
                        int randomIndex = Random.Range(1, rooms.Count);
                        generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        Debug.Log("del segundo para adelante, se genero " + generatedRoom.name);
                    }

                    generatedRoom.transform.SetParent(null);

                    if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                    {
                        if (dungeonPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
                        {
                            generatedRooms.Add(dungeonPart);
                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            //doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1EntryPoint, room2Entrypoint);
                            Debug.Log("se alineo " + randomGeneratedRoom + " y " + generatedRoom);

                            if (HandleIntersection(dungeonPart))
                            {
                                Debug.Log("los cuartos colisionan iiii");
                                dungeonPart.UnuseEntrypoint(room2Entrypoint);
                                randomGeneratedRoom.UnuseEntrypoint(room1EntryPoint);
                                bool success = RetryPlacement(generatedRoom, doorToAlign);

                                if (!success)
                                {
                                    Debug.Log("Reintentamos crear otra pieza");
                                    i--;
                                    continue; // vuelve al for como si nunca pasó
                                }
                            }
                        }
                    }
                }
            }

        }
    }

    private void GenerateAlternativeEntrances()
    {
        if (alternateEntrances.Count < 1) return;
        for (int i = 0; i < alternateEntrances.Count; i++)
        {
            {
                DungeonPart randomGeneratedRoom = null;
                Transform room1Entrypoint = null;
                int totalRetries = 100;
                int retryIndex = 0;

                while (randomGeneratedRoom == null && retryIndex < totalRetries)
                {
                    int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count);
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

                    if (roomToTest.HasAvailableEntrypoint(out room1Entrypoint))
                    {
                        randomGeneratedRoom = roomToTest;
                        break;
                    }
                    retryIndex++;
                }

                int randomIndex = Random.Range(0, alternateEntrances.Count);
                GameObject generatedRoom = Instantiate(alternateEntrances[randomIndex], transform.position, transform.rotation);

                generatedRoom.transform.SetParent(null);

                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);

                if (generatedRoom.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
                {
                    if (dungeonPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
                    {
                        generatedRooms.Add(dungeonPart);
                        doorToAlign.transform.position = room1Entrypoint.transform.position;
                        doorToAlign.transform.rotation = room1Entrypoint.transform.rotation;
                        AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1Entrypoint, room2Entrypoint);

                        if (HandleIntersection(dungeonPart))
                        {
                            Debug.Log("se intersectan y vamos a hacer cositas");
                            dungeonPart.UnuseEntrypoint(room2Entrypoint);
                            randomGeneratedRoom.UnuseEntrypoint(room1Entrypoint);
                            RetryPlacement(generatedRoom, doorToAlign);
                            continue;
                        }
                    }
                }
            }
        }
    }

    private void AlignRooms(Transform room1, Transform room2, Transform room1Entry, Transform room2Entry)
    {
        Debug.Log("cuarto 1 " + room1.name + " y " + room2.name);
        Vector3 offset = room1Entry.position - room2Entry.position;

        room2.position += offset;

        Physics2D.SyncTransforms();
    }
    
    private bool HandleIntersection(DungeonPart dungeonPart)
    {
        Debug.Log("vamos a ver si se tocan, uy");
        bool didIntersect = false;

        BoxCollider2D box = dungeonPart.GetComponent<BoxCollider2D>();

        Collider2D[] hits = Physics2D.OverlapBoxAll(box.bounds.center, box.bounds.size, 0f, roomsLayerMask);

        foreach (Collider2D hit in hits)
        {
            if (hit == box) continue;
            
                didIntersect = true;
            Debug.Log("COLISION: " + dungeonPart.name + "  <-->  " + hit.gameObject.name);
                break;
        }
        Debug.Log("que tal, se intersectan? " + didIntersect);
        return didIntersect;
    }

    //private void RetryPlacement(GameObject itemToPlace, GameObject doorToPlace)
    //{
    //    Debug.Log("retry?"); 
    //    Debug.Log("retry " + itemToPlace.name + " y " + doorToPlace.name); 
    //    DungeonPart randomGeneratedRoom = null; 
    //    Transform room1Entrypoint = null; 
    //    int totalRetries = 100; 
    //    int retryIndex = 0; 
    //    while (randomGeneratedRoom == null && retryIndex < totalRetries) 
    //    { 
    //        int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count - 1); 
    //        DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex]; 
    //        Debug.Log("eligio de la lista al cuerto " + roomToTest); 
    //        if (roomToTest.HasAvailableEntrypoint(out room1Entrypoint)) 
    //        { 
    //            randomGeneratedRoom = roomToTest; break; 
    //        } 
    //        retryIndex++; 
    //    }
    //
    //    //si son 2 partes creadas y que no caberan jamas entre si, sera un loop infinito, a menos que a cierto numero de
    //    //intentos eliminemos la part2 y creemos otra parte
    //    if (itemToPlace.TryGetComponent<DungeonPart>(out DungeonPart dungeonPart))
    //    {
    //        Debug.Log("si " + itemToPlace.name + " tiene el dungeonPart");
    //        if (dungeonPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
    //        {
    //            doorToPlace.transform.position = room1Entrypoint.transform.position; 
    //            //doorToPlace.transform.rotation = room1Entrypoint.transform.rotation;
    //            AlignRooms(randomGeneratedRoom.transform, itemToPlace.transform, room1Entrypoint, room2Entrypoint);
    //            Debug.Log("Alineamos " + randomGeneratedRoom.name + " y " + itemToPlace.name + " en el retry");
    //
    //            if (HandleIntersection(dungeonPart)) 
    //            { 
    //                Debug.Log("esto es una mmd, falla todo"); 
    //                dungeonPart.UnuseEntrypoint(room2Entrypoint); 
    //                randomGeneratedRoom.UnuseEntrypoint(room1Entrypoint); 
    //                RetryPlacement(itemToPlace, doorToPlace);
    //            } 
    //        } 
    //    } 
    //}

    private bool RetryPlacement(GameObject itemToPlace, GameObject doorToPlace)
    {
        Debug.Log("Retry");
        Debug.Log("Restry " + itemToPlace.name + " y " + doorToPlace.name);
        const int MAX_ATTEMPTS = 25;

        if (!itemToPlace.TryGetComponent(out DungeonPart dungeonPart))
            return false;

        for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
        {
            DungeonPart randomGeneratedRoom = null;
            Transform room1Entrypoint = null;

            int retries = 0;

            while (randomGeneratedRoom == null && retries < 50)
            {
                int index = Random.Range(0, generatedRooms.Count - 1);
                DungeonPart roomToTest = generatedRooms[index];
                Debug.Log("en retry testeamos eligiendo de la lista a " + roomToTest);

                if (roomToTest.HasAvailableEntrypoint(out room1Entrypoint))
                {
                    randomGeneratedRoom = roomToTest;
                    break;
                }

                retries++;
            }

            if (randomGeneratedRoom == null)
                continue;

            if (!dungeonPart.HasAvailableEntrypoint(out Transform room2Entrypoint))
                continue;

            doorToPlace.transform.position = room1Entrypoint.position;

            AlignRooms(randomGeneratedRoom.transform, itemToPlace.transform, room1Entrypoint, room2Entrypoint);
            Debug.Log("alineamos en retry" + randomGeneratedRoom.name + " con su punto " + room1Entrypoint.name + " y " + itemToPlace.name + " con su punto " + room2Entrypoint.name);

            if (!HandleIntersection(dungeonPart))
            {
                Debug.Log("Placement exitoso después de " + attempt + " intentos");
                return true;
            }

            dungeonPart.UnuseEntrypoint(room2Entrypoint);
            randomGeneratedRoom.UnuseEntrypoint(room1Entrypoint);
        }

        Debug.Log("Falló después de 25 intentos → destruimos pieza");

        generatedRooms.Remove(dungeonPart);
        Debug.Log("retiramos de la lista a " + dungeonPart.name);
        Destroy(itemToPlace);
        Debug.Log("destruimos " + itemToPlace.name);
        Destroy(doorToPlace);
        Debug.Log("destruimos " + doorToPlace);

        return false;
    }

    private void FillEmptyEntrances()
    {
        generatedRooms.ForEach(room => room.FillEmptyDoors());
    }
}
