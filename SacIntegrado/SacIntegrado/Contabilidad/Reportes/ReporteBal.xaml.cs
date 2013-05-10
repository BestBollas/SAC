using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Reporting.WinForms;

namespace SacIntegrado
{
    /// <summary>
    /// Lógica de interacción para ReporteBal.xaml
    /// </summary>
    /// 

    class BalanzaCompro
    {
        public int indice { get; set; }
        public int idCuenta { get; set; }//
        public int papa { get; set; }//
        public string cuenta { get; set; }
        public string nombre { get; set; }
        public double cargo { get; set; }//
        public double abono { get; set; }//
        public double saldoIni { get; set; }
        public double saldoFin { get; set; }

    }


    public partial class ReporteBal : Page
    {
        Db dat = new Db();
        ObservableCollection<BalanzaCompro> balanzaCompro = new ObservableCollection<BalanzaCompro>();

        public ReporteBal(String user, int id, String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;

        }

        private void Page_Loaded_1(object sender, RoutedEventArgs e)
        {
            //var cuentas = from ele in dat.CuentaEnc
            //              from eme in dat.CuentaDet
            //              where ele.IdCuenta == eme.idCuenta
            //              select new { ele.Cuenta, ele.Nombre, eme.SaldoInicial, eme.SaldoFinal };
            //foreach (var s in cuentas)
            //{
            //    balanzaCompro.Add(new BalanzaCompro { cuenta = s.Cuenta, nombre = s.Nombre, saldoIni = s.SaldoInicial.Value, saldoFin = s.SaldoFinal.Value });
            //}

            //var cuentas = from ele in dat.CuentaEnc
            //              from eme in dat.CuentaDet
            //              from ene in dat.Movimiento
            //              from ach in dat.TPoliza
            //              where ach.idPeriodo == 5 && ene.idPoliza == ach.idPoliza && ene.idCuenta == ele.IdCuenta && eme.idCuenta == ele.IdCuenta
            //              //group ene by ene.Importe);
            //             // group new { ele, eme, ene, ach } by new {ach.idPoliza, ele.Cuenta, ele.Nombre, eme.SaldoInicial, ene.Importe, eme.SaldoFinal, ach.Tipo } into g
            //              group new { ele, eme, ene, ach } by new { ach.idPoliza, ele.Cuenta, ele.Nombre, eme.SaldoInicial, ene.Importe, eme.SaldoFinal, ach.Tipo } into g
            //             // orderby g.Key.Cuenta
            //              select new { g.Key.idPoliza,g.Key.Cuenta, g.Key.Nombre, g.Key.SaldoInicial,g.Key.Tipo, g.Key.Importe, g.Key.SaldoFinal };
                          
                          
                //group o by new{o.  Cuenta, ele.Nombre, eme.SaldoInicial, ene.Tipo,eme.SaldoFinal, ene.Importe};
                /// select new { ele.Cuenta, ele.Nombre, eme.SaldoInicial, ene.Tipo, eme.SaldoFinal, ene.Importe });
                /// 
            //foreach (var f in cuentas)
            //{
            //    balanzaCompro.Add(new BalanzaCompro { id = f.idPoliza, cuenta = f.Cuenta, nombre = f.Nombre, saldoIni = f.SaldoInicial.Value, abono = f.Importe.Value, cargo = f.Importe.Value, saldoFin = f.SaldoFinal.Value });
            //}

 //           select ce.Cuenta,ce.Nombre,ce.TipoCuenta, cd.SaldoInicial,cd.SaldoFinal
 //from CuentaEnc ce, CuentaDet cd 
 //where ce.IdCuenta=cd.idCuenta
 //order by ce.Cuenta

            //var q = from b in listOfBoxes
            //    group b by b.Owner into g
            //    select new
            //               {
            //                   Owner = g.Key,
            //                   Boxes = g.Count(),
            //                   TotalWeight = g.Sum(item => item.Weight),
            //                   TotalVolume = g.Sum(item => item.Volume)
            //               };

//            select ce.IdCuenta,ce.Cuenta,ce.Nombre,cd.SaldoInicial,cd.SaldoFinal, m.Tipo, SUM(m.Importe)
// from Movimiento m, CuentaEnc ce, CuentaDet cd
//where m.idCuenta=ce.IdCuenta and cd.idCuenta=ce.IdCuenta
//group by ce.Nombre,ce.Cuenta, cd.SaldoInicial, cd.SaldoFinal, m.Tipo,ce.IdCuenta
//order by ce.Cuenta

            var cuentas = from ce in dat.CuentaEnc
                          from cd in dat.CuentaDet
                          
                          where ce.IdCuenta == cd.idCuenta 
                          orderby ce.Padre
                          select new {ce.Padre,ce.IdCuenta,ce.Cuenta,ce.Nombre,ce.TipoCuenta,cd.SaldoInicial,cd.SaldoFinal };

            int miIndice = 0;

            foreach (var f in cuentas)
            {
               

                balanzaCompro.Add(new BalanzaCompro { indice = miIndice, idCuenta = f.IdCuenta, cuenta = f.Cuenta, nombre = f.Nombre, saldoIni = f.SaldoInicial.Value, abono = 0, cargo = 0, saldoFin = f.SaldoFinal.Value, papa = f.Padre.Value });
                miIndice++;
            }

            foreach (var f in cuentas)
            {
                var abono = from m in dat.Movimiento
                            where m.idCuenta == f.IdCuenta
                            group m by m.Tipo into g
                            select new
                            {
                                llave = g.Key,
                                importe = g.Sum(x => x.Importe)
                            };
                double? car=0;
                double? abo=0;
                char tipo='A';

                foreach (var w in abono)
                {
                   // MessageBox.Show("tippo: " + w.llave + "   importe:  " + w.importe);
                    if (w.llave == 'C')
                    {
                        
                        ActCargoAbono(f.IdCuenta, f.Padre.Value, w.importe.Value, 'C');                        
                    }
                    else
                    {
                        
                        ActCargoAbono(f.IdCuenta, f.Padre.Value,w.importe.Value, 'A');
                    }     
               
                    tipo=w.llave.Value;
                }                               
              
            }

           var orden = balanzaCompro.OrderBy(x => x.cuenta);

            pantalla.LocalReport.DataSources.Add(new ReportDataSource("DataSet1",orden));
            //pantalla.LocalReport.ReportPath = "pack://application:,,,/Reportes/ReporteBalanza.rdlc";//"" + new Uri(@"Reportes/ReporteBalanza.rdlc", UriKind.Relative);//Convert.ToString(new Uri(@"Reportes\ReporteBalanza.rdlc", UriKind.Relative));//"C:\\Users\\wassaurus\\Desktop\\respaldo_ultimo_sac_bueno\\ultimo_sac\\SacIntegradoUltimo\\SacIntegrado\\SacIntegrado\\Contabilidad\\Reportes\\ReporteBalanza.rdlc";
            pantalla.LocalReport.ReportPath = "C:\\Users\\Fozzie\\Documents\\AppsWPF\\PruebasMias\\Expue12NovIntegrado\\SacIntegrado\\SacIntegrado\\Contabilidad\\Reportes\\ReporteBalanza.rdlc";
            pantalla.RefreshReport();
        }

        public void ActCargoAbono(int cta, int papa, double importe, char tipo)
        {

            var renglon = (from r in balanzaCompro
                           where r.idCuenta == cta
                           select r).SingleOrDefault();

            if (tipo == 'A')
            {
                balanzaCompro.ElementAt(renglon.indice).abono = balanzaCompro.ElementAt(renglon.indice).abono + importe;
            }
            else
            {
                balanzaCompro.ElementAt(renglon.indice).cargo = balanzaCompro.ElementAt(renglon.indice).cargo + importe;
            }

            if (papa == 0) return;
            var rpapa = (from l in balanzaCompro
                         where l.idCuenta == papa
                         select l).SingleOrDefault();

            //if (tipo == 'A')
            //{
            //    balanzaCompro.ElementAt(rpapa.indice).abono = balanzaCompro.ElementAt(rpapa.indice).abono + importe;
            //}
            //else
            //{
            //    balanzaCompro.ElementAt(rpapa.indice).cargo = balanzaCompro.ElementAt(rpapa.indice).cargo + importe;
            //}

            ActCargoAbono(rpapa.idCuenta, rpapa.papa, importe, tipo);
        }

        private void menuCerrarSesion_Click(object sender, MouseButtonEventArgs e)
        {
            //CONFIRMACIÓN PARA SALIR
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
            }
            else { }
        }
    }
}
