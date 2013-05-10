using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SacIntegrado
{
	public partial class Autorizacion
	{
        string fechAutoriRP = DateTime.Today.ToShortDateString();//OBTENER LA FECHA
		public static String nomLog;
        int Id_empleado = 0;
        Db conex = new Db();
        ObservableCollection<RequiPorRevisar> oc = new ObservableCollection<RequiPorRevisar>();
        ObservableCollection<RequiPorRevisar> ocDos = new ObservableCollection<RequiPorRevisar>();
        ObservableCollection<DatosRequi> listDatosRequi = new ObservableCollection<DatosRequi>();
        ObservableCollection<ProductosRequeridos> listaProductosRequi = new ObservableCollection<ProductosRequeridos>();
        
		public Autorizacion()
		{
			this.InitializeComponent();
            oc.Clear();
            llenaRequiXRevisar();
            llenaRequiRevisada();
			// A partir de este punto se requiere la inserción de código para la creación del objeto.
		}
		
		public Autorizacion(String user){
			this.InitializeComponent();
           
			usuario.Content=user;
            oc.Clear();
            llenaRequiXRevisar();
            llenaRequiRevisada();
		}

        public Autorizacion(string user, int id,string nombre) {
            this.InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user,id,nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            Id_empleado = id;
            oc.Clear();
            llenaRequiXRevisar();
            llenaRequiRevisada();
        }

        private void cerrarSesion(object sender, System.Windows.Input.MouseButtonEventArgs e)
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

        //LENAR LISTA REQUISICIONES POR REVISAR
        private void llenaRequiXRevisar()
        {
            int ideResposab = 0;
            MessageBox.Show("ID Emp "+Id_empleado);
            int ideAdmin = (from mple in conex.Empleado where mple.Nombre == "Administrador" select mple.idEmpleado).SingleOrDefault();
            MessageBox.Show("ID admin "+ideAdmin);

            oc.Clear();
            var consultar = from re in conex.RequiEnc
                            from p in conex.Proyecto
                            from e in conex.Empleado
                            from d in conex.Departamento
                            where
                              re.StatusRP == 0 &&
                              re.idProyecto == p.idProyecto &&
                              e.idEmpleado == re.idEmpleado &&
                              d.idDepto == p.idDepto &&
                              (p.idResponsable == Id_empleado || Id_empleado == ideAdmin)
                              //e.idEmpleado == Id_empleado
                            select new
                            {
                                re.Folio,
                                re.fechaElaboracion,
                                re.FechaRequerida,
                                d.NombreDepto,
                                e.Nombre,
                                e.apPater,
                                e.apMater,
                                re.Motivo,
                                re.Observaciones,
                                p.idResponsable
                            };

            foreach (var e in consultar)
            {
                ideResposab = e.idResponsable.Value;
                string fecha_elalboracion = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                oc.Add(new RequiPorRevisar { folio = e.Folio.Value, fechaElab = fecha_elalboracion, fechaReq = fecha_requerida, nombreDepto = e.NombreDepto, nombreEmpleado = e.Nombre + " " + e.apPater + " " + e.apMater });
                reqXrevisar.ItemsSource = oc;
            }
            if (Id_empleado == ideResposab)
            {
                btnAutorizarR.IsEnabled = true;
            }
            else { } //btnAutorizarR.IsEnabled = false; }
        }
        //..............................................................................................

        private void llenaRequiRevisada()
        {

            var consultar = from re in conex.RequiEnc
                            from p in conex.Proyecto
                            from e in conex.Empleado
                            from d in conex.Departamento
                            where
                            re.StatusRP == 1 &&
                              re.idProyecto == p.idProyecto &&
                              e.idEmpleado == re.idEmpleado &&
                              d.idDepto == p.idDepto &&
                              p.idResponsable==Id_empleado//LA QUITE EN LA PRESENTACION DE LULA
                             // e.idEmpleado == Id_empleado

                            select new
                            {
                                re.Folio,
                                re.fechaElaboracion,
                                re.FechaRequerida,
                                d.NombreDepto,
                                e.Nombre,
                                e.apPater,
                                e.apMater
                            };
            foreach (var e in consultar)
            {
                string fecha_elaboracion = Convert.ToString(e.fechaElaboracion).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                //DateTime fecha_elaboracioon = e.fechaElaboracion;

                ocDos.Add(new RequiPorRevisar { folio = e.Folio.Value, fechaElab = fecha_elaboracion, fechaReq = fecha_requerida, nombreDepto = e.NombreDepto, nombreEmpleado = e.Nombre + " " + e.apPater + " " + e.apMater });
                reqRevis.ItemsSource = ocDos;
            }

        }

        //.......................................................

        //METODO PARA
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
                            nombre=e.Nombre,
                            d.NombreDepto
                        };

            foreach (var e in requi)
            {
                listaProductosRequi.Clear();
                string fecha_elaboracion = Convert.ToString(e.fechaElaboracion).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                listDatosRequi.Add(new DatosRequi { nombreProy = e.Nombre, folio = e.Folio.Value, fechaElab = fecha_elaboracion, fechaReq = fecha_requerida, nombreSolicitante = e.nombre, nombreDepto = e.NombreDepto, mot = e.Motivo, obs = e.Observaciones });
                proyectPresup.Text = e.Nombre;
                folio.Content = "" + e.Folio;
                fechaElab.Content = "" + fecha_elaboracion;
                fechaReq.Text = "" + fecha_requerida;
                solicitante.Text = "" + e.nombre;
                dptoBenef.Text = e.NombreDepto;
                txtMotiv.Text = e.Motivo;
                txtObserv.Text = e.Observaciones;
            }

        }

        //----------------------------------------------------------------------------------------------------

        int folioSelected = 0;
        private void reqPendiete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RequiPorRevisar rprev = reqXrevisar.SelectedItem as RequiPorRevisar;
            if (rprev == null)
            { //MessageBox.Show("Nulo"); 
                //reqXRev.ItemsSource = null;
                oc.Clear();
                listaProductosRequi.Clear();
                llenaRequiXRevisar();
                //MessageBox.Show("Usted NO tiene requisiciones pendientes");
            }
            else
            {
                btnAutorizarR.IsEnabled = true;
                var renglon = (from p in oc
                               where p.folio == rprev.folio
                               select p);
                foreach (var x in renglon)
                {
                    //MessageBox.Show("----: "+x.folio);
                    folioSelected = x.folio;
                }
                muestraTotal();
                muestraDatosRequi();
                llenaProductosdeRequi();
            }
        }

        //---------------------------------------------------------------------------------------------------------

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

        //EVENTO CLIC DEL BOTON AUTORIZAR
        private void btnAutorizar(object sender, RoutedEventArgs e)
        {
            //folioSelected
            //ConexionDataContext cn = new ConexionDataContext();
            if (folioSelected == 0)
            {
                MessageBox.Show("No tiene Requisiciones por Revisar");
            }
            else
            {
                var actualizar = (from a in conex.RequiEnc
                                  where a.Folio == folioSelected
                                  select a).Single();

                actualizar.StatusRP = 1;
                //PARA GUARDAR LAS FECHAS DE AUTORIZACION
                actualizar.fechAutoriRP = Convert.ToDateTime(fechAutoriRP);
                //HASTA AQUI LO DE LAS FECHAS DE AUTORIZACION
                conex.SubmitChanges();
                MessageBox.Show("Done");
                oc.Clear();// = null;// new ObservableCollection<RequiPorRevisar>();
                ocDos.Clear();// = null;// new ObservableCollection<RequiPorRevisar>();
                llenaRequiXRevisar();
                llenaRequiRevisada();
                //MessageBox.Show("Folio que aun tiene (debería): "+folioSelected);
                limpiar();
            }
        }

        //----------------------------------------------------------------------------------------------------------------------

        public void limpiar()
        {
          //  MessageBox.Show("Se puede Limpiar el Folio y la fecha???? WTF");
            proyectPresup.Text = "";
            folio.Content = "0000";
            fechaElab.Content = "00/00/0000";
            fechaReq.Text = "";
            solicitante.Text = "";
            dptoBenef.Text = "";
            dGproductRequi.ItemsSource = "";
            folioSelected = 0;
            txtMotiv.Text = "";
            txtObserv.Text = "";
            btnAutorizarR.IsEnabled = false;
            //oc.Clear();
            //llenaRequiXRevisar();
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
            //MessageBox.Show("Folio Seleccionado: " + folioSelected);
            //MessageBox.Show("Total: "+ttl);
        }

	}
}