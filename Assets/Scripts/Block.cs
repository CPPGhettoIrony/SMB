// Se utiliza esta interfaz para evitar repetir código del jugador por cada tipo de bloque con el que interactúa
// La siguiente interfaz contiene una única función, disparada cuando Mario golpea el bloque por debajo

interface Block {
    public void hit();
}