using System;
using System.Collections.Generic;
using System.Text;

namespace EventosVivos.Domain.Entities
{
    public class Venue
    {
        public int Id { get; private set; }
        public string Nombre { get; private set; } = string.Empty;
        public int Capacidad { get; private set; }
        public string Ciudad { get; private set; } = string.Empty;

        private Venue() { }

        public static Venue Create(int id, string nombre, int capacidad, string ciudad)
            => new() { Id = id, Nombre = nombre, Capacidad = capacidad, Ciudad = ciudad };
    }
}
