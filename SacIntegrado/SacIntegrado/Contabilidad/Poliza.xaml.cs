using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Linq;
using System.ComponentModel;

namespace SacIntegrado
{
    class Mov
    {
        public int Movi { get; set; }
        public String Cuenta { get; set; }
        public float Cargo { get; set; }
        public float Abono { get; set; }
        public String Referencia { get; set; }
        public String Concepto { get; set; }
        public int IdCta { get; set; }
        public String Nombre { get; set; }
        public int Papa { get; set; }
    }

    class Movit
    {
        public int Movi { get; set; }
        public String Cuenta { get; set; }
        public String Nombre { get; set; }
        public float Cargo { get; set; }
        public float Abono { get; set; }
        public String Referencia { get; set; }
        public String Concepto { get; set; }


    }

    class Cpoliza
    {
        public int IdPoliza { get; set; }

        public String concepto { get; set; }

        public int IdPeriodo { get; set; }//ste perio de la tabla periodo

        public int numeroP { get; set; }//

    }

    class Partida
    {
        public int IdCta { get; set; }
        public String Nombre { get; set; }
        public String Cuenta { get; set; }
        public int Padre { get; set; }
    }

    public partial class Poliza : Page
    {
        String nombreU;
        ObservableCollection<Mov> listaMov = new ObservableCollection<Mov>();
        ObservableCollection<Movit> listaMovi = new ObservableCollection<Movit>();
        ObservableCollection<Partida> listaPartida = new ObservableCollection<Partida>();
        ObservableCollection<Cpoliza> listaCon = new ObservableCollection<Cpoliza>();
        ObservableCollection<Cpoliza> listaNavegador = new ObservableCollection<Cpoliza>();
        // private ObservableCollection<Per> Periodo = new ObservableCollection<Per>();
        String[] mes = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre", "Cierre" };
        Db dat = new Db();
        int idCta;
        int padre;
        int periodo;
        char tipoChar;
        bool banderaCambio;
        int folioIngreso;
        int folioEgreso;
        int folioDiario;
        int numMov;
        int numMovi = 1;
        int idPolNavegador = 0, numeroPolizas = 0;
        float cargoAnt, abonoAnt;
        string periodoAct;
        int idp;

        public Poliza(String user, int id, String nombre)
        {
            this.InitializeComponent();

            bGrabar.IsEnabled = false;
            bNueva.IsEnabled = false;

            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            nombreU = nombre;
            usuarioname.Content = nombre;
            String[] tipo = { "Ingresos", "Egresos", "Diario" };
            Ctipo.ItemsSource = tipo;
            tablaPoliza.ItemsSource = listaMov;
            listaCta.ItemsSource = listaPartida;
            listaCta.DisplayMemberPath = "Nombre";
            listaCta.SelectedValuePath = "IdCta";
            listaCta.Visibility = Visibility.Hidden;
            periodo = (from ele in dat.Parametros select ele.idPeriodo).SingleOrDefault().Value;
            numMov = 1;
            llenarConcepto();
            mostrarPeriodo();
            polizasNavegador();


        }

        public Poliza() { }


        //private void menuCerrarSesion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    // TODO: Agregar implementación de controlador de eventos aquí.
        //    MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if(r==MessageBoxResult.Yes){
        //        InicioLogin inic = new InicioLogin();
        //        this.NavigationService.Navigate(inic);
        //        NavigationCommands.BrowseBack.InputGestures.Clear();
        //        NavigationCommands.BrowseForward.InputGestures.Clear();
        //    }
        //    else{}
        //}
        //******
        private void llenarConcepto()
        {

            var con = (from c in dat.TPoliza
                       orderby c.Concepto descending
                       select c).Take(20);//take me deja agrandar elementos a mostrar en la lista
            foreach (var el in con)
            {
                listaCon.Add(new Cpoliza { IdPoliza = el.idPoliza, concepto = el.Concepto });
            }
            ComboConcepto.ItemsSource = listaCon;//

            ComboConcepto.DisplayMemberPath = "concepto";

            ComboConcepto.SelectedIndex = 0;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // this.Background = Brushes.LightGray;
            if (fecha.Text == "")
            {
                MessageBox.Show("Ingresa una Fecha");
                fecha.Focus();
            }
            else if (Ctipo.Text == "")
            {
                MessageBox.Show("Selecciono un tipo de Movimento");
                Ctipo.Focus();
            }
            else if (ComboConcepto.Text == "")
            {
                MessageBox.Show("Selecciona o Agrega un Concepto");
                ComboConcepto.Focus();
            }

            else
            {
                movimiento.Text = numMov.ToString();
                sumAbono.Text = totAbono.Text;
                sumCargo.Text = totCargo.Text;
                banderaCambio = false;
                Partidas.IsOpen = true;
                bNueva.IsEnabled = true;
                // bAceptar.IsEnabled = false;//este es el boton aceptar del popup
                fecha.IsEnabled = false;
                Ctipo.IsEnabled = false;
                ComboConcepto.IsEnabled = false;
                navegadores(1);//desactivar
            }


        }

        private void bAceptar_Click(object sender, RoutedEventArgs e)
        {

            movimiento.IsReadOnly = false;
            if (cuenta.Text == "")
            {
                MessageBox.Show("Ingresa el numero de Cuenta");
                cuenta.Focus();
            }

            //else if (concepto.Text == "")
            //{
            //    MessageBox.Show("Ingresa un Concepto");
            //    concepto.Focus();
            //}

            else if (banderaCambio)
            {
                int i = int.Parse(movimiento.Text.Trim()) - 1;
                float abonoTemp = float.Parse(abono.Text.Trim());
                float cargoTemp = float.Parse(cargo.Text.Trim());
                listaMov.ElementAt(i).Abono = abonoTemp;
                listaMov.ElementAt(i).Cargo = cargoTemp;
                listaMov.ElementAt(i).Cuenta = cuenta.Text.Trim();
                listaMov.ElementAt(i).Nombre = nombre.Content.ToString();
                listaMov.ElementAt(i).Referencia = referencia.Text.Trim();
                listaMov.ElementAt(i).Concepto = concepto.Text.Trim();
                listaMov.ElementAt(i).IdCta = idCta;
                listaMov.ElementAt(i).Papa = padre;
                banderaCambio = false;
                tablaPoliza.Items.Refresh();
            }
            else
            {
                listaMov.Add(new Mov
                {
                    Movi = numMov,
                    Cuenta = cuenta.Text.Trim(),
                    Cargo = float.Parse(cargo.Text.Trim()),
                    Abono = float.Parse(abono.Text.Trim()),
                    Referencia = referencia.Text.Trim(),
                    Concepto = concepto.Text.Trim(),
                    IdCta = idCta,
                    Nombre = nombre.Content.ToString(),
                    Papa = padre
                });
                numMov++;
                movimiento.Text = numMov.ToString();

                totAbono.Text = sumAbono.Text;
                totCargo.Text = sumCargo.Text;
                cargo.Text = "0";
                abono.Text = "0";

            }
        }

        private void bSalir_Click(object sender, RoutedEventArgs e)
        {
            Partidas.IsOpen = false;
            if (float.Parse(sumCargo.Text.Trim()) != float.Parse(sumAbono.Text.Trim()))
            {
                MessageBox.Show("Checa cargos y abonos");
            }
            else
            {
                validarTotales();
            }

            fecha.IsEnabled = true;
            Ctipo.IsEnabled = true;
            ComboConcepto.IsEnabled = true;
            // navegadores(2);//activar
        }

        private void cargo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (banderaCambio)
            {
                float cargoTemp = float.Parse(cargo.Text.Trim());
                float cargoTot = float.Parse(sumCargo.Text.Trim()) - cargoAnt + cargoTemp;
                sumCargo.Text = cargoTot.ToString();
            }
            else
            {
                sumCargo.Text = (float.Parse(sumCargo.Text.Trim()) + float.Parse(cargo.Text.Trim())).ToString();

            }
        }

        private void abono_LostFocus(object sender, RoutedEventArgs e)
        {
            if (banderaCambio)
            {
                float abonoTemp = float.Parse(abono.Text.Trim());
                float abonoTot = float.Parse(sumAbono.Text.Trim()) - abonoAnt + abonoTemp;
                sumAbono.Text = abonoTot.ToString();
            }
            else
            {
                sumAbono.Text = (float.Parse(sumAbono.Text.Trim()) + float.Parse(abono.Text.Trim())).ToString();

            }
        }

        private void cuenta_KeyUp(object sender, KeyEventArgs e)
        {
            listaCta.Visibility = Visibility.Visible;
            String partida = cuenta.Text.Trim();
            var ctas = from ele in dat.CuentaEnc
                       where ele.Cuenta.Contains(partida) && ele.Hoja == 'D'
                       orderby ele.Cuenta
                       select ele;
            if (ctas.Count() == 0)
            {
                ctas = from ele in dat.CuentaEnc
                       where ele.Nombre.Contains(partida) && ele.Hoja == 'D'
                       orderby ele.Nombre
                       select ele;
            }
            listaPartida.Clear();
            foreach (var el in ctas)
            {
                listaPartida.Add(new Partida { IdCta = el.IdCuenta, Nombre = el.Nombre, Cuenta = el.Cuenta, Padre = el.Padre.Value });
            }
        }

        private void listaCta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaCta.SelectedValue == null) return;
            idCta = (int)listaCta.SelectedValue;
            Partida par = listaCta.SelectedItem as Partida;
            nombre.Content = par.Nombre;
            cuenta.Text = par.Cuenta;
            padre = par.Padre;
            if (float.Parse(sumAbono.Text.Trim()) > float.Parse(sumCargo.Text.Trim()))
            {
                cargo.Text = (float.Parse(sumAbono.Text.Trim()) - float.Parse(sumCargo.Text.Trim())).ToString();
                cargo.Focus();
            }
            else
            {
                abono.Text = (float.Parse(sumCargo.Text.Trim()) - float.Parse(sumAbono.Text.Trim())).ToString();
                abono.Focus();
            }
            listaCta.Visibility = Visibility.Hidden;
        }

        private void bGrabar_Click(object sender, RoutedEventArgs e)
        {
            if (float.Parse(totAbono.Text.Trim()) != float.Parse(totCargo.Text.Trim()))
            {
                MessageBox.Show("Checa cargos y abonos");
                return;
            }
            if (MessageBox.Show("Grabar la poliza? ", "Advertencia", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TPoliza po = new TPoliza { idPeriodo = periodo, Fecha = DateTime.Parse(fecha.Text.Trim()), Concepto = ComboConcepto.Text, Tipo = tipoChar, noPoliza = Convert.ToInt32(numero.Text.Trim()) };
                dat.TPoliza.InsertOnSubmit(po);
                dat.SubmitChanges();
                int idEnc = (from ele in dat.TPoliza select ele.idPoliza).Max();

                Parametros renglon = (from r in dat.Parametros
                                      select r).SingleOrDefault();
                renglon.polizaDiario = folioDiario;
                renglon.polizaEgreso = folioEgreso;
                renglon.polizaIngreso = folioIngreso;
                dat.SubmitChanges();

                float importe;
                char tipo;
                ActualizaCuentas ac = new ActualizaCuentas();
                foreach (var ele in listaMov)
                {
                    if (ele.Abono != 0)
                    {
                        importe = ele.Abono;
                        tipo = 'A';
                        ac.ActSaldos(ele.IdCta, ele.Papa, -importe);
                    }
                    else
                    {
                        importe = ele.Cargo;
                        tipo = 'C';
                        ac.ActSaldos(ele.IdCta, ele.Papa, importe);
                    }
                    Movimiento mo = new Movimiento { idPoliza = idEnc, Importe = importe, Tipo = tipo, idCuenta = ele.IdCta, Referencia = ele.Referencia, conceptoP = char.Parse(ele.Concepto) };
                    dat.Movimiento.InsertOnSubmit(mo);
                    dat.SubmitChanges();

                }
                listaMov.Clear();
                totAbono.Text = "0";
                totCargo.Text = "0";
                navegadores(2);
            }
        }

        private void Ctipo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var param = (from ele in dat.Parametros select ele).SingleOrDefault();
            folioIngreso = param.polizaIngreso.Value;
            folioEgreso = param.polizaEgreso.Value;
            folioDiario = param.polizaDiario.Value;
            switch (Ctipo.SelectedIndex)
            {
                case 0:
                    folioIngreso++;
                    numero.Text = folioIngreso.ToString();
                    tipoChar = 'I';
                    break;
                case 1:
                    folioEgreso++;
                    numero.Text = folioEgreso.ToString();
                    tipoChar = 'E';
                    break;
                case 2:
                    folioDiario++;
                    numero.Text = folioDiario.ToString();
                    tipoChar = 'D';
                    break;
            }
        }

        private void tablaPoliza_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Mov actual = tablaPoliza.SelectedItem as Mov;
            if (actual == null) return;
            movimiento.Text = (tablaPoliza.SelectedIndex + 1).ToString();
            cuenta.Text = actual.Cuenta;
            nombre.Content = actual.Nombre;
            cargoAnt = actual.Cargo;
            abonoAnt = actual.Abono;
            cargo.Text = actual.Cargo.ToString();
            abono.Text = actual.Abono.ToString();
            referencia.Text = actual.Referencia;
            concepto.Text = actual.Concepto;
            sumAbono.Text = totAbono.Text;
            sumCargo.Text = totCargo.Text;
            banderaCambio = true;
            Partidas.IsOpen = true;
        }

        //**de aqui al final
        private void cuenta_LostFocus(object sender, RoutedEventArgs e)
        {
            listaCta.Visibility = Visibility.Hidden;
        }

        public void polizasNavegador()
        {
            MessageBox.Show("ver" + idp);
            var con = (from c in dat.TPoliza
                       where c.idPeriodo == idp
                       select c);
            foreach (var el in con)
            {
                listaNavegador.Add(new Cpoliza { IdPoliza = el.idPoliza, concepto = el.Concepto, IdPeriodo = el.idPeriodo.Value, numeroP = el.noPoliza.Value });

            }

            Cpoliza pn = new Cpoliza();
            //pn = listaNavegador.First();//eeeeeeeee
            //Navegador.Text = pn.concepto;
            //ComboConcepto.Text = pn.concepto;// desabilitado para no mostrar nada en concepto
            idPolNavegador = pn.IdPoliza;
        }

        private void Primero_Click(object sender, RoutedEventArgs e)
        {
            Cpoliza pn = new Cpoliza();
            pn = listaNavegador.First();
            //Navegador.Text = pn.concepto;
            ComboConcepto.Text = pn.concepto;//
            idPolNavegador = pn.IdPoliza;
            // MessageBox.Show(""+pn.numeroP);
            mostrarPoliza(pn.IdPoliza);
            nuevaPartida.IsEnabled = false;
            // bGrabar.Visibility = Visibility.Hidden;
            bGrabar.IsEnabled = false;
            bNueva.IsEnabled = true;
            numMovi = 1;
            Ctipo.IsEnabled = false;
            fecha.IsEnabled = false;


            numeroPolizas = 0;

        }

        private void Ultimo_Click(object sender, RoutedEventArgs e)
        {
            Cpoliza pn = new Cpoliza();
            pn = listaNavegador.Last();
            //Navegador.Text = pn.concepto;
            ComboConcepto.Text = pn.concepto;//
            idPolNavegador = pn.IdPoliza;
            mostrarPoliza(pn.IdPoliza);
            nuevaPartida.IsEnabled = false;
            // bGrabar.Visibility = Visibility.Hidden;
            bGrabar.IsEnabled = false;
            bNueva.IsEnabled = true;
            numeroPolizas = listaNavegador.Count();
            numMovi = 1;
            Ctipo.IsEnabled = false;
            fecha.IsEnabled = false;

        }

        private void Anterios_Click(object sender, RoutedEventArgs e)
        {
            Cpoliza pn = new Cpoliza();

            if (idPolNavegador == listaNavegador.First().IdPoliza)
            {
                pn = listaNavegador.Last();
                numeroPolizas = listaNavegador.Count() - 1;
                pn = listaNavegador.ElementAt(numeroPolizas);
            }
            else if (idPolNavegador == listaNavegador.Last().IdPoliza)
            {
                numeroPolizas--;
                pn = listaNavegador.ElementAt(numeroPolizas);
            }
            else
            {
                numeroPolizas--;
                pn = listaNavegador.ElementAt(numeroPolizas);
            }
            //Navegador.Text = pn.concepto;
            ComboConcepto.Text = pn.concepto;//
            idPolNavegador = pn.IdPoliza;
            mostrarPoliza(pn.IdPoliza);
            nuevaPartida.IsEnabled = false;
            // bGrabar.Visibility = Visibility.Hidden;
            bGrabar.IsEnabled = false;
            bNueva.IsEnabled = true;
            numMovi = 1;
            Ctipo.IsEnabled = false;
            fecha.IsEnabled = false;
        }

        private void Siguiente_Click(object sender, RoutedEventArgs e)
        {
            Cpoliza pn = new Cpoliza();

            if (idPolNavegador == listaNavegador.Last().IdPoliza)
            {
                pn = listaNavegador.First();
                numeroPolizas = 0;
            }
            else if (idPolNavegador == listaNavegador.First().IdPoliza)
            {
                numeroPolizas++;
                pn = listaNavegador.ElementAt(numeroPolizas);
            }
            else
            {
                numeroPolizas++;
                pn = listaNavegador.ElementAt(numeroPolizas);
            }
            // Navegador.Text = pn.concepto;
            ComboConcepto.Text = pn.concepto;//
            idPolNavegador = pn.IdPoliza;
            mostrarPoliza(pn.IdPoliza);
            nuevaPartida.IsEnabled = false;
            // bGrabar.Visibility = Visibility.Hidden;
            bGrabar.IsEnabled = false;
            bNueva.IsEnabled = true;
            numMovi = 1;
            Ctipo.IsEnabled = false;
            fecha.IsEnabled = false;
        }

        private void mostrarPeriodo()
        {

            var pe = from r in dat.TablaPeriodo
                     where r.status == 'A'
                     select r;
            //Periodo.Clear();
            foreach (var k in pe)
            {
                periodoAct = mes[k.mes.Value - 1];//Convert.ToString( k.idPeriodo);
                idp = k.idPeriodo;
            }
            MessageBox.Show("este id " + idp);
            periodoActual.Text = periodoAct;

        }

        private void mostrarPoliza(int idpoliza)
        {
            //nuevaPartida.Visibility = Visibility.Hidden;

            var mos = from r in dat.TPoliza
                      where r.idPoliza == idpoliza
                      select r;
            foreach (var k in mos)
            {

                string a = "";

                MessageBox.Show(" " + k.Fecha + " " + k.Tipo + " " + k.noPoliza + " " + k.Concepto);
                fecha.Text = Convert.ToString(k.Fecha);
                if (k.Tipo.Equals('I'))
                {
                    a = "Ingresos";
                }
                if (k.Tipo.Equals('E'))
                {
                    a = "Egresos";
                }
                if (k.Tipo.Equals('D'))
                {
                    a = "Diario";
                }
                Ctipo.Text = a;
                numero.Text = Convert.ToString(k.noPoliza);
                ComboConcepto.Text = k.Concepto;
            }

            var con = from s in dat.Movimiento
                      from d in dat.CuentaEnc
                      where s.idCuenta == d.IdCuenta && s.idPoliza == idpoliza
                      select new { d.Cuenta, d.Nombre, s.Importe, s.Tipo, s.Referencia, s.conceptoP };
            listaMovi.Clear();

            float car = 0;
            float abo = 0;

            float sumCar = 0;
            float sumAbo = 0;

            foreach (var k in con)
            {
                if (k.Tipo.Equals('C'))
                {
                    car = (float)k.Importe.Value;
                    //sumCar = car;
                    sumCar = sumCar + car;
                    abo = 0;
                }
                if (k.Tipo.Equals('A'))
                {
                    car = 0;
                    abo = (float)k.Importe.Value;
                    //sumAbo = abo;
                    sumAbo = sumAbo + abo;

                }

                listaMovi.Add(new Movit
                {

                    Movi = numMovi,
                    Cuenta = k.Cuenta,
                    Nombre = k.Nombre,

                    Cargo = car,
                    Abono = abo,

                    Referencia = k.Referencia,
                    Concepto = k.conceptoP.ToString(),
                });
                numMovi++;
                //movimiento.Text = numMov.ToString();
                tablaPoliza.ItemsSource = listaMovi;

            }

            totCargo.Text = Convert.ToString(sumCar);
            totAbono.Text = Convert.ToString(sumAbo);

        }

        private void bNueva_Click(object sender, RoutedEventArgs e)
        {
            fecha.Text = "";
            Ctipo.Text = "";
            numero.Text = "";
            ComboConcepto.Text = "";
            totCargo.Text = "";
            totAbono.Text = "";
            listaMovi.Clear();

            nuevaPartida.IsEnabled = true;
            Ctipo.IsEnabled = true;
            fecha.IsEnabled = true;
            ComboConcepto.IsEnabled = true;
            navegadores(2);//activar
            Partidas.IsOpen = false;
            //bGrabar.Visibility = Visibility;
        }

        private void bBorrar_Click(object sender, RoutedEventArgs e)
        {


        }

        private void validarTotales()// se validan totales que se han iguales cargo y abono y se cierra pop y habilita el botton de guardar
        {

            if (totCargo.Text == totAbono.Text)
            {
                if (totCargo.Text.Equals(0) && totAbono.Text.Equals(0))
                {
                    bGrabar.IsEnabled = false;
                }

                bGrabar.IsEnabled = true;

            }
            else
            {
                bGrabar.IsEnabled = true;
                //Partidas.IsOpen = false;
                nuevaPartida.IsEnabled = false;
            }

        }

        private void clickNavegador(object sender, MouseButtonEventArgs e)
        {
            //eliminar 
        }

        private void navegadores(int a)//activar y desactivar los botones de navegacion
        {
            if (a == 1)
            {
                primera.IsEnabled = false;
                ultima.IsEnabled = false;
                anterior.IsEnabled = false;
                siguiente.IsEnabled = false;
            }
            else
            {
                primera.IsEnabled = true;
                ultima.IsEnabled = true;
                anterior.IsEnabled = true;
                siguiente.IsEnabled = true;
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
    }

    ///eliminar por completo herramienta navegador y sus funciones
}