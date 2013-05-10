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

namespace SacIntegrado.Adquisiciones
{
    /// <summary>
    /// Lógica de interacción para ReportComite.xaml
    /// </summary>
    public partial class ReportComite : Page
    {
        Db conex = new Db();
        ObservableCollection<OrdenDetalle> ocOd = new ObservableCollection<OrdenDetalle>();
        ObservableCollection<reporteComite> ocRc = new ObservableCollection<reporteComite>();

        public ReportComite()
        {
            InitializeComponent();
        }

        private void loadReport(object sender, RoutedEventArgs e)
        {
            int idPrvd = 0;
            int folOrdEn = 0;
            string fecOrdEn = "";
            string oencQ = "";

            var qryOe = from oe in conex.OrdenEnc select oe;
            foreach(var f in qryOe){
                idPrvd = f.idProveedor.Value;
                folOrdEn = f.folio.Value;
                fecOrdEn = f.fecha.Value.ToString();
                oencQ = (from pr in conex.Proveedor where pr.idProveedor == idPrvd select pr.Nombre).SingleOrDefault();
                ocRc.Add(new reporteComite { nombProve = oencQ, folioOrdEnc = folOrdEn, fechOrdEnc = fecOrdEn });
            }
            string nPrdc = "";
            float to = 0;
            var qryOd = from od in conex.OrdenDet select od;
            foreach (var w in qryOd) {
                float prc = (float)w.Precio.Value;
                float cnti = (float)w.Cantidad.Value;
                to = prc * cnti;
                nPrdc = (from pd in conex.Producto where pd.idProducto == w.idProducto select pd.Nombre).SingleOrDefault();
                ocOd.Add(new OrdenDetalle { nomProdOrD = nPrdc, cantidad = (float)w.Cantidad.Value, precio = (float)w.Precio.Value, totOrD = to });
            }

            repComite.LocalReport.DataSources.Add(new ReportDataSource("DataSetComiteAt", ocRc));
            repComite.LocalReport.DataSources.Add(new ReportDataSource("DataSetComiteOc", ocOd));
            repComite.LocalReport.ReportPath = "C:\\Users\\Fozzie\\Documents\\AppsWPF\\PruebasMias\\Expue12NovIntegrado\\SacIntegrado\\SacIntegrado\\Adquisiciones\\ReportComit.rdlc";
            repComite.RefreshReport();
        }
    }

    public class reporteComite {
        public int folioOrdEnc {get; set;}
        public string fechOrdEnc { get; set; }
        public string nombProve { get; set; } 
        public ObservableCollection<OrdenDetalle> ocDeta {get; set;}
    }

    public class OrdenDetalle {
        public string nomProdOrD { get; set;}
        public float cantidad { get; set; }
        public float precio { get; set; }
        public float totOrD { get; set; }
    }

}
