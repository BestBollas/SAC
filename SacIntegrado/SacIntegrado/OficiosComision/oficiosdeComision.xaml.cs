using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
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

using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.IO;

using System.Net.Mail;
using System.ComponentModel;
using System.Net;

namespace SacIntegrado.OficiosComision
{
    /// <summary>
    /// Lógica de interacción para oficiosdeComision.xaml
    /// </summary>


    public class EstadoClass
    {
        public String Nombre { get; set; }
    }
    public class tipoComisionClass
    {
        public String Nombre { get; set; }
    }
    public class TrasnporteClass
    {
        public String Nombre { get; set; }
    }
    public class OficiosdeComisionClass
    {
        public int idOficios { get; set; }
        public int NoOficios { get; set; }
        public DateTime fechaReg { get; set; }
        public String personaReg { get; set; }
        public int personaRegistro { get; set; }
        public String personaComisionada { get; set; }
        public int personaComisi { get; set; }
        public String jefeInme { get; set; }
        public String idjefeInmediato { get; set; }
        public String jefeAre { get; set; }
        public String idjefeArea { get; set; }
        public String responsable { get; set; }
        public String idresponsableProy { get; set; }
        public String autoPresu { get; set; }
        public String idautoPresup { get; set; }
        public String dptoBenefi { get; set; }
        public int iddeptoBene { get; set; }
        public DateTime fechaInicio { get; set; }
        public DateTime fechaTermino { get; set; }
        public String empresa { get; set; }
        public String estado { get; set; }
        public String municipio { get; set; }
        public String hrComision { get; set; }
        public String hrEvento { get; set; }
        public String objetivos { get; set; }
        public String acompaniantes { get; set; }
        public String transporte { get; set; }
        public String tipoComision { get; set; }
        public String observa { get; set; }
        public String proyecto { get; set; }
        public String Recurso { get; set; }
        public String Cuenta { get; set; }
        public String Area { get; set; }
    }

    public partial class oficiosdeComision : Page
    {
        ObservableCollection<Empleado> listaEmpleado = new ObservableCollection<Empleado>();
        ObservableCollection<Proyecto> listwProyectos = new ObservableCollection<Proyecto>();
        ObservableCollection<EstadoClass> listwEstado = new ObservableCollection<EstadoClass>();
        ObservableCollection<OficiosdeComisionClass> ocOficios = new ObservableCollection<OficiosdeComisionClass>();
        ObservableCollection<concepto> ocConcepto = new ObservableCollection<concepto>();
        ObservableCollection<ViaticoClase> col = new ObservableCollection<ViaticoClase>();
        Db miBD = new Db();
        string fileName = "";
        String nomUsua;
        int usuario = 0;
        int areaid = 0;
        int jefeinme = 0;
        int idres = 0;
        int deptoid = 0;
        string  RutaSintesis="";
        int comisionado = 0;
        int idProye = 0;
        int tipo = 0;
        double totalAsig = 0;
        int idOfiComi = 0;
        int viaticosCompro = 0;
        string fechaElabora = DateTime.Today.ToShortDateString();
        public oficiosdeComision()
        {

        }
        public oficiosdeComision(String user, int id, String nombre, int tipoM)
        {
            InitializeComponent();
            tipo = tipoM;
            tipoMenu();
            fechaElab.Content = fechaElabora;
            nomUsua = nombre;
            usuario = id;
            llenaCombo();
            llenaConcepto();
            llenaDtgOficios(usuario);
            lblusuario.Content = nomUsua;
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            Viaticos.IsOpen = false;
            sintesisVentana.IsOpen = false;
            ComprobaViaticos.IsOpen = false;
            lwEmpleados.Visibility = Visibility.Hidden;
            lwProyecto.Visibility = Visibility.Hidden;
            lwEstados.Visibility = Visibility.Hidden;
        }

        public void tipoMenu()
        {
            if (tipo == 1)
            {
                btnGrabar.Visibility = Visibility.Visible;
            }
            else if (tipo == 3)
            {
                lbltitulo.Content = "Autorización Jefe Inmediato";
                //btnAutori.Visibility = Visibility.Visible;
                btnlimpiarOfi.Visibility = Visibility.Collapsed;
            }
            else if (tipo == 4)
            {
                lbltitulo.Content = "Autorización Jefe Area";
                btnlimpiarOfi.Visibility = Visibility.Collapsed;
                //btnAutori.Visibility = Visibility.Visible;
            }
            else if (tipo == 5)
            {
                lbltitulo.Content = "Autorización Responsable de Proyecto";
                btnlimpiarOfi.Visibility = Visibility.Collapsed;
                //btnAutori.Visibility = Visibility.Visible;
            }
        }

        private void cerrarSesion(object sender, MouseButtonEventArgs e)
        {

        }

        private void cambiarColor(object sender, MouseButtonEventArgs e)
        {
            
            Viaticos.IsOpen = false;
            lblCerrarPopup.Foreground = new SolidColorBrush(Colors.Black);
            Viaticos.IsOpen = false;
            lblNoOficio.Content = "";
            lblComisionado.Content = "";
            lblProyecto.Content = "";
            txtClasifi.Text = "";
            cmbViaticos.SelectedIndex = -1;
            txtObservaViaticos.Text = "";
            txtMonto.Text = "";
            lblTotal.Content = "Total: ";
            col.Clear();
            //idOfiComi = 0;
            dtgViaticos.ItemsSource = col;
            dtgViaticos.ItemsSource = col;
            //dtgOficos.SelectedIndex = -1;
            //btnViatico.Visibility = Visibility.Collapsed;
            //btnAutori.Visibility = Visibility.Collapsed;
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
            comisionado = emp.idEmpleado;
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
            idProye = proy.idProyecto;
            //areaid = proy.idArea.Value;
            idres = proy.idResponsable.Value;
            deptoid = proy.idDepto.Value;
            var idarea = from x in miBD.Area
                         where x.idArea == proy.idArea
                         select x;
            foreach (var a in idarea)
            {
                areaid = a.idJefe.Value;
            }
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
                jefeinme = d.idJefe.Value;
            }
        }
        private void estado(object sender, KeyEventArgs e)
        {
            String[] estado = new String[] { "Aguascalientes", "Baja California", "Baja California Sur", "Campeche", "Chiapas", "Chihuahua", "Coahuila", "Colima", "Durango", "Guanajuato", "Guerrero", "Hidalgo", "Jalisco", "Estado de México", "Michoacán", "Morelos", "Nayarit", "Nuevo León", "Oaxaca", "Puebla", "Querétaro", "Quintana Roo", "San Luis Potosí", "Sinaloa", "Sonora", "Tabasco", "Tamaulipas", "Tlascala", "Veracruz", "Yucatán", "Zacatecas", "Distrito Federal" };
            lwEstados.Visibility = Visibility.Visible;
            var query = from x in estado
                        where x.ToLower().Contains(txtEstado.Text.ToLower())
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
            if(validar()){
            Table<OficiosdeComision> ofiComi = miBD.GetTable<OficiosdeComision>();
            OficiosdeComision tbofiComi = new OficiosdeComision();
            int numOfi = 0;
            var query = (from x in miBD.OficiosdeComision
                         select x.NoOficios).Max();
            if (query == null)
            {
                numOfi = 1;
            }
            else
            {
                numOfi = (query.Value + 1);
            }
            tbofiComi.idOficiosComi = numOfi;
            tbofiComi.NoOficios = 0;
            tbofiComi.fechaReg = Convert.ToDateTime(fechaElabora);
            tbofiComi.personaReg = usuario;
            tbofiComi.personaComisionada = comisionado;
            tbofiComi.jefeInmediato = null;
            tbofiComi.jefeArea = null;
            tbofiComi.responsableProy = null;
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
            tbofiComi.idProyecto = idProye;
            ofiComi.InsertOnSubmit(tbofiComi);
            ofiComi.Context.SubmitChanges();
            MessageBox.Show("En conformidad a lo dispuesto en la normatividad para administración del presupuesto y en caso de que mi comision genero viaticos 'Me Comprometo' a efectuar la comprobación de un plazo no mayor a 15 días después  de haber concluido el periodo de la comision, Misma que debera reunir los requisitos fiscales, en caso contrario, se turnara al departamento de Recursos Humanos para su descuento via nomina.");
            MessageBox.Show("Te Recomendamos Proporcionar los datos Bancarios");
            ocOficios.Clear();
            llenaDtgOficios(usuario);
            limpiar();
        }
        }
        public void llenaDtgOficios(int idemple)
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
                       (x.personaReg == idemple || x.personaComisionada == idemple) &&
                       a.idArea == dpto.idArea &&
                       a.idArea == pro.idArea &&
                       dpto.idDepto == pro.idDepto &&
                       dpto.idDepto == x.deptoBene &&
                       pro.idProyecto == x.idProyecto &&
                       x.personaComisionada == emp.idEmpleado
                        select new { x, emp, pro, dpto, a };
            if (tipo == 3)
            {
                query = from x in miBD.OficiosdeComision
                        from emp in miBD.Empleado
                        from pro in miBD.Proyecto
                        from dpto in miBD.Departamento
                        from a in miBD.Area
                        where
                        x.autoPresup != null &&
                       x.jefeInmediato == null &&
                        dpto.idJefe == idemple &&
                       a.idArea == dpto.idArea &&
                       a.idArea == pro.idArea &&
                       dpto.idDepto == pro.idDepto &&
                       dpto.idDepto == x.deptoBene &&
                       pro.idProyecto == x.idProyecto &&
                      x.personaComisionada == emp.idEmpleado
                        select new { x, emp, pro, dpto, a };
            }
            else if (tipo == 4)
            {
                query = from x in miBD.OficiosdeComision
                        from emp in miBD.Empleado
                        from pro in miBD.Proyecto
                        from dpto in miBD.Departamento
                        from a in miBD.Area
                        where
                       x.autoPresup != null &&
                       x.jefeInmediato == null &&
                       x.jefeArea == null &&
                       a.idJefe == idemple &&
                       a.idArea == dpto.idArea &&
                       a.idArea == pro.idArea &&
                       dpto.idDepto == pro.idDepto &&
                       dpto.idDepto == x.deptoBene &&
                       pro.idProyecto == x.idProyecto &&
                       x.personaComisionada == emp.idEmpleado
                        select new { x, emp, pro, dpto, a };
            }
            else if (tipo == 5)
            {
                query = from x in miBD.OficiosdeComision
                        from emp in miBD.Empleado
                        from pro in miBD.Proyecto
                        from dpto in miBD.Departamento
                        from a in miBD.Area
                        where
                        x.autoPresup != null &&
                       x.jefeInmediato == null &&
                       x.jefeArea == null &&
                       x.responsableProy == null &&
                       pro.idResponsable == idemple &&
                       a.idArea == dpto.idArea &&
                       a.idArea == pro.idArea &&
                       dpto.idDepto == pro.idDepto &&
                       dpto.idDepto == x.deptoBene &&
                       pro.idProyecto == x.idProyecto &&
                       x.personaComisionada == emp.idEmpleado
                        select new { x, emp, pro, dpto, a };
            }
            try
            {
                foreach (var i in query)
                {
                    String recur = "";
                    var re = from s in miBD.Recurso
                             where s.idRecurso == i.pro.idRecurso.Value
                             select s;
                    foreach(var f in re){
                        recur = f.Nombre;
                    }
                    var rp = (from r in miBD.Empleado
                              where r.idEmpleado == i.pro.idResponsable
                              select r.Nombre).SingleOrDefault();
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
                    ocOficios.Add(new OficiosdeComisionClass { idOficios = i.x.idOficiosComi, NoOficios = i.x.NoOficios.Value, fechaReg = i.x.fechaReg.Value, personaReg = i.emp.Nombre, personaComisionada = i.emp.Nombre, jefeInme = i.emp.Nombre, idjefeInmediato = statuAutoriJefeInme, jefeAre = i.emp.Nombre, idjefeArea = statuAutoriJefeArea, responsable = rp.ToString(), idresponsableProy = statuAutoriRespProy, autoPresu = i.emp.Nombre, idautoPresup = statuAutoriPresu, dptoBenefi = i.dpto.NombreDepto, iddeptoBene = i.dpto.idDepto, fechaInicio = Convert.ToDateTime(i.x.fechaInicio), fechaTermino = Convert.ToDateTime(i.x.fechaTermino), empresa = i.x.empresa, estado = i.x.estado, municipio = i.x.municipio, hrComision = Convert.ToString(i.x.hrComision), hrEvento = Convert.ToString(i.x.hrEvento), objetivos = i.x.objetivos, acompaniantes = i.x.acompaniante, transporte = i.x.transporte, tipoComision = i.x.tipoComision, observa = i.x.observaciones, proyecto = i.pro.Nombre, Area = i.a.nomArea, Recurso = recur });
                    dtgOficos.ItemsSource = ocOficios;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        public void limpiar()
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
            lblTotal.Content = "Total: ";
            folio.Content = "0000";
            fechaElab.Content = "00/00/0000";
            cmbTipo.SelectedIndex = -1;
            cmbTransporte.SelectedIndex = -1;
            if(tipo==1){
                btnGrabar.Visibility = Visibility.Visible;
                btnViatico.Visibility = Visibility.Collapsed;
                btnCompro.Visibility = Visibility.Collapsed;
                btnSintesis.Visibility = Visibility.Collapsed;
                btnImpOfico.Visibility = Visibility.Collapsed;
                btnInformComisi.Visibility = Visibility.Collapsed;
            }            
            activar();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            limpiar();
        }
        private void btnViaticos(object sender, RoutedEventArgs e)
        {
            Viaticos.IsOpen = true;
        }
        public void llenaConcepto()
        {
            var query = from x in miBD.ConceptoViaticos
                        select x;
            ocConcepto.Clear();
            foreach (var i in query)
            {
                ocConcepto.Add(new concepto { idconcepto = i.idConcepto, nombreCon = i.nombreConcepto, clasificador = i.clasificador.Value, monto = Convert.ToDouble(i.monto.Value) });
                cmbViaticos.ItemsSource = ocConcepto;
                cmbViaticos.DisplayMemberPath = "nombreCon";
                cmbViaticos.SelectedValuePath = "idconcepto";
            }
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
        public class ViaticoClase
        {
            public int idViatico { get; set; }
            public String viatico { get; set; }
            public double monto { get; set; }
            public String status { get; set; }
            public int partida { get; set; }
            public String nomPartida { get; set; }
            public String fechCompro { get; set; }
            public int idOfiComi { get; set; }
            public String Observaciones { get; set; }
            public String nomClasifi { get; set; }
            public int idClasifi { get; set; }
            public double total { get; set; }
        }
        public void activar()
        {
            proyectPresup.IsEnabled = true;
            fechaElab.IsEnabled = true;
            fechInicio.IsEnabled = true;
            fechTermino.IsEnabled = true;
            solicitante.IsEnabled = true;
            txtEstado.IsEnabled = true;
            proyectPresup.IsEnabled = true;
            dptoBenef.IsEnabled = true;
            txtEmpresa.IsEnabled = true;
            txtEstado.IsEnabled = true;
            cmbTransporte.IsEnabled = true;
            cmbTipo.IsEnabled = true;
            txtObje.IsEnabled = true;
            txtObserva.IsEnabled = true;
            txthrInicio.IsEnabled = true;
            txthrFin.IsEnabled = true;
            txthrInicioEvent.IsEnabled = true;
            txthrFinEvent.IsEnabled = true;
            txtacompa.IsEnabled = true;
            cmbTipo.IsEnabled = true;
            cmbTransporte.IsEnabled = true;
        }
        public void desabilitar()
        {
            try
            {
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
                llenarviaticos();
            }catch(Exception exc){
                MessageBox.Show(exc.Message);
            }
        }
        public void llenarviaticos()
        {
            String nombreCla = "";
            double total = 0;
            dtgViaticos.IsEnabled = true;
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
                dtgViaticosComproba.ItemsSource = col;
            }
        }
        public void llenarviaticosCompra()
        {
            String nombreCla = "";
            double total = 0;
            dtgViaticos.IsEnabled = true;
            var query = from x in miBD.Viaticos
                        where x.idOficiosComi == viaticosCompro
                        select x;
            foreach (var v in query)
            {
                var nomClasi = from cla in miBD.CuentaEnc
                               where cla.IdCuenta == v.clasificador
                               select cla;
                col.Clear();
                foreach (var nom in nomClasi)
                {
                    nombreCla = nom.Nombre;
                }
                total = (v.monto.Value + total);
                lblTotal.Content = "Total: " + total;
                col.Add(new ViaticoClase { idViatico = v.idViatico, viatico = v.viatico, monto = v.monto.Value, status = v.status, partida = v.partida.Value, fechCompro = v.fechComprometido.ToString(), nomPartida = nombreCla, Observaciones = v.observaciones });
                dtgViaticosComproba.ItemsSource = col;
            }
        }
        private void dtgOficos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (dtgOficos.SelectedIndex ==-1)
            {
                if (dtgOficos.SelectedValue == null) return;
            }
            else
            {
                if(tipo!=1){
                    btnAutori.Visibility = Visibility.Visible;
                }
                btnViatico.Visibility = Visibility.Visible;
                btnGrabar.Visibility = Visibility.Collapsed;
                OficiosdeComisionClass of = dtgOficos.SelectedItem as OficiosdeComisionClass;
                if (dtgOficos.SelectedValue == null) return;
                if (of.idjefeInmediato != "NO AUTORIZADO" && of.idautoPresup != "NO AUTORIZADO" && of.idjefeArea != "NO AUTORIZADO" && of.idresponsableProy != "NO AUTORIZADO")
                {
                    btnCompro.Visibility = Visibility.Visible;
                    btnSintesis.Visibility = Visibility.Visible;
                    btnImpOfico.Visibility = Visibility.Visible;
                    btnInformComisi.Visibility = Visibility.Visible;
                }
                else { btnCompro.Visibility = Visibility.Collapsed;
                    btnSintesis.Visibility = Visibility.Collapsed;
                    btnImpOfico.Visibility = Visibility.Collapsed;
                    btnInformComisi.Visibility = Visibility.Collapsed;
                }
                lblComiSinte.Content = of.personaComisionada;
                lblfolioCompro.Content = of.NoOficios;
                lblComisionadoCompro.Content = of.personaComisionada;
                viaticosCompro = of.idOficios;
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
                txthrInicio.Text = of.hrComision.Substring(0, 5);
                txthrFin.Text = of.hrComision.Substring(9);
                txthrInicioEvent.Text = of.hrEvento.Substring(0, 5);
                txthrFinEvent.Text = of.hrEvento.Substring(9);
                txtacompa.Text = of.acompaniantes; ;
                desabilitar();
            }
        }
        private void Autorizar(object sender, RoutedEventArgs e)
        {

            if (dtgOficos.SelectedValue == null) {
                MessageBox.Show("Selecciona un Oficio para Autorizar");
            }
            else
            {
                btnAutori.Visibility = Visibility.Visible;
                var actualizar = (from a in miBD.OficiosdeComision
                                  where a.NoOficios == idOfiComi
                                  select a).Single();
                switch (tipo)
                {
                    case 3:
                        //PARA LA FECHA EN Q SE AUTORIZA
                        actualizar.jefeInmediato = Convert.ToDateTime(fechaElabora);
                        //HASTA AQUI LO DE LA FECHA
                        miBD.SubmitChanges();
                        MessageBox.Show("Se Autorizo Correctamente");
                        ocOficios.Clear();
                        llenaDtgOficios(usuario);
                        limpiar();
                        break;
                    case 4:
                        actualizar.jefeArea = Convert.ToDateTime(fechaElabora);
                        //HASTA AQUI LO DE LA FECHA
                        miBD.SubmitChanges();
                        MessageBox.Show("Se Autorizo Correctamente");
                        ocOficios.Clear();
                        llenaDtgOficios(usuario);
                        limpiar();
                        break;
                    case 5:
                        actualizar.responsableProy = Convert.ToDateTime(fechaElabora);
                        //HASTA AQUI LO DE LA FECHA
                        miBD.SubmitChanges();
                        MessageBox.Show("Se Autorizo Correctamente");
                        ocOficios.Clear();
                        llenaDtgOficios(usuario);
                        limpiar();
                        break;
                }
            }
        }
        private void Comprobacion(object sender, RoutedEventArgs e)
        {
            btnGrabapop2.Visibility = Visibility.Visible;
            btnSubirArchivo.Visibility = Visibility.Visible;
            llenarviaticosCompra();
            ComprobaViaticos.IsOpen = true;
        }

        private void CerrarPopupComprova(object sender, MouseButtonEventArgs e)
        {
            dtgOficos.SelectedIndex = -1;
            ComprobaViaticos.IsOpen = false;
        }

        private void InformeComision(object sender, RoutedEventArgs e)
        {
             String statuAutoriJefeInme = "";
            String statuAutoriJefeArea = "";
            String statuAutoriRespProy = "";
            String statuAutoriPresu = "";
            MessageBox.Show("No Oficio "+idOfiComi);
            reporteOficios.Visibility = Visibility.Visible;
            btnCerrarInforme.Visibility = Visibility.Visible;
            var query = from x in miBD.OficiosdeComision
                        from emp in miBD.Empleado
                        from pro in miBD.Proyecto
                        from dpto in miBD.Departamento
                        from a in miBD.Area
                        where
                       x.NoOficios==idOfiComi &&
                       a.idArea == dpto.idArea &&
                       a.idArea == pro.idArea &&
                       dpto.idDepto == pro.idDepto &&
                       dpto.idDepto == x.deptoBene &&
                       pro.idProyecto == x.idProyecto &&
                       x.personaComisionada == emp.idEmpleado
                        select new { x, emp, pro, dpto, a };
            ocOficios.Clear();
            try
            {
                foreach (var i in query)
                {
                    String recur = "";
                    MessageBox.Show("Area " + i.a.nomArea);
                    var re = from s in miBD.Recurso
                             where s.idRecurso == i.pro.idRecurso
                             select s;
                    foreach (var f in re)
                    {
                        recur = f.Nombre;
                        MessageBox.Show("Recurso " + f.Nombre);
                    }
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
                    ocOficios.Add(new OficiosdeComisionClass { idOficios = i.x.idOficiosComi, NoOficios = i.x.NoOficios.Value, fechaReg = i.x.fechaReg.Value, personaReg = i.emp.Nombre, personaComisionada = i.emp.Nombre, jefeInme = i.emp.Nombre, idjefeInmediato = statuAutoriJefeInme, jefeAre = i.emp.Nombre, idjefeArea = statuAutoriJefeArea, responsable = i.emp.Nombre, idresponsableProy = statuAutoriRespProy, autoPresu = i.emp.Nombre, idautoPresup = statuAutoriPresu, dptoBenefi = i.dpto.NombreDepto, iddeptoBene = i.dpto.idDepto, fechaInicio = Convert.ToDateTime(i.x.fechaInicio), fechaTermino = Convert.ToDateTime(i.x.fechaTermino), empresa = i.x.empresa, estado = i.x.estado, municipio = i.x.municipio, hrComision = Convert.ToString(i.x.hrComision), hrEvento = Convert.ToString(i.x.hrEvento), objetivos = i.x.objetivos, acompaniantes = i.x.acompaniante,  transporte = i.x.transporte, tipoComision = i.x.tipoComision, observa = i.x.observaciones, proyecto = i.pro.Nombre, Area = i.a.nomArea, Recurso = recur });
                    reportInformeComision.LocalReport.DataSources.Add(new ReportDataSource("oficiosComision", ocOficios));
                    reportInformeComision.LocalReport.ReportPath = @"C:\Users\Best\Documents\SacIntegrado\SacIntegrado\OficiosComision\Report1.rdlc";
                    reportInformeComision.RefreshReport();
                    //-----------------------------------------------------------------------------------------------------------------------
                }
                    String nombreCla = "";
                    double total = 0;
                    var query2 = from x in miBD.Viaticos
                                where x.idOficiosComi == idOfiComi
                                select x;
                    foreach (var v in query2)
                    {
                        var nomClasi = from cla in miBD.CuentaEnc
                                       where cla.IdCuenta == v.clasificador
                                       select cla;
                        col.Clear();
                        foreach (var nom in nomClasi)
                        {
                            nombreCla = nom.Nombre;
                        }
                        total = (v.monto.Value + total);
                        lblTotal.Content = "Total: " + total;
                        col.Add(new ViaticoClase { idViatico = v.idViatico, viatico = v.viatico, monto = v.monto.Value, status = v.status, partida = v.partida.Value, fechCompro = v.fechComprometido.ToString(), nomPartida = nombreCla, Observaciones = v.observaciones,total=total });
                    }
                    reportInformeComision.LocalReport.DataSources.Add(new ReportDataSource("Viaticos", col));
                    reportInformeComision.LocalReport.ReportPath = @"C:\Users\Best\Documents\SacIntegrado\SacIntegrado\OficiosComision\Report1.rdlc";
                    reportInformeComision.RefreshReport();
                }
            
            catch (Exception exx)
            {
                MessageBox.Show(exx.Message);
            }
            
        }
        private void btnCerrarReporte(object sender, RoutedEventArgs e)
        {
            ocOficios.Clear();
            col.Clear();
            llenaDtgOficios(usuario);
            reporteOficios.Visibility = Visibility.Collapsed;
            btnCerrarInforme.Visibility = Visibility.Collapsed;
        }
        private void subirServidor(object sender, RoutedEventArgs e)
        {
            string path = "";
            Microsoft.Win32.OpenFileDialog fDialog = new Microsoft.Win32.OpenFileDialog();
            fDialog.Title = "Attach customer proposal document";
            fDialog.Filter = "Doc Files|*.doc|Docx File|*.docx|PDF doc|*.pdf|Excel Files| *.xls;*.xlsx";
            fDialog.InitialDirectory = @"C:\";
            if (fDialog.ShowDialog() == true)
            {
                fileName = System.IO.Path.GetFileName(fDialog.FileName);
                path = fDialog.FileName;
                txtRuta.Text = path;
            }
            lblRuta.Visibility = Visibility.Visible;
            
            //Guarda en el servidor-------------------------------------------------------------------------------------
            WebClient client = new WebClient();
            //NetworkCredential nc = new NetworkCredential();
            //ICredentials credentials = CredentialCache.DefaultCredentials;         
            Uri addy = new Uri(@"C:\UploadDocs\" + fileName);
            //Uri addy = new Uri(@"http:\\172.16.9.172\\UploadDocs\" + fileName);
            //Uri addy = new Uri(@"http:\\172.16.9.172\\C:\UploadDocs\" + fileName);
            //Uri addy = new Uri(@"\\172.16.9.172\UploadDocs\"+fileName);
            //NetworkCredential credential = credentials.GetCredential(addy, "Basic");
            //lblRuta.Visibility = Visibility.Collapsed;            
            //client.Credentials = credential;
            //client.Credentials = CredentialCache.DefaultCredentials;
            byte[] arrReturn = client.UploadFile(addy, "POST",  txtRuta.Text = path);
            MessageBox.Show("Se Guardo Correctamente ");
            lblRuta.Content = addy;
        }
        private void dtgViaticosComproba_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViaticoClase of = dtgViaticosComproba.SelectedItem as ViaticoClase;
            txtclasifiCompro.Text = of.nomPartida;
            txtConceptoCompro.Text = of.viatico;
        }
        private void Sintesis(object sender, RoutedEventArgs e)
        {
            sintesisVentana.IsOpen = true;
        }
        private void cerrarSintesisVentana(object sender, MouseButtonEventArgs e)
        {
            sintesisVentana.IsOpen = false;
            txtRuraArchivo.Text = "";
            txtSintesis.Text = "";
            dtgOficos.SelectedIndex = -1;
        }
        private void hanover(object sender, MouseEventArgs e)
        {
            if (lblcerrarSintesis.IsMouseOver == false)
            {       
                lblcerrarSintesis.Background = new SolidColorBrush(Colors.White);
                lblcerrarSintesis.Foreground = new SolidColorBrush(Colors.Black);
            }         
        }
        private void color(object sender, MouseEventArgs e)
        {
            lblcerrarSintesis.Background = new SolidColorBrush(Colors.LightBlue);
            lblcerrarSintesis.Foreground = new SolidColorBrush(Colors.White);       
        }
        private void Label_MouseEnter_1(object sender, MouseEventArgs e)
        {
            lblCerrarComproba.Background = new SolidColorBrush(Colors.LightBlue);
            lblCerrarComproba.Foreground = new SolidColorBrush(Colors.White);  
        }
        private void Label_MouseLeave_1(object sender, MouseEventArgs e)
        {
            if (lblcerrarSintesis.IsMouseOver == false)
            {
                lblCerrarComproba.Background = new SolidColorBrush(Colors.White);
                lblCerrarComproba.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void lblCerrarPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            lblCerrarPopup.Background = new SolidColorBrush(Colors.LightBlue);
            lblCerrarPopup.Foreground = new SolidColorBrush(Colors.White);  
        }
        private void lblCerrarPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            if (lblcerrarSintesis.IsMouseOver == false)
            {
                lblCerrarPopup.Background = new SolidColorBrush(Colors.White);
                lblCerrarPopup.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void btnExaminarSintesis_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            
            Microsoft.Win32.OpenFileDialog fDialog = new Microsoft.Win32.OpenFileDialog();
            fDialog.Title = "Attach customer proposal document";
            fDialog.Filter = "Word Files|*.doc;*.docx|PDF doc|*.pdf";
            fDialog.InitialDirectory = @"C:\";
            if (fDialog.ShowDialog() == true)
            {
                fileName = System.IO.Path.GetFileName(fDialog.FileName);
                path = fDialog.FileName;
                txtRuraArchivo.Text = path;
            }
            lblRuta.Visibility = Visibility.Visible;

            //Guarda en el servidor-------------------------------------------------------------------------------------
            WebClient client = new WebClient();
            //NetworkCredential nc = new NetworkCredential();
            //ICredentials credentials = CredentialCache.DefaultCredentials;         
            Uri addy = new Uri(@"C:\UploadDocs\" + fileName);
            RutaSintesis=addy.ToString();
            //Uri addy = new Uri(@"http:\\172.16.9.172\\UploadDocs\" + fileName);
            //Uri addy = new Uri(@"http:\\172.16.9.172\\C:\UploadDocs\" + fileName);
            //Uri addy = new Uri(@"\\172.16.9.172\UploadDocs\"+fileName);
            //NetworkCredential credential = credentials.GetCredential(addy, "Basic");
            //lblRuta.Visibility = Visibility.Collapsed;            
            //client.Credentials = credential;
            //client.Credentials = CredentialCache.DefaultCredentials;
            byte[] arrReturn = client.UploadFile(addy, "POST", txtRuraArchivo.Text = path);
            MessageBox.Show("Se Guardo Correctamente ");
            lblRuta.Content = addy;
        }

        private void btngrabarSintesis_Click(object sender, RoutedEventArgs e)
        {
            if (txtSintesis.Text == "" && txtRuraArchivo.Text == "")
            {
                MessageBox.Show("Los Campos no deben estar Vacios");
            }
            else 
            {
                Table<Sintesis> sinte = miBD.GetTable<Sintesis>();
                Sintesis tbSintesis = new Sintesis();
                tbSintesis.idSintesis = 0;
                tbSintesis.comisionado = lblComiSinte.Content.ToString();
                tbSintesis.nomArchivo = fileName;
                tbSintesis.ruta = RutaSintesis;
                tbSintesis.sintesis1 = txtSintesis.Text;
                tbSintesis.idOficiosComi = idOfiComi;
                sinte.InsertOnSubmit(tbSintesis);
                sinte.Context.SubmitChanges();
                MessageBox.Show("Seguardo Correctamente");
                lblComiSinte.Content = "--";
                txtRuraArchivo.Text = "";
                txtSintesis.Text = "";
                sintesisVentana.IsOpen = false;
            }
        }
        public bool validar() {
            bool algo = true;
            if(solicitante.Text==""){
                MessageBox.Show("Selecciona una persona Comisionada");
                algo = false;
            }else if(proyectPresup.Text==""){
                MessageBox.Show("Selecciona un Proyecto ");
                algo = false;
            }else if(dptoBenef.Text==""){
                MessageBox.Show("Selecciona un Departamento ");
                algo = false;
            }else if(txtEmpresa.Text==""){
                MessageBox.Show("Escribir Nombre de la Enpresa ");
                algo = false;
            }else if(txtEstado.Text==""){
                MessageBox.Show("Selecciona un Estado");
                algo = false;
            }
            else if (cmbTransporte.SelectedIndex==-1)
            {
                MessageBox.Show("Indica el Tipo de transporte");
                algo = false;
            }
            else if (txthrInicio.Text == "")
            {
                MessageBox.Show("Indica la Hora de inicio del la Comison ");
                algo = false;
            }
            else if (txthrFin.Text == "")
            {
                MessageBox.Show("Indica la Hora de termino del la Comison ");
                algo = false;
            }
            else if (txthrInicioEvent.Text == "")
            {
                MessageBox.Show("Indica la Hora de inicio del Evento ");
                algo = false;
            }
            else if (txthrFinEvent.Text == "")
            {
                MessageBox.Show("Indica la Hora de termino del Evento ");
                algo = false;
            }
            else if (txtObje.Text == "")
            {
                MessageBox.Show("Escribe el Objetivo de la Comision ");
                algo = false;
            }
            else if(cmbTipo.SelectedIndex==-1){
                MessageBox.Show("Indica el tipo del la Comison ");
                algo = false;
            }
            else if (fechInicio.Text=="")
            {
            MessageBox.Show("Indica la Fecha de inicio del la Comison ");
                algo = false;
            }
            else if (fechTermino.Text == "")
            {
                MessageBox.Show("Indica la Fecha de Termino del la Comison ");
                algo = false;
            }
            return algo;
        }
    }
}

