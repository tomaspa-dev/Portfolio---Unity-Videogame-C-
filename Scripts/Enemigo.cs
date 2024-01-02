using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(ControlDelArma))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemigo : Personaje
{   
    public enum Estado {Libre, Detenerse, Avanzar, Retroceder}
    Estado estadoActual;
    Material colorDelEnemigo;
    Color colorOriginalDelEnemigo;
    //float shaderPuntosDeVida;
    float shaderOriginalPuntosDeVida;
    bool seOtorgoPuntos;
    NavMeshAgent agente;
    Transform objetivoDelEnemigo;
    Transform enemigo;
    public Image imgBarraDeVida;
    public Canvas canvasDelEnemigo;
    //TipoDePersonaje tipoDePersonaje;
    ControlDelArma controlDelArma;
    //public LayerMask esJugador;
    public enum claseDeEnemigos {basico, medio, avanzado}
    public Animator animator;
    float[] intervaloDeDisparos = new float[4] {1.0f, 0.85f, 0.5f, 0.5f};//Comentario - Segundos en los que tarda en volver a realizar un ataque.
    float[] distanciaDelDisparo = new float[4] {6.5f, 6f, 6.95f, 7f};//Comentario - Rango máximo para realizar un ataque.
    float[] puntosDeVidaEnemigo = new float[4] {75f, 30f, 15f, 100f};
    float[] velocidadDelEnemigo = new float[4] {2.15f, 2.35f, 2.85f, 3.05f};//Comentario - Valores que tomará la Velocidad de movimiento de los diferentes tipos de enemigos.
    float velocidad;
    public string funcionDelEnemigo;
    float distanciaDeSeparacion;
    float distanciaDePersecucion = 4.00f;//3f;
    float distanciaDeDisparo;//15f

    float tiempoEntreDisparos; 
    float intervaloDeDisparo;
    float velocidadDeRetroceso;     

    float direccionContraria;
    bool mostrarMensajes;
    bool seMueve;
    bool seDetiene;
    bool seAvanza;
    bool seRetrocede;
    bool esPerseguido;
    Vector3 posicionDelJugador;

    bool fueAtacado;
    bool enRangoDeVision;
    bool enRangoDeAtaque;

    float rangoDeVision;
    Arma arma;

    void Awake()
    {    
        mostrarMensajes = true;
        controlDelArma = GetComponent<ControlDelArma>();
        intervaloDeDisparo = IntervaloDeDisparo();
        tiempoEntreDisparos = intervaloDeDisparo;   
        string tipoDeEnemigo = gameObject.tag;    
    }

    protected override void Start() 
    {
        base.Start ();  

        if (mostrarMensajes)
        {
            Debug.Log("Enemigo - " +gameObject.tag + ", función - " + funcionDelEnemigo + ". Puntos de Vida -" + puntosDeVidaIniciales);//Comentario - Mostrar info del Enemigo instanciado.
        }

        //enemigo = gameObject.GetComponent<Transform>(); 
        agente = GetComponent<NavMeshAgent>();   
        ObtenerColorDelEnemigo();    
        IdentificarAlJugador(); 
        CalcularPuntosDeVida();
        CalcularVelocidad();
        CalcularDistanciaDeDisparo();    

        if (objetivoDelEnemigo != null && !muerto)
        {
            StartCoroutine(ActualizarCamino());
        }
    }

    void Update()
    {
        switch (estadoActual)
        {
            case Estado.Libre:
                //Libre();
                break;

            case Estado.Detenerse:
                Detenerse();
                break;
            
            case Estado.Avanzar:
                Avanzar();
                break;

            case Estado.Retroceder:
                Retroceder();
                break;       
        }

        if (InterfazDeUsuario.ganoPartida == false)
        {
            if (objetivoDelEnemigo != null)
            {
                distanciaDeSeparacion = Vector3.Distance(transform.position, objetivoDelEnemigo.position);
            }

            if (((distanciaDeSeparacion > distanciaDeDisparo) && !muerto) && objetivoDelEnemigo != null)
            {   
                estadoActual = Estado.Avanzar;
            }

            else if (((distanciaDeSeparacion < distanciaDeDisparo && distanciaDeSeparacion > distanciaDePersecucion) || muerto) || objetivoDelEnemigo == null)
            {
                estadoActual = Estado.Detenerse;                           
                LanzarDisparos();      
            }

            else if (((distanciaDeSeparacion < distanciaDePersecucion) && !muerto) && objetivoDelEnemigo != null)
            {        
                estadoActual = Estado.Retroceder;
                LanzarDisparos();                
            }

            ActualizarAnimacionDetenerse();
            ActualizarAnimacionCaminar();
            ActualizarAnimacionEsPerseguido();
            ActualizarAnimacionRetroceder();
        }

        else if ((objetivoDelEnemigo == null))
        {
            StopCoroutine(ActualizarCamino());
        }

        if (InterfazDeUsuario.ganoPartida == true || objetivoDelEnemigo == null || InterfazDeUsuario.misionFallida == true)
        {
            GameObject.Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        MoverCanvasDelEnemigo();

        if (gameObject != null && !muerto && objetivoDelEnemigo != null && distanciaDeSeparacion < distanciaDeDisparo && distanciaDeSeparacion > distanciaDePersecucion || distanciaDeSeparacion < distanciaDePersecucion)
        {
            Vector3 objetivoEnAlturaCorrecta = new Vector3 (objetivoDelEnemigo.position.x, transform.position.y, objetivoDelEnemigo.position.z);
            transform.LookAt(objetivoEnAlturaCorrecta);
        }
    }

    private void IdentificarAlJugador()
    {
        objetivoDelEnemigo = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    private void ObtenerColorDelEnemigo()
    {
        colorDelEnemigo = GetComponent<Renderer>().material;
        colorOriginalDelEnemigo = colorDelEnemigo.color;  
    }

    public void CambiarColorDelEnemigo()
    {
        colorDelEnemigo.color = Color.red;
    }

    public void RestaurarColorDelEnemigo()
    {
        colorDelEnemigo.color = colorOriginalDelEnemigo;
    }

    private void MoverCanvasDelEnemigo()
    {
        canvasDelEnemigo.transform.LookAt(Camera.main.transform.position + Camera.main.transform.forward);
        canvasDelEnemigo.transform.Rotate(0, 180, 0); 
    }

    IEnumerator CambiarColor()
    {
        CambiarColorDelEnemigo();
        yield return new WaitForSeconds(.10f);
        RestaurarColorDelEnemigo();
    }

    public void LanzarDisparos()
    {
        if (tiempoEntreDisparos <= 0) 
        {
            if (!muerto)
            {
                LanzarDisparo();
            }

            tiempoEntreDisparos = intervaloDeDisparo; 
        }

        else 
        {
            tiempoEntreDisparos -= Time.deltaTime;
        }   
    }

    private void MoverPosicion(Vector3 posicionDelJugador)
    {
        if (!muerto)
        {
            agente.SetDestination(posicionDelJugador);
        }        
    }

    IEnumerator ActualizarCamino()
    {
        float tazaDeRegresco = 0.12f;
        
        while (objetivoDelEnemigo != null)  
        {
            posicionDelJugador = new Vector3 (objetivoDelEnemigo.position.x, 0f, objetivoDelEnemigo.position.z);  

            if (!seRetrocede)
            {
                MoverPosicion(posicionDelJugador);
            }

            else
            {
                MoverPosicion(-posicionDelJugador);
            }

            yield return new WaitForSeconds(tazaDeRegresco);
        }           
    }

    float IntervaloDeDisparo()
    {
        string tipoDeEnemigo = gameObject.tag;

        switch (funcionDelEnemigo)
        {
            case "Tanque":
                intervaloDeDisparo = intervaloDeDisparos[0];
                break;

            case "Rango Bajo":
                intervaloDeDisparo = intervaloDeDisparos[1];
                break;

            case "Rango Alto":
                intervaloDeDisparo = intervaloDeDisparos[2];
                break;

            case "Especial":
                intervaloDeDisparo = intervaloDeDisparos[3];
                break;

            default:
                intervaloDeDisparo = intervaloDeDisparos[0];
                break;
        }

        return intervaloDeDisparo;
    }

    public void CalcularVelocidad()
    {
        switch (funcionDelEnemigo)
        {
            case "Tanque":
                velocidad = velocidadDelEnemigo[0];
                break;

            case "Rango Bajo":
                velocidad = velocidadDelEnemigo[1];
                break;

            case "Rango Alto":
                velocidad = velocidadDelEnemigo[2];
                break;

            case "Especial":
                velocidad = velocidadDelEnemigo[3];
                break;

            default:
                Debug.LogError("La" + funcionDelEnemigo + "funcion de enemigo no disponible");
                velocidad = velocidadDelEnemigo[0];
                break;
        }
    }

    public void CalcularDistanciaDeDisparo()
    {
        switch (funcionDelEnemigo)
        {
            case "Tanque":
                distanciaDeDisparo = distanciaDelDisparo[0];
                break;

            case "Rango Bajo":
                distanciaDeDisparo = distanciaDelDisparo[1];
                break;

            case "Rango Alto":
                distanciaDeDisparo = distanciaDelDisparo[2];
                break;

            case "Especial":
                distanciaDeDisparo = distanciaDelDisparo[3];
                break;

            default:
                Debug.LogError("Error, La funcion - " + funcionDelEnemigo + " del enemigo, no se encuentra disponible");
                distanciaDeDisparo = distanciaDelDisparo[0];
                break;
        }
    }

    public void CalcularPuntosDeVida()
    {
        switch (funcionDelEnemigo)
        {
            case "Tanque":
                
                if (InterfazDeUsuario.nivelAmostrar < 20)
                {
                    puntosDeVidaIniciales = puntosDeVidaEnemigo[0];
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 20 && InterfazDeUsuario.nivelAmostrar < 30)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[0] + 0.20f * puntosDeVidaEnemigo[0]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 30 && InterfazDeUsuario.nivelAmostrar < 40)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[0] + 0.65f * puntosDeVidaEnemigo[0]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 40 && InterfazDeUsuario.nivelAmostrar < 50)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[0] + 0.60f * puntosDeVidaEnemigo[0]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 50)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[0] + 1.00f * puntosDeVidaEnemigo[0]);
                }
                
                break;

            case "Rango Bajo":

                if (InterfazDeUsuario.nivelAmostrar < 20)
                {
                    puntosDeVidaIniciales = puntosDeVidaEnemigo[1];
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 20 && InterfazDeUsuario.nivelAmostrar < 30)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[1] + 0.20f * puntosDeVidaEnemigo[1]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 30 && InterfazDeUsuario.nivelAmostrar < 40)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[1] + 0.50f * puntosDeVidaEnemigo[1]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 40 && InterfazDeUsuario.nivelAmostrar < 50)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[1] + 0.65f * puntosDeVidaEnemigo[1]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 50)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[1] + 1.00f * puntosDeVidaEnemigo[1]);
                }
                
                break;

            case "Rango Alto":

                if (InterfazDeUsuario.nivelAmostrar < 20)
                {
                    puntosDeVidaIniciales = puntosDeVidaEnemigo[2];
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 20 && InterfazDeUsuario.nivelAmostrar < 30)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[2] + 0.30f * puntosDeVidaEnemigo[2]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 30 && InterfazDeUsuario.nivelAmostrar < 40)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[2] + 0.35f * puntosDeVidaEnemigo[2]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 40 && InterfazDeUsuario.nivelAmostrar < 50)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[2] + 0.65f * puntosDeVidaEnemigo[2]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 50)
                {
                    puntosDeVidaIniciales = (puntosDeVidaEnemigo[2] + 1.00f * puntosDeVidaEnemigo[2]);
                }

                break;

            case "Especial":

                if (InterfazDeUsuario.nivelAmostrar >= 20 && InterfazDeUsuario.nivelAmostrar < 30)
                {
                    puntosDeVidaIniciales = (0.15f * puntosDeVidaEnemigo[3]);
                }   

                else if (InterfazDeUsuario.nivelAmostrar >= 30 && InterfazDeUsuario.nivelAmostrar < 40)
                {
                    puntosDeVidaIniciales = (0.20f * puntosDeVidaEnemigo[3]);
                }   

                else if (InterfazDeUsuario.nivelAmostrar >= 40 && InterfazDeUsuario.nivelAmostrar < 50)
                {
                    puntosDeVidaIniciales = (0.30f * puntosDeVidaEnemigo[3]);
                }

                else if (InterfazDeUsuario.nivelAmostrar >= 50)
                {
                    puntosDeVidaIniciales = puntosDeVidaEnemigo[3];
                }

                break;

            default:
                Debug.LogError("Error, La funcion - " + funcionDelEnemigo + "del enemigo, no se encuentra disponible, no se puede identificar los puntos de Vida");
                //puntosDeVidaIniciales = puntosDeVidaEnemigo[0];
                break;
        }

        puntosDeVida = puntosDeVidaIniciales;
    }

    public void LanzarDisparo()
    {
        controlDelArma.Disparar(); 
    }

    public override void RecibirDisparo(float daño, RaycastHit objetivoGolpeado)
    {        
        StartCoroutine(CambiarColor());

        base.RecibirDaño(daño);    

        imgBarraDeVida.fillAmount = puntosDeVida / puntosDeVidaIniciales;   

        if (muerto && !seOtorgoPuntos)                    
        {
            CalcularPuntos();
        }
    }

    void CalcularPuntos()
    {
        string tipoDeEnemigo = gameObject.tag;

        if (muerto && InterfazDeUsuario.ganoPartida == false)
        {       
            switch (tipoDeEnemigo)
            {
                case "Tipo1":
                    InterfazDeUsuario.OtorgarPuntos("Tipo1");                   
                    break;

                case "Tipo2":
                    InterfazDeUsuario.OtorgarPuntos("Tipo2");
                    break;

                case "Tipo3":
                    InterfazDeUsuario.OtorgarPuntos("Tipo3");
                    break;

                case "Tipo4":
                    InterfazDeUsuario.OtorgarPuntos("Tipo4");
                    break;

                default:
                    InterfazDeUsuario.OtorgarPuntos("Bot");
                    break;
            }

            InterfazDeUsuario.CalcularBajaDeEnemigos();
            
            if (InterfazDeUsuario.tipoDeMision == 2)
            {
                InterfazDeUsuario.CalcularObjetivosDeEnemigos();

                if (mostrarMensajes)
                {
                    Debug.Log("Enemigo Destruido: +" + InterfazDeUsuario.enemigoDestruido);//Mostrar Informacion - 
                }                
            }            

            seOtorgoPuntos = true;
        }  

        if (puntosDeVida <= 0 && muerto) 
        {
            //      
        }  
    }

    public void Libre()
    {
        //
    }

    public void Detenerse()
    {
        seDetiene = true;
        seAvanza = false;
        seRetrocede = false;
        esPerseguido = false;
        agente.stoppingDistance = distanciaDeSeparacion;
    }

    public void Avanzar()
    {
        seAvanza = true;
        seDetiene = false;
        seRetrocede = false;                
        esPerseguido = false;
        agente.stoppingDistance = 0;
        agente.speed = velocidad;
    }

    public void Retroceder()
    {
        seDetiene = false;
        seAvanza = false;
        seRetrocede = true;
        esPerseguido = true;
        agente.stoppingDistance = 0;
        velocidadDeRetroceso = 3.05f;
        agente.speed = velocidadDeRetroceso; 
    }

    void ActualizarAnimacionDetenerse()
    {
        animator.SetBool("seDetiene", seDetiene);
    }

    void ActualizarAnimacionCaminar()
    {
        animator.SetBool("seAvanza", seAvanza);
    }

    void ActualizarAnimacionEsPerseguido()
    {
        animator.SetBool("esPerseguido", esPerseguido);
    }

    void ActualizarAnimacionRetroceder()
    {
        animator.SetBool("seRetrocede", seRetrocede);
    }

    public void MostrarMensajes()
    {
        Debug.Log("funcionDelEnemigo" + funcionDelEnemigo);
        Debug.Log("intervaloDeDisparo" + intervaloDeDisparo);
        Debug.Log("distanciaDeDisparo" + distanciaDeDisparo);
        Debug.Log("velocidad" + velocidad);
        Debug.Log("Agente velocidad" + agente.speed );
        Debug.Log("Pos Jugador2" + objetivoDelEnemigo.position);
        Debug.Log("Distancia a detenerse" + agente.stoppingDistance);
    }
}
