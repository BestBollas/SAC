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
using System.Data.Linq;

namespace SacIntegrado.Presupuesto
{
    /// <summary>
    /// Lógica de interacción para pre_presupuesto.xaml
    /// </summary>
    public partial class pre_presupuesto : Page
    {
        private ObservableCollection<ProyectoC> misProyectos = new ObservableCollection<ProyectoC>();
        private ObservableCollection<PartidaProyectoC> misPartidas = new ObservableCollection<PartidaProyectoC>();
        ObservableCollection<Empleado> listaEmpleado = new ObservableCollection<Empleado>();
        Db re = new Db();
        String nombreU;
        String nomResProy;
        public int idProyecto { get; set; }
        int Bandera = 0;
        int porcentaje;
        public pre_presupuesto(String user, int id, String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            nombreU = nombre;
            parti.IsExpanded = false;
            parti.IsEnabled = false;
            listaEmple.Visibility = Visibility.Hidden;
            
        }

        public void consultaProyecto(String nombr)
        {
            var pro = (from p in re.Proyecto
                       from e in re.Empleado
                       from d in re.Departamento
                       from cf in re.ClasificadorFuncional
                       from cp in re.ClasificadorProgramatico
                       from ap in re.ActividadPoa
                       from r in re.Recurso
                       from gsa in re.GastoSocialAdmin
                       from tg in re.TipoGasto
                       from area in re.Area
                       where e.Nombre==nombr 
                             && p.idActPoa == ap.idActPoa 
                             && p.idClasifFuncional == cf.idClasifFuncional
                             && p.idRecurso == r.idRecurso
                             && p.idResponsable == e.idEmpleado
                             && p.idClasifProgramatico == cp.idClasificadorPro
                             && p.idDepto == d.idDepto
                             && p.idGSA == gsa.idGSA
                             && p.idTG == tg.idTG
                             && p.idArea == area.idArea

                       select new
                       {
                           p.idProyecto,
                           p.Nombre,
                           p.justificacion,
                           p.anioAplica,
                           e.idEmpleado,
                           d.idDepto,
                           cf.idClasifFuncional,
                           cp.idClasificadorPro,
                           ap.idActPoa,
                           r.idRecurso,
                           nres = e.Nombre,
                           d.NombreDepto,
                           ncf = cf.Nombre,
                           ncp = cp.Nombre,
                           nap = ap.Nombre,
                           nr = r.Nombre,
                           gsa.nombreGSA,
                           tg.nombreTG,
                           area.idArea,
                           area.nomArea,
                           tg.idTG,
                           gsa.idGSA

                       }).Distinct();
            misProyectos.Clear();

            foreach (var p in pro)
            {

                misProyectos.Add(new ProyectoC
                {
                    idProyecto = p.idProyecto,
                    nombreP = p.Nombre,
                    justificacion = p.justificacion,
                    idResponsable = p.idEmpleado,
                    idDepto = p.idDepto,
                    idActPoa = p.idActPoa,
                    idClasifFuncional = p.idClasifFuncional,
                    idClasifProgramatico = p.idClasificadorPro,
                    idRecurso = p.idRecurso,
                    nombreCF = p.ncf,
                    nombreCP = p.ncp,
                    nombreD = p.NombreDepto,
                    nombrePoa = p.nap,
                    nombreR = p.nr,
                    nombreRes = p.nres,
                    anioAplica = p.anioAplica.Value,
                    nombreGSA = p.nombreGSA,
                    nombreTG = p.nombreTG,
                    idArea = p.idArea,
                    nombreArea = p.nomArea,
                    porciento=0,
                    idGSA=p.idGSA,
                    idTG=p.idTG.Value
                });
            }

            if (pro.Count() == 0)
            {
                MessageBox.Show("No tiene Proyectos Asignados");
                
            }
            else
            {
                tablaPre.ItemsSource = misProyectos;
                txtnombre.Text = "";
            }
        }

        public void cargarPartidas(int idp)
        {
            var pe = from r in re.PartidaProyecto
                     from c in re.CuentaEnc
                     where r.idCuenta == c.IdCuenta && r.idProyecto == idp
                     select new { r.idCtaProy, r.idCuenta, r.idPeriodo, r.saldoFin, r.saldoInicial, c.Cuenta, c.Nombre };

            misPartidas.Clear();
            foreach (var ele in pe)
            {
                misPartidas.Add(new PartidaProyectoC { idCtaProy = ele.idCtaProy, idCuenta = ele.idCuenta.Value, Cuenta = ele.Cuenta, Nombre = ele.Nombre, idPeriodo = ele.idPeriodo.Value, saldoInicial = ele.saldoInicial.Value, saldoFin = ele.saldoFin.Value, });
                
                tablaPartida.ItemsSource = misPartidas;
                
            }
            
            
        }
        public void cargarPorcentaje()
        {
            
            ProyectoC pro = tablaPre.SelectedItem as ProyectoC;
            int indexTabla = 0;
            int anio = pro.anioAplica + 1;
            foreach (var es in misProyectos)
            {
                if (es.nombreP.Equals(pro.nombreP))
                {
                    break;
                }
                indexTabla++;
            }
            porcentaje = misProyectos.ElementAt(indexTabla).porciento;
            if (porcentaje >= 101)
            {
                MessageBox.Show("No puedes Agregar un porsentaje mayor a 100 %");
            }
            else
            {
                tablaPre.Items.Refresh();
                ///////Se utiliza para obtener el contenido del datagrid y se inserta en la tabla con nuevos valores////////////////////////////////////////////////////////
                Table<Proyecto> tabpro = re.GetTable<Proyecto>();
                Proyecto proyect = new Proyecto();
                proyect.Nombre = pro.nombreP;
                proyect.idResponsable = pro.idResponsable;
                proyect.idRecurso = pro.idRecurso;
                //proyect.idsOtrosClasificadores = 0;
                proyect.idDepto = pro.idDepto;
                proyect.idActPoa = pro.idActPoa;
                proyect.idClasifFuncional = pro.idClasifFuncional;
                proyect.idClasifProgramatico = pro.idClasifProgramatico;
                proyect.anioAplica = anio;
                proyect.justificacion = pro.justificacion;
                proyect.idGSA = pro.idGSA;
                proyect.idTG = pro.idTG;
                proyect.idArea = pro.idArea;
                proyect.saldoInicial = pro.saldoInicial;
                proyect.saldoFinal = pro.saldoFinal;
                tabpro.InsertOnSubmit(proyect);
                tabpro.Context.SubmitChanges();

                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                int idmaximo = 0;

                var idmax = (from idmas in re.Proyecto
                             select idmas.idProyecto).Max();
                var pe = from r in re.PartidaProyecto
                         from c in re.CuentaEnc
                         where r.idCuenta == c.IdCuenta && r.idProyecto == idProyecto
                         select new { r.idCtaProy, r.idCuenta, r.idPeriodo, r.saldoFin, r.saldoInicial, c.Cuenta, c.Nombre, r.idProyecto };



                idmaximo = idmax;


                foreach (var x in pe)
                {

                    int por = (Convert.ToInt32(x.saldoFin) * porcentaje) / 100;
                    int saldo = por + Convert.ToInt32(x.saldoFin);
                    Table<PartidaProyecto> tabPartida = re.GetTable<PartidaProyecto>();
                    PartidaProyecto partida = new PartidaProyecto();
                    partida.idCtaProy = 0;
                    partida.idCuenta = x.idCuenta;
                    partida.idProyecto = idmaximo;
                    partida.idPeriodo = x.idPeriodo;
                    partida.saldoInicial = saldo;
                    partida.saldoFin = saldo;
                    tabPartida.InsertOnSubmit(partida);
                    tabPartida.Context.SubmitChanges();


                }

                Bandera = 1;
                misProyectos.Clear();
                misPartidas.Clear();
                consultaProyecto(nomResProy);
                txtnombre.Text = "";
                idProyecto = 0;

                MessageBox.Show("Se inserto correctamente");

            }

        }
        private void cerrarSesion(object sender, MouseButtonEventArgs e)
        {
            // TODO: Agregar implementación de controlador de eventos aquí.
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
                NavigationCommands.BrowseBack.InputGestures.Clear();
                NavigationCommands.BrowseForward.InputGestures.Clear();
            }
            else { }
        }

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    nomResProy=txtnombre.Text.Trim();
        //    consultaProyecto(nomResProy);
            
        //}

        private void tablaPre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          
            if (Bandera==0)
            {
                
                if (tablaPre.SelectedIndex==-1)
                {
                    misPartidas.Clear();
                    parti.IsExpanded = false;
                    parti.IsEnabled = false;
                }
                else
                {
                    ProyectoC pre = tablaPre.SelectedItem as ProyectoC;
                    idProyecto = pre.idProyecto;
                    
                    cargarPartidas(idProyecto);
                    parti.IsExpanded = true;
                    parti.IsEnabled = true;
                }
            }
            else {
                Bandera = 0;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Bandera en el boton porcentaje " + Bandera);
            cargarPorcentaje();   
        }

        private void empleado(object sender, KeyEventArgs e)
        {
            listaEmple.Visibility = Visibility.Visible;
            String emple = txtnombre.Text.Trim();
            var ctas = from ele in re.Empleado
                       where ele.Nombre.Contains(emple) 
                       select ele;
            listaEmpleado.Clear();
            foreach (var el in ctas)
            {
                listaEmpleado.Add(new Empleado { idEmpleado = el.idEmpleado, Nombre = el.Nombre});
                listaEmple.ItemsSource = listaEmpleado;
                listaEmple.DisplayMemberPath = "Nombre";
                listaEmple.SelectedValuePath = "idEmpleado";
            }

        }

        private void empleadoNombre(object sender, SelectionChangedEventArgs e)
        {
            if (listaEmple.SelectedValue == null) return;
            int idempleado = (int)listaEmple.SelectedValue;
            
            Empleado emp = listaEmple.SelectedItem as Empleado;
            txtnombre.Text = emp.Nombre;
            nomResProy = emp.Nombre;
            consultaProyecto(nomResProy);
            listaEmple.Visibility = Visibility.Hidden;
            
            
        }
    }
}
