using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
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
using SacIntegrado.Adquisiciones;

namespace SacIntegrado.Adquisiciones
{
    /// <summary>
    /// Lógica de interacción para cotizacion.xaml
    /// </summary>
    public partial class cotizacion : Page
    {
        int idProdSeleccCotiz = 0;
        public static int idProveToCotiza = 0;//PARA GUARDAR EL ID DEL PROV Q SE SELECCIONA PARA HACER LA COTIZACION
        string nomProvSelec = "";//PARA GUARDAR EL NOMBRE DEL PROV QUE SELECCIONA PARA LA COTIZ
        int idReqDetCotiz = 0;//PARA GUARDAR EL IDREQDET DEL PROD Q SE SELECC A COTIZAR
        int k = 1;
        int xo = 0;
        int idReqEncCotiz = 0;
        int idCotizaDetCuad = 0;//VAR PARA GUARDAR EL IDCOTIZACIONDET CUANDO SE CARGAN EN EL DG DEL POPUP
        int indiceActual = 0;
        int ideCotizToCuadro = 0;
        char ideTipoToCuadro;
        int idProdSelecToCot=0;

        ProdRequeridos rowToCotizar;//VAR PARA USAR AL MOMENTO DEL CLIC EN LA TABLA DE LOS PRODUCTOS DE LA REQUI
        Db conex = new Db();
        //COLLECCIONES
        ObservableCollection<RequiPorRevisar> oc = new ObservableCollection<RequiPorRevisar>();
        ObservableCollection<RequiPorRevisar> ocDos = new ObservableCollection<RequiPorRevisar>();
        ObservableCollection<DatosRequi> listDatosRequi = new ObservableCollection<DatosRequi>();
        ObservableCollection<ProdRequeridos> listaProductosRequi = new ObservableCollection<ProdRequeridos>();
        ObservableCollection<ProdRequeridos> collCotizProdReq = new ObservableCollection<ProdRequeridos>();//COLL PARA LS PROD D LA REQUI A COTIZAR
        ObservableCollection<DataGridPopup> collDgPopp = new ObservableCollection<DataGridPopup>();

        int Id_empleado = 0;
        int idCmpdr = 0;

        public cotizacion()
        {
            InitializeComponent();
        }

        public cotizacion(string usuario, int id, string nomb)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, usuario, id, nomb);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            this.usuario.Content = nomb;
            Id_empleado = id;
            idCmpdr = id;
            oc.Clear();
            llenaRequiXCotizar();
            llenaRequiCotizada();
            dpFechaCotizPop.Text = DateTime.Today.ToShortDateString();
        }

        //----------------------------------------------------------------------------------------//EVENTO DE LA ETIQUETA PARA CERRAR SESIÓN

        private void cerrarSesion(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                InicioLogin inic = new InicioLogin();
                this.NavigationService.Navigate(inic);
            }
            else { }
        }
        //----------------------------------------------------------------------------------------HABILITAR OBSERV DE LA COTIZ---------------------------------------
        private void habilitarObserv(object sender, System.Windows.RoutedEventArgs e)
        {
            if (chckCotizacion.IsChecked == true) { txtObserv.IsEnabled = true; btnRecObserv.IsEnabled = true; }
            else { txtObserv.IsEnabled = false; btnRecObserv.IsEnabled = false; }
        }
        //----------------------------------------------------------------------------------------HABILITAR OBSERV DE LA COTIZ---------------------------------------
        private void habObservac(object sender, System.Windows.RoutedEventArgs e)
        {
            if (chckCotizacion.IsChecked == true) { txtObserv.IsEnabled = true; btnRecObserv.IsEnabled = true; }
            else { txtObserv.IsEnabled = false; btnRecObserv.IsEnabled = false; }
        }

        //----------------------------------------------------------------------------------------SELECT CHANGED DATAGRID POR COTIZAR---------------------------------------
        int folioSelected = 0;
        private void reqPendiete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RequiPorRevisar rprev = reqXrevisar.SelectedItem as RequiPorRevisar;
            if (rprev == null)
            {
                oc.Clear();
                listaProductosRequi.Clear();
                llenaRequiXCotizar();
            }
            else
            {
                var renglon = (from p in oc
                               where p.folio == rprev.folio
                               select p);
                foreach (var x in renglon)
                {
                    folioSelected = x.folio;
                }
                muestraTotal();
                muestraDatosRequi();
                llenaProductosdeRequi();
                btnAcepCoti.IsEnabled = true;
            }
        }

        //------------------------------------------------------------------------------------------PARA MOSTRAR TOTAL DE LA REQUI/ORDEN/GASTO--------------------------------

        public void muestraTotal()
        { 
            var to = from t in listaProductosRequi select new{ t.cantidad, t.precio};
            double totReq = 0;
            double totPro = 0;
            double impor = 0;

            foreach(var qt in to){
                totPro = qt.cantidad * qt.precio;
                totReq = totPro;
                impor = totReq + impor;
            }

            lblTot.Content = "" + impor;            
        }


        //-------------------------------------------------------------------------------------------------------MOSTRAR DATOS EN CAMPOS DE LA REQUI POR COTIZAR-------------------
        public void muestraDatosRequi()
        {
            MessageBox.Show("Folio Seleccionado "+folioSelected);
            var requi = from re in conex.RequiEnc
                        from p in conex.Proyecto
                        from e in conex.Empleado
                        from d in conex.Departamento

                        where
                            p.idProyecto == re.idProyecto && re.idEmpleado == e.idEmpleado && re.idProyecto == p.idProyecto && d.idDepto == p.idDepto &&
                            re.Folio == folioSelected

                        select new
                        {
                            re.Folio,
                            re.Motivo,
                            re.Observaciones,
                            p.Nombre,
                            re.fechaElaboracion,
                            re.FechaRequerida,
                            nombre = e.Nombre,
                            d.NombreDepto,
                            re.idRequisicion//MODIFICADO 15 ENERO
                        };

            foreach (var e in requi)
            {
                //listaProductosRequi.Clear();
                string fecha_elaboracion = Convert.ToString(e.fechaElaboracion).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                listDatosRequi.Add(new DatosRequi { nombreProy = e.Nombre, folio = e.Folio.Value, fechaElab = fecha_elaboracion, fechaReq = fecha_requerida, nombreSolicitante = e.nombre, nombreDepto = e.NombreDepto, mot = e.Motivo, obs = e.Observaciones, idRqEncabezado = e.idRequisicion });
                idReqEncCotiz = e.idRequisicion;
                proyectPresup.Text = e.Nombre;
                folio.Content = "" + e.Folio;
                fechaElab.Content = "" + fecha_elaboracion;
                fechaReq.Content/*Text*/ = "" + fecha_requerida;
                solicitante.Text = "" + e.nombre;
                dptoBenef.Text = e.NombreDepto;
                txtMotiv.Text = e.Motivo;
                txtObserv.Text = e.Observaciones;
            }
        }

        //----------------------------------------------------------------------------------------------LLENAR EL DATAG DE LOS PRODS DE LA REQUI SELECCIONADA-----------------------
        private void llenaProductosdeRequi()
        {
            var prod = from p in conex.Producto
                       from re in conex.RequiEnc
                       from rd in conex.RequiDet
                       where re.Folio == folioSelected &&
                       re.idRequisicion == rd.idRequiEnc &&
                       p.idProducto == rd.idProducto

                       select new
                       {   p.idProducto, p.Nombre, rd.cantidad, rd.precio, rd.total, re.Observaciones, re.Motivo, p.UnidadMed, rd.idRequiDet, rd.StatusCotiz, rd.idProvAsig };

            int xv = 0;

            foreach (var reg in prod)
            {
                char s = reg.StatusCotiz.Value;
                string stCo = "";
                if (s == '0')
                {
                    stCo = "No Cotizado";
                }
                else
                {
                    stCo = "Cotizado";
                }
                //MessageBox.Show("Este era el pedo " + reg.idProvAsig);
                listaProductosRequi.Add(new ProdRequeridos { idPro = reg.idProducto, nombreProdu = reg.Nombre, cantidad = reg.cantidad.Value, precio = reg.precio.Value, total = reg.total.Value, umedi = reg.UnidadMed, coti = "Cotizar", idReqDetProdReq = reg.idRequiDet, oCgM = 1, stat = stCo, idProvAsigCot = reg.idProvAsig.Value, idCotSelecToCuad = idCotizaDetCuad, tipoCotToCuad = '/', indice = xv });
                MessageBox.Show("Atributo Indice en foreach al llear tabla -> "+xv);
                xv++;
                txtMotiv.Text = reg.Motivo;
                txtObserv.Text = reg.Observaciones;
            }
            dGproductRequi.ItemsSource = listaProductosRequi;
        }


        //------------------------------------------------------------------------------------------------CARGAR DATAG DE LAS REQUIS QUE SE VAN A COTIZAR----------------
        private void llenaRequiXCotizar()
        {

            oc.Clear();
            var consultar = from re in conex.RequiEnc
                            from p in conex.Proyecto
                            from e in conex.Empleado
                            from d in conex.Departamento
                            where
                              re.StatusRP == 1 &&
                              re.StatusCot == '0' || re.StatusCot == null &&
                              re.idProyecto == p.idProyecto &&
                              e.idEmpleado == re.idEmpleado &&
                              d.idDepto == p.idDepto &&
                              //p.idResponsable == Id_empleado &&
                              re.idComprador == Id_empleado &&
                              re.StatusJA == 1
                            //e.idEmpleado == Id_empleado
                            select new
                            {
                                re.Folio,
                                re.fechaElaboracion,
                                re.FechaRequerida,
                                d.NombreDepto,
                                e.Nombre,
                                e.apPater,
                                e.apMater,
                                re.Motivo,
                                re.Observaciones
                            };

            foreach (var e in consultar)
            {
                string fecha_elalboracion = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                oc.Add(new RequiPorRevisar { folio = e.Folio.Value, fechaElab = fecha_elalboracion, fechaReq = fecha_requerida, nombreDepto = e.NombreDepto, nombreEmpleado = e.Nombre + " " + e.apPater + " " + e.apMater });
                reqXrevisar.ItemsSource = oc;
            }
        }

        //------------------------------------------------------------------------------------------------LOAD REQUIS THAT HAVE BEEN COTIZADAS [AUN NO LO HACE]-----------------------
        private void llenaRequiCotizada()
        {

            var consultar = from re in conex.RequiEnc
                            //from rd in conex.RequiDet
                            from p in conex.Proyecto
                            from e in conex.Empleado
                            from d in conex.Departamento
                            where
                            re.StatusRP == 1 &&
                            re.StatusJA == 1 &&
                            re.StatusJAC == 1 &&
                            re.StatusCot == '1' &&
                            //re.idRequisicion == rd.idRequiEnc &&
                            //rd.StatusCotiz == 1 &&
                              re.idProyecto == p.idProyecto &&
                              e.idEmpleado == re.idEmpleado &&
                              d.idDepto == p.idDepto &&
                              //e.idDepto == d.idDepto &&
                               /*p.idResponsable*/re.idComprador == Id_empleado
                               
                            //e.idEmpleado == Id_empleado

                            select new
                            {
                                re.Folio,
                                re.fechaElaboracion,
                                re.FechaRequerida,
                                d.NombreDepto,
                                e.Nombre,
                                e.apPater,
                                e.apMater
                            };
            foreach (var e in consultar)
            {
                string fecha_elaboracion = Convert.ToString(e.fechaElaboracion).Substring(0, 11);
                string fecha_requerida = Convert.ToString(e.FechaRequerida).Substring(0, 11);
                //DateTime fecha_elaboracioon = e.fechaElaboracion;
                ocDos.Add(new RequiPorRevisar { folio = e.Folio.Value, fechaElab = fecha_elaboracion, fechaReq = fecha_requerida, nombreDepto = e.NombreDepto, nombreEmpleado = e.Nombre + " " + e.apPater + " " + e.apMater });
                reqRevis.ItemsSource = ocDos;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------
        private void cotiz(object sender, RoutedEventArgs e)
        {
            //I need to know which row this button is on so I can retrieve the "id"
            MessageBox.Show("Boton");
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        //private void lblCotizar(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    MessageBoxResult r = MessageBox.Show("¿Desea cotizar este producto?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
        //    if (r == MessageBoxResult.Yes)
        //    {
        //        //MessageBox.Show("OK");
        //        cotizac.IsOpen = true;
        //        //EMPIEZO PARA VER QUE MOSTRAR EN POPUP

        //        int idProdSelecc = 0;
        //        ProdRequeridos prodCot = dGproductRequi.SelectedItem as ProdRequeridos;
        //        //Lunes
        //        /*DataRowView isource = dGproductRequi.CurrentItem as DataRowView;
        //        int ind = dGproductRequi.CurrentColumn.DisplayIndex;
        //        string valor = isource.Row.ItemArray[ind - 1].ToString();
        //        MessageBox.Show("Row: "+valor);*/
        //        if (prodCot == null)
        //        {
        //            oc.Clear();
        //            collCotizProdReq.Clear();
        //            llenaRequiXCotizar();
        //            MessageBox.Show("Entro al null de LBL");
        //        }
        //        else
        //        {

        //            var roww = (from p in /*collCotizProdReq*/listaProductosRequi
        //                        where p.idPro == prodCot.idPro
        //                        select p);
        //            foreach (var x in roww)
        //            {
        //                idProdSelecc = x.idPro;
        //            }
        //            MessageBox.Show("ID R " + idProdSelecc);
        //            muestraTotal();
        //            muestraDatosRequi();
        //            llenaProductosdeRequi();
        //        }
        //    }
        //    else { }
        //}

        //-------------------------------------------------------------------------------------------------------------------DON'T SHOW POPUP-------------------
        private void cerrarPop(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBoxResult r = MessageBox.Show("No ha cotizado el producto\n ¿Está segur@ que desea salir?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (r == MessageBoxResult.Yes)
            {
                cotizac.IsOpen = false;
                cleanPopp();
            }
            else
            {
                cotizac.IsOpen = true;
            }
        }

        //--------------------------------------------------------------------------------------------------------SELECT CHANGE PARA TOMAR EL RENGLON DEL PROD A COTIZAR
        private void 
            cotizarProdDg(object sender, SelectionChangedEventArgs e)
        {
            
            ProdRequeridos prodCo = dGproductRequi.SelectedItem as ProdRequeridos;
            
            if (prodCo == null)
            {
                return;
            }
            MessageBox.Show("IndiceActual " + prodCo.indice + " NomProdSelec "+prodCo.nombreProdu);
            //Console.Write("Indice Actual "+prodCo.indice);
            //Console.Read();
            rowToCotizar = prodCo;
            MessageBox.Show("IndiceActual con rowToCotizar " + rowToCotizar.indice);
            //MessageBox.Show("IndAct rowToCo " + rowToCotizar.indice + " IndAct " + prodCo.indice);
            indiceActual = prodCo.indice;


            //if (dGproductRequi.CurrentColumn.Header != null)
              //{
            if (dGproductRequi.CurrentColumn.Header.Equals("OC/GM") ||/* dGproductRequi.CurrentColumn.Header.Equals("Status") ||*/ dGproductRequi.CurrentColumn.Header.Equals("IDP"))
            {
                return;
            }
            else {
                if (prodCo.stat.Equals("Cotizado"))
                {

                    MessageBoxResult r = MessageBox.Show("El producto ya esta cotizado\n ¿Desea volver a cotizarlo?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    //indiceActual = prodCo.indice;
                    //MessageBox.Show("Indice Actual en SelectChanged para el prod a cotizar "+indiceActual);
                    if (r == MessageBoxResult.Yes)
                    {

                        openPopToCotiz();
                        //openPopToCotiz(rowToCotizar);
                        /*
                        cotizac.IsOpen = true;
                        blockInterfaz();

                        var roww = (from p in listaProductosRequi
                                    where p.idPro == prodCo.idPro
                                    select p);
                        foreach (var x in roww)
                        {
                            idProdSeleccCotiz = x.idPro;
                            idReqDetCotiz = x.idReqDetProdReq;
                        }
                        MessageBox.Show("ID R " + idProdSeleccCotiz + " ID Rdet -> " + idReqDetCotiz);
                        muestraTotal();
                        muestraDatosRequi();
                        llenaProductosdeRequi();
                        //LLENA EL POPUP
                        fillPopProd();*/
                    }
                    else { return; }

                }
                else
                {

                    if (dGproductRequi.CurrentColumn.Header.Equals("OC/GM") /*|| dGproductRequi.CurrentColumn.Header.Equals("Status") */|| dGproductRequi.CurrentColumn.Header.Equals("IDP"))
                    {
                        return;
                    }
                    openPopToCotiz();
                    //openPopToCotiz(rowToCotizar);
                }
            }


            //}else{ return; }

        }

        //-----------------------------------------------------------------------------------------------------------OPEN POPUP TO COTIZAR SELECTED PROD------------
        public void openPopToCotiz() {

            cotizac.IsOpen = true;
            blockInterfaz();
            
            var roww = (from p in listaProductosRequi
                        where p.idPro == /*prodCo*/rowToCotizar.idPro//ROWTOCOTIZAR ESTA EN EL CHANGED DEL DATAG DE LOS PRODS
                        select p);
            foreach (var x in roww)
            {//QUITAR ESTE FOREACH
                idProdSeleccCotiz = x.idPro;
                idReqDetCotiz = x.idReqDetProdReq;
            }
            MessageBox.Show("Metodo OpenPopCotiz\n ID Prod a Cotizar " + idProdSeleccCotiz + " ID Requi Det -> " + idReqDetCotiz);
            muestraTotal();
            muestraDatosRequi();
            //llenaProductosdeRequi();
            //LLENA EL POPUP
            fillPopProd();
        }

        //-----------------------------------------------------------------------------------------------------FILL PROD TO COTIZ ON POPUP-------
        private void fillPopProd()
        {

            var fillPop = from pop in listaProductosRequi
                          from cl in conex.CategoriaProd
                          from pr in conex.Producto
                          where pop.idPro == idProdSeleccCotiz &&
                          pr.idProducto == idProdSeleccCotiz &&
                          pr.idClasificador == cl.idCategoria

                          select new { pop, cl, pr };

            foreach (var reg in fillPop)
            {
                txtProdCotiz.Text = reg.pop.nombreProdu;
                lblCant.Content = reg.pop.cantidad.ToString();
                txtClasif.Text = reg.cl.Nombre;
                lblUnidMed.Content = reg.pr.UnidadMed;
                txtPrec.Text = "00.000";
                ObservCotizPop.Text = txtObserv.Text;
                //LLENO EL COMBO DE PROVEEDORES
                fillComboProvePop();
                //LLENA TABLA SI EL PROD TIENE CONTRATO
                fillDatagridPopContrato();//--------------------------ESTE ME DA PEDOS CON EL TRAPEADOR - YA NO :D
                //LLENA TABLA SI EL PROD HAS BEEN COTIZED
                fillDatagridPopProdCotizado();
            }
        }


        //------------------------------------------------------------------------------------------METODO PARA LLENAR EL COMBO DEL POPUP CON TODOS LOS PROVEEDORES
        private void fillComboProvePop()
        {

            ObservableCollection<Provee> colProve = new ObservableCollection<Provee>();

            colProve.Clear();
            var fillComProv = from com in conex.Proveedor
                              select com;

            foreach (var v in fillComProv)
            {
                colProve.Add(new Provee { idProve = v.idProveedor, nomProve = v.Nombre });
                cmbbProvPop.ItemsSource = colProve;
                cmbbProvPop.DisplayMemberPath = "nomProve";
                cmbbProvPop.SelectedValuePath = "idProve";
            }

        }

        //-----------------------------------------------------------------------------------------LOAD COTIZACIONES CON CONTRATO DEL PRODUCTO SELECCIONADO 
        private void fillDatagridPopContrato()
        {

            //MessageBox.Show("ID Metodo Contrato " + idProdSeleccCotiz);
            collDgPopp.Clear();
            var fillDgCot = //from cdt in conex.CotizacionDet
                //from cenc in conex.CotizacionEnc
                            from cpv in conex.ContratoProv
                            from cdtll in conex.ContratoDetalle
                            //from rdd in conex.RequiDet
                            //from ree in conex.RequiEnc
                            from prvee in conex.Proveedor
                            from pdct in conex.Producto
                            //from empl in conex.Empleado

                            where cdtll.idProducto == idProdSeleccCotiz &&//QUE EL ID DEL PROD SELEC ESTE EN CONTRATO DET
                                prvee.idContrato == cpv.idContrato &&
                                cpv.idContrato == cdtll.idContrato &&
                                pdct.idProducto == idProdSeleccCotiz

                            select new { cdtll.Precio, prvee.Nombre, nomProd = pdct.Nombre, cpv.fechaRealizacion, cpv.observaciones, prvee.idProveedor, cpv.idContrato };
            int y = fillDgCot.Count();
            //MessageBox.Show("metodo fillDgContrato RS " + y);
            if (y == 0)
            {//NOT FOUND ANY
                return;
                k = 1;
            }
            foreach (var wq in fillDgCot)
            {
                idProveToCotiza = wq.idProveedor;
                //collDgPopp.Add(new DataGridPopup { dgproveed = wq.prvee.Nombre, dgproduc = wq.pdct.Nombre, dgprecio = (float)wq.cdt.precio.Value, dgfechReg = wq.cpv.fechaRealizacion.ToString(), dgfechCoti = wq.cenc.fecha.ToString() ,dgPersonaReg = wq.empl.Nombre, dgObservCot = wq.cenc.Observaciones});
                collDgPopp.Add(new DataGridPopup { dgproveed = wq.Nombre, dgproduc = wq.nomProd, dgprecio = (float)wq.Precio.Value, dgfechReg = wq.fechaRealizacion.ToString(), dgfechCoti = "00-00-0000", dgPersonaReg = "", dgObservCot = wq.observaciones, indexIdProv = idProveToCotiza, isCh = false, idCotiDet = wq.idContrato, tipoCot = 'C' });
                dgPopCotiz.ItemsSource = collDgPopp;
                k++;
            }
            //MessageBox.Show("Valor k en Metodo contrato " + k);
        }

        //----------------------------------------------------------------------------------------------LOAD COTIZACIONES HECHAS ANTES DEL PRODUCTO SELECCIONADO
        private void fillDatagridPopProdCotizado()
        {
            //collDgPoppDos.Clear();
            var fillDgCotDos = from cdt in conex.CotizacionDet
                               from cenc in conex.CotizacionEnc
                               //from cpv in conex.ContratoProv
                               //from cdtll in conex.ContratoDetalle
                               from rdd in conex.RequiDet
                               from ree in conex.RequiEnc
                               from prvee in conex.Proveedor
                               from pdct in conex.Producto
                               from empl in conex.Empleado

                               where //cdtll.idProducto == idProdSeleccCotiz &&
                                   //prvee.idContrato == cdtll.idContrato &&
                                   pdct.idProducto == idProdSeleccCotiz &&
                                   cdt.idProd == idProdSeleccCotiz &&
                                   rdd.idProducto == idProdSeleccCotiz &&
                                   rdd.idRequiEnc == ree.idRequisicion &&//PROD CARTES DE REQUIENC A REQUIDET
                                   cenc.idEmpleado == empl.idEmpleado &&
                                   cdt.idCotizacionEnc == cenc.idCotizacionEnc &&//PROD CARTESIANO DE COTIZDET A COTIZENC
                                   cenc.idProveedor == prvee.idProveedor &&
                                   cdt.idRequiDet == rdd.idRequiDet

                               /*
                               where cdt.idProd == idProdSeleccCotiz &&
                               rdd.idProducto == idProdSeleccCotiz &&
                               rdd.idRequiEnc == ree.idRequisicion &&
                               cpv.idProveedor == cenc.idProveedor &&
                               pdct.idProducto == cdt.idProd &&
                               cenc.idEmpleado == empl.idEmpleado &&
                               cdt.idCotizacionEnc == cenc.idCotizacionEnc*/
                               //orderby cenc.fecha

                               //select new { cdt, cenc, cpv, cdtll, rdd, ree, prvee, pdct, empl  };//SELECCIONANDO TODO, CUANDO ME DABA ERROR
                               select new { cdt.precio, prvee.Nombre, nomProd = pdct.Nombre, cenc.fecha, nomEmp = empl.Nombre, cenc.Observaciones, prvee.idProveedor, cdt.idCotizacionDet };//FUNCIONO PERO ANTES DE HACER 2 METODOS
            //select new { cdtll.Precio, prvee.Nombre, nomProd = pdct.Nombre, cpv.fechaRealizacion, cpv.observaciones };
            int v = fillDgCotDos.Count();
            //MessageBox.Show("Metodo fillDgCotizado RS " + v);
            if (v == 0) { return; }
            k = 1;
            foreach (var wq in fillDgCotDos)
            {
                //MessageBox.Show("Valor k en Metodo cotizado inicio for " + k);
                idProveToCotiza = wq.idProveedor;
                idCotizaDetCuad = wq.idCotizacionDet;
                //collDgPopp.Add(new DataGridPopup { dgproveed = wq.prvee.Nombre, dgproduc = wq.pdct.Nombre, dgprecio = (float)wq.cdt.precio.Value, dgfechReg = wq.cpv.fechaRealizacion.ToString(), dgfechCoti = wq.cenc.fecha.ToString() ,dgPersonaReg = wq.empl.Nombre, dgObservCot = wq.cenc.Observaciones});
                collDgPopp/*collDgPoppDos*/.Add(new DataGridPopup { dgproveed = wq.Nombre, dgproduc = wq.nomProd, dgprecio = (float)wq.precio.Value, dgfechReg = "00-00-0000", dgfechCoti = wq.fecha.ToString(), dgPersonaReg = wq.nomEmp, dgObservCot = wq.Observaciones, indexIdProv = idProveToCotiza, isCh = false, idCotiDet = wq.idCotizacionDet, tipoCot = 'S' });
                dgPopCotiz.ItemsSource = collDgPopp/*collDgPoppDos*/;
                k++;
            }
            //MessageBox.Show("Valor k en Metodo cotizado " + k);
        }
        //---------------------------------------------------------------------------------------------------------------CLEAN POPUP AND PUT TODAY DATE
        private void cleanPopp()
        {
            txtPrec.IsEnabled = true;
            txtProdCotiz.Text = "";
            lblCant.Content = "";
            txtClasif.Text = "";
            lblUnidMed.Content = "";
            txtPrec.Text = "";
            ObservCotizPop.Text = "";
            dpFechaCotizPop.Text = DateTime.Today.ToShortDateString();
            collDgPopp.Clear();
            //HABILITAR ELEMENTS AGAIN
            habilInterfaz();

        }
        //-----------------------------------------------------------------------------------------------------------------------ENABLE ELEMENTS WHEN POPUP IS CLOSED
        public void habilInterfaz() {

            chckCotizacion.IsEnabled = true;
            dGproductRequi.IsEnabled = true;
            reqXrevisar.IsEnabled = true;
            txtPrec.IsEnabled = true;
            //dptoBenef.IsEnabled = true;
            //solicitante.IsEnabled = true;
            //proyectPresup.IsEnabled = true;
            lblLogout.Visibility = Visibility.Visible;
        }
        //-----------------------------------------------------------------------------------------------------------------------DISABLE ELEMENTS WHEN POPUP IS OPEN
        private void blockInterfaz(){

            chckCotizacion.IsEnabled = false;;
            dGproductRequi.IsEnabled = false;
            reqXrevisar.IsEnabled = false;
            dptoBenef.IsEnabled = false;
            solicitante.IsEnabled = false;
            proyectPresup.IsEnabled = false;
            lblLogout.Visibility = Visibility.Collapsed;
        }

        //-------------------------------------------------------------------------------------------------BUTTON ASIGNAR PROVEEDOR AL PRODUCTO Y GUARDAR COTIZACION Y CUADRO
        private void btnAsignarProvCotiz(object sender, RoutedEventArgs e)
        {
            menuPrincipal.IsEnabled = false;
            MessageBox.Show("IndiceActual "+indiceActual);
            if (dpFechaCotizPop.Text.Equals("Seleccione una fecha")||dpFechaCotizPop.Text.Length>10||dpFechaCotizPop.Text.Length<10)
            {
                MessageBox.Show("Establesca un formato correcto de fecha");
                return;
            }
            else
            {
                if (collDgPopp.Count() < 3)
                {
                    MessageBox.Show("Debe guardar al menos 3 cotizaciones por producto, para el cuadro comparativo");
                    return;
                }
                else
                {
                    var chekCh = from g in collDgPopp
                                where g.isCh == true
                                select g;
                    if (chekCh.Count() < 3)
                    {
                        MessageBox.Show("Debe seleccionar al menos 3 cotizaciones\n para guardar el Cuadro comparativo");
                        return;
                    }

                    //CODIGO PARA GUARDAR EN BD EL CUADRO COMPARATIVO
                    CuadroEnc cen = new CuadroEnc { idCuadroEnc = 0, fecha = DateTime.Parse(dpFechaCotizPop.Text), Observaciones = ObservCotizPop.Text };

                    try
                    {
                        conex.CuadroEnc.InsertOnSubmit(cen);
                        conex.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar cuadro comparativo [Enc]\n" + ex);
                    }

                    int idCuaEnc;
                    try
                    {
                        var qr = (from som in conex.CuadroEnc select (som.idCuadroEnc)).Max();
                        idCuaEnc = qr;
                    }
                    catch (InvalidOperationException)
                    {
                        idCuaEnc = 1;
                    }
                    MessageBox.Show("Sigue Consulta de Checks");
                    var cheka = from b in collDgPopp
                                where b.isCh == true
                                select b;
                    if (cheka.Count()<3)
                    {
                        MessageBox.Show("Debe seleccionar al menos 3 cotizaciones\n para guardar el Cuadro comparativo");
                        return;
                    }
                    else
                    {

                        MessageBox.Show("Else de la validacion de Cheks");
                        foreach (var b in cheka)
                        {
                            /*string*/
                            int p = b.idCotiDet;//b.dgproveed;
                            MessageBox.Show("IDs Prov Checados " + p + "ID ProvChck2 "+b.indexIdProv);
                            CuadroDet cdt = new CuadroDet { idCuadroDet = 0, idCuadroEnc = idCuaEnc, idReqDet = idReqDetCotiz, idProd = idProdSeleccCotiz, idCotizacionDet = p/*ideCotizToCuadro*/ };
                            MessageBox.Show("Esto va a insertar en CuadroDet idCuadEnc " + idCuaEnc + " - idReqD " + idReqDetCotiz + " - idProd " + idProdSeleccCotiz + " - idCotizaDet " + p);

                            try
                            {
                                conex.CuadroDet.InsertOnSubmit(cdt);
                                conex.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error al guardar cuadro comparativo [Det]\n" + ex);
                            }
                        }


                        //--TERMINA COD DE CUAD COMPARATIVO

                        idProdSelecToCot = idProdSeleccCotiz;

                        //MessageBox.Show("ID Prov Marcado " + xo);

                        if (this.dgPopCotiz.SelectedItem is DataGridPopup)
                        {
                            DataGridPopup dpp = dgPopCotiz.SelectedItem as DataGridPopup;
                            //MessageBox.Show("Checbox está: " + dpp.isCh);
                            int chbChec = dpp.indexIdProv;
                            MessageBox.Show("ID Prov Dspues d insertar en Cuadros " + chbChec);
                        }
                        //funciona con uno
                        //*/

                        var cheq = from b in collDgPopp
                                   where b.isCh == true
                                   select b;

                        foreach (var b in cheq)
                        {
                            string p = b.dgproveed;
                            MessageBox.Show("IDs Prov Checados " + p);
                        }

                        MessageBox.Show("Entro IndiceActual Btn Asignar prov " + indiceActual);
                        listaProductosRequi.ElementAt(indiceActual).tipoCotToCuad = ideTipoToCuadro;
                        listaProductosRequi.ElementAt(indiceActual).idCotSelecToCuad = ideCotizToCuadro;
                        listaProductosRequi.ElementAt(indiceActual).idProvAsigCot = xo;
                        listaProductosRequi.ElementAt(indiceActual).stat = "Cotizado";
                        listaProductosRequi.ElementAt(indiceActual).precio = double.Parse(txtPrec.Text.Trim());
                        listaProductosRequi.ElementAt(indiceActual).total = listaProductosRequi.ElementAt(indiceActual).cantidad * double.Parse(txtPrec.Text.Trim());
                        dGproductRequi.Items.Refresh();


                        //dGproductRequi.Columns[7].IsReadOnly = true;
                        habilInterfaz();
                        cotizac.IsOpen = false;
                        muestraTotal();
                    }
                }
            }

        }
//-------------------------------------------------------------------------------------------------------------------------GRABA LA ORDEN DE GASTO/COMPRA
        public void grabarOrdenGasto()
        {

            foreach (var q in listaProductosRequi) {

                var reg = (from f in conex.RequiDet
                           where f.idRequiDet == q.idReqDetProdReq &&
                           f.idProducto == q.idPro
                           select f).SingleOrDefault();

                reg.idProvAsig = q.idProvAsigCot;//xo;///---------------------------------------------ESTE ERA EL PEDO
                reg.StatusCotiz = '1';
                reg.precio = q.precio;
                reg.total = q.precio * q.cantidad;
                conex.RequiDet.Context.SubmitChanges();
                conex.SubmitChanges();
                
            }



            var t = from y in listaProductosRequi
                    orderby y.idProvAsigCot, y.oCgM
                    select y;

            int idePrvAnterior;
            int ideOcGmAnterior;

            int idePrvActual;
            int ideOcGmActual;
            double total;
            char typ = 'G';

            var ar = t.ToArray();

            int i = 0;

            //for (int w = 0; w < ar.Length; w++) {
                //MessageBox.Show("Posicion "+w+" OCGM "+ar[w].oCgM+ " ID Prov Asig "+ar[w].idProvAsigCot);
            //}

                while (i < ar.Length)
                {
                    total = 0;
                    idePrvAnterior = ar[i].idProvAsigCot;
                    ideOcGmAnterior = ar[i].oCgM;
                    //MessageBox.Show("While 1 -> i<ar.lenght GM/OC " + ideOcGmAnterior);
                    idePrvActual = idePrvAnterior;
                    ideOcGmActual = ideOcGmAnterior;

                    //GRABAR ENCABEZADO OBTENIENDO EL NUEVO ID ENCABEZADO
                    int idOrdEncAct = recordOrdenEnc(idePrvAnterior, typ);

                    while (ideOcGmAnterior == ideOcGmActual && i < ar.Length)
                    {
                      //  MessageBox.Show("OC/GM ANTE = AL ACTUAL " + ideOcGmAnterior + " - " + ideOcGmActual);
                        //GRABAR DETALLE

                        OrdenDet od = new OrdenDet { idOrdenDet = 0, idOrdenEnc = idOrdEncAct, idProducto = ar[i].idPro, Cantidad = ar[i].cantidad, PorRecibir = ar[i].cantidad, Precio = ar[i].precio, tipo = ar[i].tipoCotToCuad, idCotizContrato = ar[i].idCotSelecToCuad};
                        try
                        {
                            conex.OrdenDet.InsertOnSubmit(od);
                            conex.SubmitChanges();
                        }
                        catch (Exception ex) { MessageBox.Show("Error al grabar en Orden Detalle" + ex); }
                        //

                        total = total + ar[i].total;//cantidad * ar[i].precio;
                        i++;
                        //MessageBox.Show("i dentro del while donde OCGM Act = OCGM Ant " + i);
                        if (i >= ar.Length) {
                            if (total > 1000){
                                typ = 'O';
                            }
                            else{
                                typ = 'G';
                            }
                            modifyType(typ, idOrdEncAct);
                            break; }
                        //char typ;
                        idePrvActual = ar[i].idProvAsigCot;
                        ideOcGmActual = ar[i].oCgM;
                        //MessageBox.Show("OCGM Actual despues de insertar en OrdenDet " + ideOcGmActual);
                        if (ideOcGmAnterior != ideOcGmActual)
                        {

                            ideOcGmAnterior = ideOcGmActual;
                          //  MessageBox.Show("OCGM ANT != OCGM ACT, Valor OCGM Anterior aquí " + ideOcGmAnterior);

                            if (total > 1000)
                            {//VAR DE LIMITE DE GM GUARDAR EN TABLA PARAMETROS
                                //MODIFICAR EL TIPO DE ORDENENC A ORDEN DE COMPRA
                                typ = 'O';
                            }
                            else
                            {
                                //MODIFICAR EL TIPO A GM
                                typ = 'G';
                            }
                            /*
                            var tr = (from u in conex.OrdenEnc
                                      where u.idOrdenEnc == idOrdEncAct
                                      select u).SingleOrDefault();
                            tr.tipo = typ;
                            conex.OrdenEnc.Context.SubmitChanges();
                             */
                            modifyType(typ, idOrdEncAct);
                            idOrdEncAct = recordOrdenEnc(idePrvAnterior, typ);
                            total = 0;
                        }//SI OCGM ANT ES IGUAL AL OCGM ACTUAL

                        if (idePrvAnterior != idePrvActual)
                        {
                            //MessageBox.Show("El ide del prov ant es difere al act " + idePrvActual);
                            if (total > 0)
                            {
                                if (total > 1000)
                                {//VAR DE LIMITE DE GM GUARDAR EN TABLA PARAMETROS
                                    //MODIFICAR EL TIPO DE ORDENENC A ORDEN DE COMPRA
                                    typ = 'O';
                                }
                                else
                                {
                                    //MODIFICAR EL TIPO A GM
                                    typ = 'G';
                                }
                                modifyType(typ, idOrdEncAct);
                                total = 0;
                            }
                            //GRABAR ENCABEZADO OBTENIENDO EL NUEVO ID DE ENCABEZADO
                            ideOcGmAnterior = ideOcGmActual;
                            idePrvAnterior = idePrvActual;
                            idOrdEncAct = recordOrdenEnc(idePrvAnterior, typ);
                        }
                    }
                    i++;
                }
            
            //PARA CAMBIAR EL STATUS DE COTIZACION DE TODA LA REQUI ENCABEZADO
                MessageBox.Show("ID Requi Enc al cambiar Status "+idReqEncCotiz);
                var updRqE = (from rqe in conex.RequiEnc
                              where rqe.idRequisicion == idReqEncCotiz
                              select rqe).SingleOrDefault();

                updRqE.StatusCot = '1';    
                conex.RequiEnc.Context.SubmitChanges();
                conex.SubmitChanges();

        }


        public void modifyType(char tipo, int idOrdEncAct)
        {

            var tr = (from u in conex.OrdenEnc
                      where u.idOrdenEnc == idOrdEncAct
                      select u).SingleOrDefault();
            tr.tipo = tipo;
            conex.OrdenEnc.Context.SubmitChanges();

        }


        //---------------------------------------------------------------------------------------------------------------------GRABAR EN TABLA ORDENENC
        public int recordOrdenEnc(int xo, char tip)
        {
            //MessageBox.Show("Prov recordOrdenEnc "+xo);
            MessageBox.Show("IDOrdEnc " + 0 + " idReqEnc " + idReqEncCotiz + " idProv " + xo + " folio " + 1 + " fecha " + DateTime.Parse(dpFechaCotizPop.Text) + " tipo " + tip);
            OrdenEnc oe = new OrdenEnc { idOrdenEnc = 0, idReqEnc = idReqEncCotiz, idProveedor = xo, idComprador = idCmpdr, folio = 1, fecha = DateTime.Parse(dpFechaCotizPop.Text), tipo = tip/*'G'*/ };
            
            try 
            {
                conex.OrdenEnc.InsertOnSubmit(oe);
                conex.SubmitChanges();
            }
            catch (Exception ex) { MessageBox.Show("Error al grabar en Orden Encabezado" + ex); }
            return (from yu in conex.OrdenEnc select yu.idOrdenEnc).Max();
        }

        //-----------------------------------------------------------------------------------------------------------------BTN AGREGAR Y GRABAR UNA COTIZACION AL PRODUCTO
        private void btnAddToCotizar(object sender, RoutedEventArgs e)
        {

            if (txtPrec.Text == "00.000" || txtPrec.Text == ".") { MessageBox.Show("Por favor, debe asignar un precio"); btnAsigProvProd.IsEnabled = false; return; }
            if (dpFechaCotizPop.Text.Length < 10 || dpFechaCotizPop.Text.Length > 10) { MessageBox.Show("Por favor, revise el formato de la fecha"); btnAsigProvProd.IsEnabled = false; return; }

            if (k != 1)
            {
                k = 1;
            }

            if (txtPrec.Text.Length < 1 || txtPrec.Text == "0" || txtPrec.Text == "00.000" || txtPrec.Text == "0.0" || txtPrec.Text == ".") { MessageBox.Show("Por favor revisa el costo"); return; }
            MessageBox.Show("Salio del if");
            //DataGridPopup adddgProv = new DataGridPopup();
            collDgPopp.Clear();
            insertCotiz();
            //LLENA TABLA SI EL PROD TIENE CONTRATO
            fillDatagridPopContrato();
            //LLENA TABLA SI EL PROD HAS BEEN COTIZED
            fillDatagridPopProdCotizado();
            string fec = DateTime.Today.ToShortDateString();
            cmbbProvPop.Text = "";
            //MessageBox.Show("Proveedor: "+nomProvSelec);

        }
        //----------------------------------------------------------------------------------------------------------------------SAVE COTIZACION EN TABLA
        private void insertCotiz()
        {
            CotizacionEnc cotEncInsert = new CotizacionEnc { idCotizacionEnc = 0, fecha = Convert.ToDateTime(DateTime.Today.ToShortDateString()), idEmpleado = Id_empleado, idProveedor = Convert.ToInt32(cmbbProvPop.SelectedValue), Observaciones = ObservCotizPop.Text };
            try
            {
                conex.CotizacionEnc.InsertOnSubmit(cotEncInsert);
                conex.SubmitChanges();
                MessageBox.Show("Se inserto Cotización en CotizacionEnc");
            }
            catch (InvalidOperationException) { MessageBox.Show("No se pudo insertar en la BD CotEnc"); }

            int idCotEnca;
            try
            {
                var qrt = (from some in conex.CotizacionEnc select (some.idCotizacionEnc)).Max();
                idCotEnca = qrt;
                //MessageBox.Show("ID Max CotizEnc " + idCotEnca);
            }
            catch (InvalidOperationException)
            {
                idCotEnca = 1;
            }

            CotizacionDet cotDetInsert = new CotizacionDet { idCotizacionDet = 0, idCotizacionEnc = idCotEnca, idRequiDet = idReqDetCotiz, idProd = idProdSeleccCotiz, precio = float.Parse(txtPrec.Text) };
            try
            {
                conex.CotizacionDet.InsertOnSubmit(cotDetInsert);
                conex.SubmitChanges();
                MessageBox.Show("Se inserto Cotización en CotizacionDet");
            }
            catch (InvalidOperationException) { MessageBox.Show("No se pudo insertar en la BD CotDet"); }
        }

        //-------------------------------------------------------------------------------------------------------------EVNT SELCHANG DEL COMBO PROVEEDOR EN POPUP
        private void changeCmbProvPop(object sender, SelectionChangedEventArgs e)
        {

            if (cmbbProvPop.SelectedValue == null) { return; }
            else
            {
                int idProvSelec = (int)cmbbProvPop.SelectedValue;

                var qwe = from vbn in conex.Proveedor
                          where vbn.idProveedor == idProvSelec
                          select vbn;
                foreach (var xc in qwe)
                {
                    nomProvSelec = xc.Nombre;
                }
                //MessageBox.Show("ID sel " + idProvSelec);
            }
        }


        private void rbtSeleccionado()
        {
            int a = collDgPopp.Count();
        }
        //-------------------------------------------------------------------------------------------------------------------EVENTO BUTTON PROBAR DONDE CUENTO MIS FILAS
        private void contarFilas(object sender, RoutedEventArgs e)
        {

            int a = collDgPopp.Count();
            //MessageBox.Show("Filas " + a);
            int z = 1;
            foreach (var b in collDgPopp)
            {
                //MessageBox.Show("Filas " + a + " var foreach " + z);
                z++;
            }

            var chek = from b in collDgPopp
                       where b.isCh == true
                       select b;

            foreach (var b in chek)
            {
                string p = b.dgproveed;
                MessageBox.Show("IDs Prov Checados " + p);
            }

        }
        //----------------------------------------------------------------------------------------------------------------------------EVENTO RADIOBUTTON
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            DataGridPopup dpp = dgPopCotiz.SelectedItem as DataGridPopup;
            btnAsigProvProd.IsEnabled = true;

            if (this.dgPopCotiz.SelectedItem is DataGridPopup)
            {

                if (dpp.tipoCot.Equals("C"))
                {
                    MessageBox.Show("Es Tipo C");
                    txtPrec.IsEnabled = true;
                    MessageBox.Show("ID Provedor Seleccionado " + dpp.indexIdProv + " Precio " + dpp.dgprecio);
                    xo = dpp.indexIdProv;//VAR PARA GUARDAR EL ID DEL PROV MARCADO CON EL RADIOBUTTON Y UTILIZARLA AL ASIGNAR
                    txtPrec.Text = dpp.dgprecio.ToString();
                    MessageBox.Show("IDE Cotizacion " + dpp.idCotiDet + " Tipo " + dpp.tipoCot);
                    ideCotizToCuadro = dpp.idCotiDet;
                    ideTipoToCuadro = dpp.tipoCot;
                }
                else {
                    txtPrec.IsEnabled = false;
                    //MessageBox.Show("ID Provedor Seleccionado " + dpp.indexIdProv + " Precio " + dpp.dgprecio);
                    xo = dpp.indexIdProv;//VAR PARA GUARDAR EL ID DEL PROV MARCADO CON EL RADIOBUTTON Y UTILIZARLA AL ASIGNAR
                    txtPrec.Text = dpp.dgprecio.ToString();
                    MessageBox.Show("IDE Cotizacion " + dpp.idCotiDet + " Tipo " + dpp.tipoCot);
                    ideCotizToCuadro = dpp.idCotiDet;
                    ideTipoToCuadro = dpp.tipoCot;
                }
            }
        }
//--------------------------------------------------------------------------------------------------------------------------EVENTO CUANDO EL CHECKB ES MARCADO
        public void checar(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Checado");

            if (this.dgPopCotiz.SelectedItem is DataGridPopup)
            {
                DataGridPopup dpp = dgPopCotiz.SelectedItem as DataGridPopup;
                //MessageBox.Show("Checbox está: " + dpp.isCh);
                int chboxMarcado = dpp.indexIdProv;
                MessageBox.Show("ID Prov " + chboxMarcado);
            }

        }

        //---------------------------------------------------------------------------------------------------------------EVENTO PARA LA ETIQUETA EN EL DATAG DE LOS PRODUCTOS
        private void cotizMouseD(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            ProdRequeridos prodCot = dGproductRequi.SelectedItem as ProdRequeridos;
            rowToCotizar = prodCot;
            
            if (prodCot == null)
            {
                //MessageBox.Show("Null en click");
                return;
            }


                if (prodCot.stat.Equals("Cotizado"))
                {

                    MessageBoxResult r = MessageBox.Show("El producto ya esta cotizado\n ¿Desea volver a cotizarlo?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (r == MessageBoxResult.Yes)
                    {
                        openPopToCotiz();
                    }
                    else
                    {
                        return;
                    }
                }
                openPopToCotiz();
                //openPopToCotiz(prodCot);
        }
        //--------------------------------------------------------------------------------------------------------------EVENTO INPUT DEL TEXTB DEL PRECIO EN POPUP
        private void precioJustNum(object sender, TextCompositionEventArgs e)
        {
            int ascci = Convert.ToInt32(Convert.ToChar(e.Text));
            //MessageBox.Show("ascci "+ascci);
            if (ascci >= 45 && ascci <= 47)
            {
                e.Handled = false;
            }
            else
            {
                if (ascci >= 48 && ascci <= 57)
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }

        }
        //--------------------------------------------------------------------------------------------------------------------EVENTO BOTON ACEPTAR PARA GUARDAR PRODS COTIZADOS
        private void saveProdsCotizados(object sender, RoutedEventArgs e)
        {
            
            int countList = listaProductosRequi.Count();
            /*
            bool[] xz;// = { false };
            xz = new bool[countList];
            int yiy = 0;
            
            for (yiy = 0; yiy < countList; yiy++)
            {
                //MessageBox.Show("Valor Y For " + yiy);
                int oiu = listaProductosRequi.ElementAt(yiy).idProvAsigCot;

                if (listaProductosRequi.ElementAt(yiy).idProvAsigCot != 0)
                {
                    xz[yiy] = false;
                }
                else
                {
                    xz[yiy] = true;
                }
                MessageBox.Show("Valor Y ++" + yiy);
            }
            MessageBox.Show("Sale del For y entra al segundo for");
            int pne = 0;
            for (int w = 0; w < xz.Length; w++)
            {
                if (xz[w] == true)
                {
                    pne++;
                }
                else
                {
                    pne = 0;
                }
            }

            if (pne != 0)
            {
                grabarOrdenGasto();
            }
            else { MessageBox.Show("No haz cotizado los productos"); return; }
        */
            
            bool verd = true;

            for (int yiy = 0; yiy < countList; yiy++)
            {
                if (listaProductosRequi.ElementAt(yiy).idProvAsigCot == 0)
                {
                    verd = false; break;
                }
            }
            if (verd)
            {
                grabarOrdenGasto();
                cleanInterfaceCoti();
            }
            else { MessageBox.Show("Debe de cotizar los productos"); return; }
            
        }
        //------------------------------------------------------------------------------------------------BTN TO RECORD OBSERV IN THE SELECTED REQI
        private void recObservCot(object sender, RoutedEventArgs e)
        {
            if (folio.Content.Equals("0000")) { MessageBox.Show("Debe seleccionar una Requisición"); btnRecObserv.IsEnabled = false; txtObserv.IsEnabled = false; chckCotizacion.IsChecked = false; }
            else
            {
                int fl = int.Parse(folio.Content.ToString());
                var recO = (from re in conex.RequiEnc where re.idRequisicion == idReqEncCotiz && re.Folio == fl select re).SingleOrDefault();
                recO.Observaciones = txtObserv.Text;
                conex.RequiEnc.Context.SubmitChanges();
            }
        }

        private void cleanInterfaceCoti() {
            folio.Content = "0000";
            fechaElab.Content = "00/00/0000";
            proyectPresup.Text = "";
            solicitante.Text = "";
            dptoBenef.Text = "";
            txtMotiv.Text = "¿Por qué?";
            txtObserv.Text = "¿Algo qué comentar?";
            listaProductosRequi.Clear();
            oc.Clear();
            ocDos.Clear();
            llenaRequiXCotizar();
            llenaRequiCotizada();
        }

        private void dobleCoti(object sender, MouseButtonEventArgs e)
        {
            ProdRequeridos prodCot = dGproductRequi.SelectedItem as ProdRequeridos;
            rowToCotizar = prodCot;

            if (prodCot == null)
            {
                //MessageBox.Show("Null en click");
                return;
            }

            if (dGproductRequi.CurrentColumn.Header.Equals("OC/GM"))
            {
                return;
            }
            else{
                indiceActual = prodCot.indice;
                if (prodCot.stat.Equals("Cotizado"))
                {
                    
                MessageBoxResult r = MessageBox.Show("El producto ya esta cotizado\n ¿Desea volver a cotizarlo?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (r == MessageBoxResult.Yes)
                {
                    openPopToCotiz();
                }
                else
                {
                    return;
                }
            }
            openPopToCotiz();
            }
            
        }

        private void comeBackCotiz_SelCh(object sender, SelectionChangedEventArgs e)
        {
            RequiPorRevisar reCot = reqRevis.SelectedItem as RequiPorRevisar;
            if (reCot == null)
            {
                ocDos.Clear();
                listaProductosRequi.Clear();
                llenaRequiCotizada();
            }
            else
            {
                var rengReCot = (from p in ocDos
                               where p.folio == reCot.folio
                               select p);
                foreach (var x in rengReCot)
                {
                    folioSelected = x.folio;
                    MessageBox.Show("Folio en Foreach seleccionado "+folioSelected);
                }
                muestraTotal();
                muestraDatosRequi();
                llenaProductosdeRequi();
                btnAcepCoti.IsEnabled = true;
            }
        }


    }



}

class ProdRequeridos
{

    public int idPro { get; set; }
    public String nombreProdu { get; set; }
    public double cantidad { get; set; }
    public double precio { get; set; }
    public double total { get; set; }
    public string umedi { get; set; }
    public string coti { get; set; }
    public int idReqDetProdReq { get; set; }
    public int oCgM { get; set; }
    public string stat { get; set; }
    public int idProvAsigCot { get; set; }
    public int idCotSelecToCuad { get; set; }
    public char tipoCotToCuad { get; set; }
    public int indice { get; set; }
}

class Provee
{
    public int idProve { get; set; }
    public string nomProve { get; set; }
}

class DataGridPopup
{
    cotizacion c = new cotizacion();
    public string dgproveed { get; set; }
    public string dgproduc { get; set; }
    public float dgprecio { get; set; }
    public string dgfechReg { get; set; }
    public string dgfechCoti { get; set; }
    public string dgPersonaReg { get; set; }
    public string dgObservCot { get; set; }
    //public int indexIdProv { get { return indexIdProv; } set { indexIdProv = cotizacion.idProveToCotiza; } }
    public int indexIdProv { get; set; }
    //public bool checkRdb { get; set; }
    public bool isCh;
    public bool isch
    {
        get { return isCh; }
        set { isCh = value; }
    }
    public int idCotiDet { get; set; }
    public char tipoCot { get; set; }
}

