﻿namespace UndeFacemRevelionul.Models
{
    public class SuperstitionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign Key to Partier
        public int PartierId { get; set; }
        public PartierModel Partier { get; set; }  // Navigation Property

        // Foreign Key to Party
        public int PartyId { get; set; }
        public PartyModel Party { get; set; }  // Navigation Property

        public bool IsCompleted { get; set; }
        public int Points { get; set; }

        // Calea fișierului de imagine
        public string ImagePath { get; set; } = string.Empty;
    }
}