using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;

    public float moveSpeed;
    public CharacterController charCon;
    public Transform camTrans;
    public float mouseSensitivity;
    public bool invertX;
    public bool invertY;
    public float gravityModifier;
    public float jumpPower;
    public Transform groundCheckPoint;
    public LayerMask whatisGround;
    public float runSpeed = 12f;
    public Animator anim;

    //public GameObject bullet;
    public Transform firePoint;

    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public int currentGun;

    public List<Gun> unlocableGuns = new List<Gun>();

    public AudioSource footStepSlow;
    public float viewAngle = 60f;


    private Vector3 moveInput;
    private bool canJump;
    private bool canDoubleJump;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentGun--;
        SwitchGun();
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.instance.pauseScreen.activeInHierarchy)
        {

            //player movement
            //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

            //store y velocity
            float yStore = moveInput.y;

            Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
            Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

            moveInput = horiMove + vertMove;
            moveInput.Normalize();

            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveInput = moveInput * runSpeed;
            }
            else
            {
                moveInput *= moveSpeed;
            }

            moveInput.y = yStore;
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

            if (charCon.isGrounded)
            {
                moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
            }

            canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatisGround).Length > 0;

            if (canJump)
            {
                canDoubleJump = false;
            }

            //Handle Jumping
            if (Input.GetKeyDown(KeyCode.LeftControl) && canJump)
            {
                moveInput.y = jumpPower;
                canDoubleJump = true;
                AudioManager.instance.PlaySoundEffects(8);
            }
            else if (canDoubleJump && Input.GetKeyDown(KeyCode.LeftControl))
            {
                moveInput.y = jumpPower;
                canDoubleJump = false;
                AudioManager.instance.PlaySoundEffects(8);
            }


            charCon.Move(moveInput * Time.deltaTime);

            // camera controll rotation
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            if (invertX)
            {
                mouseInput.x = -mouseInput.x;
            }

            if (invertY)
            {
                mouseInput.y = -mouseInput.y;
            }

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + mouseInput.x,
                transform.rotation.eulerAngles.z);

            camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));

            if(camTrans.rotation.eulerAngles.x > viewAngle && camTrans.rotation.eulerAngles.x < 180f)
            {
                camTrans.rotation = Quaternion.Euler(viewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
            }
            else
            {
                if(camTrans.rotation.eulerAngles.x > 180f && camTrans.rotation.eulerAngles.x < 360f - viewAngle)
                {
                    camTrans.rotation = Quaternion.Euler(-viewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
                }
            }

            //Handle SHooting
            //single shot
            if (Input.GetKeyDown(KeyCode.Space) && activeGun.fireCounter <= 0)
            {

                RaycastHit hit;
                if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
                {
                    if (Vector3.Distance(camTrans.position, hit.point) > 2f)
                    {
                        firePoint.LookAt(hit.point);
                    }
                }
                else
                {
                    firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
                }

                //Instantiate(bullet, firePoint.position, firePoint.rotation);
                FireShot();
            }

            //Repeats shots
            if (Input.GetKey(KeyCode.Space) && activeGun.canAutoFire)
            {
                if (activeGun.fireCounter <= 0)
                {
                    FireShot();
                }
            }


            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchGun();
            }

            anim.SetFloat("moveSpeed", moveInput.magnitude);
            anim.SetBool("onGround", canJump);
        }
    }

    public void FireShot()
    {
        if (allGuns.Count != 0)
        {

            if (activeGun.currentAmmo > 0)
            {
                activeGun.currentAmmo--;

                Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);

                activeGun.fireCounter = activeGun.fireRate;

                UIController.instance.ammoText.text = "Ammo: " + activeGun.currentAmmo;

            }
        }
    }

    public void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);
        currentGun++;

        if (currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = "Ammo: " + activeGun.currentAmmo;

        firePoint.position = activeGun.firePoint.position;
    }

    public void addGun(string gunToAdd)
    {
        bool gunUnlocked = false;

        if (unlocableGuns.Count > 0)
        {
            for (int i = 0; i < unlocableGuns.Count; i++)
            {
                if (unlocableGuns[i].gunName == gunToAdd)
                {
                    gunUnlocked = true;
                    allGuns.Add(unlocableGuns[i]);
                    unlocableGuns.RemoveAt(i);
                    i = unlocableGuns.Count;
                }
            }
        }

        if (gunUnlocked)
        {
            currentGun = allGuns.Count - 1;
            SwitchGun();
        }
    }
}
