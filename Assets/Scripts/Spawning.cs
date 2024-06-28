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
    public float MinSpawnRadius = 10.0f; 
    public float SpawnRadius = 10.0f;// Rayon de la zone de spawn
    public float cooldown = 1.0f;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = cooldown;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer<=0)
        {
          RecupPoint();
          timer = cooldown;
        }
         
        
    }

    void RecupPoint()
    {
        // Générer un angle aléatoire en degrés
        float angle = Random.Range(0f, 360f);
        
        // Convertir l'angle en radians
        float angleRad = angle * Mathf.Deg2Rad;
        
        // Générer une distance aléatoire entre MinSpawnDistance et SpawnRadius
        float distance = Random.Range(MinSpawnRadius, SpawnRadius);

        // Calculer la position de spawn en utilisant des coordonnées 
        float x = Mathf.Cos(angleRad) * SpawnRadius;
        float z = Mathf.Sin(angleRad) * SpawnRadius;

        // Utiliser la hauteur du joueur
        Vector3 spawnPosition = new Vector3(x,0, z) + Player.transform.position;

        // Instancier l'ennemi à la position finale
        Instantiate(EnnemiePrefabs, spawnPosition, Quaternion.identity);
       Debug.Log(spawnPosition);
    }
    
}
