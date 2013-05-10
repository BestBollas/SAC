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
using System.Data.Linq;

namespace SacIntegrado.Presupuesto
{
    /// <summary>
    /// Lógica de interacción para altaTipoGasto.xaml
    /// </summary>
    /// 

    public partial class altaTipoGasto : Page
    {
        Db con2 = new Db();
        String nombreU;
        int bandera = 0;
        int id_Empleado=0;
        string fechRegistro = DateTime.Today.ToShortDateString();
        ObservableCollection<TipoGastoClass> octipGasto = new ObservableCollection<TipoGastoClass>();
        public altaTipoGasto(String user, int id, String nombre)
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
            else
            {

            }
        }

        public void llenaAnio()
        {
            int anioA = DateTime.Today.Year - 1;
            int anioP = DateTime.Today.Year;
            int anioS = DateTime.Today.Year + 1;
            String[] anio = { "" + anioA, "" + anioP, "" + anioS };
            Canio.ItemsSource = anio;

        }

        //Metodo para llenar la tabla
        public void llenarTabla()
        {
            var query = from x in con2.TipoGasto
                        from emp in con2.Empleado
                        where x.idEmpleado == emp.idEmpleado
                        select new { x, emp };
            string vigenteTab = "";
            foreach (var i in query)
            {
                if (i.x.vigente == true)
                {
                    vigenteTab = "Si";
                }
                else
                {
                    vigenteTab = "No";
                }
                octipGasto.Add(new TipoGastoClass { idTG = i.x.idTG.Value, nombreTG = i.x.nombreTG, clavePresupuestal = i.x.clavePresupuestal, anioAplica = i.x.anioAplica.Value, fechaReg = Convert.ToString(i.x.fechaReg), Empleado = i.emp.Nombre, vigenteTab = vigenteTab });
                dtgTG.ItemsSource = octipGasto;
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
                    TipoGastoClass tabTG = dtgTG.SelectedItem as TipoGastoClass;
                    var actualizar = (from a in con2.TipoGasto
                                      where a.idTG == tabTG.idTG
                                      select a).Single();
                    if (actualizar.vigente == true)
                    {

                        actualizar.vigente = false;
                        MessageBox.Show("El Recurso fue Desactivado");

                    }
                    else
                    {
                        actualizar.vigente = true;
                        MessageBox.Show("El Recurso fue Activado");
                    }

                    con2.SubmitChanges();
                    bandera = 0;
                    octipGasto.Clear();
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

                if (txtNombre.Text == "")
                {
                    MessageBox.Show("Ingresar Nombre");
                }
                else if (txtClave.Text == "")
                {
                    MessageBox.Show("Ingresar Clave");
                }
                else if (Canio.Text == "")
                {
                    MessageBox.Show("Ingresar Año");
                }
                else if (CHvigente.IsChecked == false)
                {
                    MessageBox.Show("La vigencia esta Desactivada");
                }
                else
                {
                    Table<TipoGasto> tablaTG = con2.GetTable<TipoGasto>();
                    TipoGasto tTG = new TipoGasto();
                    tTG.idTG = 0;
                    tTG.nombreTG = txtNombre.Text;
                    tTG.clavePresupuestal = txtClave.Text;
                    tTG.anioAplica = Convert.ToInt32(Canio.Text);
                    tTG.fechaReg = Convert.ToDateTime(fechRegistro);
                    tTG.idEmpleado = id_Empleado;
                    tTG.vigente = CHvigente.IsChecked;
                    tablaTG.InsertOnSubmit(tTG);
                    tablaTG.Context.SubmitChanges();
                    octipGasto.Clear();
                    llenarTabla();
                    MessageBox.Show("Se Inserto Correctamente");
                   
                }
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);

                return;

            }
        }

        private void dtgTG_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bandera = 1;
        }
    }
}
