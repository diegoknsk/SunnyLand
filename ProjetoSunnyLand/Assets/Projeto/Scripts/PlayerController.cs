using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Animator playerAnimator;
    private Rigidbody2D playerRigidBody2d;
    private ControllerGame _ControleGame;
    private SpriteRenderer srPlayer;
    private bool facingRight = true;
    private bool playerInvencivel;

    public GameObject PlayerDie;
    public Transform groundCheck;
    public bool isGround = false;
    public AudioSource fxGame;
    public AudioClip fxPulo;
    public ParticleSystem _poeira;
    //sfx sound effect

    public float speed;
    public float touchRun;

    public int vidas;
    public Color hitColor;
    public Color noHitColor;
    public bool jump = false;
    public int numberJumps = 0;
    public int maximoJump = 2;
    public float jumpForce;
    private float maxVelocityY = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerRigidBody2d = GetComponent<Rigidbody2D>();
        _ControleGame = FindObjectOfType(typeof(ControllerGame)) as ControllerGame;
        srPlayer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        isGround = VerificaPlayerEstaPisandoChao();
        playerAnimator.SetBool("IsGrounded", isGround);
        touchRun = Input.GetAxisRaw("Horizontal");
        Debug.Log(touchRun.ToString());
        if (Input.GetButtonDown("Jump"))
        {
            maxVelocityY = jumpForce / playerRigidBody2d.mass * Time.fixedDeltaTime;
            jump = true;
        }

        SetaMovimento();
    }

    private bool VerificaPlayerEstaPisandoChao()
    {
        // 1 << LayerMask.NameToLayer("Ground") verifica se ta na layer ground
        return Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
    }

    private void FixedUpdate()
    {
        MovePlayer(touchRun);
        if (jump)
        {
            JumpPlayer();
        }
    }

    private void JumpPlayer()
    {
        if (isGround)
        {
            numberJumps = 0;
            CriarPoeira();
        }

        if (isGround || numberJumps < maximoJump)
        {
            playerRigidBody2d.AddForce(new Vector2(0f, jumpForce));
            isGround = false;
            numberJumps++;
            fxGame.PlayOneShot(fxPulo);
            CriarPoeira();
        }

        jump = false;
    }

    void MovePlayer(float movimentoH)
    {
        playerRigidBody2d.velocity = new Vector2(movimentoH * speed, playerRigidBody2d.velocity.y);
        if ((movimentoH < 0 && facingRight) || (movimentoH > 0 && !facingRight))
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        //Vector3 theScale = transform.localScale;
        //theScale.x *= -1;

        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        CriarPoeira();
    }

    void SetaMovimento()
    {
        playerAnimator.SetBool("Walk", playerRigidBody2d.velocity.x != 0 && isGround);
        playerAnimator.SetBool("Jump", !isGround);
        if (!isGround)
        {
            Debug.Log("eixo y " + playerRigidBody2d.velocity.y);
            Debug.Log("eixoY calculado" + "EixoY" + (playerRigidBody2d.velocity.y * 4 / maxVelocityY));
            float eixoY = playerRigidBody2d.velocity.y * 4 / maxVelocityY;
            playerAnimator.SetFloat("EixoY", eixoY);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Coletaveis":
                Destroy(collision.gameObject);
                _ControleGame.Pontuar(1);
                break;
            case "Inimigo":
                // adiciona for;ca ao pulo                
                Debug.Log("entrou no evento de trigger");
                GameObject tempExplosao = Instantiate(_ControleGame.hitPrefab, transform.position, transform.localRotation);
                Destroy(tempExplosao, 0.5f);

                Rigidbody2D rigidbody = GetComponentInParent<Rigidbody2D>(); //busquei o rigidy body do mesmo pai
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
                rigidbody.AddForce(new Vector2(0, 600));

                _ControleGame.fxGame.PlayOneShot(_ControleGame.fxExplosao);
                //destroi o inimigo
                Destroy(collision.gameObject);
                break;
            case "Damage":
                Hurt();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Plataforma":
                this.transform.parent = collision.transform;
                break;

            case "Inimigo":
                Hurt();
                break;
       
            default:
                break;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Plataforma":
                this.transform.parent = null;
                break;
          
            default:
                break;
        }
    }

    private void Hurt()
    {
        if (!playerInvencivel)
        {
            playerInvencivel = true;
            vidas--;
            StartCoroutine("Dano");
            _ControleGame.AtualizaBarraVidas(vidas);
            Debug.Log($"Perdeu uma vida, qtd de vidas{vidas}");
            playerInvencivel = false;

            if (vidas < 1)
            {
                GameObject pDieTemp = Instantiate(PlayerDie, transform.position, Quaternion.identity);
                Rigidbody2D rbDie = pDieTemp.GetComponent<Rigidbody2D>();
                rbDie.AddForce(new Vector2(150f, 500f));
                _ControleGame.fxGame.PlayOneShot(_ControleGame.fxDie);
                Invoke("CarregaJogo", 4f);
                gameObject.SetActive(false);
            }
        }        
    }

    private void CarregaJogo()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Dano()
    {
        srPlayer.color = noHitColor;
        for (float i = 0; i < 1; i += 0.1f)
        {
            srPlayer.enabled = false;
            yield return new WaitForSeconds(0.1f);
            srPlayer.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        srPlayer.color = Color.white;
    }

    void CriarPoeira()
    {
        _poeira.Play();
    }

}
