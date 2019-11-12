using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerDebug : MonoBehaviour
{
    public Transform SpawnLocation;
    public GameObject SpawnPrefab;

    public void SpawnPumpkin()
    {
        var pumpkin = Instantiate(SpawnPrefab);

        pumpkin.GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.Impulse);
    }
}
