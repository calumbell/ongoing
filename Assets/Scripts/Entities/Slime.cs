using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : EntityBaseClass
{

    private Transform target;
    public float chaseRadius;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }

    void CheckDistance()
    {
        if(Vector3.Distance(target.position, transform.position) < chaseRadius
            && Vector3.Distance(target.position, transform.position) > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }
}
