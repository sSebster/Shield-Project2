using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class Spawning : MonoBehaviour
{
    public GameObject Player;
    public GameObject EnnemiePrefabs; //prefab
    private Vector3 Vecennemis;
    //private float SpawnInterval = 3.0f;// time 
    public float SpawnRadius = 3.0f;// distance
    
    // Start is called before the first frame update
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
        }

        RecupPoint();
    }

    void RecupPoint()
    {
        //Vecennemis =  new Vector3(Player.transform.position.x-Random.Range(-1.0f, 1.0f) ,Player.transform.position.y ,Player.transform.position.z-Random.Range(-1.0f,1.0f));
        Vecennemis = new Vector3(Random.Range(Player.transform.position.x -1,Player.transform.position.x +1 ), /*Player.transform.position.y*/ 0, Random.Range(Player.transform.position.z-1,Player.transform.position.z +1 ));
        Instantiate(EnnemiePrefabs, Vecennemis, Quaternion.identity);
        Debug.Log(Vecennemis);
        //sort les fichier 
        //faire la normalize sur le vecteur apres la construction de deux points 
    }
    
}
