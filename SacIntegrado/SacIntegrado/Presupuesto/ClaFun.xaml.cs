using SacIntegrado.Presupuesto;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.SqlClient;
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

namespace SacIntegrado
{
    /// <summary>
    /// Lógica de interacción para ClaFun.xaml
    /// </summary>
    /// 

    public class ClaFuncional
    {
        public int idClasifFuncional { get; set; }
        public String nombre { get; set; }
        public String clave { get; set; }
        public int anio { get; set; }
        public String fechaRegistro { get; set; }
        public String personaRegistro { get; set; }
        public int vigente { get; set; }
    }
    public partial class ClaFun : Page
    {
       
        //SqlConnection con = new SqlConnection();
        Db con2 = new Db();
        String nombreU;
        public ClaFun(String user, int id,String nombre)
        {

            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user,id,nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuarioname.Content = nombre;
            nombreU = nombre;
            //con.ConnectionString = "Data Source=(local);Initial Catalog=sac;Integrated Security=True";
            ConsultaFuncional();
            llenaAnio();
        }

      

        public void llenaAnio() {
            int anioA = DateTime.Today.Year - 1;
            int anioP = DateTime.Today.Year;
            int anioS = DateTime.Today.Year + 1;
            String[] anio = {""+anioA,""+anioP,""+anioS};
            Canio.ItemsSource = anio;
          
        }

        public void ConsultaFuncional()
        {


           
            var res = from clasificador in con2.ClasificadorFuncional
                      select clasificador;
            TablaClasificador.ItemsSource = res;

        }

        //private void menuCerrarSesion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    // TODO: Agregar implementación de controlador de eventos aquí.
        //    MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if (r == MessageBoxResult.Yes)
        //    {
        //        InicioLogin inic = new InicioLogin();
        //        this.NavigationService.Navigate(inic);
        //        NavigationCommands.BrowseBack.InputGestures.Clear();
        //        NavigationCommands.BrowseForward.InputGestures.Clear();
        //    }
        //    else { }
        //}

        private void Bnuevo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Table<ClasificadorFuncional> tf = con2.GetTable<ClasificadorFuncional>();
                ClasificadorFuncional cf = new ClasificadorFuncional();
                cf.Nombre = Tnombre.Text;
                cf.Clave = Tclave.Text;
                cf.Anio = Convert.ToInt32(Canio.Text);
                cf.PersonaRegistro = nombreU;
                cf.Fecha = DateTime.Today.Date;
                cf.vigente = CHvigente.IsChecked;
                tf.InsertOnSubmit(cf);
                tf.Context.SubmitChanges();
                ConsultaFuncional();
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);

                return;

            }

        }

        private void Bborrar_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult r = MessageBox.Show("¿Estas seguro de eliminar?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                try
                {
                    ClasificadorFuncional claSelect;
                    claSelect = TablaClasificador.SelectedItem as ClasificadorFuncional;
                    var eliCla = (from p in con2.ClasificadorFuncional
                                  where p.idClasifFuncional == claSelect.idClasifFuncional
                                  select p).Single();
                    con2.ClasificadorFuncional.DeleteOnSubmit(eliCla);
                    con2.SubmitChanges();
                    MessageBox.Show("Registro eliminado");
                    TablaClasificador.Items.Refresh();
                    ConsultaFuncional();
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
            try{
            ClasificadorFuncional claSelect;
            claSelect = TablaClasificador.SelectedItem as ClasificadorFuncional;
            var modifCla = (from p in con2.ClasificadorFuncional
                          where p.idClasifFuncional == claSelect.idClasifFuncional
                          select p).Single();
            modifCla.Nombre = claSelect.Nombre;
            modifCla.Clave = claSelect.Clave;
            modifCla.Anio = claSelect.Anio;
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

        //private void MenuPoliza_Click(object sender, System.Windows.RoutedEventArgs e)
        //{

        //    this.NavigationService.Navigate(new Poliza(nombreU));
        //}

        //private void MenuItem_ClasProgramatico(object sender, RoutedEventArgs e)
        //{
        //    this.NavigationService.Navigate(new ClasifiProgra(nombreU));
        //}

        //private void MenuItem_ClasFuncional(object sender, RoutedEventArgs e)
        //{
        //    this.NavigationService.Navigate(new ClaFun(nombreU));
        //}
    }
}
