using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestruccionFuego : MonoBehaviour
{
    public GameObject LineadeFuego;

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
     if (LineadeFuego.transform.position.x - transform.position.x > 0)
        {
            Destroy(gameObject);
        }
        

    }
}
