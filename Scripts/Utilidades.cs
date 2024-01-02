using System.Collections;
using System;


public static class Utilidades
{
    public static T[] BarajarArreglo<T>(T[] arreglo, int semilla)//Comentario - Metodo Generico en una clase no genérica, T es el Tipo.
    {
        System.Random numeroAleatorio = new System.Random(semilla);

        for (int i = 0; i < arreglo.Length - 1; i++)
        {
            int indiceAleatorio = numeroAleatorio.Next(i, arreglo.Length);
            T numeroTemporal = arreglo[indiceAleatorio];
            arreglo[indiceAleatorio] = arreglo[i];
            arreglo[i] = numeroTemporal;
        }

        return arreglo;
    }  

    private static readonly Random pseudoAleatorio = new Random();

    public static int ObtenerNumeroAleatorio(int valorMin, int valorMax)
    {
        lock(pseudoAleatorio)//Comentario - Se ejecuta 1 petición a la vez, luego de realizarla se libera el bloqueo.
        {
            return pseudoAleatorio.Next(valorMin, valorMax);
        }
    }
}
