using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ClaseCta
    {
        public String Cuenta { get; set; }
        public String Nombre { get; set; }
        public int Padre { get; set; }
        public int IdCuenta { get; set; }
    }

    public class Per
    {
        public int idPeriodo { get; set; }
        public int mes { get; set; }
        public String periodo { get; set; }
        public int anio { get; set; }
        public char status { get; set; }
    }

    public class Contabilidad
    {
        //tab CuentaEnc
        public int idCuenta { get; set; }
        public String Cuenta { get; set; }
        public String Nombre { get; set; }
        public int Periodo { get; set; }
        public double SInicial { get; set; }
        public double SFinal { get; set; }
        public char ho { get; set; }
        public char tipoC { get; set; }
    }

    public partial class Conta_Presu : Page
    {

        String clasificacion;
        Char tipoCuenta;
        private ObservableCollection<ClaseCta> MisCuentas = new ObservableCollection<ClaseCta>();
        private ObservableCollection<Per> MisPeriodos = new ObservableCollection<Per>();
        private ObservableCollection<Contabilidad> cuentas = new ObservableCollection<Contabilidad>();//
        Boolean primera = true;
        // Conta c = new Conta();
        Db db = new Db();
        private string ctaPadre = null;
        SqlConnection con = new SqlConnection();
        private String ctaNueva;
        private int sigue;
        String[] mes = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre", "Cierre" };
        private int periodo;
        private int idCuenta;
        bool llenaTabla;

        public Conta_Presu()
            : this("No identificado", 0, "ninguno")
        {
        }

        public Conta_Presu(String user, int id, String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            con.ConnectionString = "Data Source=WASSAURUS-PC\\MSSQLSERVERWAS;Initial Catalog=sac;Integrated Security=True";
            usuario.Content = nombre;
            llenaLista(0);
            llenaPeriodos();
            mostrarEnTabla();
            llenaTabla = false;
        }

        private void llenaPeriodos()
        {
            var pe = from r in db.TablaPeriodo
                     where r.status == 'A' || r.status == 'P'
                     orderby r.status
                     select new { r.idPeriodo, r.mes, r.anio, r.status };
            MisPeriodos.Clear();
            foreach (var ele in pe)
            {
                MisPeriodos.Add(new Per { idPeriodo = ele.idPeriodo, mes = ele.mes.Value, anio = ele.anio.Value, status = ele.status.Value, periodo = mes[ele.mes.Value - 1] + " " + ele.anio.Value });
            }
            periodos.DisplayMemberPath = "periodo";
            periodos.SelectedValuePath = "idPeriodo";
            periodos.SelectedIndex = 0;
            periodos.ItemsSource = MisPeriodos;

        }

        private void llenaLista(int padre)
        {
            String ultimo = null;
            var cuentaM = from r in db.CuentaEnc
                          where r.Padre == padre
                          orderby r.Cuenta
                          select new { r.IdCuenta, r.Nombre, r.Cuenta, r.Padre };
            MisCuentas.Clear();
            foreach (var ele in cuentaM)
            {
                MisCuentas.Add(new ClaseCta { Cuenta = ele.Cuenta, Nombre = ele.Nombre, Padre = ele.Padre.Value, IdCuenta = ele.IdCuenta });
                ultimo = ele.Cuenta;
            }
            listaCta.ItemsSource = MisCuentas;
            if (ultimo != null)
            {
                String ul = ultimo.Substring(ultimo.IndexOf('0') - 1, 1);
                sigue = Convert.ToInt32(ul) + 1;
            }
            else sigue = 1;
            ctaNueva = ctaGenerada.Text.Trim() + sigue;
            while (ctaNueva.Length < 8) ctaNueva = ctaNueva + "0";
            //detalle.Text = ctaNueva;
            validCuenta();
        }

        private void validCuenta()//acabo de agrgar
        {
            if (Convert.ToInt32(ctaNueva.Trim()) == 10000000)
            {
                //MessageBox.Show("NO mas cuentas ");
                detalle.Text = "NO mas cuentas";
                insertar.IsEnabled = false;

            }
            else
            {
                detalle.Text = ctaNueva;
                insertar.IsEnabled = true;
            }

        }

        private void menuCerrarSesion_Click(object sender, MouseButtonEventArgs e)
        {
            //CONFIRMACIÓN PARA SALIR
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
            }
            else { }
        }

        private void ctaGenerada_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (ctaPadre == null || ctaPadre.Trim().Length == 0) return;
            if (ctaGenerada.Text.Trim() == "")
            {
                llenaLista(0);
            }
            else
            {
                String cta = ctaPadre;
                while (cta.Length < 8) cta = cta + "0";
                SqlCommand command = new SqlCommand("Select padre from cuentaEnc where cuenta='" + cta.Trim() + "'", con);
                con.Open();
                SqlDataReader reader = command.ExecuteReader();
                int padre = 0;
                if (reader.Read())
                {
                    padre = reader.GetInt32(0);
                }
                llenaLista(padre);
                ctaPadre = ctaPadre.Substring(0, ctaPadre.Length - 1);
                reader.Close();
                con.Close();
            }
        }

        private void listaCta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ClaseCta ctaSelect;

            if (primera)
            {
                ctaSelect = (ClaseCta)listaCta.SelectedItem;
                primera = false;
                String miCuenta = ctaSelect.Cuenta;
                int fin = miCuenta.IndexOf('0');
                String miPapa = miCuenta.Substring(0, fin);
                ctaGenerada.Text = miPapa;
                llenaLista(ctaSelect.IdCuenta);
                idCuenta = ctaSelect.IdCuenta;

            }
            else primera = true;

        }


        private void ctaGenerada_GotFocus(object sender, RoutedEventArgs e)
        {
            ctaPadre = ctaGenerada.Text;

        }

        private void periodos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            periodo = int.Parse(periodos.SelectedValue.ToString());

        }

        //mostrar datos en datagrid desde BD al iniciar pantalla
        private void mostrarEnTabla()
        {
            llenaTabla = true;
            var con = (from e in db.CuentaEnc
                       from d in db.CuentaDet
                       where e.IdCuenta == d.idCuenta && e.Status == true
                       orderby e.Cuenta
                       select new { e.IdCuenta, e.Cuenta, e.Nombre, d.idPeriodo, d.SaldoInicial, d.SaldoFinal, e.Hoja, e.TipoCuenta });
            cuentas.Clear();
            foreach (var z in con)
            {
                cuentas.Add(new Contabilidad { idCuenta = z.IdCuenta, Cuenta = z.Cuenta, Nombre = z.Nombre, Periodo = z.idPeriodo.Value, SInicial = z.SaldoInicial.Value, SFinal = z.SaldoFinal.Value, ho = z.Hoja.Value, tipoC = z.TipoCuenta.Value });
            }
            datag.ItemsSource = cuentas;
        }

        //inserta a BD
        private void insertar_Click(object sender, RoutedEventArgs e)
        {


            if (nombre.Text == "")
            {
                MessageBox.Show("Ingresa un nombre para la cuenta");
            }
            else if (periodos.SelectedIndex == -1)
            {
                MessageBox.Show("Ingresa un periodo para la cuenta");

            }
            else
            {
                var valid = (from a in db.CuentaEnc
                             where a.Nombre == nombre.Text
                             select a).SingleOrDefault();

                if (valid != null)
                {
                    MessageBox.Show("Ya Existe una Cuenta con ese Nombre", "VERIFICACION", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {

                    try
                    {
                        if (acredora.IsChecked == true)
                        {
                            tipoCuenta = 'A';
                        }
                        else
                        {
                            tipoCuenta = 'D';
                        }
                        //********************************************
                        if (mayor.IsChecked == true)
                        {
                            clasificacion = "A";
                        }
                        else
                        {
                            clasificacion = "D";
                        }

                        if (MessageBox.Show("Estas Seguro de Guardar los Datos ", "Advertencia", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            CuentaEnc ce = new CuentaEnc { Nombre = nombre.Text, Cuenta = detalle.Text, Padre = idCuenta, TipoCuenta = tipoCuenta, Hoja = char.Parse(clasificacion), Status = true };
                            db.CuentaEnc.InsertOnSubmit(ce);
                            db.SubmitChanges();

                            var idCta = (from id in db.CuentaEnc select id.IdCuenta).Max();

                            CuentaDet cd = new CuentaDet { idCuenta = idCta, idPeriodo = periodo, SaldoInicial = 0, SaldoFinal = 0 };
                            db.CuentaDet.InsertOnSubmit(cd);
                            db.SubmitChanges();
                            if (detalle.Text.Trim().Substring(0, 2).StartsWith("12") || detalle.Text.Trim().Substring(0, 2).StartsWith("4") || detalle.Text.Trim().Substring(0, 2).StartsWith("5"))
                            {
                                PresupuestoGastos pg = new PresupuestoGastos { idCuenta = idCta, idPeriodo = periodo, saldoInicialAprobado = 0, saldoFinalAprobado = 0, saldoInicialXEjercer = 0, saldoFinalXEjercer = 0, saldoInicialModificado = 0, saldoFinalModificado = 0, saldoInicialComprometido = 0, saldoFinalComprometido = 0, saldoInicalDevengado = 0, saldoFinalDevengado = 0, saldoInicialEjercido = 0, saldoFinalEjercido = 0, saldoInicialPagado = 0, saldoFinalPagado = 0 };
                                db.PresupuestoGastos.InsertOnSubmit(pg);
                                db.SubmitChanges();
                            }
                            mostrarEnTabla();
                            nombre.Clear();
                        }

                        else
                        {
                            MessageBox.Show("El registro no se Inserto");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        db.SubmitChanges();
                    }
                }
            }

        }

        private void modifCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (acredora.IsChecked == true)
            {
                tipoCuenta = 'A';
            }
            else
            {
                tipoCuenta = 'D';
            }
            //********************************************
            if (mayor.IsChecked == true)
            {
                clasificacion = "A";
            }
            else
            {
                clasificacion = "D";
            }
            CuentaEnc renglon = (from r in db.CuentaEnc
                                 where r.IdCuenta == idCuenta
                                 select r).SingleOrDefault();
            renglon.Nombre = nombre.Text.Trim();
            renglon.Hoja = char.Parse(clasificacion);
            renglon.TipoCuenta = tipoCuenta;
            db.SubmitChanges();
            mostrarEnTabla();
        }

        private void eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Estas Seguro de Eliminar la cuentas " + nombre.Text.Trim(), "Advertencia", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                CuentaEnc renglon = (from r in db.CuentaEnc
                                     where r.IdCuenta == idCuenta
                                     select r).SingleOrDefault();
                renglon.Status = false;
                db.SubmitChanges();
                mostrarEnTabla();
                nombre.Clear();
            }

        }
        private void datag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (llenaTabla)
            {
                llenaTabla = false;
                return;
            }
            Contabilidad selec = datag.SelectedItem as Contabilidad;
            MessageBox.Show("hoja  " + selec.ho);
            if (selec.ho == 'A')
            {
                mayor.IsChecked = true;
            }
            else
            {
                detalle1.IsChecked = true;
            }
            MessageBox.Show("tipo  " + selec.tipoC);
            if (selec.tipoC == 'A')
            {
                acredora.IsChecked = true;
            }
            else
            {
                deudora.IsChecked = true;
            }
            nombre.Text = selec.Nombre;
            idCuenta = selec.idCuenta;
            modifCuenta.IsEnabled = true;
            eliminar.IsEnabled = true;
        }


        private void listaCta_KeyDown(object sender, KeyEventArgs e)
        {
            String sa = ctaGenerada.Text;

            if (e.Key == Key.Escape)
            {
                //MessageBox.Show("jjjhjk" + sa);
                if (sa != "")
                {
                    String cta = sa;
                    while (cta.Length < 8) cta = cta + "0";
                    SqlCommand command = new SqlCommand("Select Padre from CuentaEnc where Cuenta='" + cta.Trim() + "'", con);
                    con.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    int padre = 0;
                    if (reader.Read())
                    {
                        padre = reader.GetInt32(0);
                    }
                    llenaLista(padre);
                    sa = sa.Substring(0, sa.Length - 1);
                    reader.Close();
                    con.Close();
                    ctaGenerada.Text = ctaGenerada.Text.Substring(0, ctaGenerada.Text.Length - 1);
                    ctaNueva = ctaGenerada.Text.Trim() + sigue;
                    while (ctaNueva.Length < 8) ctaNueva = ctaNueva + "0";
                    //detalle.Text = ctaNueva;
                    validCuenta();

                }
            }
        }
    }
}

//Driver Genius