using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SacIntegrado.Presupuesto
{
    class TipoGastoClass
    {
            public int idTG { get; set; }
            public String nombreTG { get; set; }
            public String clavePresupuestal { get; set; }
            public int anioAplica { get; set; }
            public String fechaReg { get; set; }
            public int idEmpleado { get; set; }
            public bool vigente { get; set; }
            public string vigenteTab { get; set; }
            public String Empleado { get; set; }       
    }
}
