using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorDelNivel : MonoBehaviour
{
    private ControlDelSonido controlDelSonido;
    private GeneradorDeMapa generadorDeMapa;
    public Enemigo[] enemigo;//07.04 Ya no se usará, conservar como backup hasta lanzar la app.

    public Enemigo[] enemigoTipo1Tanque;
    public Enemigo[] enemigoTipo1DPSRangoAlto;
    public Enemigo[] enemigoTipo1DPSRangoBajo;
    public Enemigo[] enemigoTipo1Especial;
    public Enemigo[] enemigoTipo2Tanque;
    public Enemigo[] enemigoTipo2DPSRangoAlto;
    public Enemigo[] enemigoTipo2DPSRangoBajo;
    public Enemigo[] enemigoTipo2Especial;
    public Enemigo[] enemigoTipo3Tanque;
    public Enemigo[] enemigoTipo3DPSRangoAlto;
    public Enemigo[] enemigoTipo3DPSRangoBajo;
    public Enemigo[] enemigoTipo3Especial;
    public Enemigo[] enemigoTipo4Tanque;
    public Enemigo[] enemigoTipo4DPSRangoAlto;
    public Enemigo[] enemigoTipo4DPSRangoBajo;
    public Enemigo[] enemigoTipo4Especial;
    public GameObject[] objetoCuracion;
    public GameObject[] objetoPuntos;
    public GameObject[] objetoMision;
    public GameObject[] objetoTrampa;
    public GameObject[] objetoOrbe;  
    public Transform posicionInicialOrbe;  
    int enemigosPendientes;    
    Transform objetivoDelEnemigo;       
    int ejeX = 12;//Para Pruebas coord aleatorias 
    int ejeY = 10;//Para Pruebas coord aleatorias
    int numeroDeEnemigos;
    const int INCREMENTO = 1;
    int totalDeObjetos;
    string[] tipoDeObjetos = new string[4] {"objetoCuracion", "objetoPuntos", "objetoMision", "objetoTrampa"};
    //string[] tipoDeEnemigos = new string[4] {"Tipo1", "Tipo2", "Tipo3", "Tipo4"};
    string[] funcionDeLosEnemigos = new string[4] {"Tanque", "DPSRangoAlto", "DPSRangoBajo", "Especial"};
    int[] funcionDelEnemigo = new int[4];
    int[] tiempoParaInstanciarEnemigos = new int[4] {5, 15, 25, 35};
    int[] tiempoParaInstanciarObjetos = new int[4] {10, 20, 30, 40};
    int[] numeroDeObjetos = new int[4];
    int nivelActual;
    int tiempoTotal;
    int tipoDeMision;
    int tipoDeEnemigo;
    string tipoEnemigo;
    int tipoDeObjeto;
    int tipoDeOrbe;
    string tipoOrbe;
    int totalOrbes;

    bool mostrarMensajes;
    int[] totalDeEnemigos = new int[4];
    int valorMinimo;
    int valorMaximo;
    int indiceJ;
    int [,,] valorMinimoDeEnemigos = new int [4, 6, 4] { { {0, 1, 0, 0}, {1, 0, 2, 0}, {1, 2, 0, 0}, {1, 2, 0, 0}, {2, 3, 1, 1}, {2, 2, 0, 0} }, { {1, 0, 1, 0}, {0, 0, 1, 0}, {0, 1, 0, 0}, {0, 1, 0, 1}, {0, 2, 0, 1}, {1, 1, 1, 1} }, { {0, 1, 0, 0}, {0, 2, 0, 0}, {1, 2, 0, 0}, {1, 2, 0, 0}, {1, 2, 0, 0}, {2, 2, 1, 1} }, { {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 1, 0, 0}, {2, 3, 0, 1}, {0, 3, 0, 0}, {0, 3, 1, 0} } };
    int [,,] valorMaximoDeEnemigos = new int [4, 6, 4] { { {0, 1, 1, 0}, {1, 0, 2, 0}, {1, 2, 0, 0}, {1, 3, 1, 1}, {2, 4, 2, 1}, {3, 5, 0, 2} }, { {1, 0, 1, 0}, {0, 1, 2, 1}, {1, 2, 0, 1}, {0, 2, 0, 1}, {0, 4, 1, 1}, {2, 3, 2, 2} }, { {0, 2, 1, 0}, {0, 3, 0, 0}, {1, 2, 0, 0}, {1, 3, 1, 0}, {2, 4, 0, 1}, {3, 4, 2, 2} }, { {0, 0, 0, 0}, {0, 0, 0, 0}, {1, 2, 1, 0}, {3, 4, 0, 2}, {1, 4, 1, 0}, {0, 5, 3, 2} } };
    int [,,] valorTrampas = new int[4, 1, 2] { {{8, 17}}, {{13, 20}}, {{20, 25}}, {{13, 25}} };
    int[,,] valorCuracion = new int[4, 2, 2] { {{0, 2}, {0, 4}}, {{1, 3}, {2, 4}}, {{0, 4}, {3, 8}}, {{1, 5}, {2, 4}} };
    int[,,] valorPuntos = new int[4, 2, 2] { {{0, 0}, {0, 0}}, {{1, 2}, {0, 2}}, {{0, 1}, {1, 2}}, {{1, 2}, {1, 3}} };
    int[,,] valorMisiones = new int[4, 4, 2] { {{1, 3}, {3, 5},{3, 6}, {4, 6}}, {{2, 4}, {4, 6},{4, 5}, {3, 7}}, {{3, 5}, {2, 5},{4, 6}, {4, 5}}, {{4, 6}, {3, 6},{3, 5}, {4, 6}} };
    int[,] valorOrbes = new int[2, 2] { {10, 15} , {18, 25} };

    void Awake()
    {
        generadorDeMapa = GameObject.FindGameObjectWithTag("Mapa").GetComponent<GeneradorDeMapa>();
        mostrarMensajes = true;  
    }
      
    void Start()
    {
        nivelActual = InterfazDeUsuario.nivelAmostrar;
        tipoDeMision = InterfazDeUsuario.tipoDeMision;

        objetivoDelEnemigo = GameObject.FindGameObjectWithTag("Player").transform; 
        controlDelSonido = GameObject.FindGameObjectWithTag("Sonidos").GetComponent<ControlDelSonido>();//Ojo importante, permite el Sonido
        Debug.Log("El nivel actual es - " + nivelActual + ", el tiempo Total es - " + InterfazDeUsuario.tiempoTotal);
        
        if (ControlDelSonido.instanciaControlDelSonido != null)//Ejecutar el código, sólo cuando exista una instancia.
        {
            controlDelSonido.DesactivarMusicaAleatoria();  
            controlDelSonido.ActivarMusicaAleatoria();
        }

        if (objetivoDelEnemigo != null)
        {
            int numeroDeOrbes = CalcularOrbesAleatorios();
            int tiempoDeInstancia = 2;

            for (int i = 0; i < numeroDeOrbes; i++)
            {
                StartCoroutine(InstanciarOrbes(tiempoDeInstancia));
            }          
        }
         
        if (InterfazDeUsuario.tiempoTotal == 30)
        {
            for (int i = 0; i < tiempoParaInstanciarEnemigos.Length - 1; i++)
            {
                StartCoroutine(GenerarEnemigos(tiempoParaInstanciarEnemigos[i]));
            }

            for (int i = 0; i < tiempoParaInstanciarObjetos.Length - 1; i++)
            {
                StartCoroutine(GenerarObjetos(tiempoParaInstanciarObjetos[i]));
            }
        }

        else if (InterfazDeUsuario.tiempoTotal == 45)
        {
            for (int i = 0; i < tiempoParaInstanciarEnemigos.Length; i++)
            {
                StartCoroutine(GenerarEnemigos(tiempoParaInstanciarEnemigos[i]));
            }

            for (int i = 0; i < tiempoParaInstanciarObjetos.Length; i++)
            {
                StartCoroutine(GenerarObjetos(tiempoParaInstanciarObjetos[i]));
            }
        } 

        else
        {
            Debug.Log("Error, Nivel Inexistente");
        }    
    }     

    Enemigo aparicionDeEnemigo;    
    
    IEnumerator GenerarEnemigos(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);

        totalDeEnemigosPorFuncion(tiempo);        
    }

    private void totalDeEnemigosPorFuncion(float tiempo)
    {
        for (int i = 0; i < funcionDeLosEnemigos.Length; i++)
        {
            totalDeEnemigos[i] = CalcularEnemigosAleatorios(tiempo, i);
            GenerarEnemigosPorFuncion(totalDeEnemigos[i], i);

            if (mostrarMensajes)
            {
                Debug.Log("Enemigos instanciados por funcion," + i + ", tiempo - " + tiempo+", Min - " + valorMinimo + ", Max - " + valorMaximo +  ", total de enemigos - " + totalDeEnemigos[i]);//Mostrar Informacion - 
            }
            
        }        
    }

    private void GenerarEnemigosPorFuncion(int totalDeEnemigos, int tipo)
    {
        for (int i = 0; i < totalDeEnemigos; i++)
        {
            if (objetivoDelEnemigo != null && InterfazDeUsuario.ganoPartida == false || InterfazDeUsuario.misionFallida == false)
            {
                float tiempoParaInstanciar = 1.2f;
                StartCoroutine(InstanciarEnemigos(tiempoParaInstanciar, tipo));
            }            
        }  
    }

    private int CalcularOrbesAleatorios()
    {
        if (objetivoDelEnemigo != null)
        {
            if (nivelActual >= 1 && nivelActual < 10)
            {
                totalOrbes = Utilidades.ObtenerNumeroAleatorio(valorOrbes[0, 0], valorOrbes[0, 1] + INCREMENTO);
            }

            if (nivelActual >= 10)
            {
                totalOrbes = Utilidades.ObtenerNumeroAleatorio(valorOrbes[1, 0], valorOrbes[1, 1] + INCREMENTO);
            }
        }

        return totalOrbes;
    }

    private void CalcularindiceJ()
    {
            switch (nivelActual)
            {
                case int nivel when (nivel > 0 && nivel < 10 && InterfazDeUsuario.tiempoTotal == 30):
                    indiceJ = 0;
                    break;

                case int nivel when (nivel >= 10 && nivel < 20 && InterfazDeUsuario.tiempoTotal == 30):
                    indiceJ = 1;
                    break;

                case int nivel when (nivel >= 20 && nivel < 30 && InterfazDeUsuario.tiempoTotal == 45):
                    indiceJ = 2;
                    break;

                case int nivel when (nivel >= 30 && nivel < 40 && InterfazDeUsuario.tiempoTotal == 45):
                    indiceJ = 3;
                    break;

                case int nivel when (nivel >= 40 && nivel < 50 && InterfazDeUsuario.tiempoTotal == 45):
                    indiceJ = 4;
                    break;

                case int nivel when (nivel >= 50 && InterfazDeUsuario.tiempoTotal == 45):
                    indiceJ = 5;
                    break;

                default:
                    Debug.Log("Error - No existe Indice J" + indiceJ);
                    break;                    
            }

            if (mostrarMensajes)
            {
                Debug.Log("El Indice J es - " + indiceJ);
            }            
    }

    private void CalcularValores(int funcion, float tiempo)
    {
        CalcularindiceJ();

        switch (funcion)
        {
            case 0:

                switch (tiempo)
                {
                    case 5:

                        valorMinimo = valorMinimoDeEnemigos[0, indiceJ, 0];
                        valorMaximo = valorMaximoDeEnemigos[0, indiceJ, 0];
                        break;

                    case 15:

                        valorMinimo = valorMinimoDeEnemigos[1, indiceJ, 0];
                        valorMaximo = valorMaximoDeEnemigos[1, indiceJ, 0];
                        break;


                    case 25:

                        valorMinimo = valorMinimoDeEnemigos[2, indiceJ, 0];
                        valorMaximo = valorMaximoDeEnemigos[2, indiceJ, 0];
                        break;

                    case 35:

                        valorMinimo = valorMinimoDeEnemigos[3, indiceJ, 0];
                        valorMaximo = valorMaximoDeEnemigos[3, indiceJ, 0];
                        break;
                }
                break;

            case 1:

                switch (tiempo)
                {
                    case 5:

                        valorMinimo = valorMinimoDeEnemigos[0, indiceJ, 1];
                        valorMaximo = valorMaximoDeEnemigos[0, indiceJ, 1];
                        break;

                    case 15:

                        valorMinimo = valorMinimoDeEnemigos[1, indiceJ, 1];
                        valorMaximo = valorMaximoDeEnemigos[1, indiceJ, 1];
                        break;


                    case 25:

                        valorMinimo = valorMinimoDeEnemigos[2, indiceJ, 1];
                        valorMaximo = valorMaximoDeEnemigos[2, indiceJ, 1];
                        break;

                    case 35:

                        valorMinimo = valorMinimoDeEnemigos[3, indiceJ, 1];
                        valorMaximo = valorMaximoDeEnemigos[3, indiceJ, 1];
                        break;
                }
                break;

            case 2:

                switch (tiempo)
                {
                    case 5:

                        valorMinimo = valorMinimoDeEnemigos[0, indiceJ, 2];
                        valorMaximo = valorMaximoDeEnemigos[0, indiceJ, 2];
                        break;

                    case 15:

                        valorMinimo = valorMinimoDeEnemigos[1, indiceJ, 2];
                        valorMaximo = valorMaximoDeEnemigos[1, indiceJ, 2];
                        break;


                    case 25:

                        valorMinimo = valorMinimoDeEnemigos[2, indiceJ, 2];
                        valorMaximo = valorMaximoDeEnemigos[2, indiceJ, 2];
                        break;

                    case 35:

                        valorMinimo = valorMinimoDeEnemigos[3, indiceJ, 2];
                        valorMaximo = valorMaximoDeEnemigos[3, indiceJ, 2];
                        break;
                }
                break;

            case 3:

                switch (tiempo)
                {
                    case 5:

                        valorMinimo = valorMinimoDeEnemigos[0, indiceJ, 3];
                        valorMaximo = valorMaximoDeEnemigos[0, indiceJ, 3];
                        break;

                    case 15:

                        valorMinimo = valorMinimoDeEnemigos[1, indiceJ, 3];
                        valorMaximo = valorMaximoDeEnemigos[1, indiceJ, 3];
                        break;


                    case 25:

                        valorMinimo = valorMinimoDeEnemigos[2, indiceJ, 3];
                        valorMaximo = valorMaximoDeEnemigos[2, indiceJ, 3];
                        break;

                    case 35:

                        valorMinimo = valorMinimoDeEnemigos[3, indiceJ, 3];
                        valorMaximo = valorMaximoDeEnemigos[3, indiceJ, 3];
                        break;
                }
                break;
        }
    }

    private int CalcularEnemigosAleatorios(float tiempo, int funcion)
    {
        if (objetivoDelEnemigo != null)
        {
            CalcularValores(funcion, tiempo);
            enemigosPendientes = Utilidades.ObtenerNumeroAleatorio(valorMinimo, valorMaximo + INCREMENTO);
        }
        
        //Debug.Log("Enemigos Instanciados, Min - " + valorMinimo + ", Max - " + valorMaximo +  ", Total de Enemigos - " + enemigosPendientes);//Mostrar Informacion - 
        return enemigosPendientes;        
    }

    private string CalcularTipoOrbe()
    {
        if (nivelActual >= 1 && nivelActual < 10)
        {
            tipoDeOrbe = Utilidades.ObtenerNumeroAleatorio(1, 4);;
        }

        else if (nivelActual >= 10)
        {
            tipoDeOrbe = Utilidades.ObtenerNumeroAleatorio(4, 7);;
        }

        if (objetivoDelEnemigo != null)
        {
            switch (tipoDeOrbe)
            {
                case 1:
                    tipoOrbe = "Orbe1";
                    break;

                case 2:
                    tipoOrbe = "Orbe2";
                    break;
                
                case 3:
                    tipoOrbe = "Orbe3";
                    break;
                
                case 4:
                    tipoOrbe = "Orbe4";
                    break;

                case 5:
                    tipoOrbe = "Orbe5";
                    break;

                case 6:
                    tipoOrbe = "Orbe6";
                    break;

                default:
                    Debug.Log("No existe el valor de dicho Orbe - " + tipoDeOrbe);
                    break;
            }

            if (mostrarMensajes)
            {
                Debug.Log( "Valor - " + tipoDeOrbe + ", " + tipoOrbe);
            }            
        }

        return tipoOrbe;  
    }

    private string CalcularTipoEnemigo()
    {
        CalcularTipoDeEnemigo();

        if (objetivoDelEnemigo != null)
        {
            switch (tipoDeEnemigo)
            {
                case 1:
                    tipoEnemigo = "Tipo1";
                    break;

                case 2:
                    tipoEnemigo = "Tipo2";
                    break;

                case 3:
                    tipoEnemigo = "Tipo3";
                    break;

                case 4:
                    tipoEnemigo = "Tipo4";
                    break;

                default:
                    Debug.LogError("Valor incorrecto. No se encuentra el - " + tipoDeEnemigo + ", de enemigo solicitado");
                    tipoEnemigo = "Tipo1";
                    break;
            }       
        }

        return tipoEnemigo;        
    }

    private int CalcularTipoDeEnemigo()
    {
        if (nivelActual >= 1 && nivelActual < 20)
        {
            tipoDeEnemigo = 1;
        }

        else if (nivelActual >= 20 && nivelActual < 30)
        {
            tipoDeEnemigo = 2;
        }

        else if (nivelActual >= 30 && nivelActual < 40)
        {
            tipoDeEnemigo = Utilidades.ObtenerNumeroAleatorio(2, 4);
        }

        else if (nivelActual >= 40 && nivelActual < 50)
        {
            tipoDeEnemigo = 3;
        }

        else if (nivelActual >= 50)
        {
           tipoDeEnemigo = Utilidades.ObtenerNumeroAleatorio(3, 5);
        }

        else
        {
           tipoDeEnemigo = Utilidades.ObtenerNumeroAleatorio(0, 0);
           Debug.LogError("Nivel Incorrecto - " + nivelActual + ", no se generarán enemigos");
        }

        if (mostrarMensajes)
        {
            Debug.Log("Tipo de enemigo - " + tipoDeEnemigo + ", del nivel actual - " + nivelActual );
        }

        return tipoDeEnemigo;
    }   

    IEnumerator InstanciarOrbes(float tiempo) 
    {
        float tiempoParaDestruirse = 3.95f;//1.95f

        CalcularTipoOrbe();

        yield return new WaitForSeconds(tiempo);

        switch (tipoOrbe)
        {
            case "Orbe1":
                Destroy(Instantiate(objetoOrbe[0], posicionInicialOrbe.position, Quaternion.identity), tiempoParaDestruirse);
                break;

            case "Orbe2":
                Destroy(Instantiate(objetoOrbe[1], posicionInicialOrbe.position, Quaternion.identity), tiempoParaDestruirse);
                break;
            
            case "Orbe3":
                Destroy(Instantiate(objetoOrbe[2], posicionInicialOrbe.position, Quaternion.identity), tiempoParaDestruirse);
                break;

            case "Orbe4":
                Destroy(Instantiate(objetoOrbe[3], posicionInicialOrbe.position, Quaternion.identity), tiempoParaDestruirse);
                break;

            case "Orbe5":
                Destroy(Instantiate(objetoOrbe[4], posicionInicialOrbe.position, Quaternion.identity), tiempoParaDestruirse);
                break;

             case "Orbe6":
                Destroy(Instantiate(objetoOrbe[5], posicionInicialOrbe.position, Quaternion.identity), tiempoParaDestruirse);
                break;

            default:
                Debug.LogError("No se encuentra el tipo de Orbe - " + tipoOrbe + " solicitado");
                break;
        }

        if (mostrarMensajes)
        {
            Debug.Log("Se instancio Orbe - " + tipoOrbe + " solicitado");
        }
    }

    private int IndiceAleatoriedad (int tamañoDelArreglo)
    {
        int indiceAleatoriedad = Utilidades.ObtenerNumeroAleatorio(0, tamañoDelArreglo);
        return indiceAleatoriedad;
    }

    IEnumerator InstanciarEnemigos(float tiempo, int funcion)
    {        
        Color colorFinal = Color.red;
        Transform espacioLibre = generadorDeMapa.GetEspacioLibre();

        StartCoroutine(generadorDeMapa.GenerarEfectoAlInstanciar(espacioLibre, colorFinal));
     
        CalcularTipoEnemigo();

        yield return new WaitForSeconds(tiempo);

        switch (tipoEnemigo)       
        {
            case "Tipo1":

                switch (funcion)
                { 
                    case 0:
                        aparicionDeEnemigo = Instantiate (enemigoTipo1Tanque[IndiceAleatoriedad(enemigoTipo1Tanque.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 1:
                        aparicionDeEnemigo = Instantiate (enemigoTipo1DPSRangoAlto[IndiceAleatoriedad(enemigoTipo1DPSRangoAlto.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 2:
                        aparicionDeEnemigo = Instantiate (enemigoTipo1DPSRangoBajo[IndiceAleatoriedad(enemigoTipo1DPSRangoBajo.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 3:
                        aparicionDeEnemigo = Instantiate (enemigoTipo1Especial[IndiceAleatoriedad(enemigoTipo1Especial.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    default:
                        break;
                }
                
                break;

            case "Tipo2":
            
                switch (funcion)
                {
                    case 0:
                        aparicionDeEnemigo = Instantiate (enemigoTipo2Tanque[IndiceAleatoriedad(enemigoTipo2Tanque.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 1:
                        aparicionDeEnemigo = Instantiate (enemigoTipo2DPSRangoAlto[IndiceAleatoriedad(enemigoTipo2DPSRangoAlto.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 2:
                        aparicionDeEnemigo = Instantiate (enemigoTipo2DPSRangoBajo[IndiceAleatoriedad(enemigoTipo2DPSRangoBajo.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 3:
                        aparicionDeEnemigo = Instantiate (enemigoTipo2Especial[IndiceAleatoriedad(enemigoTipo2Especial.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    default:
                        break;
                }
                
                break;

            case "Tipo3":
           
                switch (funcion)
                {
                    case 0:
                        aparicionDeEnemigo = Instantiate (enemigoTipo3Tanque[IndiceAleatoriedad(enemigoTipo3Tanque.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 1:
                        aparicionDeEnemigo = Instantiate (enemigoTipo3DPSRangoAlto[IndiceAleatoriedad(enemigoTipo3DPSRangoAlto.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 2:
                        aparicionDeEnemigo = Instantiate (enemigoTipo3DPSRangoBajo[IndiceAleatoriedad(enemigoTipo3DPSRangoBajo.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 3:
                        aparicionDeEnemigo = Instantiate (enemigoTipo3Especial[IndiceAleatoriedad(enemigoTipo3Especial.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    default:
                        break;
                }
                
                break;

            case "Tipo4":
               
                switch (funcion)
                {
                    case 0:
                        aparicionDeEnemigo = Instantiate (enemigoTipo4Tanque[IndiceAleatoriedad(enemigoTipo4Tanque.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 1:
                        aparicionDeEnemigo = Instantiate (enemigoTipo4DPSRangoAlto[IndiceAleatoriedad(enemigoTipo4DPSRangoAlto.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 2:
                        aparicionDeEnemigo = Instantiate (enemigoTipo4DPSRangoBajo[IndiceAleatoriedad(enemigoTipo4DPSRangoBajo.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    case 3:
                        aparicionDeEnemigo = Instantiate (enemigoTipo4Especial[IndiceAleatoriedad(enemigoTipo4Especial.Length)], espacioLibre.position + Vector3.up, Quaternion.identity) as Enemigo;
                        break;

                    default:
                        break;
                }

                break;

            default:
                Debug.LogError("No se encuentra el " + tipoEnemigo + ", de enemigo solicitado");
                break;
        }          
    }

    IEnumerator GenerarObjetos(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
        
        if ((objetivoDelEnemigo != null && InterfazDeUsuario.ganoPartida == false) || InterfazDeUsuario.misionFallida == false)
        {
            numeroDeObjetos[0] = CalcularObjetosAleatorios("objetoCuracion", tiempo);
            numeroDeObjetos[1] = CalcularObjetosAleatorios("objetoPuntos", tiempo);

            if (InterfazDeUsuario.tipoDeMision == 3 || InterfazDeUsuario.tipoDeMision == 4)
            {
                numeroDeObjetos[2] = CalcularObjetosAleatorios("objetoMision", tiempo);
            }

            numeroDeObjetos[3] = CalcularObjetosAleatorios("objetoTrampa", tiempo);
    
            for (int i = 0; i < tipoDeObjetos.Length; i++)
            {
                for (int j = 0; j < numeroDeObjetos[i]; j++)
                {                           
                    float tiempoParaInstanciar = 1.2f;
                    StartCoroutine(InstanciarObjetos(tipoDeObjetos[i], tiempoParaInstanciar));
                }            
            }
        }
    } 

    IEnumerator GeneraObjetoPuntos(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);
    }

    private int CalcularObjetosAleatorios(string tipoDeObjeto, float tiempo)
    {
        string tipoDeObjetos = tipoDeObjeto;     

        if (objetivoDelEnemigo != null)
        {         
            switch (tiempo)
            {
                case 10: 
                 
                    if (nivelActual < 10)
                    {
                        if (tipoDeObjetos == "objetoCuracion")
                        {                            
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorCuracion[0,0,0], valorCuracion[0,0,1] + INCREMENTO);
                            break;
                        }

                        else if (tipoDeObjetos == "objetoPuntos")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorPuntos[0,0,0], valorPuntos[0,0,1] + INCREMENTO);
                            break;                            
                        }  

                        else 
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0); 
                            break;
                        }                      
                    }

                    else if (10 <= nivelActual && tipoDeObjetos == "objetoTrampa")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorTrampas[0,0,0], valorTrampas[0,0,1] + INCREMENTO);
                        break;
                    }                 

                    else if (20 <= nivelActual && nivelActual < 30 && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[0,0,0], valorMisiones[0,0,1] + INCREMENTO);
                        break;
                    }
                    
                    else if (30 <= nivelActual && nivelActual < 40 && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[0,1,0], valorMisiones[0,1,1] + INCREMENTO);
                        break;
                    }

                    else if (40 <= nivelActual && nivelActual < 50  && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[0,2,0], valorMisiones[0,2,1] + INCREMENTO);
                        break;
                    }

                    else if (50 <= nivelActual && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[0,3,0], valorMisiones[0,3,1] + INCREMENTO);
                        break;
                    }

                    else 
                    {
                        if (tipoDeObjetos == "objetoCuracion")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorCuracion[0,1,0], valorCuracion[0,1,1] + INCREMENTO);
                            break;
                        }

                        else if (tipoDeObjetos == "objetoPuntos")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorPuntos[0,1,0], valorPuntos[0,1,1] + INCREMENTO);
                            break;                            
                        }  

                        else 
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0); 
                            break;
                        }                         
                    }                             

                case 20:

                    if (nivelActual < 10)
                    {
                        if (tipoDeObjetos == "objetoCuracion")
                        {                            
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorCuracion[1,0,0], valorCuracion[1,0,1] + INCREMENTO);
                            break;
                        }

                        else if (tipoDeObjetos == "objetoPuntos")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorPuntos[1,0,0], valorPuntos[1,0,1] + INCREMENTO);
                            break;                            
                        }  

                        else 
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0); 
                            break;
                        }   
                    }

                    else if (10 <= nivelActual && tipoDeObjetos == "objetoTrampa")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorTrampas[1,0,0], valorTrampas[1,0,1] + INCREMENTO);
                        break;
                    }   

                    else if (20 <= nivelActual && nivelActual < 30 && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[1,0,0], valorMisiones[1,0,1] + INCREMENTO);
                        break;
                    }

                    else if (30 <= nivelActual && nivelActual < 40 && tipoDeObjetos == "objetoMision" && InterfazDeUsuario.tipoDeMision == 3)
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[1,1,0], valorMisiones[1,1,1] + INCREMENTO);
                        break;
                    }

                    else if (40 <= nivelActual && nivelActual < 50  && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[1,2,0], valorMisiones[1,2,1] + INCREMENTO);
                        break;
                    }

                    else if (50 <= nivelActual && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[1,3,0], valorMisiones[1,3,1] + INCREMENTO);
                        break;
                    }

                    else 
                    {
                        if (tipoDeObjetos == "objetoCuracion")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorCuracion[1,1,0], valorCuracion[1,1,1] + INCREMENTO);
                            break;
                        }
               
                        else if (tipoDeObjetos == "objetoPuntos")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorPuntos[1,1,0], valorPuntos[1,1,1] + INCREMENTO);
                            break;                            
                        }  

                        else 
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0); 
                            break;
                        }  
                    }   

                case 30:

                    if (nivelActual < 10)
                    {
                        if (tipoDeObjetos == "objetoCuracion")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorCuracion[2,0,0], valorCuracion[2,0,1] + INCREMENTO);
                            break;
                        }

                        else if (tipoDeObjetos == "objetoPuntos")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorPuntos[2,0,0], valorPuntos[2,0,1] + INCREMENTO);
                            break;                            
                        }  

                        else 
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0); 
                            break;
                        }     
                    }

                    else if (10 <= nivelActual && tipoDeObjetos == "objetoTrampa")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorTrampas[2,0,0], valorTrampas[2,0,1] + INCREMENTO);
                        break;
                    }   

                    else if (20 <= nivelActual && nivelActual < 30 && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[2,0,0], valorMisiones[2,0,1] + INCREMENTO);
                        break;
                    }

                    else if (30 <= nivelActual && nivelActual < 40 && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[2,1,0], valorMisiones[2,1,1] + INCREMENTO);
                        break;
                    }

                    else if (40 <= nivelActual && nivelActual < 50  && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[2,2,0], valorMisiones[2,2,1] + INCREMENTO);
                        break;
                    }

                    else if (50 <= nivelActual && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[2,3,0], valorMisiones[2,3,1] + INCREMENTO);
                        break;
                    }

                    else 
                    {
                        if (tipoDeObjetos == "objetoCuracion")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorCuracion[2,1,0], valorCuracion[2,1,1] + INCREMENTO);
                            break;
                        }

                         else if (tipoDeObjetos == "objetoPuntos")
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorPuntos[2,1,0], valorPuntos[2,1,1] + INCREMENTO);
                            break;                            
                        }  

                        else 
                        {
                            totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0); 
                            break;
                        }   
                    }   

                case 40:

                    if (tipoDeObjetos == "objetoCuracion")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorCuracion[3,0,0], valorCuracion[3,0,1] + INCREMENTO);
                        break;
                    }

                    else if (tipoDeObjetos == "objetoPuntos")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorPuntos[3,0,0], valorPuntos[3,0,1] + INCREMENTO);
                        break;                            
                    }

                    else if (10 <= nivelActual && tipoDeObjetos == "objetoTrampa")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorTrampas[3,0,0], valorTrampas[3,0,1] + INCREMENTO);
                        break;
                    }    

                    else if (20 <= nivelActual && nivelActual < 30 && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[3,0,0], valorMisiones[3,0,1] + INCREMENTO);
                        break;
                    }

                    else if (30 <= nivelActual && nivelActual < 40 && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[3,1,0], valorMisiones[3,1,1] + INCREMENTO);
                        break;
                    }

                    else if (40 <= nivelActual && nivelActual < 50  && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[3,2,0], valorMisiones[3,2,1] + INCREMENTO);
                        break;
                    }

                    else if (50 <= nivelActual && tipoDeObjetos == "objetoMision")
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(valorMisiones[3,3,0], valorMisiones[3,3,1] + INCREMENTO);
                        break;
                    }

                    else 
                    {
                        totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0); 
                        break;
                    }    

                default:
                    Debug.LogError("Tiempo erróneo" + tiempo + ", no se puede instanciar Objetos");
                    totalDeObjetos = Utilidades.ObtenerNumeroAleatorio(0, 0);
                    break;
            }    
        }

        if (mostrarMensajes)
        {
            Debug.Log("Objeto- " + tipoDeObjetos + ", Tiempo - "+ tiempo +", Total de Objetos- " + totalDeObjetos);//Comentario - Mostrar Informacion -         
        }

        return totalDeObjetos;        
    }

    private int CalcularTipoDeObjeto(string tipoObjeto)
    {
        if (tipoObjeto == "objetoCuracion")
        {
            if (nivelActual >= 1)
            {
                tipoDeObjeto = Utilidades.ObtenerNumeroAleatorio(1, 2);
            }

            else
            {
                tipoDeObjeto = Utilidades.ObtenerNumeroAleatorio(0, 0);
            }
        }

        else if (tipoObjeto == "objetoPuntos")
        {
            if (nivelActual >= 1)
            {
                tipoDeObjeto = Utilidades.ObtenerNumeroAleatorio(1, 2);
            }

            else
            {
                tipoDeObjeto = Utilidades.ObtenerNumeroAleatorio(0, 0);
            }
        }

        else if (tipoObjeto == "objetoMision")
        {
            if (nivelActual >= 20 && InterfazDeUsuario.tipoDeMision == 3)
            {
                tipoDeObjeto = 1;
            }

            else if (nivelActual >= 40 && InterfazDeUsuario.tipoDeMision == 4)
            {
                tipoDeObjeto = 2;
            }

            else
            {
                tipoDeObjeto = 0;
            }
        }

         else if (tipoObjeto == "objetoTrampa")
        {
            if (nivelActual >= 10 && nivelActual < 20)
            {
                tipoDeObjeto = 1;
            }

            else if (nivelActual >= 20)
            {
                tipoDeObjeto = 2;
            }

            else
            {
                tipoDeObjeto = 0;
            }
        }

        else 
        {
            tipoDeObjeto = 0;
        }

        return tipoDeObjeto;
    }    

    IEnumerator InstanciarObjetos(string objetoInstanciado, float tiempo)
    {        
        Vector3 posicionAleatoriaObjetos = new Vector3(Utilidades.ObtenerNumeroAleatorio(0, ejeX), 0f, Utilidades.ObtenerNumeroAleatorio(0, ejeY));    
        Color colorFinal;
        Transform espacioLibre = generadorDeMapa.GetEspacioLibre();

        switch (objetoInstanciado)
        {
            case "objetoCuracion":
                colorFinal = Color.green;
                StartCoroutine(generadorDeMapa.GenerarEfectoAlInstanciar(espacioLibre, colorFinal));   
                break;

            case "objetoPuntos":
                colorFinal = Color.blue;
                StartCoroutine(generadorDeMapa.GenerarEfectoAlInstanciar(espacioLibre, colorFinal));   
                break;

            case "objetoMision":
                colorFinal = Color.yellow;
                StartCoroutine(generadorDeMapa.GenerarEfectoAlInstanciar(espacioLibre, colorFinal));   
                break;
                
            case "objetoTrampa":
                colorFinal = Color.red;
                StartCoroutine(generadorDeMapa.GenerarEfectoAlInstanciar(espacioLibre, colorFinal));   
                break;
        }
       
        yield return new WaitForSeconds(tiempo);  

        if (objetoInstanciado == "objetoCuracion") 
        {
            colorFinal = Color.green;
            CalcularTipoDeObjeto(objetoInstanciado);
            InstanciarObjeto(objetoInstanciado, espacioLibre);                    
        }

        else if (objetoInstanciado == "objetoPuntos") 
        {
            colorFinal = Color.blue;
            CalcularTipoDeObjeto(objetoInstanciado);    
            InstanciarObjeto(objetoInstanciado, espacioLibre);     
        }

        else if (objetoInstanciado == "objetoMision" && (InterfazDeUsuario.tipoDeMision == 3 || InterfazDeUsuario.tipoDeMision == 4)) 
        {
            colorFinal = Color.yellow;
            CalcularTipoDeObjeto(objetoInstanciado);
            InstanciarObjeto(objetoInstanciado, espacioLibre);       
        }

        else if (objetoInstanciado == "objetoTrampa") 
        {
            colorFinal = Color.red;
            CalcularTipoDeObjeto(objetoInstanciado);
            InstanciarObjeto(objetoInstanciado, espacioLibre);      
        }
    }

    public void InstanciarObjeto(string objetoInstanciado, Transform espacioLibre)
    {
        switch (tipoDeObjeto) 
        {     
            case 1:

                if (objetoInstanciado == "objetoCuracion") 
                {
                    Destroy(Instantiate(objetoCuracion[0], espacioLibre.position + Vector3.up * 0.5f, Quaternion.identity), 3.5f);//Comentario - Eliminar Objetos instanciados despues de unos segundos               
                    break;
                }                
              
                else if (objetoInstanciado == "objetoPuntos") 
                {
                    Destroy(Instantiate(objetoPuntos[0], espacioLibre.position + Vector3.up * 0.5f, Quaternion.identity), 4.5f);
                    break;
                }

                else if (objetoInstanciado == "objetoMision" && InterfazDeUsuario.tipoDeMision == 3) 
                {
                    Destroy(Instantiate(objetoMision[0], espacioLibre.position + Vector3.up * 0.5f, Quaternion.identity), 9.5f);
                    break;
                }

                else if (objetoInstanciado == "objetoTrampa")
                {
                    Destroy(Instantiate(objetoTrampa[0], espacioLibre.position + Vector3.up * 0.05f, Quaternion.identity), 9.5f);
                    break;
                }

                else { break;}                

            case 2:
      
                if (objetoInstanciado == "objetoMision" && InterfazDeUsuario.tipoDeMision == 4) 
                {
                    Instantiate(objetoMision[1], espacioLibre.position + Vector3.up * 0.5f, Quaternion.identity);              
                    break;
                }  

                else if (objetoInstanciado == "objetoTrampa")
                {
                    Destroy(Instantiate(objetoTrampa[1], espacioLibre.position + Vector3.up * 0.05f, Quaternion.identity), 9.5f);
                    break;
                }

                else { break;}  

            case 3:

                if (objetoInstanciado == "objetoMision") 
                {
                    Instantiate(objetoMision[2], espacioLibre.position + Vector3.up * 0.5f, Quaternion.identity);              
                    break;
                }  

                else { break;}  

            default:
                Debug.Log("El Objeto no se encuentra disponible");
                break;
        }   

        if (mostrarMensajes)
        {
            Debug.Log("Objeto - " + objetoInstanciado + ", del tipo Nro - " + tipoDeObjeto);//Comentario - Mostrar Informacion -  
        }        
    }
}
