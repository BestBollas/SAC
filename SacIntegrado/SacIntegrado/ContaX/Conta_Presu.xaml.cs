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

namespace SAC
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
        public int idCuentaCE { get; set; }
        public String cuentaCE { get; set; }
        public String nombreCE { get; set; }

       

        ////tab CuentaDet
        //public int idCuentaCD { get; set; }
        public int idPeriodoCD { get; set; }
        public double Inicial { get; set; }
        public double Final { get; set; }

        




    }
    public partial class Conta_Presu : Page
    {


        private ObservableCollection<ClaseCta> MisCuentas = new ObservableCollection<ClaseCta>();
        private ObservableCollection<Per> MisPeriodos = new ObservableCollection<Per>();
        private ObservableCollection<Contabilidad> cuentas = new ObservableCollection<Contabilidad>();//
        Boolean primera = true;
        Conta c = new Conta();
        private string ctaPadre = null;
        SqlConnection con = new SqlConnection();
        private String ctaNueva;
        private int sigue;
        String[] mes = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre", "Cierre" };
        private int periodo;
        private int idCuenta;

        public Conta_Presu()
            : this("No identificado")
        {
        }

        public Conta_Presu(String nomlog)
        {
            InitializeComponent();
            con.ConnectionString = "Data Source=WASSAURUS-PC\\MSSQLSERVERWAS;Initial Catalog=sac;Integrated Security=True";
            usuario.Content = nomlog;
            llenaLista(0);
            llenaPeriodos();

            mostrarEnTabla();

        }

        private void llenaPeriodos()
        {
            var pe = from r in c.TablaPeriodo
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
            periodos.ItemsSource = MisPeriodos;

        }

        private void llenaLista(int padre)
        {
            String ultimo = null;
            var cuentaM = from r in c.CuentaEnc
                          where r.Padre == padre
                          orderby r.Cuenta
                          select new { r.IdCuenta, r.Nombre, r.Cuenta, r.Padre };
            MisCuentas.Clear();
            foreach (var ele in cuentaM)
            {
                MisCuentas.Add(new ClaseCta { Cuenta = ele.Cuenta, Nombre = ele.Nombre, Padre = ele.Padre, IdCuenta = ele.IdCuenta });
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
            detalle.Text = ctaNueva;
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

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            logoo.Visibility = Visibility.Collapsed;
            Cuentas.Visibility = Visibility.Visible;
            pant_Recursos.Visibility = Visibility.Collapsed;

        }

        private void recurso_Click(object sender, RoutedEventArgs e)
        {
            logoo.Visibility = Visibility.Collapsed;
            Cuentas.Visibility = Visibility.Collapsed;
            pant_Recursos.Visibility = Visibility.Visible;
            nombre.Clear();
        }

        private void btnSalir_Click(object sender, RoutedEventArgs e)
        {

            pant_Recursos.Visibility = Visibility.Collapsed;
            Cuentas.Visibility = Visibility.Collapsed;
            logoo.Visibility = Visibility.Visible;
        }

        private void btnSalirCuenta_Click(object sender, RoutedEventArgs e)
        {
            pant_Recursos.Visibility = Visibility.Collapsed;
            Cuentas.Visibility = Visibility.Collapsed;
            logoo.Visibility = Visibility.Visible;
            nombre.Clear();
        }

        private void limpiarRecursos()
        {

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

            var con = (from e in c.CuentaEnc
                       from d in c.CuentaDet
                       where e.Padre == d.idCuenta
                       select new { e.IdCuenta, e.Cuenta, e.Nombre, d.idPeriodo, d.SaldoInicial, d.SaldoFinal }).Distinct();

            foreach (var z in con)
            {

                cuentas.Add(new Contabilidad { idCuentaCE = z.IdCuenta, cuentaCE = z.Cuenta, nombreCE = z.Nombre, idPeriodoCD = z.idPeriodo.Value, Inicial = z.SaldoInicial.Value, Final = z.SaldoFinal.Value });
                datag.ItemsSource = cuentas;
            }


        }



        //inserta a BD
        private void insertar_Click(object sender, RoutedEventArgs e)
        {

            String clasificacion;
            Char tipoCuenta;
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
                String name = "";
                var valid = from a in c.CuentaEnc
                            where a.Nombre == nombre.Text
                            select a;
                foreach (var w in valid)
                {
                    name = w.Nombre;
                }
                //if(valid.Equals(true))
                //{
                //    MessageBox.Show("ya existe esa cuenta");
                //}
                if (nombre.Text.Equals(name))
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
                            clasificacion = "M";
                        }
                        else
                        {
                            clasificacion = "D";
                        }

                        if (MessageBox.Show("Estas Seguro de Guardar los Datos ", "Advertencia", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            CuentaEnc ce = new CuentaEnc { Nombre = nombre.Text, Cuenta = detalle.Text, Padre = idCuenta, TipoCuenta = tipoCuenta, Hoja = char.Parse(clasificacion) };
                            CuentaDet cd = new CuentaDet { idCuenta = idCuenta, idPeriodo = periodo, SaldoInicial = 0, SaldoFinal = 0 };

                            if (detalle.Text.Trim().Substring(0, 2) == "12" || detalle.Text.Trim().Substring(0, 2) == "4" || detalle.Text.Trim().Substring(0, 2) == "5")
                            {
                                PresupuestoGastos pg = new PresupuestoGastos { idCuenta = idCuenta, idPeriodo = periodo, saldoInicialAprobado = 0, saldoFinalAprobado = 0, saldoInicialXEjercer = 0, saldoFinalXEjercer = 0, saldoInicialModificado = 0, saldoFinalModificado = 0, saldoInicialComprometido = 0, saldoFinalComprometido = 0, saldoInicalDevengado = 0, saldoFinalDevengado = 0, saldoInicialEjercido = 0, saldoFinalEjercido = 0, saldoInicialPagado = 0, saldoFinalPagado = 0 };

                                c.PresupuestoGastos.InsertOnSubmit(pg);
                                c.SubmitChanges();
                            }

                            c.CuentaEnc.InsertOnSubmit(ce);
                            c.CuentaDet.InsertOnSubmit(cd);

                            c.SubmitChanges();
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
                        c.SubmitChanges();
                    }
                }
            }

        }

        private void modifCuenta_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void eliminar_Click(object sender, RoutedEventArgs e)
        {

            
        }

        // no descomentar, es solo si quiero obtener el valor del id. por ejemplo de alguna linea seleccionada en el datagrid
        private void datag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Contabilidad selec = datag.SelectedItem as Contabilidad;
            int idi = 0;
            String nom = "";
            var reng = (from z in cuentas
                        where z.idCuentaCE == selec.idCuentaCE
                        select z);

            foreach (var x in reng)
            {
                idi = x.idCuentaCE;
                nom = x.nombreCE;
                //MessageBox.Show("-mm-" + x.idCuentaCE + "--" + x.idPeriodoCD);
            }
            MessageBox.Show("nn" + idi);
            nombre.Text = nom;
            mostrarEnTabla();
            modifCuenta.IsEnabled = true;
            eliminar.IsEnabled = true;
        }

        private void prueba_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("" + periodo);



        }

        private void listaCta_KeyDown(object sender, KeyEventArgs e)
        {
            String sa = ctaGenerada.Text;
            
            if (e.Key == Key.Escape)
            {
                //MessageBox.Show("jjjhjk" + sa);
                if(sa!="")
                {
                    String cta = sa;
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
                    sa = sa.Substring(0, sa.Length - 1);
                        reader.Close();
                        con.Close();
                        ctaGenerada.Text = ctaGenerada.Text.Substring(0, ctaGenerada.Text.Length - 1);
                }
            }
        }
    }
}

//Driver Genius