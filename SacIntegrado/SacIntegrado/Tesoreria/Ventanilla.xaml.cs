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
using Microsoft.Reporting.WinForms;

namespace SacIntegrado.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para Ventanilla.xaml
    /// </summary>
    public partial class Ventanilla : Page
    {
        Db conex = new Db();
        Cliente ct = new Cliente();
        ObservableCollection<DatosServicio> ds = new ObservableCollection<DatosServicio>();
        ObservableCollection<Cliente> clt = new ObservableCollection<Cliente>();
        ObservableCollection<Cliente> tp = new ObservableCollection<Cliente>();
        ObservableCollection<RecEnc> vEnc = new ObservableCollection<RecEnc>();
        ObservableCollection<RecDet> vDet = new ObservableCollection<RecDet>();
        int encontrado;
        double dscN, dscP, tot;

        public Ventanilla()
        {
            InitializeComponent();
        }

        public Ventanilla(string usuario, int id, string nomb)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, usuario, id, nomb);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            this.usuario.Content = nomb;
            fecha.Content = DateTime.Today.ToShortDateString();
            cantidad.Text = "1";
            descNeto.Text = "0";
            descPorcentual.Text = "0";
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

        //Evento para buscar cliente por nombre, matricula, numero de empleado, rfc o curp.
        private void clave_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (clave.Text.Equals(""))
            {
                listaCl.Visibility = Visibility.Hidden;
                servicio.IsEnabled = false;
                listaServ.Visibility = Visibility.Hidden;
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
                    if (al.matricula.Contains(clave.Text.Trim()) || al.nomAl.ToLower().Contains(clave.Text.Trim().ToLower()))
                    {
                        clt.Add(new Cliente { ident = al.matricula, nombreCl = al.nomAl, tbl = 1 });
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
                    if (em.numEmp.ToString().Contains(clave.Text.Trim()) || em.nomEm.ToLower().Contains(clave.Text.Trim().ToLower()))
                    {
                        clt.Add(new Cliente { ident = em.numEmp.ToString(), nombreCl = em.nomEm, tbl = 2 });
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
                    if (cl.curp_rfc.Contains(clave.Text.Trim()) || cl.cen.ToLower().Contains(clave.Text.Trim().ToLower()))
                    {
                        clt.Add(new Cliente { ident = cl.curp_rfc, nombreCl = cl.cen, tbl = 3 });
                    }
                }
                listaCl.SelectedValuePath = "ident";
                listaCl.DisplayMemberPath = "nombreCl";
                listaCl.ItemsSource = clt;
            }
        }

        //Evento para buscar servicios por medio del nombre o codigo del servicio
        private void servicio_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (servicio.Text.Equals(""))
            {
                listaServ.Visibility = Visibility.Hidden;
            }
            else
            {
                listaServ.Visibility = Visibility.Visible;
                var sv = from sev in conex.ServicioVent
                         where sev.codigoServ.Contains(servicio.Text)
                         || sev.nombreServ.Contains(servicio.Text)
                         select sev;
                listaServ.SelectedValuePath = "codigoServ";
                listaServ.DisplayMemberPath = "nombreServ";
                listaServ.ItemsSource = sv;
            }
        }

        //Evento del boton añadir, el cual agrega o enlista los servicios solicitados en un DataGrid
        private void añadir_Click(object sender, RoutedEventArgs e)
        {
            encontrado = 0;
            if (listaServ.SelectedIndex >= 0)
            {
                var datSrv = (from sv in conex.ServicioVent
                              where sv.codigoServ.Equals(listaServ.SelectedValue)
                              select sv).Single();
                if (cbNeto.IsChecked == true)
                {
                    dscN = double.Parse(descNeto.Text);
                    dscP = 0;
                    tot = datSrv.precio.Value * Convert.ToDouble(cantidad.Text.Trim()) - dscN * Convert.ToDouble(cantidad.Text.Trim());

                }
                else if (cbPorcentual.IsChecked == true)
                {
                    dscN = 0;
                    dscP = double.Parse(descPorcentual.Text);
                    tot = (datSrv.precio.Value * double.Parse(cantidad.Text.Trim()))
                        - ((dscP / 100) * (datSrv.precio.Value * double.Parse(cantidad.Text.Trim())));
                }
                else if (cbNeto.IsChecked == false && cbPorcentual.IsChecked == false)
                {
                    dscN = 0;
                    dscP = 0;
                    tot = datSrv.precio.Value * double.Parse(cantidad.Text.Trim());
                }
                if (ds.Count == 0)
                {
                    ds.Add(new DatosServicio
                    {
                        claveServ = datSrv.codigoServ,
                        nombreServ = datSrv.nombreServ,
                        precio = datSrv.precio.Value,
                        cantidad = double.Parse(cantidad.Text.Trim()),
                        dNeto = dscN,
                        dPorc = dscP,
                        total = tot
                    });
                    servDg.ItemsSource = ds;
                }
                else
                {
                    for (int i = 0; i < ds.Count; i++)
                    {
                        if (listaServ.SelectedValue.Equals(ds.ElementAt(i).claveServ) && ds.ElementAt(i).dNeto == double.Parse(descNeto.Text) && ds.ElementAt(i).dPorc == double.Parse(descPorcentual.Text))
                        {
                            ds.ElementAt(i).cantidad = ds.ElementAt(i).cantidad + double.Parse(cantidad.Text);
                            if (cbNeto.IsChecked == true)
                            {
                                dscN = double.Parse(descNeto.Text);
                                dscP = 0;
                                tot = ((ds.ElementAt(i).precio * ds.ElementAt(i).cantidad) - (dscN * ds.ElementAt(i).cantidad));

                            }
                            else if (cbPorcentual.IsChecked == true)
                            {
                                dscN = 0;
                                dscP = double.Parse(descPorcentual.Text);
                                double pr = ((dscP / 100) * (ds.ElementAt(i).precio * ds.ElementAt(i).cantidad));
                                tot = (ds.ElementAt(i).precio * ds.ElementAt(i).cantidad) - pr;
                            }
                            else if (cbNeto.IsChecked == false && cbPorcentual.IsChecked == false)
                            {
                                dscN = 0;
                                dscP = 0;
                                tot = ds.ElementAt(i).precio * ds.ElementAt(i).cantidad;
                            }
                            ds.ElementAt(i).total = tot;
                            servDg.Items.Refresh();
                            encontrado = 1;
                        }
                    }
                    if (encontrado == 0)
                    {
                        ds.Add(new DatosServicio
                        {
                            claveServ = datSrv.codigoServ,
                            nombreServ = datSrv.nombreServ,
                            precio = datSrv.precio.Value,
                            cantidad = double.Parse(cantidad.Text.Trim()),
                            dNeto = dscN,
                            dPorc = dscP,
                            total = tot
                        });
                        servDg.ItemsSource = ds;
                    }
                }
                totaliza();
                cantidad.Text = "1";
            }
            else { }
        }

        //Metodo para calcular el importe total de los productos agregados en el DataGrid
        public void totaliza()
        {
            double suma = 0;
            for (int i = 0; i < ds.Count; i++)
            {
                suma += ds.ElementAt(i).total;
            }
            impTotal.Content = suma;
        }

        //Evento que determina en que tabla de la BD se encuentra el cliente seleccionado en la lista 
        //para extraer su nombre completo y direccion...
        private void listaCl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaCl.SelectedValue != null)
            {
                Cliente reg = listaCl.SelectedItem as Cliente;
                ic.Content = reg.ident;
                if (reg.tbl == 1)
                {
                    var almn = (from al in conex.Alumno
                                where al.matricula.Equals(listaCl.SelectedValue)
                                select al).Single();
                    clave.Text = almn.Nombre + " " + almn.apPater + " " + almn.apMater;
                    direccion.Content = almn.estado + " " + almn.municipio + " " + almn.colonia + " " + almn.calle;
                }
                else if (reg.tbl == 2)
                {
                    var empl = (from em in conex.Empleado
                                where em.numEmp.ToString().Equals(listaCl.SelectedValue)
                                select em).Single();
                    clave.Text = empl.Nombre + " " + empl.apPater + " " + empl.apMater;
                    direccion.Content = empl.estado + " " + empl.municipio + " " + empl.colonia + " " + empl.calle;
                }
                else if (reg.tbl == 3)
                {
                    var clex = (from ce in conex.ClienteExt
                                where ce.curp_rfc.Equals(listaCl.SelectedValue)
                                select ce).Single();
                    clave.Text = clex.Nombre + " " + clex.apPater + " " + clex.apMater;
                    direccion.Content = clex.estado + " " + clex.municipio + " " + clex.colonia + " " + clex.calle;
                }
                servicio.IsEnabled = true;
            }
            else
            {
                servicio.IsEnabled = false;
                listaServ.Visibility = Visibility.Hidden;
            }
            listaCl.Visibility = Visibility.Hidden;
        }

        //Evento que determina el servicio seleccionado y escribe el nombre del mismo en un TextBox
        private void listaServ_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaServ.SelectedValue != null)
            {
                var sc = (from sv in conex.ServicioVent
                          where sv.codigoServ.Equals(listaServ.SelectedValue.ToString())
                          select sv).SingleOrDefault();
                servicio.Text = sc.nombreServ;
                listaServ.Visibility = Visibility.Hidden;
            }
            else { }
        }

        //Evento para controlar checkbox
        private void cbNeto_Click(object sender, RoutedEventArgs e)
        {
            if (cbNeto.IsChecked == true)
            {
                descNeto.IsEnabled = true;
                cbPorcentual.IsChecked = false;
                descPorcentual.IsEnabled = false;
                descPorcentual.Text = "0";
            }
            else if (cbNeto.IsChecked == false)
            {
                descNeto.IsEnabled = false;
                descNeto.Text = "0";
            }
        }

        //Evento para controlar checkbox
        private void cbPorcentual_Click(object sender, RoutedEventArgs e)
        {
            if (cbPorcentual.IsChecked == true)
            {
                descPorcentual.IsEnabled = true;
                cbNeto.IsChecked = false;
                descNeto.IsEnabled = false;
                descNeto.Text = "0";
            }
            else if (cbPorcentual.IsChecked == false)
            {
                descPorcentual.IsEnabled = false;
                descPorcentual.Text = "0";
            }
        }

        //Evento del boton impRecibo que guarda los datos de la venta en sus respectivas tablas
        private void impRecibo_Click(object sender, RoutedEventArgs e)
        {
            if (clave.Text != "" && servDg.ItemsSource != null)
            {
                //Venta Encabezado
                Table<VentasVEnc> ve = conex.GetTable<VentasVEnc>();
                VentasVEnc vec = new VentasVEnc();
                vec.folio = 0;
                vec.fecha = DateTime.Parse(fecha.Content.ToString());
                vec.identClien = ic.Content.ToString();
                vec.total = double.Parse(impTotal.Content.ToString());
                ve.InsertOnSubmit(vec);
                ve.Context.SubmitChanges();
                //Venta Detalle
                for (int i = 0; i < ds.Count; i++)
                {
                    Table<VentasVDet> vd = conex.GetTable<VentasVDet>();
                    VentasVDet vvd = new VentasVDet();
                    var idSrv = (from sv in conex.ServicioVent
                                 where sv.codigoServ.Equals(ds.ElementAt(i).claveServ)
                                 select sv).Single();
                    vvd.folio = vec.folio;
                    vvd.idServicio = idSrv.idServicio;
                    vvd.cantidad = ds.ElementAt(i).cantidad;
                    vvd.descNeto = ds.ElementAt(i).dNeto;
                    vvd.descPorc = ds.ElementAt(i).dPorc;
                    vvd.totalArticulo = ds.ElementAt(i).total;
                    vd.InsertOnSubmit(vvd);
                    vd.Context.SubmitChanges();
                    if (ds.ElementAt(i).dNeto != 0)
                    {
                        vDet.Add(new RecDet { concepto = ds.ElementAt(i).nombreServ, desc = ds.ElementAt(i).dNeto, importeArt = ds.ElementAt(i).total });
                    }
                    else if (ds.ElementAt(i).dPorc != 0)
                    {
                        vDet.Add(new RecDet { concepto = ds.ElementAt(i).nombreServ, desc = ds.ElementAt(i).dPorc, importeArt = ds.ElementAt(i).total });
                    }
                    else
                    {
                        vDet.Add(new RecDet { concepto = ds.ElementAt(i).nombreServ, desc = 0, importeArt = ds.ElementAt(i).total });
                    }

                }
                conRep.Visibility = Visibility.Visible;
                vEnc.Add(new RecEnc { folio = vec.folio.ToString(), nombreCl = clave.Text, direccion = direccion.Content.ToString(), ident = ic.Content.ToString(), importeTot = double.Parse(impTotal.Content.ToString()), vendedor = usuario.Content.ToString() });
                vistaRep.LocalReport.DataSources.Add(new ReportDataSource("RecDet", vDet));
                vistaRep.LocalReport.DataSources.Add(new ReportDataSource("RecEnc", vEnc));
                vistaRep.LocalReport.ReportPath = @"C:\Users\Public\Documents\SacIntegrado\SacIntegrado\Tesoreria\ReciboIngresos.rdlc";
                vistaRep.RefreshReport();
                terminaVent();
                MessageBox.Show("Venta Terminada", "¡Correcto!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                MessageBox.Show("No se puede realizar operación...", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        //Metodo para limpiar todos los datos de la venta al darla por terminada
        public void terminaVent()
        {
            ds.Clear();
            servDg.ItemsSource = ds;
            clave.Text = "";
            direccion.Content = "";
            servicio.Text = "";
            cantidad.Text = "1";
            cbNeto.IsChecked = false;
            cbPorcentual.IsChecked = false;
            impTotal.Content = "000.00";
            descNeto.IsEnabled = false;
            descNeto.Text = "0";
            descPorcentual.IsEnabled = false;
            descPorcentual.Text = "0";
        }

        //Evento para quitar una fila del DataGrid
        private void quitar_Click(object sender, RoutedEventArgs e)
        {
            if (servDg.SelectedIndex >= 0)
            {
                DatosServicio fl = servDg.SelectedItem as DatosServicio;
                ds.Remove(fl);
                servDg.Items.Refresh();
                totaliza();
            }
            else { MessageBox.Show("No selecciono ninguna fila", "Información", MessageBoxButton.OK, MessageBoxImage.Information); }

        }

        //AGREGAR O MODIFICAR UN CLIENTE
        private void agregaMod_Click(object sender, RoutedEventArgs e)
        {
            agregaNvo.Visibility = Visibility.Visible;
            tp.Clear();
            tp.Add(new Cliente { ident = "Alumno", tbl = 1 });
            tp.Add(new Cliente { ident = "Profesor", tbl = 2 });
            tp.Add(new Cliente { ident = "Cliente Externo", tbl = 3 });
            tipoCl.SelectedValuePath = "tbl";
            tipoCl.DisplayMemberPath = "ident";
            tipoCl.ItemsSource = tp;
        }

        private void salirAc_Click(object sender, RoutedEventArgs e)
        {
            agregaNvo.Visibility = Visibility.Collapsed;
            contDatos.Visibility = Visibility.Collapsed;
        }

        private void tipoCl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tipoCl.SelectedIndex >= 0)
            {
                contDatos.Visibility = Visibility.Visible;
            }
            else { }
        }

        private void guardar_Click(object sender, RoutedEventArgs e)
        {
            if (clIdent.Text != "" && nomAc.Text != "" && apPater.Text != "" && apMater.Text != "" && calle.Text != ""
                && colonia.Text != "" && municipio.Text != "" && estado.Text != "")
            {
                if ((int)tipoCl.SelectedValue == 1)
                {
                    Table<Alumno> al = conex.GetTable<Alumno>();
                    Alumno alm = new Alumno();
                    alm.Nombre = nomAc.Text;
                    alm.apPater = apPater.Text;
                    alm.apMater = apMater.Text;
                    alm.calle = calle.Text;
                    alm.colonia = colonia.Text;
                    alm.num = num.Text;
                    alm.municipio = municipio.Text;
                    alm.estado = estado.Text;
                    alm.telefono = telefono.Text;
                    alm.matricula = clIdent.Text;
                    al.InsertOnSubmit(alm);
                    al.Context.SubmitChanges();
                }
                else if ((int)tipoCl.SelectedValue == 2)
                {
                    Table<Empleado> em = conex.GetTable<Empleado>();
                    Empleado emp = new Empleado();
                    emp.Nombre = nomAc.Text;
                    emp.apPater = apPater.Text;
                    emp.apMater = apMater.Text;
                    emp.calle = calle.Text;
                    emp.colonia = colonia.Text;
                    emp.num = num.Text;
                    emp.municipio = municipio.Text;
                    emp.estado = estado.Text;
                    emp.telefono = telefono.Text;
                    emp.numEmp = Int32.Parse(clIdent.Text);
                    em.InsertOnSubmit(emp);
                    em.Context.SubmitChanges();
                }
                else if ((int)tipoCl.SelectedValue == 3)
                {
                    Table<ClienteExt> ce = conex.GetTable<ClienteExt>();
                    ClienteExt cex = new ClienteExt();
                    cex.Nombre = nomAc.Text;
                    cex.apPater = apPater.Text;
                    cex.apMater = apMater.Text;
                    cex.calle = calle.Text;
                    cex.colonia = colonia.Text;
                    cex.num = num.Text;
                    cex.municipio = municipio.Text;
                    cex.estado = estado.Text;
                    cex.telefono = telefono.Text;
                    cex.curp_rfc = clIdent.Text;
                    ce.InsertOnSubmit(cex);
                    ce.Context.SubmitChanges();
                }
                MessageBox.Show("Cliente Agregado", "Correcto", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Verifique la información. Hacen falta algunos datos., ", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }
        //AGREGAR O MODIFICAR UN CLIENTE

        private void cerrarRep_Click(object sender, RoutedEventArgs e)
        {
            conRep.Visibility = Visibility.Hidden;
        }

    }

    class DatosServicio
    {
        public String claveServ { get; set; }
        public String nombreServ { get; set; }
        public double cantidad { get; set; }
        public double dNeto { get; set; }
        public double dPorc { get; set; }
        public double precio { get; set; }
        public double total { get; set; }
    }

    class Cliente
    {
        public String ident { get; set; }
        public String nombreCl { get; set; }
        public int tbl { get; set; }
    }

    class RecDet
    {
        public String concepto { get; set; }
        public double importeArt { get; set; }
        public double desc { get; set; }
    }

    class RecEnc
    {
        public String folio { get; set; }
        public String fecha { get; set; }
        public String ident { get; set; }
        public String nombreCl { get; set; }
        public String direccion { get; set; }
        public String vendedor { get; set; }
        public double importeTot { get; set; }
    }
}