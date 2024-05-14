using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bullet : MonoBehaviour
{
    #region Variable
    public enum bulletType : int
    {
        Simple,
        Bouncing,
    }

    public float bulletPower;
    public bulletType whatIsThisBulletType;

    #endregion
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Shield>())
        {
            if(whatIsThisBulletType == bulletType.Simple)
            {
                //Die
            }
            else if(whatIsThisBulletType == bulletType.Bouncing)
            {
                //Rebondis
            }
        }
    }
}