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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace SacIntegrado
{
    /// <summary>
    /// Lógica de interacción para VoBoJC.xaml
    /// </summary>
    public partial class VoBoJC : Page
    {
        string fechAutoriJA = DateTime.Today.ToShortDateString();
        int Id_empleado = 0;
        Db conex = new Db();
        ObservableCollection<RequiPorRevisar> oc = new ObservableCollection<RequiPorRevisar>();
        ObservableCollection<RequiPorRevisar> ocDos = new ObservableCollection<RequiPorRevisar>();
        ObservableCollection<DatosRequi> listDatosRequi = new ObservableCollection<DatosRequi>();
        ObservableCollection<ProductosRequeridos> listaProductosRequi = new ObservableCollection<ProductosRequeridos>();
        //EDER
        ObservableCollection<EmpleadoClass> ocComboCompr = new ObservableCollection<EmpleadoClass>();
        ObservableCollection<EmpleadoClass> ocEmpleado = new ObservableCollection<EmpleadoClass>();//REVISAR SI LA USO  O NO
        int id;

        public VoBoJC()
        {
          
        }

        public VoBoJC(string user, int id,string nombre)
        {
            //llenarComboAsig();
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user,id,nombre);
            cmbComprAsignar.SelectedIndex = 0;//CARGAR POR DEFAULT EL COMPRADOR QUE SE ENCUENTRE EN LA POSICION 0
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            Id_empleado = id;
            oc.Clear();
            llenaRequiXRevisar();
            llenaRequiRevisada();
            //llenarCombo();
            llenacmbCompaAsignar();
            llenaComboEmple();
            expandDgRequis.IsExpanded = true;
            expandAutori.Visibility = Visibility.Collapsed;
            expandAddDelComp.Visibility = Visibility.Collapsed;
            fechaReq.Text = DateTime.Today.ToShortDateString();//ESTABLECER LA FECHA ACTUAL EN EL DATEPICKER
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

        //-------------------------------------------------------------------------------------------------------

        //LENAR LISTA REQUISICIONES POR REVISAR
        private void llenaRequiXRevisar()
        {
            
            oc.Clear();
            var consultar = from re in conex.RequiEnc
                            from p in conex.Proyecto
                            from e in conex.Empleado
                            from d in conex.Departamento
                            where
                              re.StatusRP == 1 &&
                              re.StatusJA == 0 &&
                              re.idProyecto == p.idProyecto &&
                              e.idEmpleado == re.idEmpleado &&
                              d.idDepto == p.idDepto &&
                              e.idEmpleado == Id_empleado
                              //Id_empleado==1//YO TENIA ESTA SIN COMENTAR Y EDER LA DE ARRIBA
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
                                re.Observaciones
                            };

            foreach (var e in consultar)
            {
                string fecha_elalboracion = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                oc.Add(new RequiPorRevisar { folio = e.Folio.Value, fechaElab = fecha_elalboracion, fechaReq = fecha_requerida, nombreDepto = e.NombreDepto, nombreEmpleado = e.Nombre + " " + e.apPater + " " + e.apMater });
                reqXrevisar.ItemsSource = oc;

            }

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
                             re.StatusJA == 1 &&
                              re.idProyecto == p.idProyecto &&
                              e.idEmpleado == re.idEmpleado &&
                              d.idDepto == p.idDepto &&
                             e.idEmpleado == Id_empleado
                             //  Id_empleado==1//ESTA LA TENIA SIN COMENTAR Y EDER COMENTADA Y SIN COMENTAR LA DE ARRIBA
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
            //llenarComboAsig();//ESTA LINEA YO LA TENIA SIN COMENTAR Y EDER LA TENIA COMENTADA
            //llenacmbCompaAsignar();
            //ocComboCompr.Clear();
            //llenarComboAsig();
            //MessageBox.Show("FOLIO SELECTED: OTRO METODO: " + folioSelected);

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
                cmbComprAsignar.IsEnabled = true;
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
            }
            dGproductRequi.ItemsSource = listaProductosRequi;
        }

        private void btnVoBoAsigna(object sender, RoutedEventArgs e)
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

                actualizar.StatusJA = 1;
                actualizar.idComprador = id;
                //PARA LA FECHA EN Q SE AUTORIZA
                actualizar.fechAutoriJA = Convert.ToDateTime(fechAutoriJA);
                //HASTA AQUI LO DE LA FECHA
                conex.SubmitChanges();
                oc.Clear();// = null;// new ObservableCollection<RequiPorRevisar>();
                ocDos.Clear();// = null;// new ObservableCollection<RequiPorRevisar>();
                llenaRequiXRevisar();
                llenaRequiRevisada();
                //MessageBox.Show("Folio que aun tiene (debería): "+folioSelected);
                limpiar();
                llenacmbCompaAsignar();
                expandAddDelComp.IsExpanded = false;

            }
        }

        //----------------------------------------------------------------------------------------------------------------------

        public void limpiar()
        {
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
            cmbComprAsignar.Text = "";
            cmbComprAsignar.IsEnabled = false;
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

        //------------------------------------------------------------------------------------------------------------------------
        //METODO PARA LLENAR EL COMBO DE COMPRADOR PARA Q ASIGNE ALGUNO
        public void llenarComboAsig()
        {
            //BDDataContext db = new BDDataContext();
            //cmbComprAsignar.Items.Clear();
            ocComboCompr.Clear();
            var query = from emp in conex.Empleado
                        from com in conex.Comprador
                        where emp.idEmpleado == com.idEmpleado
                        select new { emp, com.idComprador };
            foreach (var x in query)
            {
                ocComboCompr.Add(new EmpleadoClass { idEmpleado = x.emp.idEmpleado, numEmp = x.emp.numEmp.Value, nombre = x.emp.Nombre, apPater = x.emp.apPater, apMater = x.emp.apMater, idGpo = x.emp.idGpo.Value, password = x.emp.Password, usuario = x.emp.Usuario });
                cmbComprAsignar.ItemsSource = ocComboCompr;
                //cmbComprAsignar.ItemsSource = query;//ESTA LA TENIA PERO NO MOSTRABA LOS NOMBRES EN EL COMBO DONDE SE ASIGNA COMPRADOR
                cmbComprAsignar.DisplayMemberPath = "nombre";
                cmbComprAsignar.SelectedValuePath = "idComprador";

            }
        }

        //-------------------------------------------------------------------------------------------
        //PARA LLENAR EL COMBO DE EMPLEADO Y ADD UN NUEVO COMPRADOR
        public void llenaComboEmple()
        {

            var query = from emp in conex.Empleado
                        select emp;
            foreach (var x in query)
            {
                ocEmpleado.Add(new EmpleadoClass { idEmpleado = x.idEmpleado, numEmp = x.numEmp.Value, nombre = x.Nombre, apPater = x.apPater, apMater = x.apMater, idGpo = x.idGpo.Value, password = x.Password, usuario = x.Usuario });
                comboEmpleado.ItemsSource = ocEmpleado;
                comboEmpleado.DisplayMemberPath = "nombre";
                comboEmpleado.SelectedValuePath = "idEmpleado";
            }
        }
        //-------------------------------------------------------------------------------------------
        /*
        public void llenarCombo()
        {
            var query = from emp in conex.Empleado
                        from com in conex.Comprador
                        where emp.idEmpleado == com.idEmpleado
                        select emp;
            foreach (var x in query)
            {
                ocComboCompr.Add(new EmpleadoClass { idEmpleado = x.idEmpleado, numEmp = x.numEmp.Value, nombre = x.Nombre, apPater = x.apPater, apMater = x.apMater, idGpo = x.idGpo.Value, password = x.Password, usuario = x.Usuario });
                comboComprador.ItemsSource = ocComboCompr;
                comboComprador.DisplayMemberPath = "nombre";
                comboComprador.SelectedValuePath = "idEmpleado";

            }


        }*/


        public void llenacmbCompaAsignar()
        {
            /*var cuery = from emp in conex.Empleado
                        from com in conex.Puesto
                        where emp.idPuesto == com.idPuesto &&
                        com.nombrePuesto == "Comprador"
                        select emp;*/
            var cuery = from emp in conex.Empleado
                        from com in conex.Comprador
                        where emp.idEmpleado == com.idEmpleado
                        select new { emp, com };
            foreach (var x in cuery)
            {
                ocComboCompr.Add(new EmpleadoClass { idEmpleado = x.emp.idEmpleado, numEmp = x.emp.numEmp.Value, nombre = x.emp.Nombre, apPater = x.emp.apPater, apMater = x.emp.apMater, idGpo = x.emp.idGpo.Value, password = x.emp.Password, usuario = x.emp.Usuario, idPuesto = x.emp.idPuesto.Value });
                //MessageBox.Show("Nombre del Emp"+x.Nombre);
                //cmbComprAsignar.ItemsSource = ocComboCompr;
                cmbComprAsignar.ItemsSource = ocComboCompr/*cuery*/;
                comboComprador.ItemsSource = ocComboCompr;
                cmbComprAsignar.DisplayMemberPath = "nombre";
                cmbComprAsignar.SelectedValuePath = "idEmpleado";
                comboComprador.DisplayMemberPath = "nombre";
                comboComprador.SelectedValue = "idEmpleado";
            }
        }


        //EVENTO BOTON PARA ADD NUEVO COMPRADOR A TAB COMPRADOR
        private void btnAddCompr(object sender, RoutedEventArgs e)
        {
            EmpleadoClass emp = (EmpleadoClass)comboEmpleado.SelectedItem;
            int id = emp.idEmpleado;

            int idEmple = 0;
            var query = from x in conex.Comprador
                        where x.idEmpleado == id
                        select x;
            foreach (var s in query)
            {
                idEmple = s.idEmpleado;
            }
            MessageBox.Show("idEmple" + idEmple);
            if (emp.idEmpleado == idEmple)
            {
                comboEmpleado.Text = "";
                MessageBox.Show("El Empleado ya es comprador");
            }
            else
            {
                System.Data.Linq.Table<Comprador> comprador = conex.GetTable<Comprador>();
                Comprador cmp = new Comprador();
                cmp.idComprador = 0;
                cmp.idEmpleado = id;
                comprador.InsertOnSubmit(cmp);
                comprador.Context.SubmitChanges();
                //llenarCombo();
                
                MessageBox.Show("El registro se agregó correctamente");
                ocComboCompr.Clear();
                ocEmpleado.Clear();
                llenacmbCompaAsignar();
                //llenarCombo();
                llenaComboEmple();
            }
        }

        private void btnEliminarComp(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("¿Seguro que desea eliminar el registro?", "Atención", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //BDDataContext db = new BDDataContext();
                    EmpleadoClass cmp = (EmpleadoClass)comboComprador.SelectedItem;
                    int x = cmp.idEmpleado;
                    MessageBox.Show("Entras a eliminar" + x);

                    Comprador elim = (from p in conex.Comprador
                                      where p.idEmpleado == x
                                      select p).Single();//Devuelve el unicoelemento de una secuencia selecciona solo una cosa y sin Single marca error en DeleteOnSudmit
                    conex.Comprador.DeleteOnSubmit(elim);
                    conex.SubmitChanges();
                    MessageBox.Show("El registro se eliminó correctamente.");
                    ocComboCompr.Clear();
                    //llenarCombo();
                    llenacmbCompaAsignar();

                }
                else
                {
                    MessageBox.Show("No se eliminó correctamente.");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("¿Seleccionó el producto a eliminar?");
                return;
            }
        }

        private void expandedDgRequis(object sender, RoutedEventArgs e)
        {
                expandAddDelComp.IsExpanded = false;
                expandAddDelComp.Visibility = Visibility.Collapsed;
                expandAutori.IsExpanded = false;
                expandAutori.Visibility = Visibility.Collapsed;
        }

        private void expandedAddDel(object sender, RoutedEventArgs e)
        {
                expandDgRequis.IsExpanded = false;
                //expandDgRequis.Visibility = Visibility.Collapsed;
                expandAutori.IsExpanded = false;
                //expandAutori.Visibility = Visibility.Visible;
        }

        private void expandedAutor(object sender, RoutedEventArgs e)
        {
            //if (expandAutori.IsExpanded == true)
            //{
                expandAddDelComp.IsExpanded = false;
                expandAddDelComp.Visibility = Visibility.Collapsed;
                expandDgRequis.IsExpanded = false;
                //expandDgRequis.Visibility = Visibility.Collapsed;
            //}
            //expandAddDelComp.Visibility = Visibility.Visible;
            //expandDgRequis.Visibility = Visibility.Visible;
        }



        private void cmbComprAsignar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmbComprAsignar.SelectedValue == null)
                {
                    return;
                }
                else
                {
                    id = (int)cmbComprAsignar.SelectedValue;//CON SOLO ESTA LINEA NO OBTIENE EL ID Y OBTIENE NULL
                    /*EmpleadoClass ec = cmbComprAsignar.SelectedItem as EmpleadoClass;
                    id = ec.idComprador;*/
                    btnVoBoReq.IsEnabled = true;
                }
            }catch(Exception){}
        }

        private void collaDgRequis(object sender, RoutedEventArgs e)
        {
            expandAddDelComp.IsExpanded = false;
            expandAddDelComp.Visibility = Visibility.Visible;
            expandAutori.IsExpanded = false;
            expandAutori.Visibility = Visibility.Visible;
            expandDgRequis.IsExpanded = false;
            expandDgRequis.Visibility = Visibility.Visible;
        }

        private void collAutori(object sender, RoutedEventArgs e)
        {
            expandAddDelComp.IsExpanded = false;
            expandAddDelComp.Visibility = Visibility.Visible;
            expandAutori.IsExpanded = false;
            expandAutori.Visibility = Visibility.Visible;
            expandDgRequis.IsExpanded = false;
            expandDgRequis.Visibility = Visibility.Visible;
        }

        private void collapAddCom(object sender, RoutedEventArgs e)
        {
            expandAddDelComp.IsExpanded = false;
            expandAddDelComp.Visibility = Visibility.Visible;
            expandAutori.IsExpanded = false;
            expandDgRequis.IsExpanded = false;
        }
        //-------------------------------------------------------------------------------------------

    }
}
