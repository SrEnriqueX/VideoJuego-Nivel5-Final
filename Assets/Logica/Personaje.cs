using UnityEngine;
using System.Collections;

public class Personaje : MonoBehaviour
{
    public float velocidad;
    public float fuerzaSalto;
    public float fuerzaRebote=3f;
    public int vida = 3;
    public int saltosMaximos;
    public LayerMask capaSuelo;

    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider;
    private bool mirandoDerecha = true;
    private int saltosRestantes;
    private bool estabaEnSuelo;
    private bool atacando;
    private bool recibiendoDanio;
    public bool muerto;
    private Animator animator;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        saltosRestantes = saltosMaximos;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool enSuelo = EstaEnSuelo();

        if (!muerto)
        {
            ProcesarMovimiento(enSuelo);
            ProcesarSalto(enSuelo);
            ActualizarAnimaciones(enSuelo);
            procesarAtaque(enSuelo);
        }
        
        //animator.SetBool("IsAttack", atacando);
        animator.SetBool("IsDamage",recibiendoDanio);
        animator.SetBool("IsDie", muerto);
    }

    bool EstaEnSuelo()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            0.2f,
            capaSuelo
        );
        return raycastHit.collider != null;
    }

    void ProcesarSalto(bool enSuelo)
    {
        if (enSuelo && !estabaEnSuelo)
        {
            saltosRestantes = saltosMaximos;
        }
        estabaEnSuelo = enSuelo;

        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0 && !recibiendoDanio)
        {
            saltosRestantes--;
            rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, 0f);
            rigidbody2D.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
    }

    void ProcesarMovimiento(bool enSuelo)
    {
        // Solo mover si no está atacando
        if (!atacando && !recibiendoDanio)
        {
            float inputMovimiento = Input.GetAxis("Horizontal");
            animator.SetBool("isRunning", inputMovimiento != 0f && enSuelo);
            rigidbody2D.linearVelocity = new Vector2(inputMovimiento * velocidad, rigidbody2D.linearVelocity.y);
            GestionarOrientacion(inputMovimiento);
        }
        else if (atacando)
        {
            rigidbody2D.linearVelocity = new Vector2(0, rigidbody2D.linearVelocity.y);
        }

        animator.SetBool("IsAttack", atacando);
    }
    void GestionarOrientacion(float inputMovimiento)
    {
        if ((mirandoDerecha && inputMovimiento < 0) || (!mirandoDerecha && inputMovimiento > 0))
        {
            mirandoDerecha = !mirandoDerecha;
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }

    void ActualizarAnimaciones(bool enSuelo)
    {
        if (recibiendoDanio)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isRunning", false);
            return; // Ignorar otras animaciones
        }

        float velocidadY = rigidbody2D.linearVelocity.y;
        animator.SetBool("isJumping", !enSuelo && velocidadY > 0.1f);
        animator.SetBool("isFalling", !enSuelo && velocidadY < -0.1f);
    }
    public void procesarAtaque(bool enSuelo) 
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !atacando && enSuelo)
        {
            Atacando();
        }
    }
    public void Atacando()
    {
        atacando = true;
    }
    public void DesactivaAtaque()
    {
        atacando = false;
    }
    public void RecibeDanio(Vector2 direccion, int cantidadDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            atacando = false; // por si está atacando, cancelar
            vida-=cantidadDanio;
            if (vida<=0)
            { 
                muerto = true;
            }

            if (!muerto)
            {
                // Calcular dirección de retroceso (hacia la izquierda o derecha)
                float direccionX = Mathf.Sign(transform.position.x - direccion.x);
                Vector2 rebote = new Vector2(direccionX, 0.5f).normalized;

                // Cancelar velocidad previa y aplicar rebote
                rigidbody2D.linearVelocity = Vector2.zero;
                rigidbody2D.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
            }
            

            // Iniciar corrutina para desactivar estado de daño
            StartCoroutine(DesactivaDanio());
        }
    }

    IEnumerator DesactivaDanio()
    {
        yield return new WaitForSeconds(0.4f);
        recibiendoDanio = false;

        // Detener movimiento al terminar retroceso
        rigidbody2D.linearVelocity = Vector2.zero;
    }

}
