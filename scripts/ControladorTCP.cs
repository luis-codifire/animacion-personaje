using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControladorTPC : MonoBehaviour
{
    [Header("Ajustes Movimiento")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 8f;
    [Tooltip("Suavizado para la rotación del personaje")]
    [SerializeField] float rotationSmoothTime = 0.1f;

    [Header("Físicas")]
    [SerializeField] float gravity = -15f;
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float jumpCooldown = 0.5f;

    [Header("Referencias")]
    [Tooltip("Transform de la cámara (puede ser Camera.main.transform o el Transform de una Virtual Camera)")]
    [SerializeField] Transform cameraTransform;

    CharacterController controller;
    Animator animator;

    Vector3 verticalVelocity = Vector3.zero;
    float currentRotationVelocity;           // para SmoothDampAngle
    float jumpTimer;
    readonly int animIDSpeed = Animator.StringToHash("Velocidad");

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (jumpTimer > 0f) jumpTimer -= Time.deltaTime;

        HandleMoveAndRotation();
        HandleGravityAndJump();
    }

    void HandleMoveAndRotation()
    {
        // Leer input (WASD / flechas)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(h, 0f, v).normalized;

        bool running = Input.GetButton("Fire3");
        float targetSpeed = running ? runSpeed : walkSpeed;

        Vector3 horizontalMove = Vector3.zero;

        if (inputDir.magnitude >= 0.1f)
        {
            // Ángulo objetivo relativo a la cámara
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + (cameraTransform ? cameraTransform.eulerAngles.y : 0f);

            // Rotación suave del personaje hacia targetAngle
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentRotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Dirección de avance (orientada por la cámara)
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            horizontalMove = moveDir.normalized * targetSpeed;
        }

        // Animación: enviar velocidad (se puede tunear el parámetro del Animator)
        float animSpeed = (inputDir.magnitude >= 0.1f) ? targetSpeed : 0f;
        animator.SetFloat(animIDSpeed, animSpeed, 0.1f, Time.deltaTime);

        // Aplicar movimiento horizontal + vertical en una sola llamada
        Vector3 totalMove = horizontalMove + new Vector3(0f, verticalVelocity.y, 0f);
        controller.Move(totalMove * Time.deltaTime);
    }

    void HandleGravityAndJump()
    {
        if (controller.isGrounded)
        {
            // asegúrate de que no quede un pequeño valor negativo
            if (verticalVelocity.y < 0f) verticalVelocity.y = -2f;

            if (Input.GetButtonDown("Jump") && jumpTimer <= 0f)
            {
                verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpTimer = jumpCooldown;
            }
        }

        // gravedad aplicada siempre
        verticalVelocity.y += gravity * Time.deltaTime;
    }
}