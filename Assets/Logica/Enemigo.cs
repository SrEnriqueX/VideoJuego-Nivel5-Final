using System.Collections;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Transform jugador;
    public float detectarRadio = 5.0f;
    public float velocidad = 2.0f;
    public float fuerzaRebote = 3f;
    public int vida = 3;

    private bool recibiendoDanio;
    private Rigidbody2D rigidbody2d;
    private Vector2 movimiento;
    private bool jugadorVivo;
    private bool enMovimiento;
    private bool muerto;

    private Personaje scriptJugador;

    private int danoActual = 25;

    public GameObject panelYouWin;

    private Animator animador;

    public AudioClip sonidoDanio;
    private AudioSource audioSource;

    void Start()
    {
        jugadorVivo=true;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        scriptJugador = jugador.GetComponent<Personaje>();
        audioSource = GetComponent<AudioSource>();

    }


    void Update()
    {
        if (jugadorVivo && !muerto)
        {
            Movimiento();
        }
        else
        {
            // El jugador ha muerto o el enemigo está muerto
            animador.SetBool("IsAttack", false);
            animador.SetBool("IsRunning", false);
        }

        if (vida <= 50 && !muerto)
        {
            danoActual = 45;
        }
        else
        {
            danoActual = 25;
        }

        animador.SetBool("IsDie", muerto);

    }
    public int ObtenerDanoActual()
    {
        return vida <= 50 ? 45 : 25;
    }


    private void Movimiento()
    {
        // 1. Actualiza si el jugador está vivo
        jugadorVivo = !scriptJugador.muerto;

        if (!jugadorVivo)
        {
            enMovimiento = false;
            animador.SetBool("IsRunning", false);
            animador.SetBool("IsAttack", false);
            return; // DETIENE todo movimiento y ataque si el jugador está muerto
        }

        float distanciaDelJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaDelJugador < detectarRadio)
        {
            Vector2 direccion = (jugador.position - transform.position).normalized;

            if (direccion.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (direccion.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            movimiento = new Vector2(direccion.x, 0);
            enMovimiento = true;
        }
        else
        {
            movimiento = Vector2.zero;
            enMovimiento = false;
        }

        if (!recibiendoDanio)
        {
            rigidbody2d.MovePosition(rigidbody2d.position + movimiento * velocidad * Time.deltaTime);
            animador.SetBool("IsRunning", enMovimiento);

            float distanciaAtaque = 0.5f;

            if (distanciaDelJugador <= distanciaAtaque)
            {
                animador.SetBool("IsAttack", true);
                animador.SetBool("IsRunning", false);
                movimiento = Vector2.zero;
            }
            else
            {
                animador.SetBool("IsAttack", false);
            }
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            Personaje personaje = collision.gameObject.GetComponent<Personaje>();
            personaje.RecibeDanio(direccionDanio,1);
            jugadorVivo = !personaje.muerto;
            if (!jugadorVivo)
            {
                enMovimiento = false;
                animador.SetBool("IsRunning", false);
            }
        }
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 20);
        }

    }
 
    public void RecibeDanio(Vector2 direccion, int cantidadDanio)
    {
        if (!recibiendoDanio && !muerto)
        {
            vida -=cantidadDanio;
            recibiendoDanio = true;
            animador.SetBool("IsDamage", true);
            animador.SetBool("IsRunning", false);

            if (vida <=0)
            {
                muerto = true;
                enMovimiento = false;

                animador.SetBool("IsDamage", false); 
                animador.SetBool("IsDie", true);
                animador.SetBool("IsAttack", false);
                StartCoroutine(MostrarVictoria());

            }
            else
            {
                Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
                rigidbody2d.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
                StartCoroutine(DesactivaDanio());
            }
            audioSource.PlayOneShot(sonidoDanio);

            rigidbody2d.MovePosition(rigidbody2d.position + movimiento * velocidad * Time.deltaTime);

        }

    }
    IEnumerator MostrarVictoria()
    {
        yield return new WaitForSeconds(0.5f);
        panelYouWin.SetActive(true);
        Time.timeScale = 0f;
    }

    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio=false;
        animador.SetBool("IsDamage", false);
        rigidbody2d.linearVelocity=Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,detectarRadio);

    }

}
