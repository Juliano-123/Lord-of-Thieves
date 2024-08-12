﻿using UnityEngine;
using System.Collections;
using System.ComponentModel.Design;
using Unity.Burst.CompilerServices;
using UnityEngine.InputSystem;
using UnityEditor.ShaderGraph.Internal;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    //debug
    bool _logRotation = false;


    //components PROPIOS
    GameObject _imagen;
    Animator _animator;
    SpriteRenderer _spriteRenderer;
    GameObject _rotatePoint;
    Apuntar _rotatePointApuntar;
    Controller2D _controller;
    DetectorColisiones _detectorColisiones;
    BoxCollider2D _boxCollider;


    //COMPONENTES AJENOS
    [SerializeField]
    GameObject _gameManager;
    HealthManager _healthManager;
    CreadorMounstruos _creadorMounstruos;


    [SerializeField]
    LayerMask collisionPiso;


    public GameOverScreen gameOverScreen;
    public AudioSource audioJugador;
    public AudioSource audioGemas;
    public AudioClip salto, dashlisto, dasheando, agarrogema;


    //para golpeo

    bool _jugadorGolpeado = false;
    bool GolpeadoIzquierda = false;
    bool GolpeadoDerecha = false;
    bool GolpeadoArriba = false;
    bool GolpeadoAbajo = false;
    float _tiempoGolpeado = 0.5f;


    [SerializeField]
    float _maxJumpHeight = 1.8F;
    [SerializeField]
    float _minJumpHeight = 0.5F;
    [SerializeField]
    float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.1f;
    float aceleracionPuntoMasAlto = 0.075f;
    float accelerationTimeGrounded = 0.005f;
    float moveSpeed = 7f;
    int orientacionX = 1;
    float velocidadtTiempoExtraAire = 0.5f;
    float multiplicadorGravedadCaida = 1.5f;
    float multiplicadorGravedadPuntoMasAlto = 0.5f;


    public static float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    [SerializeField]
    Vector2 velocity;
    float velocityXSmoothing;
    
    //COYOTE
    float tiempoCoyote = -1f;
    bool tiempoCoyoteON = false;
    


    int _jumpApretado;
    float tiempoJump1 = -1;
    bool _jumpSoltado = false;
    int _saltosTotales = 0;
    int _saltosMaximos = 2;
    bool _isJumping = false;
    

    int boxContados = 0;
    int gemasContadas = 0;

    //DASH
    int _dashTotales = 0;
    int _dashMaximos = 1;
    bool _dasheando = false;
    int _dashApretado = 0;
    bool _dashSoltado = false;
    Vector2 _dashvelocitydirection;

    float timerdash = 1f;
    float dashVelocity = 15f;


    GameObject _mira;
    Vector2 _lugarSpawn;


    float flashTime = 0.008f;
    public Ghost ghost;

    //nuevo movimiento
    [SerializeField]
    Vector2 _directionalInput;

    //timer para poner idle (soluciona volteo sprite)
    float _directionalInputX0Timer;
    float _directionalInputX0Time = 0.06f;


    Vector2 _attackDirection;

    



    private void Awake()
    {
        gemasContadas = 0;
        _controller = GetComponent<Controller2D>();
        _detectorColisiones = GetComponent<DetectorColisiones>();
        _imagen = transform.Find("Imagen").gameObject;
        _spriteRenderer = _imagen.GetComponent<SpriteRenderer>();
        _animator = _imagen.GetComponent<Animator>();
        _rotatePoint = transform.Find("RotatePoint").gameObject;
        _rotatePointApuntar = _rotatePoint.GetComponent<Apuntar>();
        _mira = _rotatePoint.transform.Find("SlicePoint").gameObject;
        _boxCollider = GetComponent<BoxCollider2D>();
        _healthManager = _gameManager.GetComponent<HealthManager>();
        _creadorMounstruos = _gameManager.GetComponent<CreadorMounstruos>();
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
        // AGREGA GRAVEDAD
        velocity.y += gravity * Time.deltaTime;

        //SETEA TARGET VELOCITY COMO EL MOVESPEED TOTAL CON SINGO POSITIVO/NEGATIVO
        float targetVelocityX = (_directionalInput.x * moveSpeed);
        //Hace que uno vaya acelerando y que no sea 0 100
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (_controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        //VOLTEA EL SPRITE SEGUN DONDE VOY
        CambiarDireccionSprite();



        //SI ME GOLPEARON NO TOMA INPUT Y HACE ESTO
        if (_jugadorGolpeado == true)
        {
            Debug.Log("Entro a handlegolpeo");
            _detectorColisiones.enemigos.Reset();
            HandleGolpeo();
        }


        //SI NO ME GOLEPARON TOMA LOS INPUTS Y ACTUA EN CONSECUENCIA
        if (_jugadorGolpeado == false)
        {            

            //CheckeaColisiones
            HandleCollisions();

            //SETEA ANIMACIONES
            AnimarElPJ();





            //GENERACION DE FANTASMAS CUANDO PUEDO DASHEAR
            //timeForNextDash -= Time.deltaTime;
            //if (timeForNextDash <= 0)
            //{
            //    ghost.makeGhost = true;
            //    if (yaSonoElDash == false)
            //    {
            //        audioJugador.clip = dashlisto;
            //        audioJugador.Play();
            //        yaSonoElDash = true;
            //    }
            //}
            //else if (timerdash >= 0.3f)
            //{
            //    ghost.makeGhost = false;
            //}

            //DECIDIR QUE DASHEO
            if (_dashApretado > 0 && _dashTotales < _dashMaximos)
            {
                timerdash = 0;
                _dashTotales += 1;
                _dashApretado = 0;
                _jumpApretado = 0;
                _isJumping = false;
                _saltosTotales = 1;
                audioJugador.clip = dasheando;
                audioJugador.Play();
            }


            //QUE HAGO SI DASHEO
            if (timerdash <= 0.3f)
            {
                timerdash += Time.deltaTime;
                if (_dasheando == false)
                {
                    _dashvelocitydirection = new Vector2(_mira.transform.position.x - transform.position.x, _mira.transform.position.y - transform.position.y) * dashVelocity;
                    _dasheando = true;
                }

                if (_jumpApretado > 0)
                {
                    _dasheando = false;
                    _jumpApretado = 0;
                    _jumpSoltado = false;
                    _isJumping = true;
                    velocity.x = 0;
                    velocity.y = maxJumpVelocity;
                    _saltosTotales = 2;
                    timerdash = 1f;
                }
                
                
            }
            else
            {

                _dasheando = false;
            }


            

            //SALTO
            //SI APRETE JUMP, ESTOY TOCANDO PISO o RECIEN LO TOQUE, Y ESTOY DENTRO DEL TIEMPO BUFFER
            if (_jumpApretado > 0 && ((_controller.collisions.below && (_controller.collisions.objetoGolpeadoVertical.tag == "Piso" || _controller.collisions.objetoGolpeadoVertical.tag == "Plataforma")) || tiempoCoyoteON) && Time.time - tiempoJump1 < 0.15 && _saltosTotales == 0)
            {
                Saltar();
            }

            //DOBLE SALTO
            //SI APRETE JUMP, NO ESTOY TOCANDO SUELO, NI DENTRO DEL BUFFER DE COYOTE Y ME QUEDAN SALTOS
            if (_jumpApretado > 0 && !_controller.collisions.below && !tiempoCoyoteON && _saltosTotales < _saltosMaximos)
            {
                Saltar();
            }




            //si suelto salto me baja la velocidad
            if (_jumpSoltado == true && _isJumping == true)
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



            //PONE UN LIMITE MAXIMO A LA VELOCIDAD DE CAIDA
            if (velocity.y <= -10)
            {
                velocity.y = -10;
            }





            //LLAMA A LA FUNCION MOVE, PARA QUE SE MUEVA DETECTANDO COLISION
            //CHECKEA PRIMERO OVERRIDES
            if (_dasheando == true)
            {
                velocity = _dashvelocitydirection;
            }

            _detectorColisiones.CheckEnemigos();
            _controller.Move(velocity * Time.deltaTime, tiempoCoyoteON, collisionPiso);
            



        }
    }


    void Saltar()
    {
        audioJugador.clip = salto;
        audioJugador.Play();
        _saltosTotales += 1;
        velocity.y = maxJumpVelocity;
        _jumpApretado = 0;
        _isJumping = true;
        _jumpSoltado = false;
    }




    void HandleGolpeo()
    {

        //GOLPEADO
        FlashRed();
        velocity.y = -12f;
        _controller.Move(velocity * Time.deltaTime, tiempoCoyoteON, collisionPiso);
    }

    void HandleCollisions()
    {
        //MANEJAR CASOS DE COLISION CON PISO
        if (_controller.collisions.hayGolpe)
        {
            if(_controller.collisions.objetoGolpeadoVertical !=null)
            {
                switch(_controller.collisions.objetoGolpeadoVertical.tag)
                {
                    //SI ESTOY TOCANDO ABAJO y objeto piso MANDA EL CONTADOR A 0, LE DA FALSO AL YA SALTE X 2 y AL YA DASHEE
                    case "Piso":
                        {
                            if (_controller.collisions.below)
                            {
                                _saltosTotales = 0;
                                _dashTotales = 0;
                                boxContados = 0;
                                //Tiempo de coyote
                                tiempoCoyote = 0.15f;
                                _isJumping = false;
                            }
                            break;
                        }

                    case "Plataforma":
                        {
                            if (_controller.collisions.below)
                            {
                                _saltosTotales = 0;
                                _dashTotales = 0;
                                boxContados = 0;
                                //Tiempo de coyote
                                tiempoCoyote = 0.15f;
                                _isJumping = false;
                            }
                            break;
                        }

                }

            }
        }

        //manejar casos de colision CON ENEMIGOS Y COSAS
        if (_detectorColisiones.enemigos.hayGolpe)
        {
            if (_detectorColisiones.enemigos.objetoGolpeadoHorizontal != null)
            {
                switch (_detectorColisiones.enemigos.objetoGolpeadoHorizontal.tag)
                {

                    //SI TOCO GEMA la destruyo, la sumo y mando a 0 el timer de lentitud
                    case "GEMA":
                        Destroy(_detectorColisiones.enemigos.objetoGolpeadoHorizontal);
                        gemasContadas++;
                        audioGemas.clip = agarrogema;
                        audioGemas.Play();
                        break;

                    case "Enemigo":
                        if (_detectorColisiones.enemigos.edge == true)
                        {
                            Destroy(_detectorColisiones.enemigos.objetoGolpeadoHorizontal);
                            //Debug.Log("DESTRUIDO POR DETECTORCOLISIONEs HORIZONAL DISTINTO DE NULL Y TAG ENEMIGO Y EDGE");
                            _creadorMounstruos.RestarMostros(1);

                            _jumpApretado = 0;
                            _saltosTotales = 1;
                            _dashTotales = 0;
                            _jumpSoltado = false;
                            //falso para que no baje la velocidad por soltar jump
                            _isJumping = false;
                            velocity.y = maxJumpVelocity/2;
                            boxContados++;
                            audioJugador.clip = salto;
                            audioJugador.Play();
                        }
                        else
                        {
                            _jugadorGolpeado = true;
                            if (_detectorColisiones.enemigos.left)
                            {
                                GolpeadoIzquierda = true;
                            }
                            else
                            {
                                GolpeadoDerecha = true;
                            }

                            Invoke(nameof(ResetJugadorGolpeado), _tiempoGolpeado);
                            _boxCollider.isTrigger = true;
                            _healthManager.SetCurrentHealth(-1);
                            Debug.Log("Bajo vida x horizontal");

                            _jumpSoltado = false;
                        }
                        break;

                }

            }
            else if(_detectorColisiones.enemigos.objetoGolpeadoVertical != null)
            {
                switch (_detectorColisiones.enemigos.objetoGolpeadoVertical.tag)
                {
                    //SI TOCO GEMA la destruyo, la sumo y mando a 0 el timer de lentitud
                    case "GEMA":
                        Destroy(_detectorColisiones.enemigos.objetoGolpeadoVertical);
                        gemasContadas++;
                        audioGemas.clip = agarrogema;
                        audioGemas.Play();
                        break;

                    case "Enemigo":
                        if (_detectorColisiones.enemigos.below)
                        {
                            Destroy(_detectorColisiones.enemigos.objetoGolpeadoVertical);
                            Debug.Log("DESTRUIDO POR DETECTORCOLISIONE VERTICAL DISTINTO DE NULL Y TAG ENEMIGO Y BELOW");
                            _creadorMounstruos.RestarMostros(1);
                            _jumpApretado = 0;
                            _saltosTotales = 1;
                            _dashTotales = 0;
                            _jumpSoltado = false;
                            _isJumping = false;
                            velocity.y = maxJumpVelocity / 2;
                            boxContados++;
                            audioJugador.clip = salto;
                            audioJugador.Play();
                        }
                        else
                        {
                            _jugadorGolpeado = true;
                            GolpeadoArriba = true;

                            Invoke(nameof(ResetJugadorGolpeado), _tiempoGolpeado);
                            _healthManager.SetCurrentHealth(-1);
                            Debug.Log("BAJO VIDA X VERTICAL");
                            _boxCollider.isTrigger = true;
                            _jumpSoltado = false;

                        }

                        break;
                }

            }
                        
        }
    }


    void ResetJugadorGolpeado()
    {
        _jugadorGolpeado = false;
        GolpeadoAbajo = false;
        GolpeadoArriba = false;
        GolpeadoDerecha = false;
        GolpeadoIzquierda = false;
        _boxCollider.isTrigger = false;
    }


    void FlashRed()
    {
        _spriteRenderer.color = Color.black;
        Invoke(nameof(ResetColor), flashTime);
    }

    void ResetColor()
    {
        _spriteRenderer.color = Color.white;
    }

    private void OnDestroy()
    {
        //gameOverScreen.Activate();
        Time.timeScale = 0.05f;

    }



    //seteo de animaciones
    void AnimarElPJ()
    {     
        _animator.SetBool("Idle", false);
        _animator.SetBool("Corriendo", false);

        //timer para idle
        if (_directionalInput.x == 0)
        {
            _directionalInputX0Timer += Time.deltaTime;

        }
        else
        {
            _directionalInputX0Timer = 0;
        }
        //seteo de idle segun timer
        if (_directionalInputX0Timer >= _directionalInputX0Time && _controller.collisions.below == true)
        {
            _animator.SetBool("Idle", true);
        }


        //corriendo en piso
        if (_directionalInput.x != 0 && _controller.collisions.below == true)
        {
            _animator.SetBool("Corriendo", true);
        }


        //subiendo y cayendo
        if (velocity.y > 0)
        {
            _animator.SetBool("Subiendo", true);
            _animator.SetBool("Cayendo", false);

        }

        if (velocity.y < 0 && _controller.collisions.below == false)
        {
            _animator.SetBool("Cayendo", true);
            _animator.SetBool("Subiendo", false);
        }

        if (_controller.collisions.below == true)
        {
            _animator.SetBool("Subiendo", false);
            _animator.SetBool("Cayendo", false);
        }


    }

    //CAMBIA LA DIRECCION DEL SPRITE DEL PLAYER
    void CambiarDireccionSprite()
    {
        //GUARDA LA ULTIMA ORIENTACION
        if (_directionalInput.x > 0)
        {
            orientacionX = 1;
        }
        else if (_directionalInput.x < 0)
        {
            orientacionX = -1;
        }

        //VOLTEA EL SPRITE
        if (orientacionX == 1)
        {
            _spriteRenderer.flipX = false;
        }
        else if (orientacionX == -1)
        {
            _spriteRenderer.flipX = true;
        }
    }



    //ACA ESTAN TODOS LOS INGRESOS DE INPUT QUE TOMA DE INPUTJULIAN
    public void SetDirectionalInput(Vector2 input)
    {
        _directionalInput = input;
    }

    public void SetAttackDirection(Vector2 direction)
    {
        _attackDirection = direction;
    }

    public void SetJumpApretado(int input)
    {
        _jumpApretado += input;
        tiempoJump1 = Time.time;
    }

    public void SetJumpSoltado()
    {
        _jumpSoltado = true;
    }

    public void SetDashApretado(int input)
    {
        _dashApretado += input;

    }

    public void SetDashSoltado()
    {
        _dashSoltado = true;
    }

    public void SetShootApretado(int input)
    {
        //if (_shootTime >= _shootTimer)
        //{
        //    _shootApretado = input;
        //}
    }

    public void SetShootSoltado(int input)
    {
        //_shootSoltado = input;
    }


}


