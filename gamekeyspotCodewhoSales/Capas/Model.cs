using System;
using System.Collections.Generic;
namespace Capas
{
    public class ProductosModel
    {
        public string categoriaId { get; set; }
        public string categoriaNombre { get; set; }
        public string tipo { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string productCodewhosaleId { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string platform { get; set; }
        public string quantity { get; set; }
        public string images { get; set; }
        public string regions { get; set; }
        public string languages { get; set; }
        public decimal prices { get; set; }
        public int escodewhosale { get; set; }
        public string releaseDate { get; set; }

        public string officialTitle { get; set; }
        public string category { get; set; }
        public string developerName { get; set; }
        public string developerHomepage { get; set; }

        public string visible { get; set; }

        //cuando paso el listener //
        public string fecha_update { get; set; }

        public List<galeria> galeria { get; set; }
    }

    public class galeria
    {
        public string productoId { get; set; }
        public string titulo { get; set; }
        public string foto { get; set; }
        public string video { get; set; }
    }
}
