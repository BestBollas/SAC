using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Net;
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

using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.IO;

using System.Net.Mail;
using System.ComponentModel;

using SacIntegrado.OficiosComision;


namespace SacIntegrado.OficiosComision
{
    /// <summary>
    /// Lógica de interacción para AsignaPresupOficioComi.xaml
    /// </summary>
    /// 
    public class concepto{
        public int idconcepto { get; set; }
        public String nombreCon { get; set; }
        public int clasificador { get; set; }
        public double monto { get; set; }

    }
    public partial class AsignaPresupOficioComi : Page
    {
        private ObservableCollection<ClaseCta> MisCuentas = new ObservableCollection<ClaseCta>();
        ObservableCollection<Empleado> listaEmpleado = new ObservableCollection<Empleado>();
        ObservableCollection<Proyecto> listwProyectos = new ObservableCollection<Proyecto>();
        ObservableCollection<EstadoClass> listwEstado = new ObservableCollection<EstadoClass>();
        ObservableCollection<OficiosdeComisionClass> ocOficios = new ObservableCollection<OficiosdeComisionClass>();
        ObservableCollection<OficiosdeComisionClass> ocOficiosxPagar = new ObservableCollection<OficiosdeComisionClass>();
        ObservableCollection<concepto> ocConcepto = new ObservableCollection<concepto>();
        ObservableCollection<ViaticoClase> col = new ObservableCollection<ViaticoClase>();//ObservableCollection<InfoReqDet> col;
        Db miBD = new Db();
        String nomUsua;
        int usuario = 0;
        int areaid = 0;
        int idres = 0;
        int deptoid = 0;
        int partida = 0;
        int idOfiComi = 0;
        int idClasi = 0;
        int idClasiPago = 0;
        double totalAsig=0;
        string fechaElabora = DateTime.Today.ToShortDateString();
        public AsignaPresupOficioComi(String user, int id, String nombre)
        {
            InitializeComponent();
            fechaElab.Content = fechaElabora;
            nomUsua = nombre;
            usuario = id;
            llenaCombo();
            llenaDtgOficios();
            llenadtgOfixPagar();
            lblusuario.Content = nomUsua;
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            llenaConcepto();
            menuPrincipal.Items.Add(temp.MiMenu);
            lwEmpleados.Visibility = Visibility.Hidden;
            lwProyecto.Visibility = Visibility.Hidden;
            lwEstados.Visibility = Visibility.Hidden;
            
            lwclasifi.Visibility = Visibility.Hidden;
            Viaticos.IsOpen = false;
            expOfiPendientes.IsExpanded = false;
            expOfixPagar.IsExpanded = false;
            //------------------leer pdf
      
            //--------------------------
        }





        private void cerrarSesion(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Cerrar POPUP");
            try
            {
                
                //----------------------------------------------------------------------------------------------------------
                //Descargar del Servidor------------------------------------------------------------------------------------
                //System.Net.WebClient client2 = new System.Net.WebClient();
                ////string savePath = @"C:\Users\Best\Downloads\" + fileName;
                //string savePath = @"172.16.9.146\Users\Best\Downloads\" + fileName;
                //MessageBox.Show("Downloading from: " + addy);
                //byte[] result = client2.DownloadData(addy);
                //System.IO.File.WriteAllBytes(savePath, result);
                //MessageBox.Show("Download is done! It has been saved to: " + savePath);             
                //Lee los archivos -----------------------------------------------------------------------------------------
                //System.Diagnostics.Process proc = new System.Diagnostics.Process();
                //proc.StartInfo.FileName = @"C:\UploadDocs\" + fileName;
                //proc.Start();
                //proc.Close();
                //----------------------------------------------------------------------------------------------------------


            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message);
            }
        }

        private void empleados(object sender, KeyEventArgs e)
        {
            lwEmpleados.Visibility = Visibility.Visible;
            String emple = solicitante.Text.Trim();
            var ctas = from ele in miBD.Empleado
                       where ele.Nombre.Contains(emple)
                       select ele;
            listaEmpleado.Clear();
            foreach (var el in ctas)
            {
                listaEmpleado.Add(new Empleado { idEmpleado = el.idEmpleado, Nombre = el.Nombre });
                lwEmpleados.ItemsSource = listaEmpleado;
                lwEmpleados.DisplayMemberPath = "Nombre";
                lwEmpleados.SelectedValuePath = "idEmpleado";
            }
        }
        private void lwEmpleados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lwEmpleados.SelectedValue == null) return;
            Empleado emp = lwEmpleados.SelectedItem as Empleado;
            solicitante.Text = emp.Nombre;
            lwEmpleados.Visibility = Visibility.Hidden;
        }

        private void proyecto(object sender, KeyEventArgs e)
        {
            lwProyecto.Visibility = Visibility.Visible;
            String emple = proyectPresup.Text.Trim();
            var ctas = from ele in miBD.Proyecto
                       where ele.Nombre.Contains(emple)
                       select ele;
            listwProyectos.Clear();
            foreach (var el in ctas)
            {
                listwProyectos.Add(new Proyecto { idProyecto = el.idProyecto, Nombre = el.Nombre, idArea = el.idArea, idResponsable = el.idResponsable, idDepto = el.idDepto });
                lwProyecto.ItemsSource = listwProyectos;
                lwProyecto.DisplayMemberPath = "Nombre";
                lwProyecto.SelectedValuePath = "idProyecto";
            }
        }

        private void lwProyecto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lwProyecto.SelectedValue == null) return;
            Proyecto proy = lwProyecto.SelectedItem as Proyecto;
            proyectPresup.Text = proy.Nombre;
            areaid = proy.idArea.Value;
            idres = proy.idResponsable.Value;
            deptoid = proy.idDepto.Value;
            //MessageBox.Show("algo" + proy.idDepto.Value);
            deptoBene(proy.idDepto.Value);
            lwProyecto.Visibility = Visibility.Hidden;
        }
        public void deptoBene(int iddepto)
        {
            var query = from x in miBD.Departamento
                        where x.idDepto == iddepto
                        select x;
            foreach (var d in query)
            {
                dptoBenef.Text = d.NombreDepto;
            }
        }

        private void estado(object sender, KeyEventArgs e)
        {
            String[] estado = new String[] { "Aguascalientes", "Baja California", "Baja California Sur", "Campeche", "Chiapas", "Chihuahua", "Coahuila", "Colima", "Durango", "Guanajuato", "Guerrero", "Hidalgo", "Jalisco", "Estado de México", "Michoacán", "Morelos", "Nayarit", "Nuevo León", "Oaxaca", "Puebla", "Querétaro", "Quintana Roo", "San Luis Potosí", "Sinaloa", "Sonora", "Tabasco", "Tamaulipas", "Tlascala", "Veracruz", "Yucatán", "Zacatecas", "Distrito Federal" };
            lwEstados.Visibility = Visibility.Visible;
            var query = from x in estado
                        where x.Contains(txtEstado.Text)
                        select x;
            listwEstado.Clear();
            foreach (var h in query)
            {
                listwEstado.Add(new EstadoClass { Nombre = h });
                lwEstados.ItemsSource = listwEstado;
                lwEstados.DisplayMemberPath = "Nombre";
                lwEstados.SelectedValuePath = "Nombre";
            }
        }

        private void lwEstados_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lwEstados.SelectedValue == null) return;
            EstadoClass estado = lwEstados.SelectedItem as EstadoClass;
            txtEstado.Text = estado.Nombre;
            lwEstados.Visibility = Visibility.Hidden;
        }
        public void llenaCombo()
        {
            String[] tipo = { "Nacional", "Internacional" };
            var query = from tp in tipo
                        select tp;
            foreach (var o in query)
            {
                cmbTipo.Items.Add(new tipoComisionClass { Nombre = o });
                cmbTipo.DisplayMemberPath = "Nombre";
                cmbTipo.SelectedValuePath = "Nombre";
            }
            String[] transporte = new String[] { "Propio", "Autobus UTSJR", "Autobus Publico", "Autobus Rendato", "Autobus que proporciona la Empresa" };
            var cuery = from trans in transporte
                        select trans;
            foreach (var t in cuery)
            {
                cmbTransporte.Items.Add(new TrasnporteClass { Nombre = t });
                cmbTransporte.DisplayMemberPath = "Nombre";
                cmbTransporte.SelectedValuePath = "Nombre";
            }

        }

        private void grabarOficio(object sender, RoutedEventArgs e)
        {
            String fechaInicio = fechInicio.Text;
            String fechaTermino = fechTermino.Text;
            String hrComiInici = txthrInicio.Text;
            String hrComiFin = txthrFin.Text;
            String hrEventInici = txthrInicioEvent.Text;
            String hrEventFin = txthrFinEvent.Text;


            Table<OficiosdeComision> ofiComi = miBD.GetTable<OficiosdeComision>();
            OficiosdeComision tbofiComi = new OficiosdeComision();
            tbofiComi.idOficiosComi = 0;
            tbofiComi.NoOficios = 2;
            tbofiComi.fechaReg = Convert.ToDateTime(fechaElabora);
            tbofiComi.personaReg = usuario;
            tbofiComi.personaComisionada = int.Parse(lwEmpleados.SelectedValue.ToString());
            tbofiComi.jefeInmediato = null;
            tbofiComi.jefeArea = null;
            tbofiComi.responsableProy =null;
            tbofiComi.autoPresup = null;
            tbofiComi.deptoBene = deptoid;
            tbofiComi.fechaInicio = Convert.ToDateTime(fechaElabora);
            tbofiComi.fechaTermino = Convert.ToDateTime(fechaElabora);
            tbofiComi.empresa = txtEmpresa.Text;
            tbofiComi.estado = txtEstado.Text;
            tbofiComi.municipio = "SJN";
            tbofiComi.hrComision = hrComiInici + " " + " a: " + hrComiFin;
            tbofiComi.hrEvento = hrEventInici + " " + " a: " + hrEventFin;
            tbofiComi.objetivos = txtObje.Text;
            tbofiComi.acompaniante = txtacompa.Text;
            tbofiComi.transporte = cmbTransporte.Text;
            tbofiComi.tipoComision = cmbTipo.Text;
            tbofiComi.observaciones = txtObserva.Text;
            tbofiComi.idProyecto =int.Parse(lwProyecto.SelectedValue.ToString());
            ofiComi.InsertOnSubmit(tbofiComi);
            ofiComi.Context.SubmitChanges();
            MessageBox.Show("En conformidad a lo dispuesto en la normatividad para administración del presupuesto y en caso de que mi comision genero viaticos 'Me Comprometo' a efectuar la comprobación de un plazo no mayor a 15 días después  de haber concluido el periodo de la comision, Misma que debera reunir los requisitos fiscales, en caso contrario, se turnara al departamento de Recursos Humanos para su descuento via nomina.");
            MessageBox.Show("Te Recomendamos Proporcionar los datos Bancarios");
            ocOficios.Clear();
            llenaDtgOficios();
            limpiar();
        }

        public void llenadtgOfixPagar() {
            
            String statuAutoriJefeInme = "";
            String statuAutoriJefeArea = "";
            String statuAutoriRespProy = "";
            String statuAutoriPresu = "";

            var query = from x in miBD.OficiosdeComision
                        from emp in miBD.Empleado
                        from pro in miBD.Proyecto
                        from dpto in miBD.Departamento
                        from a in miBD.Area
                        where
                        x.autoPresup!=null &&
                        x.jefeInmediato!=null &&
                        a.idArea == dpto.idArea &&
                       a.idArea == pro.idArea &&
                       dpto.idDepto == pro.idDepto &&
                       dpto.idDepto == x.deptoBene &&
                       pro.idProyecto == x.idProyecto &&
                       x.personaComisionada == emp.idEmpleado
                        select new { x, emp, pro, dpto, a };
            ocOficiosxPagar.Clear();
           
            foreach (var i in query)
            {
                statuAutoriJefeInme = Convert.ToString(i.x.jefeInmediato);
                statuAutoriJefeArea = Convert.ToString(i.x.jefeArea);
                statuAutoriRespProy = Convert.ToString(i.x.responsableProy);
                statuAutoriPresu = Convert.ToString(i.x.autoPresup);
                var rp = (from r in miBD.Empleado
                          where r.idEmpleado == i.pro.idResponsable
                          select r.Nombre).SingleOrDefault();
                if (statuAutoriPresu.Equals(""))
                {
                    statuAutoriPresu = "NO AUTORIZADO";
                }
                if (statuAutoriJefeInme.Equals(""))
                {
                    statuAutoriJefeInme = "NO AUTORIZADO";
                }
                if (statuAutoriJefeArea.Equals(""))
                {
                    statuAutoriJefeArea = "NO AUTORIZADO";
                }
                if (statuAutoriRespProy.Equals(""))
                {
                    statuAutoriRespProy = "NO AUTORIZADO";
                }
                ocOficiosxPagar.Add(new OficiosdeComisionClass { idOficios = i.x.idOficiosComi, NoOficios = i.x.NoOficios.Value, fechaReg = i.x.fechaReg.Value, personaReg = i.emp.Nombre, personaComisionada = i.emp.Nombre, jefeInme = i.emp.Nombre, idjefeInmediato = statuAutoriJefeInme, jefeAre = i.emp.Nombre, idjefeArea = statuAutoriJefeArea, responsable = rp.ToString(), idresponsableProy = statuAutoriRespProy, autoPresu = i.emp.Nombre, idautoPresup = statuAutoriPresu, dptoBenefi = i.dpto.NombreDepto, iddeptoBene = i.dpto.idDepto, fechaInicio = Convert.ToDateTime(i.x.fechaInicio), fechaTermino = Convert.ToDateTime(i.x.fechaTermino), empresa = i.x.empresa, estado = i.x.estado, municipio = i.x.municipio, hrComision = Convert.ToString(i.x.hrComision), hrEvento = Convert.ToString(i.x.hrEvento), objetivos = i.x.objetivos, acompaniantes = i.emp.Nombre, transporte = i.x.transporte, tipoComision = i.x.tipoComision, observa = i.x.observaciones, proyecto = i.pro.Nombre });
                dtgOficiosxPagar.ItemsSource = ocOficiosxPagar;
            }
        }
        public void llenaDtgOficios()
        {
            String statuAutoriJefeInme = "";
            String statuAutoriJefeArea = "";
            String statuAutoriRespProy = "";
            String statuAutoriPresu = "";
            var query = from x in miBD.OficiosdeComision
                        from emp in miBD.Empleado
                        from pro in miBD.Proyecto
                        from dpto in miBD.Departamento
                        from a in miBD.Area
                        where
                        x.autoPresup == null &&
                        a.idArea == dpto.idArea &&
                        a.idArea == pro.idArea &&
                        dpto.idDepto == pro.idDepto &&
                        dpto.idDepto == x.deptoBene &&
                        pro.idProyecto == x.idProyecto &&
                        x.personaComisionada == emp.idEmpleado
                        select new { x, emp, pro, dpto, a };

            foreach (var i in query)
            {
                statuAutoriJefeInme = Convert.ToString(i.x.jefeInmediato);
                statuAutoriJefeArea = Convert.ToString(i.x.jefeArea);
                statuAutoriRespProy = Convert.ToString(i.x.responsableProy);
                statuAutoriPresu = Convert.ToString(i.x.autoPresup);
                if (statuAutoriPresu.Equals(""))
                {
                    statuAutoriPresu = "NO AUTORIZADO";
                }
                if (statuAutoriJefeInme.Equals(""))
                {
                    statuAutoriJefeInme = "NO AUTORIZADO";
                }
                if (statuAutoriJefeArea.Equals(""))
                {
                    statuAutoriJefeArea = "NO AUTORIZADO";
                }
                if (statuAutoriRespProy.Equals(""))
                {
                    statuAutoriRespProy = "NO AUTORIZADO";
                }
                ocOficios.Add(new OficiosdeComisionClass { idOficios = i.x.idOficiosComi, NoOficios = i.x.NoOficios.Value, fechaReg = i.x.fechaReg.Value, personaReg = i.emp.Nombre, personaComisionada = i.emp.Nombre, jefeInme = i.emp.Nombre, idjefeInmediato = statuAutoriJefeInme, jefeAre = i.emp.Nombre, idjefeArea = statuAutoriJefeArea, responsable = i.emp.Nombre, idresponsableProy = statuAutoriRespProy, autoPresu = i.emp.Nombre, idautoPresup = statuAutoriPresu, dptoBenefi = i.dpto.NombreDepto, iddeptoBene = i.dpto.idDepto, fechaInicio = Convert.ToDateTime(i.x.fechaInicio), fechaTermino = Convert.ToDateTime(i.x.fechaTermino), empresa = i.x.empresa, estado = i.x.estado, municipio = i.x.municipio, hrComision = Convert.ToString(i.x.hrComision), hrEvento = Convert.ToString(i.x.hrEvento), objetivos = i.x.objetivos, acompaniantes = i.emp.Nombre,  transporte = i.x.transporte, tipoComision = i.x.tipoComision, observa = i.x.observaciones, proyecto = i.pro.Nombre });
                dtgOficosPen.ItemsSource = ocOficios;
            }
            
        }

        public void limpiar()
        {
            try
            {
                solicitante.Text = "";
                proyectPresup.Text = "";
                dptoBenef.Text = "";
                txtEmpresa.Text = "";
                txtEstado.Text = "";
                txthrInicio.Text = "";
                txthrFin.Text = "";
                txthrInicioEvent.Text = "";
                txthrFinEvent.Text = "";
                fechInicio.Text = "";
                fechTermino.Text = "";
                txtObje.Text = "";
                txtObserva.Text = "";
                txtacompa.Text = "";
                cmbTipo.SelectedIndex = -1;
                cmbTransporte.SelectedIndex = -1;
            }catch(Exception exc){
            
            }
        }

        private void btnLimpiar(object sender, RoutedEventArgs e)
        {
            limpiar();
        }

        private void cambiarColor(object sender, MouseButtonEventArgs e)
        {
            lblCerrarPopup.Foreground = new SolidColorBrush(Colors.Red);               
        }

        private void asigViaticos(object sender, RoutedEventArgs e)
        {

            Viaticos.IsOpen = true;
            
        }

        private void cerrarSesionPopup(object sender, MouseButtonEventArgs e)
        {
            lblCerrarPopup.Foreground = new SolidColorBrush(Colors.Black);
            Viaticos.IsOpen = false;
            lblNoOficio.Content = "";
            lblComisionado.Content ="";
            lblProyecto.Content = "";
            txtClasifi.Text = "";
            cmbViaticos.SelectedIndex = -1;
            txtObservaViaticos.Text = "";
            txtMonto.Text = "";
            col.Clear();
            idOfiComi = 0;
            dtgViaticos.ItemsSource = col;
            dtgViaticos.ItemsSource = col;
            dtgOficiosxPagar.SelectedValue = -1;
            dtgOficosPen.SelectedValue = -1;
        }

        public void llenarCmbClasifiPop(String clasi,ListView list) {
            var query = from c in miBD.CuentaEnc
                        where c.Cuenta.Substring(0,3)=="821" &&
                         c.Nombre.Contains(clasi) 
                        select c;
            MisCuentas.Clear();
            foreach(var x in query){
                MisCuentas.Add(new ClaseCta {IdCuenta=x.IdCuenta,Nombre=x.Nombre,Cuenta=x.Cuenta,Padre=x.Padre.Value });
                list.ItemsSource = MisCuentas;
                list.DisplayMemberPath = "Nombre";
                list.SelectedValuePath = "IdCuenta";
            }
        
        }

        private void clasificadores(object sender, KeyEventArgs e)
        {
            String clasfi = txtClasifi.Text.Trim();
            llenarCmbClasifiPop(clasfi, lwclasifi);
            lwclasifi.Visibility = Visibility.Visible;
        }

        private void lwclasifi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lwclasifi.SelectedValue == null) return;
            ClaseCta cla = lwclasifi.SelectedItem as ClaseCta;
            txtClasifi.Text = cla.Nombre;
            idClasi = cla.IdCuenta;
            lwclasifi.Visibility = Visibility.Hidden;
        }

        private void dtgOficosPen_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtgOficosPen.SelectedValue == null ||dtgOficosPen.SelectedIndex==-1) return;
            dtgViaticos.IsEnabled = false;
            btnPagoViatico.IsEnabled = false;
            btnAsig.IsEnabled = true;
            buttonLimpiar.IsEnabled = true;
            btnInformComisi.IsEnabled = true;
            btnImpOfico.IsEnabled = true;
            OficiosdeComisionClass of = dtgOficosPen.SelectedItem as OficiosdeComisionClass;
            idOfiComi = of.idOficios;
            lblNoOficio.Content = of.NoOficios;
            lblComisionado.Content = of.personaComisionada;
            lblProyecto.Content = of.proyecto;
            folio.Content = of.NoOficios;
            proyectPresup.Text = of.proyecto;
            fechaElab.Content = of.fechaReg;
            fechInicio.Text = of.fechaInicio.ToString();
            fechTermino.Text = of.fechaTermino.ToString();
            solicitante.Text = of.personaComisionada;
            txtEstado.Text = of.estado;
            proyectPresup.Text = of.proyecto;
            dptoBenef.Text = of.dptoBenefi.ToString();
            txtEmpresa.Text = of.empresa;
            txtEstado.Text = of.estado;
            cmbTransporte.Text = of.transporte;
            cmbTipo.Text = of.tipoComision;
            txtObje.Text = of.objetivos;
            txtObserva.Text = of.observa;
            txthrInicio.Text = of.hrComision.Substring(0,5);
            txthrFin.Text = of.hrComision.Substring(9);
            txthrInicioEvent.Text = of.hrEvento.Substring(0,5);
            txthrFinEvent.Text = of.hrEvento.Substring(9);
            txtacompa.Text = of.acompaniantes;
        }

        public void llenaConcepto() {
            var query = from x in miBD.ConceptoViaticos
                        select x;
            ocConcepto.Clear();
            foreach(var i in query){
                ocConcepto.Add(new concepto { idconcepto=i.idConcepto,nombreCon=i.nombreConcepto,clasificador=i.clasificador.Value,monto=Convert.ToDouble(i.monto.Value)});
                cmbViaticos.ItemsSource = ocConcepto;
                cmbViaticos.DisplayMemberPath = "nombreCon";
                cmbViaticos.SelectedValuePath = "idconcepto";
            }
            
        }

        private void adDataG(object sender, RoutedEventArgs e)
        {

            if (txtClasifi.Text == "")
            {
                MessageBox.Show("Selecciona el clasificador");
            }
            else if (cmbViaticos.SelectedIndex == -1)
            {
                MessageBox.Show("Selecciona un Concepto");
            }
            else if (txtMonto.Text == "")
            {
                MessageBox.Show("Escribir el Monto del concepto");
            }
            else
            {

                var query = from con in ocConcepto
                            where con.idconcepto == int.Parse(cmbViaticos.SelectedValue.ToString())
                            select con;

                foreach (var i in query)
                {
                    partida = i.clasificador;

                }

                col.Add(new ViaticoClase { viatico = cmbViaticos.Text, monto = Convert.ToDouble(txtMonto.Text), status = "Asignado", partida = partida, nomPartida = txtClasifi.Text, fechCompro = fechaElabora, Observaciones = txtObservaViaticos.Text, idOfiComi = idOfiComi, idClasifi = idClasi, nomClasifi = txtClasifi.Text });
                totalAsig = Convert.ToDouble(txtMonto.Text) + totalAsig;
                lblTotal.Content = "Total: " + totalAsig;
                dtgViaticos.ItemsSource = col;
                txtClasifi.Text = "";
                cmbViaticos.SelectedIndex = -1;
                txtObservaViaticos.Text = "";
                txtMonto.Text = "";
            }
        }
        public class ViaticoClase{

            public int idViatico { get; set; }
            public String viatico { get; set; }
            public double monto { get; set; }
            public String status { get; set; }
            public int partida { get; set; }
            public String nomPartida { get; set; }
            public String fechCompro{get;set;}
            public int idOfiComi { get; set; }
            public String Observaciones { get; set; }
            public String nomClasifi { get; set; }
            public int idClasifi { get; set; }
        
        }

        private void notificar(object sender, RoutedEventArgs e)
        {
            var query = from t in col select t;                 
                foreach (var x in query)
                {
                   Viaticos v = new Viaticos { idViatico = x.idViatico, viatico = x.viatico, monto = x.monto, status = x.status, partida = x.partida, fechComprometido = Convert.ToDateTime(x.fechCompro), clasificador = x.idClasifi, idOficiosComi = x.idOfiComi, observaciones = x.Observaciones }; 
                    try
                    {
                        miBD.Viaticos.InsertOnSubmit(v);
                        miBD.Viaticos.Context.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("" + ex);
                    }
                }
                var actualizar = (from a in miBD.OficiosdeComision
                                  where a.NoOficios == idOfiComi
                                  select a).Single();
                actualizar.autoPresup = Convert.ToDateTime(fechaElabora);
                //HASTA AQUI LO DE LA FECHA
                miBD.SubmitChanges();
                col.Clear();
                ocOficios.Clear();
                ocOficiosxPagar.Clear();
                llenadtgOfixPagar();
                llenaDtgOficios();
                MessageBox.Show("La Notificación se realizo con exito");
                Viaticos.IsOpen = false; 
        }

        private void dtgViaticos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtgViaticos.SelectedValue == null) return;
            ViaticoClase of = dtgViaticos.SelectedItem as ViaticoClase;
            txtClasifi.Text = of.nomPartida;
            txtObservaViaticos.Text = of.Observaciones;
            cmbViaticos.Text = of.viatico;
            txtMonto.Text = of.monto.ToString();

        }

        private void expandOfiPendientes(object sender, RoutedEventArgs e)
        {
            expOfixPagar.IsExpanded = false;
            dtgOficiosxPagar.SelectedIndex = -1;
            limpiar();
            expOfixPagar.Visibility = Visibility.Collapsed;
        }

        private void collaExpOfiPend(object sender, RoutedEventArgs e)
        {
            expOfixPagar.IsExpanded = false;
            expOfixPagar.Visibility = Visibility.Visible;
            expOfiPendientes.IsExpanded = false;
            expOfiPendientes.Visibility = Visibility.Visible;
        }

        private void collaExpOfixPagar(object sender, RoutedEventArgs e)
        {
            expOfixPagar.IsExpanded = false;
            expOfixPagar.Visibility = Visibility.Visible;
            expOfiPendientes.IsExpanded = false;
            expOfiPendientes.Visibility = Visibility.Visible;
        }

        private void expOfixPagarEvent(object sender, RoutedEventArgs e)
        {
            expOfiPendientes.IsExpanded = false;
            dtgOficosPen.SelectedIndex = -1;
            limpiar();
            expOfiPendientes.Visibility = Visibility.Collapsed;
        }

        private void dtgOficiosxPagar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //oficiosdeComision ofi = new oficiosdeComision();
            //ofi.desabilitar();
            proyectPresup.IsEnabled = false;
            fechaElab.IsEnabled = false;
            fechInicio.IsEnabled = false;
            fechTermino.IsEnabled = false;
            solicitante.IsEnabled = false;
            txtEstado.IsEnabled = false;
            proyectPresup.IsEnabled = false;
            dptoBenef.IsEnabled = false;
            txtEmpresa.IsEnabled = false;
            txtEstado.IsEnabled = false;
            cmbTransporte.IsEnabled = false;
            cmbTipo.IsEnabled = false;
            txtObje.IsEnabled = false;
            txtObserva.IsEnabled = false;
            txthrInicio.IsEnabled = false;
            txthrFin.IsEnabled = false;
            txthrInicioEvent.IsEnabled = false;
            txthrFinEvent.IsEnabled = false;
            txtacompa.IsEnabled = false;
            cmbTipo.IsEnabled = false;
            cmbTransporte.IsEnabled = false;

            btnPagoViatico.Visibility = Visibility.Visible;
            dtgViaticos.IsEnabled = true;
            btnAsig.IsEnabled = true;
            buttonLimpiar.IsEnabled = true;
            btnInformComisi.IsEnabled = true;
            btnImpOfico.IsEnabled = true;
            btnSegi.IsEnabled = true;
            btnPagoViatico.IsEnabled = true;
            if (dtgOficiosxPagar.SelectedValue == null) return;
            OficiosdeComisionClass of = dtgOficiosxPagar.SelectedItem as OficiosdeComisionClass;
            //String acom = "";
            //var quer = from x in miBD.Empleado
            //           where x.idEmpleado == Convert.ToInt32(of.idacompa)
            //           select x;
            //foreach (var acompa in quer)
            //{
            //    acom = acompa.Nombre;
            //}
            idOfiComi = of.idOficios;
            lblNoOficio.Content = of.NoOficios;
            lblComisionado.Content = of.personaComisionada;
            lblComisionadoPago.Content = of.personaComisionada;
            lblfolioPago.Content = of.NoOficios;
            lblProyectoPago.Content = of.proyecto;
            lblResponsablePago.Content = of.responsable;
            lblProyecto.Content = of.proyecto;
            folio.Content = of.NoOficios;
            proyectPresup.Text = of.proyecto;
            fechaElab.Content = of.fechaReg;
            fechInicio.Text = of.fechaInicio.ToString();
            fechTermino.Text = of.fechaTermino.ToString();
            solicitante.Text = of.personaComisionada;
            txtEstado.Text = of.estado;
            proyectPresup.Text = of.proyecto;
            dptoBenef.Text = of.dptoBenefi.ToString();
            txtEmpresa.Text = of.empresa;
            txtEstado.Text = of.estado;
            cmbTransporte.Text = of.transporte;
            cmbTipo.Text = of.tipoComision;
            txtObje.Text = of.objetivos;
            txtObserva.Text = of.observa;
            txthrInicio.Text = of.hrComision.Substring(0, 5);
            txthrFin.Text = of.hrComision.Substring(9);
            txthrInicioEvent.Text = of.hrEvento.Substring(0, 5);
            txthrFinEvent.Text = of.hrEvento.Substring(9);
            txtacompa.Text = of.acompaniantes;

            String nombreCla = "";
            double total = 0;
            var query = from x in miBD.Viaticos
                        where x.idOficiosComi == idOfiComi
                        select x;


            foreach (var v in query)
            {
                var nomClasi = from cla in miBD.CuentaEnc
                               where cla.IdCuenta == v.clasificador
                               select cla;
                foreach (var nom in nomClasi)
                {
                    nombreCla = nom.Nombre;
                }
                total = (v.monto.Value + total);
                lblTotal.Content = "Total: " + total;
                col.Add(new ViaticoClase { idViatico = v.idViatico, viatico = v.viatico, monto = v.monto.Value, status = v.status, partida = v.partida.Value, fechCompro = v.fechComprometido.ToString(), nomPartida = nombreCla, Observaciones = v.observaciones });
                dtgViaticos.ItemsSource = col;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }



        // Agregamos un nuevo Using a la clase.
        // El código de la clase es:

        class Correos{
        /*
         * Cliente SMTP
         * Gmail:  smtp.gmail.com  puerto:587
         * Hotmail: smtp.live.com  puerto:25
        */
            SmtpClient server = new SmtpClient("smtp.gmail.com", 587);

 

       public Correos()
        {
            /*
            * Autenticacion en el Servidor
             * Utilizaremos nuestra cuenta de correo
             *
             * Direccion de Correo (Gmail o Hotmail)
             * y Contrasena correspondiente
             */
            server.Credentials = new System.Net.NetworkCredential("sigmud.bollas@gmail.com", "xxxxx");
           server.EnableSsl = true;
        }

        public void MandarCorreo(MailMessage mensaje)
        {
           server.Send(mensaje);
        }

    }


        private void Email(object sender, RoutedEventArgs e)
        {
            try
            {
                Correos Cr = new Correos();
                MailMessage mnsj = new MailMessage();
                mnsj.Subject = "Hola Mundo Prueba";
                mnsj.To.Add(new MailAddress("manuelwassaurus@gmail.com"));
                mnsj.From = new MailAddress("sigmud.bollas@gmail.com", "Eder Bollas Tovar");
                /* Si deseamos Adjuntar algún archivo*/
                //mnsj.Attachments.Add(new Attachment("E:\\LINEAMIENTOS.pdf"));
                mnsj.Body = "Pinche nahayote de shit";

                /* Enviar */

                Cr.MandarCorreo(mnsj);

                //Enviado = true;
                MessageBox.Show("El Mail se ha Enviado Correctamente", "Listo!!",MessageBoxButton.OK);//, System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnPagoViatico_Click(object sender, RoutedEventArgs e)
        {
            pagoViaticos.IsOpen = true;
        }

        private void CerrarPopupPago(object sender, MouseButtonEventArgs e)
        {
            pagoViaticos.IsOpen = false;
            lblfolioPago.Content = "";
            lblComisionadoPago.Content = "";
            lblProyectoPago.Content = "";
            lblResponsablePago.Content = "";
            txtclasifiPago.Text = "";
            txtConceptoPago.Text = "";
            txtFolioEntrega.Text = "";
            txtMedioEntrega.Text = "";
            dpFechaEntrega.Text = "";
        }

        private void Label_MouseEnter_1(object sender, MouseEventArgs e)
        {
            lblCerrarPago.Background = new SolidColorBrush(Colors.LightBlue);
            lblCerrarPago.Foreground = new SolidColorBrush(Colors.White); 
        }

        private void Label_MouseLeave_1(object sender, MouseEventArgs e)
        {
            if (lblCerrarPago.IsMouseOver == false)
            {
                lblCerrarPago.Background = new SolidColorBrush(Colors.White);
                lblCerrarPago.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void txtclasifiPago_KeyUp(object sender, KeyEventArgs e)
        {
            String clasfi = txtclasifiPago.Text.Trim();
            llenarCmbClasifiPop(clasfi,lwClasiPago);
            lwClasiPago.Visibility = Visibility.Visible;
        }

        private void lwClasiPago_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lwClasiPago.SelectedValue == null) return;
            ClaseCta cla = lwClasiPago.SelectedItem as ClaseCta;
            txtclasifiPago.Text = cla.Nombre;
            idClasiPago = cla.IdCuenta;
            lwClasiPago.Visibility = Visibility.Hidden;
        }
    }
}
