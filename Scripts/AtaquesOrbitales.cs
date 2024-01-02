using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaquesOrbitales : MonoBehaviour
{
    private GeneradorDelNivel generadorDelNivel;
    private GeneradorDeMapa generadorDeMapa;
    private Jugador jugador;
    private ControlDelSonido controlDelSonido;
    public GameObject efectoDeImpacto;
    public GameObject efectoDeImpacto2;
    float[] dañoOrbe = new float[3]{0.40f, 0.50f, 0f};

    Transform espacioLibre;
    private float velocidad;
    private float distanciaRecorridaOrbe;
    private bool moverOrbe;

    public Transform verificarSuelo;
    private float radioEsfera = 0.25f;
    private bool estaEnTierra;
    public LayerMask suelo;
    
    void Awake()
    {
        //velocidad = 20.5f;//35
        velocidad = Utilidades.ObtenerNumeroAleatorio(15, 25);
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<Jugador>();
        generadorDeMapa = GameObject.FindGameObjectWithTag("Mapa").GetComponent<GeneradorDeMapa>();
        generadorDelNivel = GameObject.FindGameObjectWithTag("GeneradorDelNivel").GetComponent<GeneradorDelNivel>();
    }


    void Start()
    {      
        if (gameObject != null)
        {
            float tiempo = 0.3f;

            controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido
            StartCoroutine(LanzarOrbe(tiempo));  
            StartCoroutine(VerificarElSuelo());  
        }   
    }

    void FixedUpdate()
    {
        if (gameObject != null)
        {
            distanciaRecorridaOrbe = velocidad * Time.deltaTime;
        }

        if (gameObject != null && moverOrbe == true)
        {
            MoverOrbe(espacioLibre);
        }   


        /*

        if (gameObject != null)
        {
            VerificarSuelo();

            if (estaEnTierra == true)
            {
                eliminarOrbe();
            }
        } 
        */       
    }

    IEnumerator LanzarOrbe(float tiempo)
    {
        moverOrbe = false;
        Color colorFinal;
        colorFinal = Color.red;
        espacioLibre = generadorDeMapa.GetEspacioLibre();
        distanciaRecorridaOrbe = velocidad * Time.deltaTime;

        StartCoroutine(generadorDeMapa.GenerarEfectoAlInstanciar(espacioLibre, colorFinal));   

        yield return new WaitForSeconds(tiempo);
        moverOrbe = true;       
    }

    private void MoverOrbe(Transform espacioLibre)
    {
        transform.position = Vector3.MoveTowards(transform.position, espacioLibre.position + new Vector3(0, 0.436f, 0), distanciaRecorridaOrbe); 
    }

    void VerificarSuelo()
    {
        estaEnTierra = Physics.CheckSphere(verificarSuelo.position, radioEsfera, suelo); 
        Debug.Log("Toca el Suelo - " + estaEnTierra);
    }

    IEnumerator VerificarElSuelo()
    {
        while (gameObject != null)
        {
            estaEnTierra = Physics.CheckSphere(verificarSuelo.position, radioEsfera, suelo); 
            Debug.Log("Toca el Suelo - " + estaEnTierra);

            yield return new WaitForSeconds(0.35f);
        } 
    }

    IEnumerator EliminarOrbes(float tiempo)
    {
        GameObject.Destroy(gameObject);

        yield return new WaitForSeconds(tiempo);

        Destroy(Instantiate(efectoDeImpacto2, transform.position, Quaternion.identity), 2.15f);
    }


    void eliminarOrbe()
    {
        GameObject.Destroy(gameObject); //Comentario - despues de impactar al Jugador destruir el objeto

        if (efectoDeImpacto!= null)
        {
            Destroy(Instantiate(efectoDeImpacto2, transform.position, Quaternion.identity), 2.15f);

            estaEnTierra = false;
        }   
    }

    
    void OnTriggerEnter(Collider colliderPersonaje) 
    {
        if (colliderPersonaje.tag == "Player")
        {          
            if (InterfazDeUsuario.nivelAmostrar < 10 && InterfazDeUsuario.nivelAmostrar > 1)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirAtaqueDeOrbe(jugador.PuntosDeVidaIniciales * dañoOrbe[0]);
                Debug.Log("Recibio del Orbe, daño - " + jugador.PuntosDeVidaIniciales * dañoOrbe[0]);
            } 

            else if (InterfazDeUsuario.nivelAmostrar >= 10)
            {
                colliderPersonaje.GetComponent<IDaño>().RecibirAtaqueDeOrbe(jugador.PuntosDeVidaIniciales * dañoOrbe[1]);
                Debug.Log("Recibio del Orbe, daño - " + jugador.PuntosDeVidaIniciales * dañoOrbe[1]);
            } 

            else
            {
                Debug.Log("No se puede determinar el valor del daño - " + jugador.PuntosDeVidaIniciales * dañoOrbe[2]);
            }

            if (ControlDelSonido.instanciaControlDelSonido != null)//Ejecutar el código, sólo cuando exista una instancia.
            {
                controlDelSonido.ActivarPista("Trampa");
            }    

            GameObject.Destroy(gameObject); //Comentario - despues de impactar al Jugador destruir el objeto

            if (efectoDeImpacto!= null)
            {
                Destroy(Instantiate(efectoDeImpacto, transform.position, Quaternion.identity), 2.2f);
            }      
        }

        if (colliderPersonaje.tag == "Suelo")
        {
            //eliminarOrbe();

            GameObject.Destroy(gameObject); //Comentario - despues de impactar al Jugador destruir el objeto

            if (efectoDeImpacto!= null)
            {
                Destroy(Instantiate(efectoDeImpacto2, transform.position, Quaternion.identity), 2.15f);

                //estaEnTierra = false;
            }   
        }

        /*
        else if (colliderPersonaje.tag == "Tile")
        {
            GameObject.Destroy(gameObject); //Comentario - despues de impactar al Jugador destruir el objeto

            if (efectoDeImpacto!= null)
            {
                Destroy(Instantiate(efectoDeImpacto, transform.position, Quaternion.identity), 2.15f);
            }   
        }
        */
    }


    /*

            for (int i = 0; i < 2; i++)
            {
                StartCoroutine(LanzarOrbe(tiempo));  
            } 

    public void LanzarOrbe(Transform espacioLibre)
    {
        distanciaRecorridaOrbe = velocidad * Time.deltaTime;
        //VerificarColision(distanciaRecorrida);
        transform.position = Vector3.MoveTowards(transform.position, espacioLibre.position + Vector3.zero, distanciaRecorridaOrbe); 
    }

     IEnumerator InstanciarObjetos

    Color colorFinal;
    Transform espacioLibre = generadorDeMapa.GetEspacioLibre();

    StartCoroutine(generadorDeMapa.GenerarEfectoAlInstanciar(espacioLibre, colorFinal));   

    */
}
