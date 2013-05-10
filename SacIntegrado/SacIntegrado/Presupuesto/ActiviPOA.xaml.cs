using SacIntegrado.Presupuesto;
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
using System.Data.SqlClient;


namespace SacIntegrado.Presupuesto
{
    /// <summary>
    /// Lógica de interacción para ActiviPOA.xaml
    /// </summary>
    ///     

    public partial class ActiviPOA : Page
    {

        public ActiviPOA() { 
        
        }
        Db con2 = new Db();
        String nombreU;
        public ActiviPOA(String user,int id,String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user,id,nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            nombreU = nombre;
            ConsultaProgramatico();
        }

   
        public void ConsultaProgramatico()
        {
            var res = from dept in con2.Departamento
                      select dept;

            TabPOA.ItemsSource = res;

        }

       

        private void Bborrar_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult r = MessageBox.Show("¿Estas seguro de eliminar?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                try
                {
                    ClasificadorProgramatico claSelect;
                    claSelect = TabPOA.SelectedItem as ClasificadorProgramatico;
                    var eliCla = (from p in con2.ClasificadorProgramatico
                                  where p.idClasificadorPro == claSelect.idClasificadorPro
                                  select p).Single();
                    con2.ClasificadorProgramatico.DeleteOnSubmit(eliCla);
                    con2.SubmitChanges();
                    MessageBox.Show("Registro eliminado");
                    TabPOA.Items.Refresh();
                    ConsultaProgramatico();
                }
                catch (Exception Ex)
                {

                    MessageBox.Show(Ex.Message);

                    return;

                }
            }
            else { return; }



        }

        private void Bgrabar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClasificadorProgramatico claSelect;
                claSelect = TabPOA.SelectedItem as ClasificadorProgramatico;
                var modifCla = (from p in con2.ClasificadorProgramatico
                                where p.idClasificadorPro == claSelect.idClasificadorPro
                                select p).Single();
                modifCla.Nombre = claSelect.Nombre;
                modifCla.Clave = claSelect.Clave;
                modifCla.anio = claSelect.anio;
                modifCla.vigente = claSelect.vigente;
                con2.SubmitChanges();

                MessageBox.Show("Se modifico rorrectamente");
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);

                return;

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string centroCostos = "";
            var query = from x in con2.Parametros
                        select x;
            foreach (var i in query)
            {
                centroCostos = i.centroCostos.Trim() + "-" + txtClave.Text;
                //MessageBox.Show("Costos: " + centroCostos);
            }

            if (txtNombre.Text == "")
            {
                MessageBox.Show("Se necesita un Nombre");
            }
            else if (txtClave.Text == "")
            {
                MessageBox.Show("Se necesita una Clave");
            }
            else {
                string name = "";
                var cuery = from x in con2.Departamento
                            where x.NombreDepto==txtNombre.Text
                            select x;
                foreach (var i in cuery)
                {
                    name = i.NombreDepto;
                    //MessageBox.Show("Nombre de BD: "+name);
                    //MessageBox.Show("Nombre del Txt: " + txtNombre.Text);
                }

                if (name == txtNombre.Text)
                {
                    MessageBox.Show("El Nombre ya Existe");
                }
                else
                {
                    //xmlns:rep="Micro"
                  

                    //MessageBox.Show(centroCostos.Trim() + "-" + txtClave.Text);
                    Table<Departamento> dp = con2.GetTable<Departamento>();
                    Departamento dpto = new Departamento();
                    dpto.idDepto = 0;
                    dpto.NombreDepto = txtNombre.Text;
                    dpto.idJefe = 0;
                    dpto.idArea = 0;
                    dpto.clavePresupuestal = centroCostos;
                    dp.InsertOnSubmit(dpto);
                    dp.Context.SubmitChanges();
                    ConsultaProgramatico();
                    limpiar();
                    MessageBox.Show("Se inserto Correctamente.");
                }
            }
            
                
            
        }
        public void limpiar() {
            try
            {
                txtNombre.Text = "";
                txtClave.Text = "";
            }
            catch (Exception e) { 
            
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            limpiar();
        }
        }
}
