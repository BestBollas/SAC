using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SacIntegrado
{
    

	public partial class MenuIU: Page
	{

		public MenuIU(String user,int idEmpleado,String nombre)
		{
			InitializeComponent();
            MenuPrincipal.datosMenu.Clear();
            MenuPrincipal.llenaDatos(idEmpleado);
            MenuPrincipal temp = new MenuPrincipal(this, user,idEmpleado,nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
			usuario.Content=nombre;
		}
		public MenuIU()
		{
			this.InitializeComponent();
		}
		
		private void menuCerrarSesion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Agregar implementación de controlador de eventos aquí.
			//CONFIRMACIÓN PARA SALIR
			MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if(r==MessageBoxResult.Yes){
				InicioLogin inic = new InicioLogin();
            	this.NavigationService.Navigate(inic);
			}
		}
	}
}