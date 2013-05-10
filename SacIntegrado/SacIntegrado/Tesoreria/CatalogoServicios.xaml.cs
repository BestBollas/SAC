using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
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

namespace SacIntegrado.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para CatalogoServicios.xaml
    /// </summary>
    public partial class CatalogoServicios : Page
    {
        Db conex = new Db();
        ObservableCollection<Servicio> oCServicios = new ObservableCollection<Servicio>();
        private int idServicio = 0;

        public CatalogoServicios(string usuario, int id, string nomb)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, usuario, id, nomb);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            this.usuario.Content = nomb;
            llenaGrid();
        }

        private void limpiar()
        {
            tBCodNom.Text = "";
            tBCodigo.Text = "";
            tBPrecio.Text = "0";
            idServicio = 0;
            llenaGrid();
        }

        private void llenaGrid()
        {
            oCServicios.Clear();
            var serv = from se in conex.ServicioVent
                       select se;
            foreach (var servicios in serv)
            {
                oCServicios.Add(new Servicio
                {
                    idServicio = servicios.idServicio.Value,
                    codigoServ = servicios.codigoServ,
                    nombreServ = servicios.nombreServ,
                    precio = (float)servicios.precio
                });
            }
            dGServ.ItemsSource = oCServicios;
            tBPrecio.Text = "0";
        }

        private void cerrarSesion(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
            }
            else { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ServicioVent reg = (from s in conex.ServicioVent
                                where s.idServicio == idServicio
                                select s).SingleOrDefault();
            if (reg == null)
            {
                //insertar
                if (tBCodigo.Text.Equals("") || tBCodNom.Text.Equals(""))
                {
                   MessageBox.Show("Te falta llenar los campos de Nombre o Código");
                }
                else
                {                    
                    Table<ServicioVent> sv = conex.GetTable<ServicioVent>();
                    ServicioVent tsv = new ServicioVent();
                    tsv.codigoServ = tBCodigo.Text;
                    tsv.nombreServ = tBCodNom.Text;
                    tsv.precio = Convert.ToDouble(tBPrecio.Text);
                    sv.InsertOnSubmit(tsv);
                    sv.Context.SubmitChanges();
                    llenaGrid();
                    //dGServ.ScrollIntoView(dGServ.Items.GetItemAt(37));
                    limpiar();
                    idServicio = 0;
                    MessageBox.Show("El registro se agrego correctamente.");
                    
                }
            }
            else
            {
                // modificar
                reg.codigoServ = tBCodigo.Text;
                reg.nombreServ = tBCodNom.Text;
                reg.precio = Convert.ToDouble(tBPrecio.Text);
                conex.SubmitChanges();
                MessageBox.Show("El registro se modifico correctamente.");
            }
            llenaGrid();
        }

        private void dGServ_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int numero = dGServ.SelectedIndex;
            if (oCServicios.Count != numero)
            {
                if (numero == -1)
                {
                    limpiar();
                }
                else
                {
                    tBCodigo.Text = oCServicios.ElementAt(numero).codigoServ;
                    tBCodNom.Text = oCServicios.ElementAt(numero).nombreServ;
                    tBPrecio.Text = oCServicios.ElementAt(numero).precio.ToString();
                    idServicio = oCServicios.ElementAt(numero).idServicio;
                }
            } 
            else {
                limpiar();
            }

        }

        private void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (tBCodNom.Text.Equals(""))
            {
                llenaGrid();
            }
            else
            {
                oCServicios.Clear();
                var serv = from se in conex.ServicioVent
                           where se.codigoServ.Contains(tBCodNom.Text) ||
                              se.nombreServ.Contains(tBCodNom.Text)
                           select se;
                foreach (var servicios in serv)
                {
                    oCServicios.Add(new Servicio
                    {
                        idServicio = servicios.idServicio.Value,
                        codigoServ = servicios.codigoServ,
                        nombreServ = servicios.nombreServ,
                        precio = (float)servicios.precio
                    });
                }
                dGServ.ItemsSource = oCServicios;
            }
        }

        private void eliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Seguro que deseas eliminar el registro", "Peligro", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ServicioVent ser = (from s in conex.ServicioVent
                                        where s.idServicio == idServicio
                                        select s).Single();
                    conex.ServicioVent.DeleteOnSubmit(ser);
                    conex.SubmitChanges();
                    MessageBox.Show("El registro se elimino correctamente.");
                    llenaGrid();
                    limpiar();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return;
            }
        }

        private void Limpiar_Click(object sender, RoutedEventArgs e)
        {
            limpiar();
        }

    }
}
public class Servicio
{
    public int idServicio { get; set; }
    public String codigoServ { get; set; }
    public String nombreServ { get; set; }
    public float precio { get; set; }
}