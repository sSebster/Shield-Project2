using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemis : MonoBehaviour
{
    #region Variable
    private PlayerControleur player;
    private Rigidbody rb;
    public enum ennemisType : int
    {
        Melee,
        Range,
    }

    public enum movementType : int
    {
        ZigZag,
        StraightLign,
        Static,
    }

    public ennemisType whatIsThisEnnemisType;
    public movementType whatIsThisEnnemisMovementType;
    public float whatIsThisEnnemisSpeed;
    public float whatIsThisEnnemisMaxSpeed;

    public Transform pointOfShoot;
    public GameObject bullet;
    public float bulletPower;

    public float cooldown;
    private float actualTime;

    #endregion
    public void Start()
    {
        player = FindObjectOfType<PlayerControleur>();
        rb = GetComponent<Rigidbody>();
        actualTime = cooldown;
    }
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Cooldown();
        Movement();
        if(actualTime <= 0 && whatIsThisEnnemisType == ennemisType.Range)
        {
            Shoot();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Shield>())
        {
            Destroy(gameObject);
        }
    }

    //Comment bouge l'ennemis
    private void Movement()
    {
        if (whatIsThisEnnemisType == ennemisType.Melee)
        {
            if (whatIsThisEnnemisMovementType == movementType.StraightLign)
            {
                StraightLignMovement();
            }
            if (whatIsThisEnnemisMovementType == movementType.ZigZag)
            {
                ZigZagMovement();
            }
        }
        else if (whatIsThisEnnemisType == ennemisType.Range)
        {

        }
    }

    //Mouvement en ligne droite
    public void StraightLignMovement()
    {
        transform.LookAt(player.transform); 
        if(rb.velocity.magnitude < whatIsThisEnnemisMaxSpeed)
        {
            rb.AddForce(transform.forward * whatIsThisEnnemisSpeed * Time.deltaTime, ForceMode.Force);
        }
    }

    //Mouvement en ZigZag
    public void ZigZagMovement()
    {
        //à définir
    }

    //Tir de l'ennemi
    public void Shoot()
    {
        transform.LookAt(player.transform);
        GameObject lastBullet;
        lastBullet = Instantiate(bullet, pointOfShoot.position, Quaternion.identity);
        lastBullet.transform.LookAt(player.transform);

        Rigidbody rbBullet;
        rbBullet = lastBullet.GetComponent<Rigidbody>();
        Vector3 bulletDirection = player.transform.position - transform.position;
        Debug.DrawLine(transform.position, bulletDirection, Color.magenta, 10);
        rbBullet.AddForce(bulletDirection * bulletPower * Time.deltaTime, ForceMode.Impulse);

        actualTime = cooldown;
    }

    private void Cooldown()
    {
        if (actualTime > 0)
        {
            actualTime -= Time.deltaTime;
        }
    }
}