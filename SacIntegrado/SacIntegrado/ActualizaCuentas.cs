using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SacIntegrado
{
    class ActualizaCuentas
    {
        Db c = new Db();

        public void ActSaldos(int cta, int papa,float importe)
        {
            CuentaEnc rp;
           
            CuentaDet renglon = (from r in c.CuentaDet
                                 where r.idCuenta == cta
                                 select r).SingleOrDefault();
            renglon.SaldoFinal = renglon.SaldoFinal+(importe);
            c.SubmitChanges();
            if (papa == 0) return;
            rp = (from ele in c.CuentaEnc
                          where ele.IdCuenta == papa
                          select ele).SingleOrDefault();
            ActSaldos(rp.IdCuenta, rp.Padre.Value, importe);
        }
    }
}
