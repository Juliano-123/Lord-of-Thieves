using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MostroBlanco : MonoBehaviour
{

    public Animator animator;
    SpriteRenderer _currentSpriteRenderer;
    Controller2D _controller;

    Vector2 _velocity = Vector2.zero;

    //La separacion X entre cajas es 2.5
    float separacionX = 2.5f;
    //La separacion Y entre cajas es 2.6
    float separacionY = 2.6f;
    //Separacion para que se pare el pj sobre la caja
    float separacionpjcajaY = 0.53f;

    Vector2[] _lugaresCajasPosibles = new Vector2[15];
    bool[] _hayAlgo = new bool[15];
    Vector2 _sizeacheckear = new Vector2(3, 3);

    void Start()
    {
        _controller = GetComponent<Controller2D>();
        _currentSpriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
    }


    void Update()
    {
        //APLICA GRAVEDAD
        _velocity.y += Player.gravity * Time.deltaTime;

        //Solo checka donde moverse si esta parado sobre algo
        if (_controller.collisions.below)
        {

            //CALCULA EL PRIMER LUGAR SEGUN DONDE ESTA EL PERSONAJE
            _lugaresCajasPosibles[1].x = transform.position.x + separacionX;
            _lugaresCajasPosibles[1].y = transform.position.y - separacionpjcajaY;

            //CALCULA LOS SIGUIENTES EN FUNCION DEL PRIMERO
            _lugaresCajasPosibles[2].x = _lugaresCajasPosibles[1].x + separacionX;
            _lugaresCajasPosibles[2].y = _lugaresCajasPosibles[1].y;

            _lugaresCajasPosibles[3].x = _lugaresCajasPosibles[1].x + separacionX;
            _lugaresCajasPosibles[3].y = _lugaresCajasPosibles[1].y - separacionY;
            
            _lugaresCajasPosibles[4].x = _lugaresCajasPosibles[1].x;
            _lugaresCajasPosibles[4].y = _lugaresCajasPosibles[1].y - separacionY;

            _lugaresCajasPosibles[5].x = _lugaresCajasPosibles[1].x - separacionX;
            _lugaresCajasPosibles[5].y = _lugaresCajasPosibles[1].y - separacionY;

            _lugaresCajasPosibles[6].x = _lugaresCajasPosibles[1].x - separacionX*2;
            _lugaresCajasPosibles[6].y = _lugaresCajasPosibles[1].y - separacionY;

            _lugaresCajasPosibles[7].x = _lugaresCajasPosibles[1].x - separacionX*3;
            _lugaresCajasPosibles[7].y = _lugaresCajasPosibles[1].y - separacionY;

            _lugaresCajasPosibles[8].x = _lugaresCajasPosibles[1].x - separacionX*3;
            _lugaresCajasPosibles[8].y = _lugaresCajasPosibles[1].y;

            _lugaresCajasPosibles[9].x = _lugaresCajasPosibles[1].x - separacionX*2;
            _lugaresCajasPosibles[9].y = _lugaresCajasPosibles[1].y;

            _lugaresCajasPosibles[10].x = _lugaresCajasPosibles[1].x - separacionX*3;
            _lugaresCajasPosibles[10].y = _lugaresCajasPosibles[1].y + separacionY;

            _lugaresCajasPosibles[11].x = _lugaresCajasPosibles[1].x - separacionX * 2;
            _lugaresCajasPosibles[11].y = _lugaresCajasPosibles[1].y + separacionY;

            _lugaresCajasPosibles[12].x = _lugaresCajasPosibles[1].x - separacionX;
            _lugaresCajasPosibles[12].y = _lugaresCajasPosibles[1].y + separacionY;

            _lugaresCajasPosibles[13].x = _lugaresCajasPosibles[1].x;
            _lugaresCajasPosibles[13].y = _lugaresCajasPosibles[1].y + separacionY;

            _lugaresCajasPosibles[14].x = _lugaresCajasPosibles[1].x + separacionX;
            _lugaresCajasPosibles[14].y = _lugaresCajasPosibles[1].y + separacionY;

            

            //guarda en el array de hay algo si hay algo
            for (int i = 1; i <= 14; i++)
            {
                if (Physics2D.OverlapBox(_lugaresCajasPosibles[i], _sizeacheckear, 0, LayerMask.GetMask('Obstaculo'))
                {
                    _hayAlgo[i] = true;
                }
                else
                {
                    _hayAlgo[i] = false;
                }

            }
            int _lugarRandom = Random.Range(1, 15);

            for (int i = 1;i <= 14;i++)
            { 
                if ()
            
            }


        }


        ////SI LE PEGO AL JUGADOR ................
        //if (_controller.collisions.left && _controller.collisions.objetoGolpeado.tag == "Player")
        //{
        //    Player.timerGolpeadoDerecha = 0;
        //}

        //if (_controller.collisions.right && _controller.collisions.objetoGolpeado.tag == "Player")
        //{
        //    Player.timerGolpeadoIzquierda = 0;
        //}




        //LLAMA A LA FUNCION MOVE, PARA QUE SE MUEVA DETECTANDO COLISION
        _controller.Move(_velocity * Time.deltaTime, false);

    }



}