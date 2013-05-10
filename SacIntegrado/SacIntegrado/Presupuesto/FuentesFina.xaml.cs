using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
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

namespace SacIntegrado.Presupuesto
{
    /// <summary>
    /// Lógica de interacción para FuentesFina.xaml
    /// </summary>
    public partial class FuentesFina : Page
    {
        Db con2 = new Db();
        String nombreU;
        int id_Empleado;
        int bandera=0;
        string fechRegistro = DateTime.Today.ToShortDateString();
        ObservableCollection<RecursoC> recur = new ObservableCollection<RecursoC>();
        public FuentesFina() { 
        
        }
        public FuentesFina(String user, int id, String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            nombreU = nombre;
            id_Empleado = id;
            llenaAnio();
            llenarTabla();
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
            else { 
            
            }
        }

        //Metodo para llenar la tabla
        public void llenarTabla() {
            var query = from x in con2.Recurso
                        from emp in con2.Empleado
                        where x.idEmpleado == emp.idEmpleado
                        select new { x, emp };
            string vigenteTab = "";
            foreach(var i in query){
                
                if (i.x.Vigente == true)
                {
                    vigenteTab = "Si";
                }
                else {
                    vigenteTab = "No";
                }
                recur.Add(new RecursoC { idRecurso = i.x.idRecurso, Nombre = i.x.Nombre, ClavePresupuestal = i.x.ClavePresupuestal.ToString(), FechaRegistro = Convert.ToDateTime(i.x.FechaRegistro), AnioAplica = i.x.AnioAplica.Value, idEmpleadó = i.x.idEmpleado.Value, NombreEmpleado = i.emp.Nombre, Vigente = i.x.Vigente.Value, SaldoInicial = i.x.SaldoInicial.Value, SaldoFinal = i.x.SaldoFin.Value, Observaciones = i.x.Observaciones, vigenteTab = vigenteTab });
                TabFinan.ItemsSource = recur;
            }
            
            
        
        }

        //Metodo para llenar el comboBox de los años
        public void llenaAnio()
        {
            try
            {
                MessageBox.Show("Entra al metodo del Año");
                int anioA = DateTime.Today.Year - 1;
                int anioP = DateTime.Today.Year;
                int anioS = DateTime.Today.Year + 1;
                String[] anio = { "" + anioA, "" + anioP, "" + anioS };
                Canio.ItemsSource = anio;
            }catch(Exception e){
            
            }
        }

        private void btnActDes_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (bandera == 0)
                {
                    MessageBox.Show("Seleccionar Recurso que desea desactivar");
                }
                else
                {
                    RecursoC tabRecu = TabFinan.SelectedItem as RecursoC;
                    var actualizar = (from a in con2.Recurso
                                      where a.idRecurso == tabRecu.idRecurso
                                      select a).Single();
                    if (actualizar.Vigente == true)
                    {

                        actualizar.Vigente = false;
                        MessageBox.Show("El Recurso fue Desactivado");

                    }
                    else
                    {
                        actualizar.Vigente = true;
                        MessageBox.Show("El Recurso fue Activado");
                    }

                    con2.SubmitChanges();
                    bandera = 0;
                    recur.Clear();
                    llenarTabla();

                }
            }
            catch (Exception ex) { }
        }

        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //MessageBox.Show("nombre: " + txtNombre.Text + "clave: " + txtClave.Text + "Año: " + Canio.Text + "Vigencia: " + CHvigente.IsChecked);
             
                if(txtNombre.Text==""){
                    MessageBox.Show("Ingresar Nombre");
                }
                else if (txtClave.Text == "") {
                    MessageBox.Show("Ingresar Clave");
                }
                else if (Canio.Text=="")
                {
                    MessageBox.Show("Ingresar Año");
                }
                else if (CHvigente.IsChecked == false)
                {
                    MessageBox.Show("La vigencia esta Desactivada");
                }
                else
                {
                    Table<Recurso> tf = con2.GetTable<Recurso>();
                    Recurso Re = new Recurso();
                    Re.idRecurso = 0;
                    Re.Nombre = txtNombre.Text;
                    Re.ClavePresupuestal = txtClave.Text;
                    Re.FechaRegistro = Convert.ToDateTime(fechRegistro);
                    Re.AnioAplica = Convert.ToInt32(Canio.Text);
                    Re.idEmpleado = id_Empleado;
                    Re.Vigente = CHvigente.IsChecked;
                    Re.SaldoInicial = 0;
                    Re.SaldoFin = 0;
                    Re.Observaciones = "";
                    tf.InsertOnSubmit(Re);
                    tf.Context.SubmitChanges();
                    MessageBox.Show("Se Inserto Correctamente");
                    recur.Clear();
                    llenarTabla();
                }
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);

                return;

            }
        }

        private void TabFinan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bandera = 1;           
        }
    }
}
