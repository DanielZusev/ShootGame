using UnityEngine;
using UnityEngine.AI;

public class SideKickController : MonoBehaviour
{
    private bool chasing;
    public float distanceToChase = 1000f;
    public float distanceToStop = 8f;
    public float distanceToShoot = 25f;

    private Vector3 targetPoint;
    private Vector3 startPoint;

    public NavMeshAgent agent;

    public float keepChasingTime = 8f;
    private float chaseCounter;

    public GameObject bullet;
    public Transform firePoint;

    public float fireRate, waitBetweenShots = 2f, timeToShoot = 1f;
    private float fireCount, shotWaitCounter, shootTimeCounter;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        startPoint = transform.position; // enemy go back to his start posion
        shootTimeCounter = timeToShoot;
        shotWaitCounter = waitBetweenShots;
    }

    // Update is called once per frame
    void Update()
    {

        targetPoint = PlayerController.instance.transform.position;
        targetPoint.y = transform.position.y;

        if (!chasing)
        {
            if (Vector3.Distance(transform.position, targetPoint) < distanceToChase)
            {
                chasing = true;
                shootTimeCounter = timeToShoot;
                shotWaitCounter = waitBetweenShots;
            }

            if (chaseCounter > 0)
            {
                chaseCounter -= Time.deltaTime;
                if (chaseCounter <= 0)
                {
                    agent.destination = transform.position;
                }
            }

            if (agent.remainingDistance < 0.25f)
            {
                anim.SetBool("isMoving", false);
            }
            else
            {
                anim.SetBool("isMoving", true);
            }

        }
        else
        {
            //transform.LookAt(targetPoint);

            //theRB.velocity = transform.forward * moveSpeed;

            if (Vector3.Distance(transform.position, targetPoint) > distanceToStop)
            {
                agent.destination = targetPoint;
            }
            else
            {
                chasing = false;
                agent.destination = transform.position;
            }


            if (shotWaitCounter > 0)
            {
                shotWaitCounter -= Time.deltaTime;
                if (shotWaitCounter <= 0)
                {
                    shootTimeCounter = timeToShoot;
                }

                anim.SetBool("isMoving", true);
            }
            else
            {
                if (EnemyController.instance.gameObject.activeInHierarchy)
                {
                    if (Vector3.Distance(EnemyController.instance.transform.position, transform.position) < distanceToShoot)
                    {
                        shootTimeCounter -= Time.deltaTime;

                        if (shootTimeCounter > 0)
                        {
                            fireCount -= Time.deltaTime;

                            if (fireCount <= 0)
                            {
                                fireCount = fireRate;

                                firePoint.LookAt(EnemyController.instance.transform.position + new Vector3(0f, 0f, 0f));

                                //check player angle
                                Vector3 targetDirection = EnemyController.instance.transform.position - transform.position;
                                float angle = Vector3.SignedAngle(targetDirection, transform.forward, Vector3.up);

                                if (Mathf.Abs(angle) < 30f)
                                {

                                    Instantiate(bullet, firePoint.position, firePoint.rotation);

                                    anim.SetTrigger("fireShot");

                                }
                                else
                                {
                                    shotWaitCounter = waitBetweenShots;
                                }
                            }

                            agent.destination = transform.position;
                        }
                        else
                        {
                            shotWaitCounter = waitBetweenShots;
                        }

                        anim.SetBool("isMoving", false);
                    }
                }
                else
                {

                }
            }
        }
    }
}
