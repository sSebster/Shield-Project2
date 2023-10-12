using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemis : MonoBehaviour
{
    #region Variable
    private PlayerControleur player;
    public enum ennemisType : int
    {
        Melee,
        Range,
    }

    public enum movementType : int
    {
        ZigZag,
        StraightLign
    }

    public ennemisType whatIsThisEnnemisType;
    public ennemisType whatIsThisEnnemisMovementType;
    public float whatIsThisEnnemisSpeed;

    #endregion
    public void Start()
    {
        player = FindObjectOfType<PlayerControleur>();
    }
    private void Update()
    {
        Movement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Shield>())
        {
            Destroy(gameObject);
        }
    }

    public void Movement()
    {
        transform.LookAt(player.transform);
        transform.Translate(Vector3.forward * whatIsThisEnnemisSpeed * Time.deltaTime);
    }
}
