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

namespace SAC
{
	public partial class Requi
	{
		public Requi()
		{
			this.InitializeComponent();

			// A partir de este punto se requiere la inserción de código para la creación del objeto.
		}

        private void menuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            Menu m = new Menu();
            this.NavigationService.Navigate(m);
        }

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
	}
}