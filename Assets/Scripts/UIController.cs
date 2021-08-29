using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public static UIController instance;
    public Slider healthSlider;
    public Text healthText;

    public Text ammoText;

    public Image damageImage;
    public float damageAlpha = .2f;
    public float damageFadeSpeed = 2f;

    public GameObject pauseScreen;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(damageImage.color.a != 0)
        {
            damageImage.color = new Color(
                damageImage.color.r,
                damageImage.color.g,
                damageImage.color.b,
                Mathf.MoveTowards(damageImage.color.a, 0f, damageFadeSpeed * Time.deltaTime));
        }
    }

    public void ShowDamage()
    {
        damageImage.color = new Color(damageImage.color.r, damageImage.color.g, damageImage.color.b, damageAlpha);
    }
}
