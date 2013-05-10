using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.Reporting.WinForms;

namespace SacIntegrado.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para ConsultaTes.xaml
    /// </summary>
    public partial class ConsultaTes : Page
    {
        //ObservableCollection<Cliente> clt = new ObservableCollection<Cliente>();
        Db conex = new Db();
        ObservableCollection<Clientes> clt = new ObservableCollection<Clientes>();
        ObservableCollection<CoVen> tabCoVen = new ObservableCollection<CoVen>();
        ObservableCollection<Servicios> observListSer = new ObservableCollection<Servicios>();
        ObservableCollection<Cliente> observListClient = new ObservableCollection<Cliente>();

        //private String claveClientes;
        private String ServCod;
        public ConsultaTes(string usuario, int id, string nomb)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, usuario, id, nomb);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            this.usuario.Content = nomb;
            listaCl.Visibility = Visibility.Collapsed;
            listBServ.Visibility = Visibility.Collapsed;
            gReporte.Visibility = Visibility.Collapsed;
        }
        public ConsultaTes() { }
        private String total { set; get; }

        /// /////////////////TexBox///////////////////////////////////////////////////
        private void tBServ_KeyUp(object sender, KeyEventArgs e)
        {

            if (tBServ.Text.Equals(""))
            {
                listBServ.Visibility = Visibility.Collapsed;
            }
            else
            {
                listBServ.Visibility = Visibility.Visible;
                if (tBServ.Text.Equals(""))
                {
                    listBServ.Visibility = Visibility.Hidden;
                }
                else
                {
                    var conServ = from eleServ in conex.ServicioVent
                                  where eleServ.codigoServ.Contains(tBServ.Text) ||
                                  eleServ.nombreServ.Contains(tBServ.Text)
                                  select eleServ;
                    listBServ.SelectedValuePath = "codigoServ";
                    listBServ.DisplayMemberPath = "nombreServ";
                    listBServ.ItemsSource = conServ;
                }

            }
        }
        private void tBClient_KeyUp(object sender, KeyEventArgs e)
        {
            if (clave.Text.Equals(""))
            {
                listaCl.Visibility = Visibility.Collapsed;
            }
            else
            {
                listaCl.Visibility = Visibility.Visible;
                if (clave.Text.Equals(""))
                {
                    listaCl.Visibility = Visibility.Hidden;
                }
                else
                {
                    listaCl.Visibility = Visibility.Visible;
                    var almn = from al in conex.Alumno
                               select new
                               {
                                   al.matricula,
                                   nomAl = al.Nombre.Trim() + " " + al.apPater.Trim() + " " + al.apMater.Trim()
                               };
                    clt.Clear();
                    foreach (var al in almn)
                    {
                        if (al.matricula.Contains(clave.Text) || al.nomAl.ToLower().Contains(clave.Text.Trim().ToLower()))
                        {
                            clt.Add(new Clientes { ident = al.matricula, nombreCl = al.nomAl, tbl = 1 });
                        }
                    }
                    var empl = from em in conex.Empleado
                               select new
                               {
                                   em.numEmp,
                                   nomEm = em.Nombre.Trim() + " " + em.apPater.Trim() + " " + em.apMater.Trim()
                               };
                    foreach (var em in empl)
                    {
                        if (em.numEmp.ToString().Contains(clave.Text) || em.nomEm.ToLower().Contains(clave.Text.Trim().ToLower()))
                        {
                            clt.Add(new Clientes { ident = em.numEmp.ToString(), nombreCl = em.nomEm, tbl = 2 });
                        }
                    }
                    var clex = from ce in conex.ClienteExt
                               select new
                               {
                                   ce.curp_rfc,
                                   cen = ce.Nombre.Trim() + " " + ce.apPater.Trim() + " " + ce.apMater.Trim()
                               };
                    foreach (var cl in clex)
                    {
                        if (cl.curp_rfc.Contains(clave.Text) || cl.cen.ToLower().Contains(clave.Text.Trim().ToLower()))
                        {
                            clt.Add(new Clientes { ident = cl.curp_rfc, nombreCl = cl.cen, tbl = 3 });
                        }
                    }
                    listaCl.SelectedValuePath = "ident";
                    listaCl.DisplayMemberPath = "nombreCl";
                    listaCl.ItemsSource = clt;

                }
            }
        }
        /// ///////////////Buttons/////////////////////////////////////////////////////////
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int i;
            if (expFecha.IsExpanded == true)
            {
                if (expServicios.IsExpanded == true)
                {
                    if (expClientes.IsExpanded == true)
                    {
                        if (expFolio.IsExpanded == true)
                        {
                            if (tBFolIn.Text.Equals("") || tBFolFin.Text.Equals("") || 
                                dPFecIn.Text.Equals("") || dPFecFin.Text.Equals("") || observListSer.Count == 0 ||
                                  observListClient.Count == 0)
                            {
                                MessageBox.Show("Especifica los Rangos de las Fechas, folios, el(los) tipo(s) de servicio y los clientes ");
                            }
                            else
                            {
                                int folioIn = int.Parse(tBFolIn.Text);
                                int folioFin = int.Parse(tBFolFin.Text);
                                int j;
                                tabCoVen.Clear();
                                for (i = 0; i <= observListSer.Count - 1; i++)
                                {
                                    for (j = 0; j <= observListClient.Count - j; j++)
                                    {
                                        busqXFolFecServClient(dPFecIn.Text, dPFecFin.Text, folioIn, folioFin,
                                            observListSer.ElementAt(i).codigoServ, observListClient.ElementAt(j).ident);
                                    }
                                }//14
                            }
                        }
                        else
                        {
                            if (dPFecIn.Text.Equals("") || dPFecFin.Text.Equals("") || observListSer.Count == 0 ||
                                  observListClient.Count == 0)
                            {
                                MessageBox.Show("Especifica los Rangos de las Fechas, el(los) tipo(s) de servicio y los clientes ");
                            }
                            else
                            {
                                int j;
                                tabCoVen.Clear();
                                for (i = 0; i <= observListSer.Count - 1; i++)
                                {
                                    for (j = 0; j <= observListClient.Count - j; j++)
                                    {
                                        busqXFecServClient(dPFecIn.Text, dPFecFin.Text, observListSer.ElementAt(i).codigoServ, observListClient.ElementAt(j).ident);
                                    }
                                }//3
                            }

                        }
                    }
                    else
                    {

                        if (expFolio.IsExpanded == true)
                        {
                            if (dPFecIn.Text.Equals("") || dPFecFin.Text.Equals("") ||
                                  tBFolIn.Text.Equals("") || tBFolFin.Text.Equals("") || observListSer.Count == 0)
                            {
                                MessageBox.Show("Especifica los Rangos de los Folios, fechas y el(los) tipo(s) de servicio ");
                            }
                            else
                            {
                                tabCoVen.Clear();
                                int folioIn = int.Parse(tBFolIn.Text);
                                int folioFin = int.Parse(tBFolFin.Text);
                                for (i = 0; i <= observListSer.Count - 1; i++)
                                {
                                    busqXFecServFolio(dPFecIn.Text, dPFecFin.Text, folioIn, folioFin, observListSer.ElementAt(i).codigoServ);
                                } //7
                            }
                        }
                        else
                        {
                            if (dPFecIn.Text.Equals("") || dPFecFin.Text.Equals("") || observListSer.Count == 0)
                            {
                                MessageBox.Show("Especifica los Rangos de las Fechas y el(los) tipo(s) de servicio ");
                            }
                            else
                            {
                                tabCoVen.Clear();
                                int j;
                                for (j = 0; j <= observListSer.Count - 1; j++)
                                {
                                    busqXFecServ(dPFecIn.Text, dPFecFin.Text, observListSer.ElementAt(j).codigoServ);
                                } //2
                            }
                        }
                    }
                }
                else
                {
                    if (expFolio.IsExpanded == true)
                    {
                        if (expClientes.IsExpanded == true)
                        {
                            if (tBFolIn.Text.Equals("") || tBFolFin.Text.Equals("") || 
                                dPFecIn.Text.Equals("") || dPFecFin.Text.Equals("") ||
                                   observListClient.Count == 0)
                            {
                                MessageBox.Show("Especifica los Rangos de los Fechas, folios y clientes");
                            }
                            else
                            {
                                tabCoVen.Clear();
                                int folioIn = int.Parse(tBFolIn.Text);
                                int folioFin = int.Parse(tBFolFin.Text);
                                for (i = 0; i <= observListClient.Count - 1; i++)
                                {
                                    busqXFecFolioClient(dPFecIn.Text, dPFecFin.Text, folioIn, folioFin, observListClient.ElementAt(i).ident);
                                } //13
                            }
                        }
                        else
                        {
                            if (dPFecIn.Text.Equals("") || dPFecFin.Text.Equals("") ||
                                      tBFolIn.Text.Equals("") || tBFolFin.Text.Equals(""))
                            {
                                MessageBox.Show("Especifica los Rangos de los Folios y fechas");
                            }
                            else
                            {
                                int folioIn = int.Parse(tBFolIn.Text);
                                int folioFin = int.Parse(tBFolFin.Text);
                                busqXFecFolio(dPFecIn.Text, dPFecFin.Text, folioIn, folioFin);//8
                            }
                        }
                    }
                    else
                    {
                        if (expClientes.IsExpanded == true)
                        {
                            if (dPFecIn.Text.Equals("") || dPFecFin.Text.Equals("") ||
                                   observListClient.Count == 0)
                            {
                                MessageBox.Show("Especifica los Rangos de los Fechas y los clientes");
                            }
                            else
                            {
                                tabCoVen.Clear();

                                for (i = 0; i <= observListClient.Count - 1; i++)
                                {
                                    busqXFecClients(dPFecIn.Text, dPFecFin.Text, observListClient.ElementAt(i).ident);
                                } //11
                            }
                        }
                        else
                        {
                            if (dPFecIn.Text.Equals("") || dPFecFin.Text.Equals(""))
                            {
                                MessageBox.Show("Especifica los Rangos de las Fechas");
                            }
                            else
                            {
                                busqXFecha(dPFecIn.Text, dPFecFin.Text);//1 
                            }
                        }
                    }
                }
            }
            else if (expServicios.IsExpanded == true)
            {
                if (expClientes.IsExpanded == true)
                {
                    if (expFolio.IsExpanded == true)
                    {
                        if (tBFolIn.Text.Equals("") || tBFolFin.Text.Equals("") || observListSer.Count == 0 ||
                                  observListClient.Count == 0)
                        {
                            MessageBox.Show("Especifica los Rangos de las folios, el(los) tipo(s) de servicio y los clientes ");
                        }
                        else
                        {
                            int folioIn = int.Parse(tBFolIn.Text);
                            int folioFin = int.Parse(tBFolFin.Text);
                            int j;
                            tabCoVen.Clear();
                            for (i = 0; i <= observListSer.Count - 1; i++)
                            {
                                for (j = 0; j <= observListClient.Count - j; j++)
                                {
                                    busqXFolServClient(folioIn, folioFin, observListSer.ElementAt(i).codigoServ, observListClient.ElementAt(j).ident);
                                }
                            }//15
                        }
                    }
                    else
                    {
                        if (observListSer.Count == 0 || observListClient.Count == 0)
                        {
                            MessageBox.Show("Especifica el(los) tipo(s) de servicio y los clientes ");
                        }
                        else
                        {
                            int j;
                            tabCoVen.Clear();
                            for (i = 0; i <= observListSer.Count - 1; i++)
                            {
                                for (j = 0; j <= observListClient.Count - j; j++)
                                {
                                    busqXServClient(observListSer.ElementAt(i).codigoServ, observListClient.ElementAt(j).ident);
                                }
                            }//4
                        }
                    }
                }
                else
                {
                    if (expFolio.IsExpanded == true)
                    {
                        if (tBFolIn.Text.Equals("") || tBFolFin.Text.Equals("") || observListSer.Count == 0)
                        {
                            MessageBox.Show("Especifica los Rangos de los Folios y el(los) tipo(s) de servicio ");
                        }
                        else
                        {
                            int folioIn = int.Parse(tBFolIn.Text);
                            int folioFin = int.Parse(tBFolFin.Text);
                            tabCoVen.Clear();
                            for (i = 0; i <= observListSer.Count - 1; i++)
                            {
                                busqXServFolio(folioIn, folioFin, observListSer.ElementAt(i).codigoServ);
                            } //6
                        }
                    }
                    else
                    {
                        if (observListSer.Count == 0)
                        {
                            MessageBox.Show("Especifica los Rangos de las Fechas, el(los) tipo(s) de servicio");
                        }
                        else
                        {
                            tabCoVen.Clear();
                            //int i;
                            for (i = 0; i <= observListSer.Count - 1; i++)
                            {
                                busqXServ(observListSer.ElementAt(i).codigoServ);
                            } //5
                        }
                    }
                }
            }
            else if (expClientes.IsExpanded == true)
            {
                if (expFolio.IsExpanded == true)
                {
                    if (tBFolIn.Text.Equals("") || tBFolFin.Text.Equals("") ||
                                   observListClient.Count == 0)
                    {
                        MessageBox.Show("Especifica los Rangos de los Folios y los clientes");
                    }
                    else
                    {
                        tabCoVen.Clear();
                        int folioIn = int.Parse(tBFolIn.Text);
                        int folioFin = int.Parse(tBFolFin.Text);
                        for (i = 0; i <= observListClient.Count - 1; i++)
                        {
                            busqXFolClients(folioIn,folioFin, observListClient.ElementAt(i).ident);
                        } //12
                    }                  
                }
                else
                {
                    if (observListClient.Count == 0)
                    {
                        MessageBox.Show("Especifica a los clientes");
                    }
                    else
                    {
                        tabCoVen.Clear();
                        for (i = 0; i <= observListClient.Count - 1; i++)
                        {
                            busqXClient(observListClient.ElementAt(i).ident);
                        } //9
                    }
                }
            }
            else if (expFolio.IsExpanded == true)
            {
                if (tBFolIn.Text.Equals("") || tBFolFin.Text.Equals(""))
                {
                    MessageBox.Show("Especifica los Rangos de los Folios");
                }
                else
                {
                    int folioIn = int.Parse(tBFolIn.Text);
                    int folioFin = int.Parse(tBFolFin.Text);
                    busqXFolio(folioIn, folioFin);//10
                }
            }
            int i1;
            double suma = 0;
            for (i1 = 0; i1 <= tabCoVen.Count - 1; i1++)
                suma = suma + tabCoVen.ElementAt(i1).totalArticulo;
            lTotal.Content = "" + suma;
            observListSer.Clear();
            dGListServ.ItemsSource = observListSer;
            observListClient.Clear();
            dGListClient.ItemsSource = observListClient;
            tBFolIn.Text = "";
            tBFolFin.Text = "";

        }
        private void menuCerrarSesion_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
            }
            else { }
        }
        //////////////////ListBox/////////////////////////////////////////////////////////////////////////////////
        private void listBServ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int encontrado = 0;

            if (listBServ.SelectedValue != null)
            {
                ServCod = listBServ.SelectedValue.ToString();
                var ser = from al in conex.ServicioVent
                          where al.codigoServ.Equals(ServCod)
                          select al;
                foreach (var serv in ser)
                {
                    //   
                    if (observListSer.Count == 0)
                    {
                        observListSer.Add(new Servicios { codigoServ = serv.codigoServ, nombreServ = serv.nombreServ });
                        dGListServ.ItemsSource = observListSer;
                    }
                    else
                    {
                        int i;
                        for (i = 0; i <= observListSer.Count - 1; i++)
                        {

                            if (listBServ.SelectedValue.ToString() == observListSer.ElementAt(i).codigoServ)
                            {
                                encontrado = 1;
                            }
                        }
                        if (encontrado == 0)
                        {
                            observListSer.Add(new Servicios { codigoServ = serv.codigoServ, nombreServ = serv.nombreServ });
                            dGListServ.ItemsSource = observListSer;
                        }
                    }
                }
            }
            tBServ.Text = "";
            listBServ.Visibility = Visibility.Hidden;
            tBServ.Text = "";
        }
        private void listaCl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String cliente = "";
            if (listaCl.SelectedValue != null)
            {
                Clientes reg = listaCl.SelectedItem as Clientes;
                if (reg.tbl == 1)
                {
                    var almn = (from al in conex.Alumno
                                where al.matricula.Equals(listaCl.SelectedValue)
                                select al).Single();
                    cliente = almn.Nombre + " " + almn.apPater + " " + almn.apMater;
                }
                else if (reg.tbl == 2)
                {
                    var empl = (from em in conex.Empleado
                                where em.numEmp.ToString().Equals(listaCl.SelectedValue)
                                select em).Single();
                    cliente = empl.Nombre + " " + empl.apPater + " " + empl.apMater;
                }
                else if (reg.tbl == 3)
                {
                    var clex = (from ce in conex.ClienteExt
                                where ce.curp_rfc.Equals(listaCl.SelectedValue)
                                select ce).Single();
                    cliente = clex.Nombre + " " + clex.apPater + " " + clex.apMater;
                }
                int encontrado = 0;
                if (observListClient.Count == 0)
                {
                    observListClient.Add(new Cliente { ident = listaCl.SelectedValue.ToString(), nombreCl = cliente });
                    dGListClient.ItemsSource = observListClient;
                }
                else
                {
                    int i;

                    for (i = 0; i <= observListClient.Count - 1; i++)
                    {
                        if (listaCl.SelectedValue.ToString() == observListClient.ElementAt(i).ident)
                        {
                            encontrado = 1;
                        }
                    }
                    if (encontrado == 0)
                    {
                        observListClient.Add(new Cliente { ident = listaCl.SelectedValue.ToString(), nombreCl = cliente });
                        dGListClient.ItemsSource = observListClient;
                    }
                }
                clave.Text = "";
            }
            else
            { }
            listaCl.Visibility = Visibility.Hidden;
        }
        private void dGListServ_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int numero = dGListServ.SelectedIndex;
            observListSer.RemoveAt(numero);
            dGListServ.ItemsSource = observListSer;
        }
        private void dGListClient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int numero = dGListClient.SelectedIndex;
            observListClient.RemoveAt(numero);
            dGListClient.ItemsSource = observListClient;
        }
        /// ////////////////////Tipos de Consultas//////////////////////////////////////////////////////////
        private String busquedaCliente(String identificador)
        {
            /*
                * el siguiente codigo hace las busquedas del nombre del cliente en cada una de las tablas
                */
            String name = "";
            var almn = from al in conex.Alumno
                       select new
                       {
                           al.matricula,
                           nomAl = al.Nombre.Trim() + " " + al.apPater.Trim() + " " + al.apMater.Trim()
                       };
            clt.Clear();
            foreach (var al in almn)
            {
                if (al.matricula.Equals(identificador))
                {
                    name = al.nomAl;
                }
            }
            var empl = from em in conex.Empleado
                       select new
                       {
                           em.numEmp,
                           nomEm = em.Nombre.Trim() + " " + em.apPater.Trim() + " " + em.apMater.Trim()
                       };
            foreach (var em in empl)
            {
                if (em.numEmp.ToString().Equals(identificador))
                {
                    name = em.nomEm;
                }
            }
            var clex = from ce in conex.ClienteExt
                       select new
                       {
                           ce.curp_rfc,
                           cen = ce.Nombre.Trim() + " " + ce.apPater.Trim() + " " + ce.apMater.Trim()
                       };
            foreach (var cl in clex)
            {
                if (cl.curp_rfc.Equals(identificador))
                {
                    name = cl.cen;
                }
            }
            return name;
        }

        private void busqXFecha(String fechaI, String fechaF)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha*/
            tabCoVen.Clear();
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF)
                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {
                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });
            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFecServ(String fechaI, String fechaF, String ServCod)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha y por servicio*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF) &&
                         eleSer.codigoServ == ServCod
                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFecServClient(String fechaI, String fechaF, String ServCod, String identif)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF) &&
                         eleSer.codigoServ == ServCod &&
                         eleVEnc.identClien == identif

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXServClient(String ServCod, String identif)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleSer.codigoServ == ServCod &&
                         eleVEnc.identClien == identif

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha), //el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });
            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXServ(String ServCod)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/


            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleSer.codigoServ == ServCod


                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            //tabCoVen.OrderBy=tab
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXServFolio(int folioIn, int folioFin, String ServCod)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin &&
                         eleSer.codigoServ == ServCod

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFecServFolio(String fechaI, String fechaF, int folioIn, int folioFin, String ServCod)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF) &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin &&
                         eleSer.codigoServ == ServCod

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFecFolio(String fechaI, String fechaF, int folioIn, int folioFin)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            tabCoVen.Clear();

            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF) &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXClient(String codClients)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/


            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.identClien == codClients

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFolio(int folioIn, int folioFin)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            tabCoVen.Clear();

            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFecClients(String fechaI, String fechaF, String codClients)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha y por servicio*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF) &&
                         eleVEnc.identClien == codClients


                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,//WTF PATO?
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFolClients(int folioIn, int folioFin, String codClients)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha y por servicio*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin &&
                         eleVEnc.identClien == codClients


                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {
                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFecFolioClient(String fechaI, String fechaF, int folioIn, int folioFin, String codClients)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF) &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin &&
                         eleVEnc.identClien == codClients
                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFolFecServClient(String fechaI, String fechaF, int folioIn, int folioFin, String ServCod, String identif)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin &&
                         eleVEnc.fecha >= DateTime.Parse(fechaI)
                         && eleVEnc.fecha <= DateTime.Parse(fechaF) &&
                         eleSer.codigoServ == ServCod &&
                         eleVEnc.identClien == identif

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ,
                    totalArticulo = el.totalArticulo.Value
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }

        private void busqXFolServClient(int folioIn, int folioFin, String ServCod, String identif)
        {
            /*se hace una consulta para seleccionar los datos de venta encabezado y detalle segun
             los rangos de fecha, por servicio y por cliente*/
            var consul = from eleVEnc in conex.VentasVEnc
                         from eleVD in conex.VentasVDet
                         from eleSer in conex.ServicioVent
                         where eleVEnc.folio == eleVD.folio &&
                         eleVD.idServicio == eleSer.idServicio &&
                         eleVEnc.folio >= folioIn
                         && eleVEnc.folio <= folioFin &&
                         eleSer.codigoServ == ServCod &&
                         eleVEnc.identClien == identif

                         select new
                         {
                             eleVEnc.fecha,
                             eleVEnc.folio,
                             eleVEnc.identClien,
                             eleVD.idServicio,
                             eleSer.nombreServ,
                             eleVD.totalArticulo

                         };
            foreach (var el in consul)
            {

                //Metodo que llena el ObservableCollection de la clase CoVen              
                tabCoVen.Add(new CoVen
                {
                    fecha = Convert.ToString(el.fecha),//el.fecha.Year + "/" + el.fecha.Month + "/" + el.fecha.Day,
                    folio = el.folio.Value,
                    identClien = busquedaCliente(el.identClien),
                    nombreServ = el.nombreServ.ToString(),
                    totalArticulo = el.totalArticulo.Value//.ToString()
                });

            }
            // llena el data grid de consulta ventanilla
            dGCoVen.ItemsSource = tabCoVen;
        }



        /*Expanded*/
        private void expServicios_Expanded(object sender, RoutedEventArgs e)
        {
            tBServ.Text = "";
            observListSer.Clear();
            dGListServ.ItemsSource = observListSer;
        }
        private void expFecha_Expanded(object sender, RoutedEventArgs e)
        {
            lFecIn.Visibility = Visibility.Visible;
            lFecFin.Visibility = Visibility.Visible;
            dPFecIn.Visibility = Visibility.Visible;
            dPFecFin.Visibility = Visibility.Visible;
            dPFecIn.Text = DateTime.Today.ToShortDateString();
            dPFecFin.Text = DateTime.Today.ToShortDateString();
        }
        private void expClientes_Expanded(object sender, RoutedEventArgs e)
        {
            clave.Text = "";
            observListClient.Clear();
            dGListClient.ItemsSource = observListClient;
            // expFolio.IsExpanded=false;
        }
        private void expFolio_Expanded(object sender, RoutedEventArgs e)
        {
            tBFolIn.Text = "";
            tBFolFin.Text = "";
            // expClientes.IsExpanded = false;
        }

        private void bGenerarReporte_Click(object sender, RoutedEventArgs e)
        {
            gReporte.Visibility = Visibility.Visible;
            visor.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", tabCoVen));
            visor.LocalReport.ReportPath = "D:\\unidD\\uni\\11vo Cuatro\\SacIntegradoUltimo2013\\SacIntegrado\\SacIntegrado\\Tesoreria\\ReporteTesoreria.rdlc";
            visor.RefreshReport();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            gReporte.Visibility = Visibility.Hidden;
        }

    }
}
public class Clientes
{
    public string ident { get; set; }
    public String nombreCl { get; set; }
    public int tbl { get; set; }
}
public class Cliente
{
    public string ident { get; set; }
    public String nombreCl { get; set; }
}
public class Servicios
{
    public int idServicio { get; set; }
    public String codigoServ { get; set; }
    public String nombreServ { get; set; }
}
public class CoVen //Clase de (Co)nsulta (Ven)tanilla
{
    public string fecha { get; set; }
    public int folio { get; set; }
    public string identClien { get; set; }
    public int idServicio { get; set; }
    public string nombreServ { get; set; }
    public double totalArticulo { get; set; }
}