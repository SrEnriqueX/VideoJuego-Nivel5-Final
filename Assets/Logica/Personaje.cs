using UnityEngine;

public class Personaje : MonoBehaviour
{
    public float velocidad;
    public float fuerzaSalto;
    public int saltosMaximos;
    public LayerMask capaSuelo;

    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider;
    private bool mirandoDerecha = true;
    private int saltosRestantes;
    private bool estabaEnSuelo;
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
        bool enSuelo = EstaEnSuelo(); // lo calculamos una sola vez por eficiencia
        ProcesarMovimiento(enSuelo);
        ProcesarSalto(enSuelo);
        ActualizarAnimaciones(enSuelo);
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

        if (Input.GetKeyDown(KeyCode.Space) && saltosRestantes > 0)
        {
            saltosRestantes--;
            rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, 0f);
            rigidbody2D.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        }
    }

    void ProcesarMovimiento(bool enSuelo)
    {
        float inputMovimiento = Input.GetAxis("Horizontal");

        // Solo activar animaci�n de caminar si est� en el suelo
        animator.SetBool("isRunning", inputMovimiento != 0f && enSuelo);

        rigidbody2D.linearVelocity = new Vector2(inputMovimiento * velocidad, rigidbody2D.linearVelocity.y);
        GestionarOrientacion(inputMovimiento);
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
        float velocidadY = rigidbody2D.linearVelocity.y;

        // Detectar si est� subiendo o bajando
        animator.SetBool("isJumping", !enSuelo && velocidadY > 0.1f);
        animator.SetBool("isFalling", !enSuelo && velocidadY < -0.1f);
    }
}
