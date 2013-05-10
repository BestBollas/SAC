using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SacIntegrado
{
    class ProductosRequeridos
    {
        public string nombreProd { get; set; }
        public double cantidad { get; set; }
        public double precio { get; set; }
        public double total { get; set; }
        //EDICION FOZZ PARA PRESUPUESTO ORDEN/GASTO
        public string clasif { get; set; }
        public string unidMe { get; set; }
        public int partidaNum { get; set; }
    }
}
