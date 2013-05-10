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

	public partial class ClasifiProgra: Page
	{

        SqlConnection con = new SqlConnection();
        Db con2 = new Db();
        String nombreU;
		public ClasifiProgra(String usuario,int id,String nombre)
		{

            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, usuario,id,nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuarioname.Content = nombre;
            nombreU = nombre;
            con.ConnectionString = "Data Source=(local);Initial Catalog=sac;Integrated Security=True";
            ConsultaProgramatico();
            llenaAnio();
		}
		
        //private void menuPrincipal_Click(object sender, RoutedEventArgs e)
        //{
        //this.NavigationService.Navigate(new Menu(nombreU));
        //}

		
        private void menuCerrarSesion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	// TODO: Agregar implementación de controlador de eventos aquí.
			MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if(r==MessageBoxResult.Yes){
				InicioLogin inic = new InicioLogin();
            	this.NavigationService.Navigate(inic);
				NavigationCommands.BrowseBack.InputGestures.Clear();
				NavigationCommands.BrowseForward.InputGestures.Clear();
			}
			else{}
        }
		

        public void llenaAnio()
        {
            int anioA = DateTime.Today.Year - 1;
            int anioP = DateTime.Today.Year;
            int anioS = DateTime.Today.Year + 1;
            String[] anio = { "" + anioA, "" + anioP, "" + anioS };
            Canio.ItemsSource = anio;

        }

        public void ConsultaProgramatico()
        {
            var res = from clasificador in con2.ClasificadorProgramatico
                      select clasificador;
            foreach(var x in res){
                MessageBox.Show("Clave: "+x.Clave);
            }
             TablaClasificador.ItemsSource = res;

        }

        private void Bnuevo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Table<ClasificadorProgramatico> tf = con2.GetTable<ClasificadorProgramatico>();
                ClasificadorProgramatico cf = new ClasificadorProgramatico();
                cf.Nombre = Tnombre.Text;
                cf.Clave = Tclave.Text;
                cf.anio = Convert.ToInt32(Canio.Text);
                cf.personaRegistro = nombreU;
                cf.fecha = DateTime.Today.Date;
                cf.vigente = CHvigente.IsChecked;
                tf.InsertOnSubmit(cf);
                tf.Context.SubmitChanges();
                ConsultaProgramatico();
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
                    ClasificadorProgramatico claSelect;
                    claSelect = TablaClasificador.SelectedItem as ClasificadorProgramatico;
                    var eliCla = (from p in con2.ClasificadorProgramatico
                                  where p.idClasificadorPro == claSelect.idClasificadorPro
                                  select p).Single();
                    con2.ClasificadorProgramatico.DeleteOnSubmit(eliCla);
                    con2.SubmitChanges();
                    MessageBox.Show("Registro eliminado");
                    TablaClasificador.Items.Refresh();
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
                claSelect = TablaClasificador.SelectedItem as ClasificadorProgramatico;
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