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
    /// Lógica de interacción para altaClavePresuArea.xaml
    /// </summary>
    public partial class altaClavePresuArea : Page
    {
        Db con2 = new Db();
        String nombreU;
        
        int id_Empleado = 0;
        string fechRegistro = DateTime.Today.ToShortDateString();
        ObservableCollection<AreaPresu> ocpresuArea = new ObservableCollection<AreaPresu>();
        public altaClavePresuArea(String user, int id, String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            usuario.Content = nombre;
            nombreU = nombre;
            id_Empleado = id;
            llenarTablaArea();
            llenarCmbArea();
        }

        public void llenarTablaArea() {
            var query = from a in con2.Area
                        select a;

            foreach (var i in query) {
                ocpresuArea.Add(new AreaPresu {idArea=i.idArea,nomArea=i.nomArea,idJefe=i.idJefe.Value,clavePresupuestal=i.clavePresupuestal });
            
            }
            dtgPresuArea.ItemsSource = ocpresuArea;
        
        
        }

        public void llenarCmbArea() {
            ocpresuArea.Clear();
            var query = from x in con2.Area
                        select x;
            
            foreach (var i in query)
            {
                ocpresuArea.Add(new AreaPresu { idArea = i.idArea, nomArea = i.nomArea, idJefe = i.idJefe.Value, clavePresupuestal = i.clavePresupuestal });

            }
             cmbAreas.ItemsSource = ocpresuArea;
             cmbAreas.SelectedValuePath = "idArea";
             cmbAreas.DisplayMemberPath = "nomArea";        
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




        private void btnGrabar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AreaPresu cmbA = cmbAreas.SelectedItem as AreaPresu;
                MessageBox.Show("nombre: " + cmbAreas.DisplayMemberPath.ToString() + "clave: " + txtClave.Text+"id Area: "+cmbA.idArea);
                

                var actualizar = (from a in con2.Area
                                  where a.idArea == cmbA.idArea
                                  select a).Single();
                MessageBox.Show("Se.:"+actualizar.nomArea.ToString());
                actualizar.clavePresupuestal = txtClave.Text;
                con2.SubmitChanges();
                txtClave.Text = "";
                ocpresuArea.Clear();
                llenarTablaArea();
                llenarCmbArea();
                MessageBox.Show("Se Inserto Correctamente");
                
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.Message);

                return;

            }
        }

        private void TabFinan_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
