using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enums
{
    public enum GameType
    {
        CatchLetter = 1, //Conseguir la celda objetivo en un tiempo límite. (En niveles superiores puede tene que alcanzar más de una celda objetivo)
        PointsChallenge = 2, //Conseguir un número de puntos dado en un tiempo límite. (En niveles superiores aumentará el número de puntos)
        NLetterWordChallenge = 3, //Conseguir una palabra con un número de letras dadas en un tiempo límite. (En niveles superiores aumentará el número de letras de la palabra)
        NLetterChallenge = 4, //Conseguir una letra un númerod e veces dado en un tiempo límite. (En niveles superiores aumentará el número de veces)
        HiddenWord = 5, //Completar la palabra dada oculta en el tablero en un tiempo límite
        Flash = 6, //Completar un número de palabras distintas dada en un tiempo límite
        Puzze = 7 //Se muestra una palabra con las letras ocultas. El jugador deberá completar la palabra, formando palabras que contengan las letras.
                   //Cuando se forme una palabra que contenga una letra de la palabra oculta, esta letra/s se develará en la palabra. Tendrá que hacerlo en un tiempo límite
                   //En niveles superiores se añadirán palabras más complejas
        
    }
}
