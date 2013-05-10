using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SacIntegrado.Presupuesto
{
    class PartidaProyectoC
    {
        public int idCtaProy { get; set; }
        public int idProyecto { get; set; }
        public int idCuenta { get; set; }
        public String Cuenta { get; set; }
        public String Nombre { get; set; }
        public int idPeriodo { get; set; }
        public int Periodo { get; set; }
        public double saldoInicial { get; set; }
        public double saldoFin { get; set; }
        public double enero { get; set; }
        public double febrero { get; set; }
        public double marzo { get; set; }
        public double abril { get; set; }
        public double mayo { get; set; }
        public double junio { get; set; }
        public double julio { get; set; }
        public double agosto { get; set; }
        public double septiembre { get; set; }
        public double octubre { get; set; }
        public double noviembre { get; set; }
        public double diciembre { get; set; }
    }
}
