using System.Collections;
using UnityEngine;

public class Enemigo : MonoBehaviour
{
    public Transform jugador;
    public float detectarRadio = 5.0f;
    public float velocidad = 2.0f;
    public float fuerzaRebote = 3f;
    private bool recibiendoDanio;
    private Rigidbody2D rigidbody2d;
    private Vector2 movimiento;
    private bool jugadorVivo;
    void Start()
    {
        jugadorVivo=true;
        rigidbody2d = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        if (jugadorVivo)
        {
            Movimiento();
        }

    }
    private void Movimiento()
    {
        float distanciaDelJugador = Vector2.Distance(transform.position, jugador.position);
        if (distanciaDelJugador < detectarRadio)
        {
            Vector2 direccion = (jugador.position - transform.position).normalized;
            movimiento = new Vector2(direccion.x, 0);
        }
        else
        {
            movimiento = Vector2.zero;
        }
        if (!recibiendoDanio)
        {
            rigidbody2d.MovePosition(rigidbody2d.position + movimiento * velocidad * Time.deltaTime);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);
            Personaje personaje = collision.gameObject.GetComponent<Personaje>();
            personaje.RecibeDanio(direccionDanio,1);
            jugadorVivo = !personaje.muerto;

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Espada"))
        {
            Vector2 direccionDanio = new Vector2(collision.gameObject.transform.position.x, 0);
            RecibeDanio(direccionDanio, 1);
        }
    }
    public void RecibeDanio(Vector2 direccion, int cantidadDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 0.2f).normalized;
            GetComponent<Rigidbody2D>().AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            StartCoroutine(DesactivaDanio());

        }

    }
    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio=false;
        rigidbody2d.linearVelocity=Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color=Color.red;
        Gizmos.DrawWireSphere(transform.position,detectarRadio);
    }
}
