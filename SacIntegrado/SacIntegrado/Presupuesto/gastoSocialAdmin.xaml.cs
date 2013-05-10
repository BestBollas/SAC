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
    /// Lógica de interacción para gastoSocialAdmin.xaml
    ///       
    /// </summary>



    public partial class gastoSocialAdminClass : Page
    {
        Db con2 = new Db();
        String nombreU;
        int bandera = 0;
        int id_Empleado;
        GsaClass tabgsa = new GsaClass();
        string fechRegistro = DateTime.Today.ToShortDateString();
        ObservableCollection<GsaClass> recur = new ObservableCollection<GsaClass>();
        public gastoSocialAdminClass(String user, int id, String nombre)
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

        //Metodo para llenar el comboBox de los años
        public void llenaAnio()
        {
            try
            {
                int anioA = DateTime.Today.Year - 1;
                int anioP = DateTime.Today.Year;
                int anioS = DateTime.Today.Year + 1;
                String[] anio = { "" + anioA, "" + anioP, "" + anioS };
                Canio.ItemsSource = anio;
            }
            catch (Exception e)
            {

            }
        }
        //Metodo para llenar la tabla
        public void llenarTabla()
        {
            var query = from x in con2.GastoSocialAdmin
                        from emp in con2.Empleado
                        where x.idEmpleado==emp.idEmpleado
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
                recur.Add(new GsaClass { idGSA = i.x.idGSA, nombreGSA = i.x.nombreGSA, clavePresu = i.x.clavePresu, anioAplica = i.x.anioAplica.Value, fechaRegistro = Convert.ToString(i.x.fechaRegistro), empleado = i.emp.Nombre,vigenteTab=vigenteTab });
                dtgGaSoAd.ItemsSource = recur;
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
                    tabgsa = dtgGaSoAd.SelectedItem as GsaClass;
                    var actualizar = (from a in con2.GastoSocialAdmin
                                      where a.idGSA == tabgsa.idGSA
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
                    Table<GastoSocialAdmin> tgsa = con2.GetTable<GastoSocialAdmin>();
                    GastoSocialAdmin gsa = new GastoSocialAdmin();
                    gsa.idGSA = 0;
                    gsa.nombreGSA = txtNombre.Text;
                    gsa.clavePresu = txtClave.Text;
                    gsa.anioAplica = Convert.ToInt32(Canio.Text);
                    gsa.fechaRegistro = Convert.ToDateTime(fechRegistro);
                    gsa.idEmpleado = id_Empleado;
                    gsa.vigente = CHvigente.IsChecked;
                    tgsa.InsertOnSubmit(gsa);
                    tgsa.Context.SubmitChanges();
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

        private void dtgGaSoAd_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            bandera = 1;
        }
    }
}
