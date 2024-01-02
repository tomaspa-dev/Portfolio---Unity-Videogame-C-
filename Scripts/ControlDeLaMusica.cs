using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDeLaMusica : MonoBehaviour
{
    public AudioClip musicaDelMenuPrincipal;
    public AudioClip musicaDeFondo;
    //public AudioClip efectoDeSonido;
   
   void Start()
   {
       //ControlDelSonido.instanciaControlDelSonido.ActivarPistaMusical(musicaDelMenuPrincipal, 2); //Activar la pista Musical       
   }



    /*
   void Update()
   {
       
       if (Input.GetKeyDown ("space"))
       {
          //ControlDelSonido.instanciaControlDelSonido.ActivarPistaDeSonido("EfectoDisparos", transform.position); //Activar el efecto de Sonido
          ControlDelSonido.instanciaControlDelSonido.ActivarPista("EfectoDisparos"); //Activar el efecto de Sonido
       }
       
   }
    */
}
