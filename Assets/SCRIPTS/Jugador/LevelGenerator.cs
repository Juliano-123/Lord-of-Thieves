using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    //prueba git commit
    public GameObject spawnObjetoBox;
    public GameObject spawnObjetoGema;
    public GameObject spawnObjetoVacio;

    public static Vector3 primeraPosicion;
    public static Vector3 ultimaPosicionSpawn;

    public static float distanciaTotalX = 0;
    public static float distanciaTotalY = 0;

    //los 32 lugares de spawneo se guardan aca
    Vector3[] lugarspawn = new Vector3[33];

    //la decision de spawnear se guarda aca
    int[] spawnboxsiono = new int[33];
    //la decision de spawnear gemas se guarda aca
    int[] spawngemasiono = new int[33];

    //para checkear el overlapbox
    Vector2 sizeacheckear = new Vector2(3,3);
    
    //La separacion X entre cajas es 2.5
    float separacionX = 2.5f;
    //La separacion Y entre cajas es 2.5
    float separacionY = 2.6f;
    //Separacion para que se pare el pj sobre la caja
    float separacionpjcajaY = 0.68f;

    void Awake()
    {
        primeraPosicion = transform.position;
        ultimaPosicionSpawn = transform.position;
        distanciaTotalX = 0;
        distanciaTotalY = 0;

        for (int i = 1; i <= 32; i++)
        {
            lugarspawn[i].z = 0;       
        }

    }

    void FixedUpdate()
    {
        distanciaTotalX = transform.position.x - primeraPosicion.x;
        distanciaTotalY = transform.position.y - primeraPosicion.y;

        float distanciaDeltaX = transform.position.x - ultimaPosicionSpawn.x;
        float distanciaDeltaY = transform.position.y - ultimaPosicionSpawn.y;

        bool spawnear = false;

        if (Mathf.Abs(distanciaDeltaX) >= separacionX)
        {
            spawnear = true;
            ultimaPosicionSpawn.x = transform.position.x;
        }

        if (Mathf.Abs(distanciaDeltaY) >= separacionY)
        {
            spawnear = true;
            ultimaPosicionSpawn.y = transform.position.y;
        }

        if (spawnear)
        {
        //Calcula el primer lugar, la primera caja que aparece enfrente de el (fuera de la vista)
            lugarspawn[1].x = primeraPosicion.x + separacionX*4 + separacionX * Mathf.FloorToInt(distanciaTotalX/separacionX);
            lugarspawn[1].y = primeraPosicion.y - separacionpjcajaY + separacionY * Mathf.FloorToInt(distanciaTotalY/separacionY);
        
        //Calcula el segundo y siguientes tomando como referencia el primer lugarspawn
            lugarspawn[2].x = lugarspawn[1].x;
            lugarspawn[2].y = lugarspawn[1].y - separacionY;

            lugarspawn[3].x = lugarspawn[1].x;
            lugarspawn[3].y = lugarspawn[1].y - separacionY*2;

            lugarspawn[4].x = lugarspawn[1].x;
            lugarspawn[4].y = lugarspawn[1].y - separacionY*3;
              
            lugarspawn[5].x = lugarspawn[1].x;
            lugarspawn[5].y = lugarspawn[1].y - separacionY*4;

            lugarspawn[6].x = lugarspawn[1].x - separacionX;
            lugarspawn[6].y = lugarspawn[1].y - separacionY * 4;

            lugarspawn[7].x = lugarspawn[1].x - separacionX*2;
            lugarspawn[7].y = lugarspawn[1].y - separacionY*4;

            lugarspawn[8].x = lugarspawn[1].x - separacionX * 3;
            lugarspawn[8].y = lugarspawn[1].y - separacionY*4;

            lugarspawn[9].x = lugarspawn[1].x - separacionX * 4;
            lugarspawn[9].y = lugarspawn[1].y - separacionY * 4;

            lugarspawn[10].x = lugarspawn[1].x - separacionX * 5;
            lugarspawn[10].y = lugarspawn[1].y - separacionY * 4;

            lugarspawn[11].x = lugarspawn[1].x - separacionX * 6;
            lugarspawn[11].y = lugarspawn[1].y - separacionY * 4;

            lugarspawn[12].x = lugarspawn[1].x - separacionX * 7;
            lugarspawn[12].y = lugarspawn[1].y - separacionY * 4;

            lugarspawn[13].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[13].y = lugarspawn[1].y - separacionY * 4;

            lugarspawn[14].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[14].y = lugarspawn[1].y - separacionY * 3;

            lugarspawn[15].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[15].y = lugarspawn[1].y - separacionY * 2;

            lugarspawn[16].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[16].y = lugarspawn[1].y - separacionY;

            lugarspawn[17].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[17].y = lugarspawn[1].y;

            lugarspawn[18].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[18].y = lugarspawn[1].y + separacionY;

            lugarspawn[19].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[19].y = lugarspawn[1].y + separacionY * 2;

            lugarspawn[20].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[20].y = lugarspawn[1].y + separacionY * 3;

            lugarspawn[21].x = lugarspawn[1].x - separacionX * 8;
            lugarspawn[21].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[22].x = lugarspawn[1].x - separacionX * 7;
            lugarspawn[22].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[23].x = lugarspawn[1].x - separacionX * 6;
            lugarspawn[23].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[24].x = lugarspawn[1].x - separacionX * 5;
            lugarspawn[24].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[25].x = lugarspawn[1].x - separacionX * 4;
            lugarspawn[25].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[26].x = lugarspawn[1].x - separacionX * 3;
            lugarspawn[26].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[27].x = lugarspawn[1].x - separacionX * 2;
            lugarspawn[27].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[28].x = lugarspawn[1].x - separacionX;
            lugarspawn[28].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[29].x = lugarspawn[1].x;
            lugarspawn[29].y = lugarspawn[1].y + separacionY * 4;

            lugarspawn[30].x = lugarspawn[1].x;
            lugarspawn[30].y = lugarspawn[1].y + separacionY*3;

            lugarspawn[31].x = lugarspawn[1].x;
            lugarspawn[31].y = lugarspawn[1].y + separacionY * 2;

            lugarspawn[32].x = lugarspawn[1].x;
            lugarspawn[32].y = lugarspawn[1].y + separacionY;

            //Genera un 0 o 1 para cada lugar de spawn de box
            for (int i = 1; i <= 32; i++)
            {
                spawnboxsiono[i] = Random.Range(0,2);
            }

            //Genera un 0 a 2 para cada lugar de spawn de gema
            for (int i = 1; i <= 32; i++)
            {
                spawngemasiono[i] = Random.Range(0, 3);
            }

            //los dos primeros espacios los instanceo segun el 0 o 1 de spawnsiono
            for (int  i = 1; i <= 2; i++)
            {
                if (!Physics2D.OverlapBox(lugarspawn[i], sizeacheckear, 0) && (lugarspawn[i].y > primeraPosicion.y)) 
                {
                    if (spawnboxsiono[i] == 0)
                    {
                        Instantiate(spawnObjetoVacio, lugarspawn[i], Quaternion.identity);
                    }
                    else
                        Instantiate(spawnObjetoBox, lugarspawn[i], Quaternion.identity);

                    if (spawngemasiono[i] == 0)
                    {
                        Instantiate(spawnObjetoGema, new Vector3(lugarspawn[i].x, lugarspawn[i].y + 1, lugarspawn[i].z), Quaternion.identity);
                    }
                }

            }

            //Los lugares 3 en adelante tienen en cuenta lo anterior
            for (int i = 3; i <= 32; i++)
            {
                if (!Physics2D.OverlapBox(lugarspawn[i], sizeacheckear, 0) && (lugarspawn[i].y > primeraPosicion.y))
                {
                    //Decido que hacer segun que no se haya spawneado 0 o 1 o 2
                    switch (spawnboxsiono[i - 1] + spawnboxsiono[i - 2])
                    {
                        case 0:
                            Instantiate(spawnObjetoBox, lugarspawn[i], Quaternion.identity);                         
                        break;

                        case 1:
                            if (spawnboxsiono[i] == 0)
                            {
                                Instantiate(spawnObjetoVacio, lugarspawn[i], Quaternion.identity);
                            }
                            else
                                Instantiate(spawnObjetoBox, lugarspawn[i], Quaternion.identity);
                        break;

                        case 2:
                            Instantiate(spawnObjetoVacio, lugarspawn[i], Quaternion.identity);
                        break;
                    }

                    if (spawngemasiono[i] == 0)
                    {
                        Instantiate(spawnObjetoGema, new Vector3(lugarspawn[i].x, lugarspawn[i].y + 1, lugarspawn[i].z), Quaternion.identity);
                    }
                }

            }

            spawnear = false;

        }

        //if (spawnear)
        //{
        //    //SPAWNEO DE CAJAS
        //    int randomBoxesTotal = Random.Range(0, 2);

        //    Instantiate(spawnObjetoBox, , Quaternion.identity);

        //    for (int i = 0; i <= randomBoxesTotal; i++)
        //    {
        //        int randomBoxesLugar = Random.Range(0, 3);
        //        while (lugaresBoxesAnteriores.Contains(randomBoxesLugar))
        //        {
        //            randomBoxesLugar = Random.Range(0, 3);
        //        }
        ////        Instantiate(spawnObjetoBox, spawnPointsBox[randomBoxesLugar].transform.position, Quaternion.identity);
        ////        lugaresBoxesAnteriores.Add(randomBoxesLugar);

        ////    }

        //}


        
    }
}
