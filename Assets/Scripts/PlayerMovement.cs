using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public int VelocidadPlayer = 10;
    private Rigidbody2D _compRigidBody2;
    private Animator _compAnimator;
    private SpriteRenderer _compSpriteRenderer;
    public bool IsMoving = false;
    public float horizontal;
    public float vertical;
    private void Awake()
    {
        _compAnimator = GetComponent<Animator>();
        _compRigidBody2 = GetComponent<Rigidbody2D>();
        _compSpriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Verifica si el jugador se está moviendo
        IsMoving = horizontal != 0 || vertical != 0;

        // Actualiza el Animator solo si el jugador se está moviendo
        if (IsMoving)
        {
            _compAnimator.SetInteger("SideMovement", (int)horizontal);

            // Cambia la dirección del sprite según el movimiento
            if (horizontal < 0)
            {
                _compSpriteRenderer.flipX = true;
            }
            else if (horizontal > 0)
            {
                _compSpriteRenderer.flipX = false;
            }
        }
        else
        {
            // Detén la animación si no hay movimiento
            _compAnimator.SetInteger("SideMovement", 0);
        }
    }
    private void FixedUpdate()
    {
        _compRigidBody2.linearVelocity = new Vector2(VelocidadPlayer * horizontal, VelocidadPlayer * vertical);
    }
}
