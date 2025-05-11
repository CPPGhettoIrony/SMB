// Esto tiene la misma función que la interfaz "Block", pero para enemigos, en caso de que se reutilize este proyecto y se añadan más de estos.
// Esta interfaz se aplica a aquellos enemigos que puedan matarse, así que, de momento, sólo es utilizada por el Goomba.
// Debido a que no ha habido tiempo suficiente para implementar la "superestrella" (que da invencibilidad temporal), los Boos no se pueden matar,
// estos no implementan la interfaz.

interface Enemy {
    public void kill();
}