using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlDelSonido : MonoBehaviour
{
    public Sonidos[] sonidos;//Comentario - Mantener Publico para agregar fácilmente la guente del archivo.
    public Sonidos[] musica;//Comentario - Mantener Publico para agregar fácilmente la guente del archivo.
    private Menu menu;
    private string[] fondosMusicales = new string[5] {"FondoMusical1", "FondoMusical2", "FondoMusical3", "FondoMusical4", "FondoMusical5"};
    public static ControlDelSonido instanciaControlDelSonido;
    public enum TipoDeVolumen {General, DeLosEfectos, Musica}
    private string fondoMusicalAleatorio;

    void Awake()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        string nombreDeLaEscena = escenaActual.name;   

        //Comentario - Generar Singleton
        if (instanciaControlDelSonido == null)
        {
            instanciaControlDelSonido = this;
        }

        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        if (nombreDeLaEscena == "Menu Principal")
        {
            menu = GameObject.FindGameObjectWithTag("Menu").GetComponent<Menu>();
        } 
          
        foreach (Sonidos s in sonidos)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.pistaDeSonido;
            s.source.outputAudioMixerGroup = menu.efectosAudioMixer;
        }

        foreach (Sonidos s in musica)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.pistaDeSonido;
            s.source.outputAudioMixerGroup = menu.musicaAudioMixer;
        }

        fondoMusicalAleatorio = fondosMusicales[Utilidades.ObtenerNumeroAleatorio(0, 5)];
    } 

    public void ActivarPista(string nombrePista)
    {
        Sonidos pista = Array.Find(sonidos, sonido => sonido.nombreDelAudio == nombrePista);

        if (pista == null)
        {
            Debug.LogWarning("BuscarPista - " + "La Pista de Sonido, " + nombrePista + "no se encuentra disponible");
            return;
        }
  
        pista.source.Play();    
    }

    public void ActivarPistaMusical(string nombrePista)
    {
        Sonidos pistaMusical = Array.Find(musica, sonido => sonido.nombreDelAudio == nombrePista);

        if (pistaMusical == null)
        {
            Debug.LogWarning("BuscarPista - " + "La Pista Musical, " + nombrePista + "no se encuentra disponible");
            return;
        }
         
        pistaMusical.source.Play();
    }

    public void DesactivarPistaMusical(string nombrePista) 
    {
        Sonidos pistaMusical = Array.Find(musica, sonido => sonido.nombreDelAudio == nombrePista);

        if (pistaMusical == null)
        {
            Debug.LogWarning("La Pista de Sonido, " + nombrePista + "no se encuentra disponible");
            return;
        } 

        pistaMusical.source.Stop();
    }

    public void PausarPistaMusical(string nombrePista) 
    {
        Sonidos pistaMusical = Array.Find(sonidos, sonido => sonido.nombreDelAudio == nombrePista);

        if (pistaMusical == null)
        {
            Debug.LogWarning("La Pista de Sonido, " + nombrePista + "no se encuentra disponible");
            return;
        } 

        pistaMusical.source.Pause();
    }

    public void ActivarMusicaAleatoria()
    {
        ActivarPistaMusical(fondoMusicalAleatorio);
    }

    public void DesactivarMusicaAleatoria()
    {
        DesactivarPistaMusical(fondoMusicalAleatorio);
    }
}
