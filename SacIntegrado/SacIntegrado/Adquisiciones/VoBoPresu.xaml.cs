using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using SacIntegrado.Adquisiciones;

namespace SacIntegrado.Adquisiciones
{
    /// <summary>
    /// Lógica de interacción para VoBoPresu.xaml
    /// </summary>
    /// 

    public partial class VoBoPresu : Page
    {
        int Id_empleado = 0;
        int folioSelected = 0;
        int xo = 0;
        string fechAutoriPresup = DateTime.Today.ToShortDateString();
        Db conex = new Db();
        //ObservableCollection<ordenCompra> ocOC = new ObservableCollection<ordenCompra>();
        //Mio
        ObservableCollection<OrdenCo> ocOrdC = new ObservableCollection<OrdenCo>();
        ObservableCollection<GastoMe> ocGasM = new ObservableCollection<GastoMe>();
        ObservableCollection<ProductosRequeridos> listaProductosRequi = new ObservableCollection<ProductosRequeridos>();
        ObservableCollection<DatosRequi> listDatosRequi = new ObservableCollection<DatosRequi>();
        public VoBoPresu() { 
        
        }
        public VoBoPresu(string user, int id, string nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            Id_empleado = id;
            llenarOCGM();
            //llenarOC();
            //llenarGM();
            
        }

        private void cerrarSesion(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
                //   InicioLogin.banderaLog = false;
            }
            else { }
        }
        private void habilitarObserv(object sender, RoutedEventArgs e)
        {
            txtObserv.IsEnabled = true;
        }

        public void muestraTotal()
        {
            var ttl = (from p in conex.Producto
                       from re in conex.RequiEnc
                       from rd in conex.RequiDet

                       where re.Folio == folioSelected &&
                       re.idRequisicion == rd.idRequiEnc &&
                       p.idProducto == rd.idProducto

                       select rd.total).Sum();

            lblTot.Content = "" + ttl;
        }

        public void muestraDatosRequi()
        {

            var requi = from re in conex.RequiEnc
                        from p in conex.Proyecto
                        from e in conex.Empleado
                        from d in conex.Departamento

                        where
                            p.idProyecto == re.idProyecto &&
                            re.idEmpleado == e.idEmpleado &&
                            re.idProyecto == p.idProyecto &&
                            d.idDepto == p.idDepto &&
                            re.Folio == folioSelected


                        select new
                        {
                            re.Folio,
                            re.Motivo,
                            re.Observaciones,
                            nProy = p.Nombre,
                            re.fechaElaboracion,
                            re.FechaRequerida,
                            e.Nombre,
                            d.NombreDepto
                        };

            foreach (var e in requi)
            {

                listaProductosRequi.Clear();
                string fecha_elaboracion = Convert.ToString(e.fechaElaboracion).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                listDatosRequi.Add(new DatosRequi { nombreProy = e.nProy, folio = e.Folio.Value/*e.Folio.Value*/, fechaElab = fecha_elaboracion, fechaReq = fecha_requerida, nombreSolicitante = e.Nombre, nombreDepto = e.NombreDepto, mot = e.Motivo, obs = e.Observaciones });
                proyectPresup.Text = e.Nombre;
                folio.Content = "" + e.Folio;
                fechaElab.Content = "" + fecha_elaboracion;
                //fechaReq.Text = "" + fecha_requerida;
                solicitante.Text = "" + e.Nombre;
                dptoBenef.Text = e.NombreDepto;
                txtMotiv.Text = e.Motivo;
                txtObserv.Text = e.Observaciones;
            }

            //MessageBox.Show("FOLIO SELECTED: OTRO METODO: " + folioSelected);

        }

        //---------------------------------------------------------------------------------------------BOTON AUTORIZAR
        private void btnAutorizar(object sender, RoutedEventArgs e)
        {

            if (ocRevisar.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un registro");
            }
            else
            {
                //MessageBox.Show("Aqui es cueando se da el VoBo" + folioSelected);
                var actualizar = (from a in conex.RequiEnc
                                  where a.Folio == folioSelected
                                  select a).Single();

                actualizar.StatusPresup = 1;
                actualizar.fechAutoriPresup = Convert.ToDateTime(fechAutoriPresup);
                conex.SubmitChanges();
                //llenarOC();
                //llenarGM();
                llenarOCGM();
                MessageBox.Show("Se Autorizo Correctamente");
              limpiar();
                
            }
        }
        //-----------------------------------------------------------------------------------------------------------METODO LIMPIAR
        public void limpiar()
        {
            try
            {
                proyectPresup.Text = " ";
                folio.Content = "0000";
                fechaElab.Content = "00/00/0000";
                //fechaReq.Text = "";
                solicitante.Text = "";
                dptoBenef.Text = "";
                dGproductRequi.ItemsSource = "";
                folioSelected = 0;
                txtMotiv.Text = "";
                txtObserv.Text = "";
                lblTot.Content = "000.00";
            }
            catch (NullReferenceException){
                MessageBox.Show("Error al limpiar campos");
            }
            

        }
        
        public void llenarGM()
        {
            var ordPurch = from ore in conex.OrdenEnc
                           //from ord in conex.OrdenDet
                           from em in conex.Empleado
                           //from rqe in conex.RequiEnc
                           //from rqd in conex.RequiDet
                           where ore.tipo == 'G' /*|| ore.tipo == 'G'*/ &&
                               //ore.idOrdenEnc == ord.idOrdenEnc &&
                           ore.idComprador == em.idEmpleado//) || (ore.tipo == 'G' && ore.idComprador == em.idEmpleado)
                           //ore.idOrdenEnc == ord.idOrdenEnc //&&
                           /*  ore.idReqEnc == rqe.idRequisicion &&
                             rqe.idRequisicion == rqd.idRequiEnc &&
                             rqd.idProvAsig == ore.idProveedor
                             select new { ore, ord, rqe, rqd }).Distinct();*/
                           //select new { ore, ord };
                           select new { ore,/* ord,*/ em };
            foreach (var qa in ordPurch)
            {
                MessageBox.Show("Entro G");
                if (qa.ore.tipo == 'G')
                {
                    ocGasM.Add(new GastoMe { idOrdE = qa.ore.idOrdenEnc, idRqiE = qa.ore.idReqEnc.Value, folioRq = qa.ore.folio.Value, fechaRegOrd = Convert.ToDateTime(qa.ore.fecha).ToShortDateString(), nCompr = "Wey", persRegis = qa.em.Nombre + " " + qa.em.apMater + " " + qa.em.apPater });
                    gmRevisar.ItemsSource = ocGasM;
                }
                else
                {
                    ocOrdC.Add(new OrdenCo { idOrdE = qa.ore.idOrdenEnc, idRqiE = qa.ore.idReqEnc.Value, folioRq = qa.ore.folio.Value, fechaRegOrd = Convert.ToDateTime(qa.ore.fecha).ToShortDateString(), nCompr = "Wey", persRegis = qa.em.Nombre + " " + qa.em.apMater + " " + qa.em.apPater });
                    gmRevisar.ItemsSource = ocOrdC;
                }
            }
        }

        public void llenarOCGM()
        //public void llenarOC()
        {


            //var query = from ren in conex.RequiEnc
            //             from cen in conex.CotizacionEnc
            //             from cde in conex.CotizacionDet
            //             from rde in conex.RequiDet
            //             from com in conex.Empleado
            //             from p in conex.Proyecto
            //             from d in conex.Departamento

            //             where ren.StatusRP == 1 &&
            //                   ren.StatusJA == 1 &&
            //                   ren.StatusJAC == 1 &&//0
            //                   ren.StatusCot == '1' &&
            //                   ren.StatusPresup == 0 &&
            //                   rde.idRequiDet == cde.idRequiDet &&
            //                   ren.idRequisicion == rde.idRequiEnc &&
            //                   cen.idCotizacionEnc == cde.idCotizacionEnc &&
            //                   ren.idComprador == com.idEmpleado &&
            //                   ren.idProyecto == p.idProyecto &&
            //                   com.idEmpleado == ren.idEmpleado &&
            //                   d.idDepto == p.idDepto

            //             select new
            //             {
            //                 cen.idCotizacionEnc,
            //                 cen.fecha,
            //                 com.Nombre,
            //                 ren.Folio
            //             };

            //foreach (var q in query)
            //{
            //    ocOC.Add(new ordenCompra { idRen = q.idCotizacionEnc, fecha = Convert.ToDateTime(q.fecha).ToShortDateString(), nomComp = q.Nombre, folio = q.Folio.Value });
            //    MessageBox.Show("Se supone esta llenando el Observable "+q.Nombre);
            //    ocRevisar.ItemsSource = ocOC;
            //}
            ////ocRevisar.ItemsSource = ocOC;
            //----------------------------------------MIO
            /*
            var quer = (from rd in conex.RequiDet
                       from re in conex.RequiEnc
                       from ore in conex.OrdenEnc
                       where re.idRequisicion == rd.idRequiEnc &&
                       re.idRequisicion == ore.idReqEnc &&
                       rd.idProvAsig == ore.idProveedor &&
                       ore.tipo == 'O'
                       select new { rd, re, ore }).Distinct();
            foreach(var d in quer){
                MessageBox.Show("Entro!");
                ocOrdC.Add(new OrdenCo { idOrdE = d.ore.idOrdenEnc, idRqiE = d.re.idRequisicion, fecha = Convert.ToDateTime(d.ore.fecha).ToShortDateString(), folioRq = d.re.Folio.Value, nCompr = "Wey", idePrv = d.ore.idProveedor.Value });
                ocRevisar.ItemsSource = ocOrdC;
            }*/
            //-------------------
            
            var ordPurch = from ore in conex.OrdenEnc
                           //from ord in conex.OrdenDet
                           from em in conex.Empleado
                           //from rqe in conex.RequiEnc
                           //from rqd in conex.RequiDet
                           where (ore.tipo == 'O' /*|| ore.tipo == 'G'*/ &&
                           //ore.idOrdenEnc == ord.idOrdenEnc &&
                           ore.idComprador == em.idEmpleado) || (ore.tipo == 'G' && ore.idComprador == em.idEmpleado)
                           //ore.idOrdenEnc == ord.idOrdenEnc //&&
                           /*  ore.idReqEnc == rqe.idRequisicion &&
                             rqe.idRequisicion == rqd.idRequiEnc &&
                             rqd.idProvAsig == ore.idProveedor
                             select new { ore, ord, rqe, rqd }).Distinct();*/
                           //select new { ore, ord };
                           select new { ore,/* ord,*/ em };
            foreach (var qa in ordPurch) {
                //MessageBox.Show("Entro");
                if (qa.ore.tipo == 'O')
                {
                    //ocOrdC.Add(new OrdenCo { idOrdE = qa.ore.idOrdenEnc, idRqiE = qa.rqe.idRequisicion, fecha = Convert.ToDateTime(qa.ore.fecha).ToShortDateString(), folioRq =  qa.rqe.Folio.Value, nCompr = "Wey", idePrv = qa.ore.idProveedor.Value });
                    //ocOrdC.Add(new OrdenCo{ idOrdE = qa.ore.idOrdenEnc, idRqiE = qa.ore.idReqEnc.Value, folioRq = qa.ore.folio.Value, fecha = Convert.ToDateTime(qa.ore.fecha).ToShortDateString(), nCompr = "Wey", idePrv = qa.ore.idProveedor.Value });
                    ocOrdC.Add(new OrdenCo { idOrdE = qa.ore.idOrdenEnc, idRqiE = qa.ore.idReqEnc.Value, folioRq = qa.ore.folio.Value, fechaRegOrd = Convert.ToDateTime(qa.ore.fecha).ToShortDateString(), nCompr = "Wey", persRegis = qa.em.Nombre + " " + qa.em.apMater + " " + qa.em.apPater });
                    ocRevisar.ItemsSource = ocOrdC;
                }
                else {
                    ocGasM.Add(new GastoMe { idOrdE = qa.ore.idOrdenEnc, idRqiE = qa.ore.idReqEnc.Value, folioRq = qa.ore.folio.Value, fechaRegOrd = Convert.ToDateTime(qa.ore.fecha).ToShortDateString(), nCompr = "Wey", persRegis = qa.em.Nombre + " " + qa.em.apMater + " " + qa.em.apPater });
                    gmRevisar.ItemsSource = ocGasM;
                }
            }
            
        }

        //-----------------------------------------------------------------------------------------SELECTIONCHANGED ORDEN COMPRA
        private void oc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("Paso Clear OC");
            OrdenCo oCmp = ocRevisar.SelectedItem as OrdenCo;
            if (oCmp != null)
            {
                char oordn = 'O';

                //if (oCmp != null) {

                //    var rowOrC = from oe in conex.OrdenEnc
                //                 from od in conex.OrdenDet
                //                 from re in conex.RequiEnc
                //                 from rd in conex.RequiDet
                //                 from p in conex.Producto
                //                 where oe.idOrdenEnc == od.idOrdenEnc &&
                //                 oe.idReqEnc == re.idRequisicion &&
                //                 re.idRequisicion == rd.idRequiEnc &&
                //                 oe.idOrdenEnc == oCmp.idOrdE &&
                //                 od.idProducto == p.idProducto &&
                //                 p.idProducto == rd.idProducto
                //                 select new { oe, od, re, rd, p };

                //    foreach(var q in rowOrC){
                //        listaProductosRequi.Add(new ProductosRequeridos { nombreProd = q.p.Nombre, cantidad = (float)q.od.Cantidad, precio = (float)q.od.Precio, total = q.od.Cantidad.Value*q.od.Precio.Value  });
                //    }
                //    dGproductRequi.ItemsSource = listaProductosRequi;
                //}
                gmRevisar.SelectedIndex = -1;
                loadProductsOcGm(oordn, oCmp);
            }
            else { return; }
        }
        //--------------------------------------------------------------------------------------------------------SELECTIONCHANGED GASTO MENOR
        private void dgGmSelecChang(object sender, SelectionChangedEventArgs e)
        {
            
            MessageBox.Show("Paso Clear GM");
            OrdenCo dgGas = gmRevisar.SelectedItem as OrdenCo;
            if (dgGas != null)
            {
                char gasM = 'G';
                ocRevisar.SelectedIndex = -1;
                loadProductsOcGm(gasM, dgGas);
            }
            else { return; }
        }
        //--------------------------------------------------------------------------------------------------LOAD PRODUCTS OF ORDEN AND GASTO
        public void loadProductsOcGm(char tip, OrdenCo coc){
            listaProductosRequi.Clear();
            if (coc != null)
            {
                var rowOrC = from oe in conex.OrdenEnc
                             from od in conex.OrdenDet
                             from re in conex.RequiEnc
                             from rd in conex.RequiDet
                             from p in conex.Producto
                             where oe.idOrdenEnc == od.idOrdenEnc &&
                             oe.idReqEnc == re.idRequisicion &&
                             re.idRequisicion == rd.idRequiEnc &&
                             oe.idOrdenEnc == coc.idOrdE &&
                             od.idProducto == p.idProducto &&
                             p.idProducto == rd.idProducto
                             select new { oe, od, re, rd, p };

                foreach (var q in rowOrC)
                {
                    listaProductosRequi.Add(new ProductosRequeridos { nombreProd = q.p.Nombre, cantidad = (float)q.od.Cantidad, precio = (float)q.od.Precio, total = q.od.Cantidad.Value * q.od.Precio.Value });
                    folioGmOc.Content = q.oe.idOrdenEnc;
                    folio.Content = q.oe.idReqEnc;
                    fechaElab.Content = q.re.fechaElaboracion;
                    fechRqda.Content = q.re.FechaRequerida;
                    dptoBenef.Text = "WTF";
                    //ocRevisar.SelectedIndex = -1;
                    //gmRevisar.SelectedIndex = -1;
                }
                dGproductRequi.ItemsSource = listaProductosRequi;
                //ocRevisar.SelectedIndex = -1;
                //gmRevisar.SelectedIndex = -1;
                //ocRevisar.SelectedItem = -1;
                //gmRevisar.SelectedItem = -1;
            }
            else { listaProductosRequi.Clear(); return; }

        }

//--------------------------------------------------------------------------------------------CLASES
    }

    public class OrdenCo {
        public int idOrdE { get; set; }
        public int idRqiE { get; set; }
        public int folioRq { get; set; }
        public string fechaRegOrd { get; set; }
        public string nCompr { get; set; }
        public string persRegis { get; set; }
    }

    public class GastoMe : OrdenCo { }

}
