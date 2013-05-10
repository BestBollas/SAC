using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SacIntegrado.Presupuesto
{
    class CuentaC
    {

        public int idCuenta { get; set; }
        public String Nombre { get; set; }
        public String Cuenta { get; set; }
        public int Padre { get; set; }
        public int TipoCta { get; set; }
        public int Hoja { get; set; }
        private ObservableCollection<CuentaC> misCuentas = new ObservableCollection<CuentaC>();
        Db con = new Db();

        public ObservableCollection<CuentaC> cargarCuenta
        {
            get
            {
                var pe = from r in con.CuentaEnc
                         select new { r.IdCuenta, r.Cuenta };

                misCuentas.Clear();
                foreach (var ele in pe)
                {
                    misCuentas.Add(new CuentaC { idCuenta = ele.IdCuenta, Cuenta = ele.Cuenta });
                }

                return misCuentas;
            }

        }
    }
}
