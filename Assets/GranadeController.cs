using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeController : MonoBehaviour
{

    public float delay = 3f;
    private float coutDown;
    bool hasExploded = false;
    public GameObject effect;
    public float radius = 5f;
    public float force = 700f;

    // Start is called before the first frame update
    void Start()
    {
        coutDown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        coutDown -= Time.deltaTime;
        if(coutDown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    public void Explode()
    {
        Instantiate(effect, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider nearBy in colliders)
        {
            Rigidbody rb = nearBy.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        Destroy(gameObject);
    }
}
