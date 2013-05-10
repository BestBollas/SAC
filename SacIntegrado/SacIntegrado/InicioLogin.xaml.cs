using System;
using System.Collections.Generic;
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
using System.Globalization;

namespace SacIntegrado
{
    /// <summary>
    /// Lógica de interacción para Page1.xaml
    /// </summary>
    public partial class InicioLogin : Page
    {
        Db con = new Db();
        

        public InicioLogin()
        {
            InitializeComponent();
            this.ActualHeight.Equals(System.Windows.SystemParameters.PrimaryScreenWidth);
           
			//this.NavigationService.Navigated+=new NavigatedEventHandler(NavigationService_Navigated);
        }
		
		private void accesar_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Menu.xaml", UriKind.RelativeOrAbsolute));
            

            if(user.Text.Equals("")){//SI USER ESTA VACIO
				MessageBoxResult r = MessageBox.Show("Identifiquese por favor", "Recuerde...", MessageBoxButton.OK, MessageBoxImage.Error);
				user.Focus();
			}
			else{
				if(pass.Password.Equals("")){
					MessageBoxResult r = MessageBox.Show("Identifiquese por favor", "Recuerde...", MessageBoxButton.OK, MessageBoxImage.Error);
					pass.Focus();
				}
				else{
                    var usuario = from em in con.Empleado
                               where em.Usuario == user.Text.Trim() &&
                                     em.Password == pass.Password.Trim()
                               select em;
                    try
                    {
                        int usuarioInt=usuario.SingleOrDefault().idEmpleado;
                        String nombre = usuario.SingleOrDefault().Nombre;
                        MenuIU m = new MenuIU(user.Text.Trim(),usuarioInt,nombre);
                        this.NavigationService.Navigate(m);
                    }
					catch(System.NullReferenceException){
						MessageBoxResult r = MessageBox.Show("No tiene permisos para acceder", "Advertencia...", MessageBoxButton.OK, MessageBoxImage.Stop);
					}
				}
			}
        }

        void Page_KeyDown(object sender, KeyEventArgs e){
			
            if(e.Key==Key.BrowserBack || e.Key==Key.Back){
                e.Handled=true;
            }
        }

		private void noBack(object sender, System.Windows.Input.KeyEventArgs e)
		{
			// TODO: Agregar implementación de controlador de eventos aquí.
			if(e.Key==Key.BrowserBack || e.Key==Key.Back){
				e.Handled=true;
			}
		}

		private void stopBack(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Agregar implementación de controlador de eventos aquí.
			this.NavigationService.RemoveBackEntry();
		}

        private void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            this.NavigationService.RemoveBackEntry();
        }

    }
}
