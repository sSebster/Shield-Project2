using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControleur : MonoBehaviour

{
    public Shield shield;
    public float shieldSpeed;
    public float distanceToPlayer;
    private Vector3 lastDirection;

    private Vector3 direction;

    void Start()
    {
        StartPosition();
    }

    void Update()
    {
        GetInputAxis();
    }

    private void GetInputAxis()
    {
        shield.transform.LookAt(gameObject.transform);

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9)
        {
            direction = (new Vector3(horizontal, 0, vertical).normalized - gameObject.transform.position) * distanceToPlayer;
        }

        if (direction != Vector3.zero)
        {
            shield.transform.position = Vector3.Slerp(shield.transform.position, direction, Time.deltaTime * shieldSpeed);
            lastDirection = direction;
        }
        else if (shield.transform.position != lastDirection && lastDirection == Vector3.zero)
        {
            shield.transform.position = Vector3.Slerp(shield.transform.position, lastDirection, Time.deltaTime * shieldSpeed);
        }
    }

    private void StartPosition()
    {
        direction = new Vector3(1, 0, 1).normalized * distanceToPlayer;
    }
}
