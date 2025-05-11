// Esta interfaz sirve para simplificar los "poderes"
// Estos, aunque sean objetos distintos, tendrán una interfaz común con una sola función que retornará el tipo de poder obtenido,
// el código del jugador reaccionará de manera distinta dependiendo de dicho tipo.

interface Powerup {
    public int getType();
}