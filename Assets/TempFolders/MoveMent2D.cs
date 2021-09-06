using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMent2D : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 0.0f;
    public float MoveSpeed { get { return moveSpeed; }set { moveSpeed = value; } }
[SerializeField]
    Vector3 MoveDirection = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += MoveDirection * moveSpeed * Time.deltaTime;
    }
    public void MoveTo(Vector3 direction) { MoveDirection = direction; }
}
