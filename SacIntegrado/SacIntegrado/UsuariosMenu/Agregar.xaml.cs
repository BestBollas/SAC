using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Linq;
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
    /// Lógica de interacción para Agregar.xaml
    /// </summary>
    public partial class Agregar : Page
    {
        Db dc = new Db();
       /* public class menuEmpleado
        {
            public int idUsuarioMenu { get; set; }
            public int idEmpleado { get; set; }
            public int idMenu { get; set; }
        }*/

        public class menuEmpleado {
            public int idMenu { get; set; }
            public String nombre { get; set; }
            public String variable { get; set; }
            public int papa { get; set; }
        }
        
                

        public class empleadoCBO {
            public int idEmpleado { get; set; }
            public String Nombre { get; set; }
        }

        public class menuSelList
        {
            public String itemSeleccionadoBD { get; set; }
        }
        
        private ObservableCollection<menuEmpleado> obcMenusEmpleado = new ObservableCollection<menuEmpleado>();
        private ObservableCollection<menuEmpleado> obcMenuEmpleado2 = new ObservableCollection<menuEmpleado>();
        private ObservableCollection<menuSelList> obcMenuSel = new ObservableCollection<menuSelList>();
        private ObservableCollection<empleadoCBO> obcEmpleado = new ObservableCollection<empleadoCBO>();

        private void llenarListBx2(int idEmplead)
        {
            //Db dc = new Db();
            var consulta = (from m in dc.UsuarioMenu
                            from d in dc.MenuTabla
                            where m.idEmpleado == idEmplead && m.idMenu == d.idMenu
                            select d );
           
            obcMenusEmpleado.Clear();
            foreach (var vFMenuBD in consulta)
            {
                obcMenusEmpleado.Add(new menuEmpleado {idMenu = vFMenuBD.idMenu, nombre = vFMenuBD.Nombre, variable = vFMenuBD.variable, papa = vFMenuBD.Papa.Value});
                
            }
            lista2.ItemsSource = obcMenusEmpleado;
            lista2.DisplayMemberPath = "nombre";
            lista2.SelectedValuePath = "idMenu";
            llenarListBx1(idEmplead);
        }

        private void llenarListBx1(int idEmplead) {
            //DatosConexion dc = new DatosConexion();

            var nom = from m in dc.MenuTabla
                      where (from u in dc.UsuarioMenu
                                               where u.idEmpleado == 1
                                            select u.idMenu).Contains(m.idMenu) && ! (from u in dc.UsuarioMenu
                                                                             where u.idEmpleado == idEmplead
                                                                             select u.idMenu).Contains(m.idMenu) 
            
                      select m;


           /* var subconsulta2 = (from u in dc.UsuarioMenu
                            where u.idEmpleado == idEmplead 
                            select u);
            
            var subconsulta = (from um in dc.UsuarioMenu
                               where um.idEmpleado == 1
                               select um);
            var consulta = (from m in dc.MenuTabla
                            where m.idMenu into subconsulta
                            select m);*/

            obcMenuEmpleado2.Clear();
            
            foreach (var vFMenuBD in nom)
            {
                obcMenuEmpleado2.Add(new menuEmpleado { idMenu = vFMenuBD.idMenu, nombre = vFMenuBD.Nombre, variable = vFMenuBD.variable, papa = vFMenuBD.Papa.Value });

            }
            lista1.ItemsSource = obcMenuEmpleado2;
            lista1.DisplayMemberPath = "nombre";
            lista1.SelectedValuePath = "idMenu";
        
        }

        private void llenarComboEmpleados() {
            
            var consulta2 = (from u in dc.Empleado
                             where u.idEmpleado != 1
                             select u);
            foreach (var vFMenuBD in consulta2)
            {
                obcEmpleado.Add(new empleadoCBO { idEmpleado = vFMenuBD.idEmpleado, Nombre = vFMenuBD.Nombre });

            }
            comboEmpleado.ItemsSource = obcEmpleado;
            comboEmpleado.DisplayMemberPath = "Nombre";
            comboEmpleado.SelectedValuePath = "idEmpleado";
        
        }
        
       
        
        public Agregar(String user,int id,String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            llenarComboEmpleados();
        }

        private void menuRegresar_Click(object sender, RoutedEventArgs e)
        {
            Menu me = new Menu();
            this.NavigationService.Navigate(me);
        }
        private void btnAgregarTodo_Click(object sender, RoutedEventArgs e)
        {
            if (lista1.SelectedIndex != -1)
            {
                Table<UsuarioMenu> usuarioMen = dc.GetTable<UsuarioMenu>();
                UsuarioMenu us = new UsuarioMenu();
                us.idEmpleado = int.Parse(comboEmpleado.SelectedValue.ToString());
                us.idMenu = int.Parse(lista1.SelectedValue.ToString());
                usuarioMen.InsertOnSubmit(us);
                usuarioMen.Context.SubmitChanges();
                llenarListBx2(us.idEmpleado);
                llenarListBx1(us.idEmpleado);
            }
        }

        
              
        private void btnBorrarTodo_Click(object sender, RoutedEventArgs e)
        {
            if (lista2.SelectedIndex != -1)
            {
                Table<UsuarioMenu> usuarioMen = dc.GetTable<UsuarioMenu>();
                UsuarioMenu us = new UsuarioMenu();
                us.idEmpleado = int.Parse(comboEmpleado.SelectedValue.ToString());
                us.idMenu = int.Parse(lista2.SelectedValue.ToString());
                UsuarioMenu consulta = (from um in dc.UsuarioMenu
                                        where um.idMenu == us.idMenu && um.idEmpleado == us.idEmpleado
                                        select um).Single();


                usuarioMen.DeleteOnSubmit(consulta);
                usuarioMen.Context.SubmitChanges();
                llenarListBx2(us.idEmpleado);
                llenarListBx1(us.idEmpleado);
            }
            
        }
        //EVENTO DESDE BLEND PARA CERRAR SESIÓN DESDE LABEL EN VENTANA MENU
        private void menuCerrarSesion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Agregar implementación de controlador de eventos aquí.
            //CONFIRMACIÓN PARA SALIR
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
            }
            else { }
        }

        private void cboEmpleado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idEmpleado = int.Parse(comboEmpleado.SelectedValue.ToString());
            
            llenarListBx2(idEmpleado);
        }

        private void lista1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            int idMenu = int.Parse(lista1.SelectedValue.ToString());
            int idEmpleado = int.Parse(comboEmpleado.SelectedValue.ToString());
            MessageBox.Show(idMenu+"  "+idEmpleado);

        }
    }
}
