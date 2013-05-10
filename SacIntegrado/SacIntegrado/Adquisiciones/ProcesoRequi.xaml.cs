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
using System.Linq;
using System.Data;
using System.Collections.ObjectModel;
using System.Data.Linq.SqlClient;

namespace SacIntegrado
{
    public partial class ProcesoRequi
    {
        //PARA COLECCIONES DE CLASES DE CADA TABLA
        ObservableCollection<RqEncab> objRqE = new ObservableCollection<RqEncab>();
        //ObservableCollection<RqDetalle> objRqD = new ObservableCollection<RqDetalle>();
        ObservableCollection<InfoReqDet> objRqD = new ObservableCollection<InfoReqDet>();
        //
        ObservableCollection<ComboDepto> ocDpt = new ObservableCollection<ComboDepto>();//PARA LLENAR EL COMBO DE DEPTO
        ObservableCollection<Producto> c = new ObservableCollection<Producto>();//PARA LLENAR EL COMBO DE PRODUCTO
        Db conex = new Db();//DATACONTEXT
        //ObservableCollection<Requisicion> col;
        ObservableCollection<InfoReqDet> col = new ObservableCollection<InfoReqDet>();//ObservableCollection<InfoReqDet> col;
        public static String nomLog;
        String usu;
        //VARS PARA GUARDAR EL ID Y EL NOMBRE DEL PROYECTO PRESUP AL SELECCIONAR UNO DEL COMBO
        int idProject = 0;
        //string nombProject = "";
        //FIN VARS PARA PROYECTO PRESE
        int idEmpLog = 0;//ID EMPLEADO LOGEADO
        string nomEmple = "";
        int idProduct = 0;//ID PRODUCTO DEL FILTRO
        int idDpt = 0;
        float totImport = 0;//PARA EL TOTAL Y EL IMPORTE
        string fecRequerida = "";
        float coste = 0;//VAR PARA GUARDAR EL PRECIO

        //CONSTRUCTOR PARA GUARDAR EL ID DEL USUARIO Q SE LOGEO
        public ProcesoRequi(String user, int id, String nombre)
        {
            this.InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            dpRequeridoNew.Text = DateTime.Today.ToShortDateString();
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            fechaElabNewR.Content = DateTime.Today.ToShortDateString();
            usuario.Content = nombre/*user*/;
            idEmpLog = id;//ID DE USUARIO LOGEADO
            nomEmple = nombre;//NOMBRE USER LOGEADO
            usu = user;
            //ObservableCollection<InfoReqDet> col = new ObservableCollection<InfoReqDet>();
            //tabNewReq.ItemsSource = col;//PARA AGREGAR LOS DATOS AL GRID E INSERTAR EN LA BD
        }

        private void menuReqNueva(object sender, System.Windows.RoutedEventArgs e)
        {
            gNueva.Visibility = Visibility.Visible; 
        }


        private void cerrarSesion(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
              //  InicioLogin.banderaLog = false;
            }
            else { }
        }

        private void btnSalirReqNew(object sender, System.Windows.RoutedEventArgs e)
        {
            MenuIU miu = new MenuIU();
            this.NavigationService.Navigate(miu);
        }

        private void habilitarObserv(object sender, System.Windows.RoutedEventArgs e)
        {
            if (chbHabObsNew.IsChecked == true) { txtObservNewReq.IsEnabled = true; }
            else{ txtObservNewReq.IsEnabled = false; }
        }

        private void habObservac(object sender, System.Windows.RoutedEventArgs e)
        {
            if (chbHabObsNew.IsChecked == true) { txtObservNewReq.IsEnabled = true; }
            else{ txtObservNewReq.IsEnabled = false; }
        }

        public void cleanInterfNewReq() {
            txtProdSearch.Text = "";
            txtProySearch.Text = "";
            txtDptoSearch.Text = "";
            txtRazonNewReq.Text = "";
            txtObservNewReq.Text = "";
            txtObservNewReq.IsEnabled = false;
            chbHabObsNew.IsEnabled = false;
            col.Clear();
            btnSendDg.IsEnabled = false;
            txtProySearch.IsEnabled = true;

        }

        //EVENTO CLICK DEL BOTON AGREGAR A DATAGRID
        private void addDgId(object sender, RoutedEventArgs e)//---------------------------------------------AGREGAR A DG
        {
            float costo;
            float totalReq;
            idDpt = (int)listviewDpto.SelectedValue;

            if (dpRequeridoNew.Text.Length < 10 || dpRequeridoNew.Text.Length > 10) { MessageBox.Show("Verifique el formato de su fecha"); btnSendDg.IsEnabled = false; return; }
            else
            {
                if (txtProySearch.Text.Length == 0 || txtDptoSearch.Text.Length == 0 || txtProdSearch.Text.Length == 0) { MessageBox.Show("Debe llenar los campos necesarios"); btnSendDg.IsEnabled = false; return; }
                else
                {
                    btnSendDg.IsEnabled = true;
                    string fechaHoy = DateTime.Today.ToShortDateString();
                    int dia = Convert.ToInt32(DateTime.Today.Day.ToString());

                    string fechaNeces = dpRequeridoNew.Text;
                    fecRequerida = fechaNeces;
                    int diaRequerido = Convert.ToInt32(fechaNeces.Substring(0, 2));

//                    if (diaRequerido > dia + 3)
//                    {
                        //MessageBox.Show("Se Puede La fecha es mayor a 3 días");
                        Producto pr = listviewProd.SelectedItem as Producto;
                        int idProduc = int.Parse(listviewProd.SelectedValue.ToString());
                        MessageBox.Show("ID Prd "+idProduc);
                    //PROCESO PARA OBTENER EL PRECIO DEL PRODUCTO SEGUN LA ULTIMA COTIZACIÓN SI EXISTE [2FEB]
                    var getPrcCot = (from cd in conex.CotizacionDet
                                    from ce in conex.CotizacionEnc
                                    where ce.idCotizacionEnc == cd.idCotizacionEnc &&
                                    cd.idProd == idProduc
                                    orderby ce.fecha
                                    select cd.precio).SingleOrDefault();

                    if (getPrcCot == null)
                    {
                        costo = 0;
                        totalReq = 0;
                        var row = (from produ in conex.Producto
                                   from cate in conex.CategoriaProd
                                   where produ.Nombre == pr.Nombre &&//listviewProd.SelectedValue/*prd.Nombre*/
                                   cate.idCategoria == pr.idClasificador
                                   select new { produ, cate }).SingleOrDefault();
                        MessageBox.Show("Clasificador "+row.cate.Nombre);
                        //foreach (var xf in row)
                        //{
                        coste = costo;//GUARDO EN LA VAR D INSTANCIA EL PRECIO DEL PRODUCTO---->//2FEB NO SE PARA Q WTF! ¬¬'
                        float cant = float.Parse(cantidProdNewReq.Text);
                        totalReq = cant * (costo);
                        totImport = totalReq;
                        string clasi = row.cate.Nombre;
                        MessageBox.Show("Clasif antes del Col "+clasi);
                        //}
                        col.Add(new InfoReqDet { idPto = idProduc, nomPto = pr.Nombre, ctdd = float.Parse(cantidProdNewReq.Text), prcio = costo, tot = totalReq, uMedda = unidMedNewReq.Content.ToString(), mot = txtRazonNewReq.Text, clasif = clasi/*row.cate.Nombre*/ });
                        tabNewReq.ItemsSource = col;

                        

                    }
                    else{
                        costo = (float)getPrcCot.Value;
                        MessageBox.Show("Dscrip "+pr.Descripcion+" Clasif "+pr.idClasificador);
                        var row = (from produ in c
                                   from cate in conex.CategoriaProd
                                   where produ.Nombre == pr.Nombre &&//listviewProd.SelectedValue/*prd.Nombre*/
                                   cate.idCategoria == pr.idClasificador
                                   select new { produ, cate }).SingleOrDefault();

                        //foreach (var xf in row)
                        //{
                            coste = costo;//GUARDO EN LA VAR D INSTANCIA EL PRECIO DEL PRODUCTO---->//2FEB NO SE PARA Q WTF! ¬¬'
                            float cant = float.Parse(cantidProdNewReq.Text);
                            totalReq = cant * (costo);
                            totImport = totalReq;
                        //}

                            col.Add(new InfoReqDet { idPto = idProduc, nomPto = pr.Nombre, ctdd = float.Parse(cantidProdNewReq.Text), prcio = costo, tot = totalReq, uMedda = unidMedNewReq.Content.ToString(), mot = txtRazonNewReq.Text, clasif = row.cate.Nombre });
                            tabNewReq.ItemsSource = col;

                            txtProySearch.IsEnabled = false;

                        /*
                        InfoReqDet rqi = new InfoReqDet();
                        rqi.idPto = idProduc;
                        rqi.nomPto = pr.Nombre;//nomPr;
                        rqi.ctdd = float.Parse(cantidProdNewReq.Text);
                        rqi.prcio = coste;
                        rqi.tot = totImport;
                        rqi.uMedda = unidMedNewReq.Content.ToString();//Convert.ToString(unidMedNewReq.Content);
                        rqi.mot = txtRazonNewReq.Text;
                        rqi.clasif = row.cate.Nombre;
                        col.Add(rqi);
                        txtProdSearch.Text = "";*/
  //                  }
 //                   else
//                    {
//                        MessageBox.Show("No es posible solicitar productos en fechas menores a 3 días");
//                        dpRequeridoNew.Focus();
//                    }
                }
                }
            }

            

            
        }

        //EVENTO DEL BOTON ENVIAR PARA AUTORIZAR
        private void saveBDReq(object sender, System.Windows.RoutedEventArgs e)
        {
            string fecEla = Convert.ToString(fechaElabNewR.Content);
            //var tr = from ot in conex.RequiEnc select ot.Folio;
            int sigFol;
            try
            {
                var qr = (from fgh in conex.RequiEnc select (fgh.Folio)).Max();
                sigFol = qr.Value + 1;
            }
            catch (InvalidOperationException)
            {
               sigFol = 1;
            }
            //MessageBox.Show("Folio "+sigFol);
            RequiEnc re = new RequiEnc { idRequisicion = 0, Folio = sigFol, idProyecto = idProject, idDepto = idDpt, idEmpleado = idEmpLog, fechaElaboracion = DateTime.Parse(fecEla), importe = totImport, Motivo = txtRazonNewReq.Text, Observaciones = txtObservNewReq.Text, FechaRequerida = DateTime.Parse(fecRequerida), ArchivoSoporte = "N/A", StatusRP = 0, StatusJA = 0, StatusJAC = 0, StatusCo = 0, StatusPresup = 0, idComprador = 0 };
            try {
                conex.RequiEnc.InsertOnSubmit(re);
                conex.SubmitChanges();
               // MessageBox.Show("Registro insertado RqEnc");
            }
            catch(Exception ex){
                MessageBox.Show("Ocurrió un error RqEnc "+ex);
            }
            int idReE;
            try
            {
                var qrt = (from some in conex.RequiEnc select (some.idRequisicion)).Max();
                idReE = qrt;
            }
            catch (InvalidOperationException)
            {
                idReE = 1;
            }

            //var fck = from t in objRqE select t;
            var fck = from t in col select t;
            foreach(var u in fck){
                int a = fck.Count();
              //  MessageBox.Show("Rows "+a);
                RequiDet rd = new RequiDet { idRequiDet = 0, idRequiEnc = idReE, idProducto = u.idPto, cantidad = u.ctdd, precio = u.prcio, total = u.tot, motivo = u.mot, StatusCotiz = '0', idProvAsig = 0 };

                try { 
                    conex.RequiDet.InsertOnSubmit(rd);
                    conex.SubmitChanges();
                 //   MessageBox.Show("Registro Insertado RqDet");
                }
                catch(Exception ex){
                    MessageBox.Show("Ocurrió un error "+ex);
                }
            }
            MessageBox.Show("La requisición se concretó correctamente");

        }
        //FILTRAR EL PRODUCTO
        private void filtrar(object sender, TextChangedEventArgs e)
        {

            if (txtProdSearch.Text.Length == 0)
            {
                listviewProd.Visibility = Visibility.Collapsed;
            }
            else
            {
                var qry = from prod in conex.Producto
                          where prod.Nombre.Contains(txtProdSearch.Text)
                          select prod;
                
                listviewProd.ItemsSource = qry;
                listviewProd.SelectedValuePath = "idProducto";
                listviewProd.DisplayMemberPath = "Nombre";
                listviewProd.Visibility = Visibility.Visible;
            }
        }
        //------------------------------------------------------------------------------------------------------FILTRAR LOS DEPTOS [2FEB]

        private void filtraProyec(object sender, TextChangedEventArgs e)
        {
            if (txtProySearch.Text.Length == 0)
            {
                listviewProy.Visibility = Visibility.Collapsed;
            }
            else {
                var qryProy = from pry in conex.Proyecto
                              where pry.Nombre.Contains(txtProySearch.Text)
                              select pry;
                listviewProy.ItemsSource = qryProy;
                listviewProy.SelectedValuePath = "idProyecto";
                listviewProy.DisplayMemberPath = "Nombre";
                listviewProy.Visibility = Visibility.Visible;
            }
        }

        //-------------------------------------------------------------------------------FILTRO DPTO BENEF
        private void filtroDpto(object sender, TextChangedEventArgs e)
        {
            if (txtDptoSearch.Text.Length == 0)
            {
                listviewDpto.Visibility = Visibility.Collapsed;
            }
            else {
                var qryDp = from dto in conex.Departamento
                            where dto.NombreDepto.Contains(txtDptoSearch.Text)
                            select dto;
                listviewDpto.ItemsSource = qryDp;
                listviewDpto.SelectedValuePath = "idDepto";
                listviewDpto.DisplayMemberPath = "NombreDepto";
                listviewDpto.Visibility = Visibility.Visible;
            }
        }



        //------------------------------------------------------------------------------------------------SELECT CHAN LV PRODUCTO

        private void listVselecChaFiltro(object sender, SelectionChangedEventArgs e)
        {
            if (listviewProd.SelectedValue != null)
            {

                MessageBox.Show("Seleccionado " + listviewProd.SelectedValue);
                Producto prc = listviewProd.SelectedItem as Producto;
                int aidipr = int.Parse(listviewProd.SelectedValue.ToString());
                var cury = (from prd in conex.Producto
                            where prd.idProducto == aidipr
                            select prd).SingleOrDefault();
                unidMedNewReq.Content = cury.UnidadMed;
                txtProdSearch.Text = prc.Nombre;
                listviewProd.Visibility = Visibility.Collapsed;
            }
            else { return; }
        }

        //-------------------------------------------------------------------------------------------------SELECT CHAN LV PROYECTO

        private void listVselectChaProyec(object sender, SelectionChangedEventArgs e)
        {
            if (listviewProy.SelectedValue != null) {
                MessageBox.Show("ID Proyecto: "+listviewProy.SelectedValue);
                Proyecto pryc = listviewProy.SelectedItem as Proyecto;
                int aidiPry = int.Parse(listviewProy.SelectedValue.ToString());
                var cryPry = (from pyt in conex.Proyecto
                              where pyt.idProyecto == aidiPry
                              select pyt).SingleOrDefault();
                txtProySearch.Text = cryPry.Nombre;
                listviewProy.Visibility = Visibility.Collapsed;
            }

        }

        private void lvSelChanDpto(object sender, SelectionChangedEventArgs e)
        {
            if (listviewDpto.SelectedValue != null)
            {
                MessageBox.Show("ID Depto: " + listviewDpto.SelectedValue);
                Departamento dpt = listviewDpto.SelectedItem as Departamento;
                int aidiDp = int.Parse(listviewDpto.SelectedValue.ToString());
                var cryD = (from dp in conex.Departamento
                              where dp.idDepto == aidiDp
                              select dp).SingleOrDefault();
                txtDptoSearch.Text = cryD.NombreDepto;
                listviewDpto.Visibility = Visibility.Collapsed;
            }
        }

        private void btnLimpiar(object sender, RoutedEventArgs e)
        {
            cleanInterfNewReq();
        }



        
    }

    //CLASE REQUISICION DONDE TENGO TODOS LOS CAMPOS DE LAS TABS REQUIENC Y REQUIDET
    public class RequisicionD
    {

        //public int idReqDet { get; set; }
        //public int idReqEnc { get; set; }
        public int idProd { get; set; }
        public float cantidad { get; set; }
        public float precio { get; set; }
        public float total { get; set; }
        //public class RequiEnc {
        //public int idReqE { get; set; }
        //public int folio { get; set; }
        public int idProyec { get; set; }
        public int idEmpl { get; set; }
        public string fechaElab { get; set; }
        public float importe { get; set; }
        public string motivo { get; set; }
        public string observ { get; set; }
        public string fechReq { get; set; }
        public string archSop { get; set; }

    }

    public class InfoReqDet {

        public int idPto { get; set; }
        public string nomPto { get; set; }
        public float ctdd { get; set; }
        public string uMedda { get; set; }
        public float prcio { get; set; }
        public float tot { get; set; }
        public string mot { get; set; }
        public string clasif { get; set; }//Add 2FEB
    }

    //CLASE REQUISICION CON NOMBRES Y NO IDS PARA PONER EN DATAGRID
    public class Requisicion
    {

        //public int idReqDet { get; set; }
        //public int idReqEnc { get; set; }
        public string nomProd { get; set; }
        public float cantidad { get; set; }
        public float precio { get; set; }
        public float total { get; set; }
        //public class RequiEnc {
        //public int idReqE { get; set; }
        //public int folio { get; set; }
        public string nomProyec { get; set; }
        public string nomEmpl { get; set; }
        public string fechaElab { get; set; }
        public float importe { get; set; }
        public string motivo { get; set; }
        public string observ { get; set; }
        public string fechReq { get; set; }
        public string archSop { get; set; }
        //public int idComprador { get; set; }

    }

    public class ComboProyecto
    {
        public int idProyComb { get; set; }
        public string nombProyComb { get; set; }
    }

    public class ComboDepto
    {
        public int idDptoComb { get; set; }
        public string nombDptoComb { get; set; }
    }

    public class RqEncab {

        public int idRq { get; set; }
        public int fol { get; set; }
        public int idPry { get; set; }
        public int idEm { get; set; }
        public string elebo { get; set; }
        public float impor { get; set; }
        public string mot { get; set; }
        public string obs { get; set; }
        public string requeri { get; set; }
        public string aSop { get; set; }
        public int stRp { get; set; }
        public int stJa { get; set; }
        public int stJac { get; set; }
        public int stCo { get; set; }
        public int stPrsp { get; set; }
        public int idCompr { get; set; }
    }

    public class RqDetalle {

        public int idRqD { get; set; }
        public int idRqE { get; set; }
        public int idPrd { get; set; }
        public float can { get; set; }
        public float prci { get; set; }
        public float tot { get; set; }
        public string mot { get; set; }
    }

}
