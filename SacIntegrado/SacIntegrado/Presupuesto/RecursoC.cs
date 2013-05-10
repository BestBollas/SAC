using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SacIntegrado.Presupuesto
{
    class RecursoC
    {
        public int idRecurso { get; set; }
        public String ClavePresupuestal { get; set; }
        public String Nombre { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int AnioAplica { get; set; }
        public int idEmpleadó { get; set; }
        public String NombreEmpleado { get; set; }
        public bool Vigente { get; set; }
        public double SaldoInicial { get; set; }
        public double SaldoFinal { get; set; }
        public String Observaciones { get; set; }
        public String vigenteTab { get; set; }


        public List<int> AnioList
        {

            get
            {

                int anioC = DateTime.Today.Year;
                int anioS = DateTime.Today.Year + 1;
                int anioA = DateTime.Today.Year - 1;
                return new List<int> { anioA, anioC, anioS };
            }


        }
        public List<string> Vigentes
        {

            get
            {
                return new List<string> { "True", "False" };
            }


        }
    
    }
}
