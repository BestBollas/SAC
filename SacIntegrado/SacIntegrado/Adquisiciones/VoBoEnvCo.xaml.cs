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
//using System.Windows.Navigation;

namespace SacIntegrado
{
    /// <summary>
    /// Lógica de interacción para VoBoEnvCo.xaml
    /// </summary>
    /// 
    class ordenCompra{
    public int idRen {get; set;}
    public String fecha {get;set;}
    public String  nomComp{get;set;}
    public Boolean comite { get; set;}
    public int folio { get; set; }
    public bool isCh;
    public bool isch{
        get { return isCh; }
        set { isCh = value; }
    }
    public bool isCh2;
    public bool isch2
    {
        get { return isCh2; }
        set { isCh2 = value; }
    }
  }

    public partial class VoBoEnvCo : Page
    {

        int Id_empleado = 0;
        int folioSelected = 0;
        int xo = 0;
        string fechAutoriComite = DateTime.Today.ToShortDateString();
        ObservableCollection<ProductosRequeridos> listaProductosRequi = new ObservableCollection<ProductosRequeridos>();
        ObservableCollection<DatosRequi> listDatosRequi = new ObservableCollection<DatosRequi>();
        ObservableCollection<ordenCompra> ocOC = new ObservableCollection<ordenCompra>();
        ObservableCollection<ordenCompra> ocGM = new ObservableCollection<ordenCompra>();
        Db conex = new Db();

        public VoBoEnvCo(string user, int id, string nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            Id_empleado = id;
            llenarOC();
            llenarGM();
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
        public void llenarGM()
        {
            ocOC.Clear();
            //char num='1';
            var query = from ren in conex.RequiEnc
                        from cen in conex.CotizacionEnc
                        from cde in conex.CotizacionDet
                        from rde in conex.RequiDet
                        from com in conex.Empleado
                        where
                              ren.StatusRP == 1 &&
                              ren.StatusJA == 1 &&
                              ren.StatusJAC == 0 &&
                              ren.StatusCot == '1' &&
                              ren.StatusPresup == 1 &&
                              ren.StatusCo == 0 &&
                              cen.idCotizacionEnc==cde.idCotizacionEnc &&
                              cde.idRequiDet==rde.idRequiDet &&
                              rde.idRequiEnc==ren.idRequisicion &&
                              ren.idComprador==com.idEmpleado 
                        select new
                        {
                            cen.idCotizacionEnc,
                            cen.fecha,
                            com.Nombre,
                            ren.Folio
                        };

            foreach (var q in query)
            {
                ocOC.Add(new ordenCompra { idRen = q.idCotizacionEnc, fecha = Convert.ToDateTime(q.fecha).ToShortDateString(), nomComp = q.Nombre, folio=q.Folio.Value });
            }
            gmRevisar.ItemsSource = ocOC;

        }

        public void llenarOC() {
            var query = from ren in conex.RequiEnc
                        from cen in conex.CotizacionEnc
                        from cde in conex.CotizacionDet
                        from rde in conex.RequiDet
                        from com in conex.Empleado
                        where
                              ren.StatusRP == 1 &&
                              ren.StatusJA == 1 &&
                              ren.StatusJAC == 0 &&
                              ren.StatusCot == '1' &&
                              ren.StatusPresup == 1 &&
                              ren.StatusCo == 0 &&
                              cen.idCotizacionEnc == cde.idCotizacionEnc &&
                              cde.idRequiDet == rde.idRequiDet &&
                              rde.idRequiEnc == ren.idRequisicion &&
                              ren.idComprador == com.idEmpleado 
                        select new
                        {                            
                            cen.idCotizacionEnc,
                            cen.fecha,
                            com.Nombre,
                            ren.Folio
                        };

            foreach (var q in query)
            {
                ocOC.Add(new ordenCompra { idRen = q.idCotizacionEnc, fecha = Convert.ToDateTime(q.fecha).ToShortDateString(), nomComp = q.Nombre, folio = q.Folio.Value });
            }
            ocRevisar.ItemsSource =ocOC;
        }

        private void habilitarObserv(object sender, RoutedEventArgs e)
        {
            txtObserv.IsEnabled = true;
        }

        private void oc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ordenCompra ocRevi = ocRevisar.SelectedItem as ordenCompra;
            var renglon = (from p in ocOC
                           where p.idRen == ocRevi.idRen
                           select p);
            foreach (var x in renglon)
            {
                //MessageBox.Show("----: "+x.folio);
                folioSelected = x.folio;
            }
            //MessageBox.Show("hola " + folioSelected);
            muestraTotal();
            muestraDatosRequi();
            llenaProductosdeRequi();
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
        /////////////////////////////////////////////////////////////
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
                            p.Nombre,
                            re.fechaElaboracion,
                            re.FechaRequerida,
                            nEmp = e.Nombre,
                            d.NombreDepto
                        };

            foreach (var e in requi)
            {

                listaProductosRequi.Clear();
                string fecha_elaboracion = Convert.ToString(e.fechaElaboracion).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                listDatosRequi.Add(new DatosRequi { nombreProy = e.Nombre, folio = e.Folio.Value/*e.Folio.Value*/, fechaElab = fecha_elaboracion, fechaReq = fecha_requerida, nombreSolicitante = e.nEmp, nombreDepto = e.NombreDepto, mot = e.Motivo, obs = e.Observaciones });
                proyectPresup.Text = e.Nombre;
                folio.Content = "" + e.Folio;
                fechaElab.Content = "" + fecha_elaboracion;
                fechaReq.Text = "" + fecha_requerida;
                solicitante.Text = "" + e.nEmp;
                dptoBenef.Text = e.NombreDepto;
                txtMotiv.Text = e.Motivo;
                txtObserv.Text = e.Observaciones;
            }

            //MessageBox.Show("FOLIO SELECTED: OTRO METODO: " + folioSelected);

        }
        private void llenaProductosdeRequi()
        {
            //MessageBox.Show("FOLIO SELECTED: OTRO METODO: " + folioSelected);
            //ConexionDataContext c = new ConexionDataContext();

            var prod = from p in conex.Producto
                       from re in conex.RequiEnc
                       from rd in conex.RequiDet
                       where re.Folio == folioSelected &&
                       re.idRequisicion == rd.idRequiEnc &&
                       p.idProducto == rd.idProducto

                       select new
                       {
                           p.Nombre,
                           rd.cantidad,
                           rd.precio,
                           rd.total,
                           re.Observaciones,
                           re.Motivo
                       };

            foreach (var reg in prod)
            {
                listaProductosRequi.Add(new ProductosRequeridos { nombreProd = reg.Nombre, cantidad = reg.cantidad.Value, precio = reg.precio.Value, total = reg.total.Value });
                //productos_requi.ItemsSource = reg.Nombre.ToString();
                txtMotiv.Text = reg.Motivo;
                txtObserv.Text = reg.Observaciones;
            }
            dGproductRequi.ItemsSource = listaProductosRequi;
        }

        private void btnAutorizar(object sender, RoutedEventArgs e)
        {

            if (ocRevisar.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar un registro");
            }
            else
            {
                var actualizar = (from a in conex.RequiEnc
                                  where a.Folio == folioSelected
                                  select a).Single();

                actualizar.StatusCo = 1;
                actualizar.fechAutoriPresup = Convert.ToDateTime(fechAutoriComite);
                conex.SubmitChanges();
                ocOC.Clear();
                llenarOC();
                llenarGM();
                MessageBox.Show("Se Autorizo Correctamente");
                VoBoPresu vobo = new VoBoPresu();
                vobo.limpiar();
            }
        }

        public void checar(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Checado OC");

            if (this.ocRevisar.SelectedItem is ordenCompra)
            {
                ordenCompra dpp = ocRevisar.SelectedItem as ordenCompra;
                MessageBox.Show("Checbox está: " + dpp.isCh);
                xo = dpp.idRen;
                MessageBox.Show("Folio " + dpp.folio);
                MessageBox.Show("ID Prov " + xo);
            }

        }

        public void checar2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Checado GM");

            if (this.gmRevisar.SelectedItem is ordenCompra)
            {
                ordenCompra dpp = gmRevisar.SelectedItem as ordenCompra;
                MessageBox.Show("Checbox está: " + dpp.isCh2);
                xo = dpp.idRen;
                MessageBox.Show("Folio " + dpp.folio);
                MessageBox.Show("ID Prov " + xo);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string nombresss=usuario.Content.ToString();
            MessageBox.Show("Nombre: "+nombresss);
            //Reportes.Report1 reportes = new Reportes.Report1();
            //NavigationService.Navigate(reportes.DesignMode);
            //VoBoPresu vobopresu = new VoBoPresu(usuario, Id_empleado, nombresss);
            //pagina.NavigationService.Navigate(vobopresu);
            
        }

        private void openReportCom(object sender, RoutedEventArgs e)
        {
            ReportComite rcom = new ReportComite();
            NavigationService.Navigate(rcom);
        }
//-------------------------------------------------------------------------
    }
}
