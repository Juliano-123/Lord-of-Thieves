﻿using UnityEngine;
using System.Collections;
using System.ComponentModel.Design;
using Unity.Burst.CompilerServices;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //components
    public Animator animator;
    SpriteRenderer playerSpriteRenderer;
    Controller2D _controller;

    //para golpeo
    [SerializeField]
    bool _jugadorGolpeado = false;
    [SerializeField]
    public float timerGolpeadoIzquierda = 1f;
    [SerializeField]
    public float timerGolpeadoDerecha = 1f;
    public static float timerGolpeadoArriba = 1f;
    public static float timerGolpeadoAbajo = 1f;

    PlayerInput _playerInput;
    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _dashAction;
    
    public GameOverScreen gameOverScreen;
    public AudioSource audioJugador;
    public AudioSource audioGemas;
    public AudioClip salto, dashlisto, dasheando, agarrogema;


    float _maxJumpHeight = 1.7F;
    float _minJumpHeight = 0.5F;
    float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.1f;
    float aceleracionPuntoMasAlto = 0.075f;
    float accelerationTimeGrounded = 0.005f;
    float moveSpeed = 4.5f;
    public static int orientacionX = 1;
    public float velocidadtTiempoExtraAire = 0.5f;
    public float multiplicadorGravedadCaida = 1.5f;
    public float multiplicadorGravedadPuntoMasAlto = 0.5f;


    public static float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    public static Vector2 velocity;
    float velocityXSmoothing;
    
    //COYOTE
    public float tiempoCoyote = -1f;
    public bool tiempoCoyoteON = false;
    

    [SerializeField]
    int jumpApretado;
    float tiempoJump1 = -1;
    public bool jumpSoltado = false;
    public int _saltosTotales = 0;
    float timerdash = 1f;

    public static int boxContados = 0;
    public static int gemasContadas = 0;

    

    int dashApretado = 0;
    public float dashVelocity = 15f;
    float timeForNextDash = 0;
    public float delayForDash = 1f;
    bool yaSonoElDash = true;





    float flashTime = 0.008f;

    public Ghost ghost;



    private void Awake()
    {
        gemasContadas = 0;
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["MOVE"];
        _jumpAction = _playerInput.actions["JUMP"];
        _dashAction = _playerInput.actions["DASH"];

        _controller = GetComponent<Controller2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }



    void Start()
    {


        //CACULA GRAVEDAD EN BASE A LOS VALORES DE ALTURA MAXIMA Y TIEMPO PARA LLEGAR A ELLA
        gravity = -(2 * _maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        //CALCULA VELOCIDAD DEL SALTO COMO GRAVEDAD * TIEMPO PARA LLEGAR ARRIBA
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        //CALCULA LA VELOCIDAD MINIMA EN BASE A GRAVEDAD Y SALTO MINIMO
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * _minJumpHeight);

    }

    void Update()
    {

        // GRAVEDAD
        velocity.y += gravity * Time.deltaTime;

        //SI ME GOLPEARON
        if (_jugadorGolpeado == true)
        {
            //GOLPEADO
            if (timerGolpeadoIzquierda <= 0.3f)
            {
                timerGolpeadoIzquierda += Time.deltaTime;
                FlashRed();
                velocity.y = 3f;
                velocity.x = 6f;
                _controller.Move(velocity * Time.deltaTime, tiempoCoyoteON);
                return;
            }


            if (timerGolpeadoDerecha <= 0.3f)
            {
                timerGolpeadoDerecha += Time.deltaTime;
                FlashRed();
                velocity.y = 3f;
                velocity.x = -6f;
                _controller.Move(velocity * Time.deltaTime, tiempoCoyoteON);
                return;
            }

            if (timerGolpeadoDerecha >= 0.3f && timerGolpeadoIzquierda >= 0.3f)
            {
                _jugadorGolpeado = false;
                
            }
        }

        if (_jugadorGolpeado == false)
        {

            //GUARDA LA ORIENTACION SEGUN el input
            if (_moveAction.ReadValue<Vector2>().x > 0)
            {
                orientacionX = 1;
            }
            else if (_moveAction.ReadValue<Vector2>().x < 0)
            {
                orientacionX = -1;
            }

            CambiarDireccionSprite();


            float targetVelocityX;
            //SETEA TARGET VELOCITY COMO EL MOVESPEED TOTAL CON SINGO POSITIVO/NEGATIVO
            targetVelocityX = (_moveAction.ReadValue<Vector2>().x * moveSpeed);

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (_controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);


            //Hay que hacer tres estados: Golpeado > Moviendose > Colisionando. Si golpeado, no hace nada de lo demas. Si no golpeado, se mueve
            //de acuerdo al input del jugador. Luego de eso checkea si hay colisiones y esas reglas overridean el movimiento.


            HandleCollisions();



            //SALTO TOMAR INPUT
            if (_jumpAction.WasPressedThisFrame())
            {
                jumpApretado = jumpApretado + 1;
                jumpSoltado = false;
                tiempoJump1 = Time.time;
            }

            //SUELTO SALTO TOMAR INPUT
            if (_jumpAction.WasReleasedThisFrame())
            {
                jumpSoltado = true;
            }

            //DASH TOMAR INPUT
            if (_dashAction.WasPressedThisFrame() && timeForNextDash <= 0)
            {
                dashApretado = dashApretado + 1;

            }

            //FUNCION FANTASMA
            timeForNextDash -= Time.deltaTime;

            if (timeForNextDash <= 0)
            {
                ghost.makeGhost = true;
                if (yaSonoElDash == false)
                {
                    audioJugador.clip = dashlisto;
                    audioJugador.Play();
                    yaSonoElDash = true;
                }
            }
            else if (timerdash >= 0.3f)
            {
                ghost.makeGhost = false;
            }

            //DECIDIR QUE DASHEO
            if (dashApretado > 0 && timeForNextDash <= 0)
            {
                timerdash = 0;
                dashApretado = 0;
                jumpApretado = 0;
                _saltosTotales = 1;
                audioJugador.clip = dasheando;
                audioJugador.Play();
                yaSonoElDash = false;
            }


            //QUE HAGO SI DASHEO
            if (timerdash <= 0.3f)
            {
                timerdash += Time.deltaTime;
                velocity.y = 0;
                velocity.x = dashVelocity * orientacionX;
                timeForNextDash = delayForDash;
                if (jumpApretado > 0)
                {
                    jumpApretado = 0;
                    velocity.x = 0;
                    velocity.y = maxJumpVelocity;
                    _saltosTotales = 2;
                    timerdash = 1f;
                }
                _controller.Move(velocity * Time.deltaTime, false);
                return;
            }

            //SALTO
            //SI APRETE JUMP, ESTOY TOCANDO PISO o RECIEN LO TOQUE, Y ESTOY DENTRO DEL TIEMPO BUFFER
            if (jumpApretado > 0 && ((_controller.collisions.below && _controller.collisions.objetoGolpeadoVertical.tag == "Piso") || tiempoCoyoteON) && Time.time - tiempoJump1 < 0.15 && _saltosTotales == 0)
            {
                animator.SetBool("Saltando", true);
                audioJugador.clip = salto;
                audioJugador.Play();
                _saltosTotales += 1;
                velocity.y = maxJumpVelocity;
                Debug.Log(velocity.y.ToString());
                jumpApretado = 0;
            }

            //DOBLE SALTO
            //SI APRETE JUMP, NO ESTOY TOCANDO SUELO, NO SALTE DOS VECES
            if (jumpApretado > 0 && !_controller.collisions.below && !tiempoCoyoteON && _saltosTotales == 1)
            {
                animator.SetBool("Saltando", true);
                audioJugador.clip = salto;
                audioJugador.Play();

                velocity.y = maxJumpVelocity;
                Debug.Log("SALTO2");

                Debug.Log(velocity.y.ToString());

                _saltosTotales += 1;
                jumpApretado = 0;
            }

                                 
            //si suelto salto me baja la velocidad
            if (jumpSoltado == true)
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }




            //Tiempo de coyote
            tiempoCoyote -= Time.deltaTime;

            if (tiempoCoyote > 0 && _saltosTotales == 0)
            {
                tiempoCoyoteON = true;
            }
            else
            {
                tiempoCoyoteON = false;
            }




            if (velocity.y <= -10)
            {
                velocity.y = -10;
            }



            //LLAMA A LA FUNCION MOVE, PARA QUE SE MUEVA DETECTANDO COLISION
            _controller.Move(velocity * Time.deltaTime, tiempoCoyoteON);
        }
    }

    void CambiarDireccionSprite()
    {
        //VOLTEA EL SPRITE
        if (orientacionX == 1)
        {
            playerSpriteRenderer.flipX = false;
        }
        else if (orientacionX == -1)
        {
            playerSpriteRenderer.flipX = true;
        }
    }


    void HandleCollisions()
    {

        //manejar casos de colision
        if (_controller.collisions.below || _controller.collisions.above || _controller.collisions.right || _controller.collisions.left)
        {
            if (_controller.collisions.objetoGolpeadoHorizontal != null)
            {
                switch (_controller.collisions.objetoGolpeadoHorizontal.tag)
                {
                    //SI TOCO GEMA la destruyo, la sumo y mando a 0 el timer de lentitud
                    case "GEMA":
                        Destroy(_controller.collisions.objetoGolpeadoVertical);
                        Destroy(_controller.collisions.objetoGolpeadoHorizontal);
                        gemasContadas++;
                        audioGemas.clip = agarrogema;
                        audioGemas.Play();
                        break;

                    case "Enemigo":
                        _jugadorGolpeado = true;
                        _saltosTotales = 1;
                        jumpApretado = 0;
                        jumpSoltado = false;
                        if (_controller.collisions.left)
                        {
                            timerGolpeadoIzquierda = 0;
                        }
                        else
                        {
                            timerGolpeadoDerecha = 0;
                        }
                        break;

                }
            }


            if (_controller.collisions.objetoGolpeadoVertical != null)
            {
                switch (_controller.collisions.objetoGolpeadoVertical.tag)
                        {
                            //SI TOCO GEMA la destruyo, la sumo y mando a 0 el timer de lentitud
                            case "GEMA":
                                Destroy(_controller.collisions.objetoGolpeadoVertical);
                                Destroy(_controller.collisions.objetoGolpeadoHorizontal);
                                gemasContadas++;
                                audioGemas.clip = agarrogema;
                                audioGemas.Play();
                                break;

                            case "Enemigo":
                                _jugadorGolpeado = true;
                                _saltosTotales = 1;
                                jumpApretado = 0;
                                jumpSoltado = false;
                                if (_controller.collisions.left)
                                {
                                    timerGolpeadoIzquierda = 0;
                                }
                                else
                                {
                                    timerGolpeadoDerecha = 0;
                                }
                                break;



                            //SI ESTOY TOCANDO ABAJO y objeto piso MANDA EL CONTADOR A 0, LE DA FALSO AL YA SALTE X 2 y AL YA DASHEE
                            case "Piso":
                                {
                                    if (_controller.collisions.below)
                                    {
                                        animator.SetBool("Saltando", false);
                                        _saltosTotales = 0;
                                        boxContados = 0;
                                        //Tiempo de coyote
                                        tiempoCoyote = 0.15f;


                                    }
                                    break;
                                }

                            //SI SALTO SOBRE CAJA ACTIVO SU SCRIPT Y REBOTO
                            case "CUBO":
                                if (_controller.collisions.below)
                                {
                                    _controller.collisions.objetoGolpeadoVertical.transform.GetComponent<EnemigoGolpeado>().enabled = true;
                                    _saltosTotales = 1;
                                    jumpApretado = 0;
                                    jumpSoltado = false;
                                    velocity.y = maxJumpVelocity;
                                    boxContados++;
                                    audioJugador.clip = salto;
                                    audioJugador.Play();
                                }
                                break;

                }
            }
        }

    }

    void FlashRed()
    {
        playerSpriteRenderer.color = Color.black;
        Invoke("ResetColor", flashTime);
    }

    void ResetColor()
    {
        playerSpriteRenderer.color = Color.white;
    }

    private void OnDestroy()
    {
        gameOverScreen.Activate();
        Time.timeScale = 0.05f;

    }


}


