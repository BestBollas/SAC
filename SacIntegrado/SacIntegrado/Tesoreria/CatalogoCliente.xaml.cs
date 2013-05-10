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
    /// Lógica de interacción para CatalogoCliente.xaml
    /// </summary>
    public partial class CatalogoCliente : Page
    {
        Db db = new Db();
        ObservableCollection<Alumno> aln = new ObservableCollection<Alumno>();
        ObservableCollection<ClienteExt> cln = new ObservableCollection<ClienteExt>();
        int tbl;

        public CatalogoCliente(string usuario, int id, string nomb)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, usuario, id, nomb);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            this.usuario.Content = nomb;
            llenaGrid();
            
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

        public void llenaGrid()
        {
            var almno = from al in db.Alumno
                        select al;
            alumnoDg.ItemsSource = almno;
            var clienteExt = from ce in db.ClienteExt
                             select ce;
            clienteDg.ItemsSource = clienteExt;
        }

        private void clienteDg_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (clienteDg.SelectedIndex >= 0)
            {
                ClienteExt reg = clienteDg.SelectedItem as ClienteExt;
                ident.Text = reg.curp_rfc;
                nomAl.Text = reg.Nombre;
                apPater.Text = reg.apPater;
                apMater.Text = reg.apMater;
                calle.Text = reg.calle;
                colonia.Text = reg.colonia;
                noExt.Text = reg.num;
                municipio.Text = reg.municipio;
                estado.Text = reg.estado;
                tel.Text = reg.telefono;
            }
            else { }
        }

        private void tabAlumno_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (alumnoDg.SelectedIndex >= 0)
            {
                Alumno reg = alumnoDg.SelectedItem as Alumno;
                ident.Text = reg.matricula;
                nomAl.Text = reg.Nombre;
                apPater.Text = reg.apPater;
                apMater.Text = reg.apMater;
                calle.Text = reg.calle;
                colonia.Text = reg.colonia;
                noExt.Text = reg.num;
                municipio.Text = reg.municipio;
                estado.Text = reg.estado;
                tel.Text = reg.telefono;
            }
            else { }
        }

        private void clave_TextChanged(object sender, TextChangedEventArgs e)
        {
            var almn = from al in db.Alumno
                       select new
                       {
                           al.matricula,
                           nomAl = al.Nombre.Trim() + " " + al.apPater.Trim() + " " + al.apMater.Trim(),
                           al.Nombre, al.apPater, al.apMater, al.calle, al.colonia, al.num,
                           al.municipio, al.estado, al.telefono
                       };
            aln.Clear();
            foreach (var al in almn)
            {
                if (al.matricula.Contains(clave.Text.Trim()) || al.nomAl.ToLower().Contains(clave.Text.Trim().ToLower()))
                {
                    aln.Add(new Alumno
                    {
                        matricula = al.matricula, Nombre = al.nomAl, calle = al.calle, apPater = al.apPater,
                        apMater = al.apMater, colonia = al.colonia, municipio = al.municipio, estado = al.estado,
                        num = al.num, telefono = al.telefono
                    });
                }
            }
            alumnoDg.ItemsSource = aln;
            var clex = from ce in db.ClienteExt
                       select new
                       {
                           ce.curp_rfc, cen = ce.Nombre.Trim() + " " + ce.apPater.Trim() + " " + ce.apMater.Trim(),
                           ce.Nombre, ce.apPater, ce.apMater, ce.calle, ce.colonia,
                           ce.num, ce.municipio, ce.estado, ce.telefono
                       };
            cln.Clear();
            foreach (var cl in clex)
            {
                if (cl.curp_rfc.Contains(clave.Text.Trim()) || cl.cen.ToLower().Contains(clave.Text.Trim().ToLower()))
                {
                    cln.Add(new ClienteExt
                    {
                        curp_rfc = cl.curp_rfc, Nombre = cl.Nombre, apPater = cl.apPater, apMater = cl.apMater,
                        calle = cl.calle, colonia = cl.colonia, municipio = cl.municipio, estado = cl.estado,
                        num = cl.num, telefono = cl.telefono
                    });
                }
            }
            clienteDg.ItemsSource = cln;
        }

        private void limpiar_Click(object sender, RoutedEventArgs e)
        {
            cln.Clear();
            clienteDg.ItemsSource = cln;
            llenaGrid();
            clave.Text = "";
            ident.Text = "";
            nomAl.Text = "";
            apPater.Text = "";
            apMater.Text = "";
            calle.Text = "";
            colonia.Text = "";
            noExt.Text = "";
            municipio.Text = "";
            estado.Text = "";
            tel.Text = "";
        }

        private void actualizar_Click(object sender, RoutedEventArgs e)
        {
            if (ident.Text != "" && nomAl.Text != "" && apPater.Text != "" && apMater.Text != "" && calle.Text != ""
                && colonia.Text != "" && municipio.Text != "" && estado.Text != "")
            {
            if (tabAlumno.IsSelected)
            {
                MessageBox.Show("Selecciono Alumno");
                Alumno ren = (from am in db.Alumno
                              where am.matricula == ident.Text
                              select am).SingleOrDefault();
                if (ren == null)
                {
                    MessageBox.Show("Hay que insertar");
                    Table<Alumno> ca = db.GetTable<Alumno>();
                    Alumno a = new Alumno();
                    a.matricula = ident.Text;
                    a.Nombre = nomAl.Text;
                    a.apPater = apPater.Text;
                    a.apMater = apMater.Text;
                    a.calle = calle.Text;
                    a.colonia = colonia.Text;
                    a.num = noExt.Text;
                    a.municipio = municipio.Text;
                    a.estado = estado.Text;
                    a.telefono = tel.Text;
                    ca.InsertOnSubmit(a);
                    ca.Context.SubmitChanges();
                    MessageBox.Show("El registro se agrego correctamente");
                }
                else
                {
                    MessageBox.Show("Hay que modificar");
                    ren.matricula = ident.Text;
                    ren.Nombre = nomAl.Text;
                    ren.apPater = apPater.Text;
                    ren.apMater = apMater.Text;
                    ren.calle = calle.Text;
                    ren.colonia = colonia.Text;
                    ren.num = noExt.Text;
                    ren.municipio = municipio.Text;
                    ren.estado = estado.Text;
                    ren.telefono = tel.Text;
                    db.SubmitChanges();
                    MessageBox.Show("El registro se modifico correctamente");
                }
            }
            else if (tabClienteEx.IsSelected)
            {
                MessageBox.Show("Selecciono Externo");
                ClienteExt clex = (from ce in db.ClienteExt
                                   where ce.curp_rfc == ident.Text
                                   select ce).SingleOrDefault();
                if (clex == null)
                {
                    //INSERTAR
                    Table<ClienteExt> ce = db.GetTable<ClienteExt>();
                    ClienteExt ex = new ClienteExt();
                    ex.curp_rfc = ident.Text;
                    ex.Nombre = nomAl.Text;
                    ex.apPater = apPater.Text;
                    ex.apMater = apMater.Text;
                    ex.calle = calle.Text;
                    ex.colonia = colonia.Text;
                    ex.num = noExt.Text;
                    ex.municipio = municipio.Text;
                    ex.estado = estado.Text;
                    ex.telefono = tel.Text;
                    ce.InsertOnSubmit(ex);
                    ce.Context.SubmitChanges();
                    MessageBox.Show("El registro se agrego correctamente");
                }
                else
                {
                    //MODIFICAR
                    clex.curp_rfc = ident.Text;
                    clex.Nombre = nomAl.Text;
                    clex.apPater = apPater.Text;
                    clex.apMater = apMater.Text;
                    clex.calle = calle.Text;
                    clex.colonia = colonia.Text;
                    clex.num = noExt.Text;
                    clex.municipio = municipio.Text;
                    clex.estado = estado.Text;
                    clex.telefono = tel.Text;
                    db.SubmitChanges();
                    MessageBox.Show("El registro se modifico correctamente");
                }
            }
            }
            else { MessageBox.Show("Faltan algunos datos..."); }
            limpiar_Click(sender, e);
        }

        private void eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Seguro que deseas eliminar el registro", "Peligro", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (tabAlumno.IsSelected)
                {
                    Alumno reg=alumnoDg.SelectedItem as Alumno;
                    Alumno elm = (from amns in db.Alumno
                                  where amns.matricula == reg.matricula
                                  select amns).Single();
                    db.Alumno.DeleteOnSubmit(elm);
                    db.SubmitChanges();
                    MessageBox.Show("El registro se elimino correctamente");
                    limpiar_Click(sender, e);
                }
                else if (tabClienteEx.IsSelected)
                {
                    ClienteExt reg = clienteDg.SelectedItem as ClienteExt;
                    //ClienteExt cext
                }
            }
            else { }

        }
    }
}

class Client
{
    public String ident { get; set; }
    public String nombreCm { get; set; }
    public String nombreSm { get; set; }
    public String apPater { get; set; }
    public String apMater { get; set; }
    public String calle { get; set; }
    public String colonia { get; set; }
    public String municipio { get; set; }
    public String estado { get; set; }
    public String noExt { get; set; }
    public String telf { get; set; }
    public int tbl { get; set; }
}
