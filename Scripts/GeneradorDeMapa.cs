using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GeneradorDeMapa : MonoBehaviour
{
    public Mapa[] mapas;
    public int indiceDelMapa;
    Mapa mapaActual;
    public Transform tilePrefab;
    public Transform obstaculoPrefab;
    public Transform[] objetosPrefab;
    public Transform sueloParaDesplazarse;
    public Transform impideDesplazamientoPrefab;
    public Vector2 tamañoMaximoDelMapa;
    List<Coordenadas> todasLasCoordenadas;
    Queue<Coordenadas> tileCoordenadasBarajadas;
    Queue<Coordenadas> tileCoordenadasLibresBarajadas;
    Transform[,] espacioDelMapa;
    const float DISTANCIAMEDIATILE = 0.5f;
    const int PRIMERVALOR = -999;
    const int VALORFINAL = 999;
    public float tamañoDelTile;   
    Transform obstaculoNuevo; 
    int contarObstaculos;
    int tipoDeObstaculo;
    int tipoDeObjeto;
    float alturaDelObstaculo;
    System.Random mapaAleatorio;
    Vector3 posicionDelObstaculo;
    Coordenadas coordenadaAleatoria;
    Transform contenedorDelMapa;

    void Start()
    {
        GenerarMapa();
    }

    public void GenerarMapa()
    {
        string contenedorDelNombre = "Mapa Generado";
        mapaActual = mapas[indiceDelMapa];
        espacioDelMapa = new Transform[mapaActual.tamañoDelMapa.coordenadaX, mapaActual.tamañoDelMapa.coordenadaY];
        mapaActual.semilla = Utilidades.ObtenerNumeroAleatorio(PRIMERVALOR, VALORFINAL);
        mapaAleatorio = new System.Random(mapaActual.semilla);
        //int contarObjetos;

        GetComponent<BoxCollider>().size = new Vector3(mapaActual.tamañoDelMapa.coordenadaX * tamañoDelTile, 0.05f, mapaActual.tamañoDelMapa.coordenadaY  * tamañoDelTile);
        //DISTANCIAMEDIATILE, hace que el box collider tome mas altura
        Debug.Log("1 - El Mapa es, semilla " + mapaActual.semilla);

        todasLasCoordenadas = new List<Coordenadas>();

        for (int x = 0; x < mapaActual.tamañoDelMapa.coordenadaX; x++)
        {
            for (int y = 0; y < mapaActual.tamañoDelMapa.coordenadaY; y++)
            {
                todasLasCoordenadas.Add(new Coordenadas(x, y));
            }
        }
        
        tileCoordenadasBarajadas = new Queue<Coordenadas>(Utilidades.BarajarArreglo (todasLasCoordenadas.ToArray(), mapaActual.semilla));              
        
        if (transform.Find(contenedorDelNombre))
        {
            DestroyImmediate(transform.Find(contenedorDelNombre).gameObject);
        }

        contenedorDelMapa = new GameObject(contenedorDelNombre).transform;
        contenedorDelMapa.parent = transform;

        for (int x = 0; x < mapaActual.tamañoDelMapa.coordenadaX; x++)
        {
            for (int y = 0; y < mapaActual.tamañoDelMapa.coordenadaY; y++)
            {
                Vector3 tilePosicion = new Vector3(-mapaActual.tamañoDelMapa.coordenadaX / 2 + DISTANCIAMEDIATILE + x, 0, -mapaActual.tamañoDelMapa.coordenadaY / 2 + DISTANCIAMEDIATILE + y);
                Transform newTile = Instantiate(tilePrefab, tilePosicion, Quaternion.Euler(Vector3.right*90)) as Transform;
                newTile.localScale = Vector3.one * tamañoDelTile;
                newTile.parent = contenedorDelMapa;
                espacioDelMapa[x, y] = newTile;
            }
        }

        bool[,] verificarObstaculos = new bool[(int)mapaActual.tamañoDelMapa.coordenadaX, (int)mapaActual.tamañoDelMapa.coordenadaY];
        int contarObstaculosActuales = 0;
        List <Coordenadas> coordenadasLibres = new List<Coordenadas>(todasLasCoordenadas);

        for (int i = 0; i < 2 ; i++)
        {
            tipoDeObstaculo = i;
            CalcultarTotalDeObstaculosPorTipo(i);
                   
            for (int j = 0; j < contarObstaculos; j++)
            {
                coordenadaAleatoria = GetCoordenadaAleatoria();
                verificarObstaculos[coordenadaAleatoria.coordenadaX, coordenadaAleatoria.coordenadaY] = true;
                contarObstaculosActuales++;

                if (coordenadaAleatoria != mapaActual.coordenadasInicialesJugador && VerificarObstaculosDelMapa(verificarObstaculos, contarObstaculosActuales))
                {                 
                    posicionDelObstaculo = ConvertirCoordenadaAPosicion(coordenadaAleatoria.coordenadaX, coordenadaAleatoria.coordenadaY);
                    tipoDeObjeto = Utilidades.ObtenerNumeroAleatorio(0, 5); 
                    //GenerarObstaculosPrefab(tipoDeObstaculo); 
                    //obstaculoNuevo.parent = contenedorDelMapa; 
                    coordenadasLibres.Remove(coordenadaAleatoria);
                }

                else
                {
                    verificarObstaculos[coordenadaAleatoria.coordenadaX, coordenadaAleatoria.coordenadaY] = false;
                    contarObstaculosActuales--;
                }   

                GenerarObstaculosPrefab(tipoDeObstaculo); 
                obstaculoNuevo.parent = contenedorDelMapa; 
                Debug.Log("Mapa - , Obstaculos Actuales"+ i + "es : " + contarObstaculosActuales);//28.03         
            }
        }

        tileCoordenadasLibresBarajadas = new Queue<Coordenadas>(Utilidades.BarajarArreglo(coordenadasLibres.ToArray(), mapaActual.semilla));              
        ImpedirDesplazamiento();
        sueloParaDesplazarse.localScale = new Vector3(tamañoMaximoDelMapa.x, tamañoMaximoDelMapa.y) * tamañoDelTile;
    }

    bool VerificarObstaculosDelMapa(bool[,] verificarObstaculos, int contarObstaculosActuales)
    {
        bool[,] mapFlags = new bool [verificarObstaculos.GetLength(0), verificarObstaculos.GetLength(1)];
        Queue<Coordenadas> queue = new Queue<Coordenadas>();
        queue.Enqueue(mapaActual.coordenadasInicialesJugador);
        mapFlags[mapaActual.coordenadasInicialesJugador.coordenadaX, mapaActual.coordenadasInicialesJugador.coordenadaY] = true;

        int contarTileDisponibles = 1;

        while (queue.Count > 0)
        {
            Coordenadas tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {                     
                    int coordenadaVecinaX = tile.coordenadaX + x;
                    int coordenadaVecinaY = tile.coordenadaY + y;

                    if (x == 0 || y == 0)
                    {
                        if (coordenadaVecinaX >= 0 && coordenadaVecinaX < verificarObstaculos.GetLength(0) && coordenadaVecinaY >= 0 && coordenadaVecinaY < verificarObstaculos.GetLength(1))
                        {
                            if (!mapFlags[coordenadaVecinaX, coordenadaVecinaY] && !verificarObstaculos[coordenadaVecinaX, coordenadaVecinaY])
                            {                                 
                                mapFlags[coordenadaVecinaX, coordenadaVecinaY] = true;
                                queue.Enqueue(new Coordenadas(coordenadaVecinaX, coordenadaVecinaY));
                                contarTileDisponibles++;
                            }
                        }
                    }
                }
            }
        }

        int contarTileDisponiblesTotales = (int)(mapaActual.tamañoDelMapa.coordenadaX * mapaActual.tamañoDelMapa.coordenadaY) - contarObstaculosActuales;
        return contarTileDisponiblesTotales == contarTileDisponibles;
    }

    Vector3 ConvertirCoordenadaAPosicion(int coordenadaX, int coordenadaY)
    {
        //Si se cambia la division de 2 a 2f se cambia la posicion de todas las tiles, afectando la instancias de obstaculos. Dejar el valor de la divion a 2 y no poner 2f.        
        return new Vector3(-mapaActual.tamañoDelMapa.coordenadaX / 2 + DISTANCIAMEDIATILE + coordenadaX, 0, -mapaActual.tamañoDelMapa.coordenadaY / 2 + DISTANCIAMEDIATILE + coordenadaY) * tamañoDelTile;
    } 

    public Coordenadas GetCoordenadaAleatoria()
    {
        Coordenadas coordenadaAleatoria = tileCoordenadasBarajadas.Dequeue();
        tileCoordenadasBarajadas.Enqueue(coordenadaAleatoria);
        return coordenadaAleatoria;
    }

    //19.10.21
    public Transform GetEspacioLibre()
    {
        Coordenadas coordenadaAleatoria = tileCoordenadasLibresBarajadas.Dequeue();
        tileCoordenadasLibresBarajadas.Enqueue(coordenadaAleatoria);
        return espacioDelMapa[coordenadaAleatoria.coordenadaX, coordenadaAleatoria.coordenadaY];//19.10.21
    }

    //20.10.21 Efecto Al Instanciar Objetos
    public IEnumerator GenerarEfectoAlInstanciar(Transform espacioLibre, Color colorFinal)
    {        
        Material materialDelEspacio = espacioLibre.GetChild(0).GetComponent<Renderer>().material;
        Color colorInicial = materialDelEspacio.color;
        float spawnTimer = 0;
        float spawnDelay = 1f;
        float tileFlashSpeed = 4;

            while (spawnTimer < spawnDelay)
            {
                materialDelEspacio.color = Color.Lerp(colorInicial, colorFinal, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
                spawnTimer += Time.deltaTime;       
                yield return null;      
            }        
    }

    [System.Serializable]
    public struct Coordenadas
    {
        public int coordenadaX;
        public int coordenadaY;

        public Coordenadas(int ejeX, int ejeY)
        {
            coordenadaX = ejeX;
            coordenadaY = ejeY;
        }
        
        public static bool operator ==(Coordenadas coordenada1, Coordenadas coordenada2)
        {
            return coordenada1.coordenadaX == coordenada2.coordenadaX && coordenada1.coordenadaY == coordenada2.coordenadaY;            
        }

        public static bool operator !=(Coordenadas coordenada1, Coordenadas coordenada2)
        {
            return !(coordenada1 == coordenada2);            
        }       

        public override bool Equals(object obj)
        {
            if (obj is Coordenadas coordenadas)
            {
                return this == coordenadas;
            }            
            
            return false;
        }

        public override int GetHashCode() => new { coordenadaX, coordenadaY }.GetHashCode();    
    } 
    
    [System.Serializable]
    public class Mapa 
    {
        public Coordenadas tamañoDelMapa;        
        public int semilla;
        public float porcentajeDeObstaculos;
        public float porcentajeDeObjetosDelMapa;
        public float alturaMinimaObstaculo;
        public float alturaMaximaObstaculo;
        public float largoObstaculo;
        public float anchoObstaculo;
        public Color colorPrincipal;
        public Color colorDeFondo;

        public Coordenadas coordenadasInicialesJugador
        {
            get 
            {
                return new Coordenadas(tamañoDelMapa.coordenadaX / 2 , tamañoDelMapa.coordenadaY / 2);
            }
        }
    } 

    private void GenerarObstaculo()
    {
        //
    }

    private void CalcultarTotalDeObstaculosPorTipo(int tipoDeObjeto)
    {
        switch (tipoDeObjeto)
        {
            case 0:
                contarObstaculos = (int)(mapaActual.tamañoDelMapa.coordenadaX * mapaActual.tamañoDelMapa.coordenadaY * mapaActual.porcentajeDeObstaculos); 
                break;

            case 1:
                contarObstaculos = (int)(mapaActual.tamañoDelMapa.coordenadaX * mapaActual.tamañoDelMapa.coordenadaY * mapaActual.porcentajeDeObjetosDelMapa);
                break;
                    
            default:
                break;
        }

        Debug.Log("Mapa - , El total de Obstaculos" + tipoDeObjeto + "es : " + contarObstaculos);//28.03
    }

    private void GenerarObstaculosPrefab(int obstaculoInstanciado)//int obstaculoInstanciado
    {
        switch (obstaculoInstanciado)
        {
            case 0:
                alturaDelObstaculo = Mathf.Lerp(mapaActual.alturaMinimaObstaculo, mapaActual.alturaMaximaObstaculo, (float)mapaAleatorio.NextDouble());
                obstaculoNuevo = Instantiate(obstaculoPrefab, posicionDelObstaculo + Vector3.up * alturaDelObstaculo / 2, Quaternion.identity) as Transform; 
                Renderer rendererDelObstaculo = obstaculoNuevo.GetComponent<Renderer>();
                Material materialDelObstaculo = new Material(rendererDelObstaculo.sharedMaterial);
                float porcentajeDeColor = coordenadaAleatoria.coordenadaX / (float)mapaActual.tamañoDelMapa.coordenadaY;
                materialDelObstaculo.color = Color.Lerp(mapaActual.colorPrincipal, mapaActual.colorDeFondo, porcentajeDeColor);
                obstaculoNuevo.localScale = new Vector3(mapaActual.largoObstaculo, alturaDelObstaculo, mapaActual.anchoObstaculo);//Cambiar tamaño de los Obstaculos                
                rendererDelObstaculo.sharedMaterial = materialDelObstaculo;      
                Debug.Log("Mapa - , Obstaculos Instanciado"+ tipoDeObstaculo + "es : tipoObstaculo");//28.03    
                break;

            case 1:
                alturaDelObstaculo = 0.3500226f;
                obstaculoNuevo = Instantiate(objetosPrefab[tipoDeObjeto], posicionDelObstaculo + Vector3.up * alturaDelObstaculo, Quaternion.identity) as Transform;
                Debug.Log("Mapa - , Obstaculos Instanciado" + tipoDeObstaculo + "es : tipoDeObjeto");//28.03   
                break;

            default:
                break;
        }       
    }

    private void ImpedirDesplazamiento()
    {
        Transform impideDesplazamientoIzquierdo = Instantiate(impideDesplazamientoPrefab, Vector3.left * (mapaActual.tamañoDelMapa.coordenadaX + tamañoMaximoDelMapa.x) / 4 * tamañoDelTile, Quaternion.identity) as Transform;
        impideDesplazamientoIzquierdo.parent = contenedorDelMapa;
        impideDesplazamientoIzquierdo.localScale = new Vector3((tamañoMaximoDelMapa.x - mapaActual.tamañoDelMapa.coordenadaX) / 2, 3.5f, mapaActual.tamañoDelMapa.coordenadaY) * tamañoDelTile;

        Transform impideDesplazamientoDerecho = Instantiate(impideDesplazamientoPrefab, Vector3.right * (mapaActual.tamañoDelMapa.coordenadaX + tamañoMaximoDelMapa.x) / 4 * tamañoDelTile, Quaternion.identity) as Transform;
        impideDesplazamientoDerecho.parent = contenedorDelMapa;
        impideDesplazamientoDerecho.localScale = new Vector3((tamañoMaximoDelMapa.x - mapaActual.tamañoDelMapa.coordenadaX) / 2, 3.5f, mapaActual.tamañoDelMapa.coordenadaY) * tamañoDelTile;

        Transform impideDesplazamientoSuperior = Instantiate(impideDesplazamientoPrefab, Vector3.forward * (mapaActual.tamañoDelMapa.coordenadaY + tamañoMaximoDelMapa.y) / 4 * tamañoDelTile, Quaternion.identity) as Transform;
        impideDesplazamientoSuperior.parent = contenedorDelMapa;
        impideDesplazamientoSuperior.localScale = new Vector3(tamañoMaximoDelMapa.x, 3.5f, (tamañoMaximoDelMapa.y - mapaActual.tamañoDelMapa.coordenadaY) / 2) * tamañoDelTile;
        
        Transform impideDesplazamientoInferior = Instantiate(impideDesplazamientoPrefab, Vector3.back * (mapaActual.tamañoDelMapa.coordenadaY + tamañoMaximoDelMapa.y) / 4 * tamañoDelTile, Quaternion.identity) as Transform;
        impideDesplazamientoInferior.parent = contenedorDelMapa;
        impideDesplazamientoInferior.localScale = new Vector3(tamañoMaximoDelMapa.x, 3.5f, (tamañoMaximoDelMapa.y - mapaActual.tamañoDelMapa.coordenadaY) / 2) * tamañoDelTile;      
    }
}
