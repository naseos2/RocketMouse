using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MouseController : MonoBehaviour
{
    public float jetpackForce = 75.0f;
    public ParticleSystem jetpack;

    public float forwardMovementSpeed = 3.0f;

    private Rigidbody2D rb;

    public Transform groundCheckTransform;
    public LayerMask groundCheckLayerMask;
    private bool grounded;
    private Animator animator;

    private bool dead = false;

    private uint coins = 0;
    public Text textCoins;

    public Image[] lifeUi;

    public GameObject buttonRestart;
    public GameObject buttonMenu;

    public AudioClip coinbCollectSound;
    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;
    public AudioSource bgMusicAudio;
 
    public ParallaxScroll parallaxScroll;

    private int lv;
    public float lvUpInterval;
    private float lvUpTimeCnt;
    public Text textLv;


    public float feverInterval;
    public float feverTime;
    public bool isFever;

    public int lifeCnt;
    private float invincibleTimeCnt;

    private SpriteRenderer sp;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        textCoins.text = "0";

        lv = 1;
        textLv.text = $"Lv.{lv}";

        // 어디에 넣어도 상관 없을 때는 보통 기존 코드의 아래에 쓴다
        LoadVolume();

        // 코르틴 시작
        StartCoroutine(FeverCtrl());

    }

    private void FixedUpdate()
    {
        bool jetpackActive = Input.GetButton("Fire1");

        if(!dead)
        {
            if (jetpackActive)
            {
                rb.AddForce(jetpackForce * Vector2.up);
            }

            Vector2 newVelocity = rb.velocity;
            newVelocity.x = forwardMovementSpeed;
            rb.velocity = newVelocity;
        }

        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
        DisplayButtons();
        AdjustFootstepsAndJetpackSound(jetpackActive);

        parallaxScroll.offset = transform.position.x;
    }

    private void Update()
    {
        if (dead)
            return; 

        lvUpTimeCnt += Time.deltaTime;
        if (lvUpTimeCnt >= lvUpInterval)
        {
            lv++;
            textLv.text = $"Lv.{lv}";
            forwardMovementSpeed = 2.5f + lv * 0.5f;

            lvUpTimeCnt = 0;
        }
        

        if (invincibleTimeCnt > 0)
            invincibleTimeCnt -= Time.deltaTime;


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coins")
            CollectCoin(collision);
        else
            HitByLaser(collision);
    }

    private void CollectCoin(Collider2D coinCollider)
    {
        ++coins;
        Destroy(coinCollider.gameObject);
        textCoins.text = coins.ToString();
        AudioSource.PlayClipAtPoint(coinbCollectSound, transform.position);
    }

    private void HitByLaser(Collider2D laserCollider)
    {
        if (invincibleTimeCnt > 0)
            return;

        if (!dead)
        {
            AudioSource laser = laserCollider.GetComponent<AudioSource>();
            laser.Play();
        }

        --lifeCnt;
        Destroy(lifeUi[lifeCnt]);
        if (lifeCnt > 0)
        {
            invincibleTimeCnt = 3f;
            StartCoroutine(InvincibleTime());
            return;
        }

        dead = true;
        animator.SetBool("dead", true);

        StopCoroutine(FeverCtrl());
    }

    private void UpdateGroundedStatus()
    {
        // mouse가 땅에 닿았는지 확인하기 위해 groundCheckTransform 위치에 반지금 0.1인 원을 만들고
        // 만약 원이 groundCheckLayerMask와 겹친다면 땅에 닿은 것으로 판정
        grounded = Physics2D.OverlapCircle(groundCheckTransform.position, 0.1f, groundCheckLayerMask);
        animator.SetBool("grounded", grounded);
    }

    private void AdjustJetpack(bool jetpackActive)
    {
        var emission = jetpack.emission;
        emission.enabled = !grounded;
        emission.rateOverTime = jetpackActive ? 300.0f : 75.0f;
    }

    private void DisplayButtons()
    {
        bool active = buttonRestart.activeSelf;
        if (grounded && dead && !active)
        {
            buttonRestart.SetActive(true);
            buttonMenu.SetActive(true);
        }
    }

    public void OnClickedRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnClickedMenuButton()
    {
        SceneManager.LoadScene("Menu");
    }

    private void AdjustFootstepsAndJetpackSound(bool jetpackActive)
    {
        footstepsAudio.enabled = !dead && grounded;
        jetpackAudio.enabled = !dead && !grounded;
        jetpackAudio.volume = jetpackActive ? 1.0f : 0.5f;
    }

    private void LoadVolume()
    {
        float volume = PlayerPrefs.GetFloat("bgVolume");
        bgMusicAudio.volume = volume;
    }

    IEnumerator InvincibleTime()
    {
        for (int i = 0; i < 3; i++)
        {
            sp.color = new Color(0.5f, 0, 0, 0.5f);   
            sp.enabled = false;
            yield return new WaitForSeconds(0.5f);
            sp.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }
        sp.color = Color.white;
    }

    // 잠깐 켜졌다가 어느정도 기다렸다가 다시 키기 가능 (코르틴이라고 무조건 반복문이 사용되는게 아님)
    IEnumerator FeverCtrl()
    {
        while(true)
        {
            yield return new WaitForSeconds(feverInterval);

            Debug.Log("피버 시작");
            isFever = true;
            forwardMovementSpeed = 10f;
            GameObject[] lasers = GameObject.FindGameObjectsWithTag("Laser");
            foreach (var obj in lasers)
                obj.SetActive(false);

            yield return new WaitForSeconds(feverTime);

            Debug.Log("피버 끝");
            isFever = false;
            // 레벨 맞춰서 속도값을 계산하면 피버타임 사이에 레벨이 올라가더라도 피버타임이 끝나고 적용시켜줄 수 있다. 
            forwardMovementSpeed = 2.5f + lv * 0.5f;
        }
    }

}
