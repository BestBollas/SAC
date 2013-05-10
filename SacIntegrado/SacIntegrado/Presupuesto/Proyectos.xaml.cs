using SacIntegrado.Presupuesto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Data.Linq;

namespace SacIntegrado
{
    /// <summary>
    /// Lógica de interacción para Proyectos.xaml
    /// </summary>
    /// 


    public partial class Proyectos : Page
    {
        private ObservableCollection<RecursoC> misRecursosProyecto = new ObservableCollection<RecursoC>();
        private ObservableCollection<PartidaProyectoC> misPartidas = new ObservableCollection<PartidaProyectoC>();
        private ObservableCollection<PartidaProyectoC> misPartidasNuevas = new ObservableCollection<PartidaProyectoC>();
        private ObservableCollection<ClasifFuncionalC> misClasifFuncionalProyecto = new ObservableCollection<ClasifFuncionalC>();
        private ObservableCollection<ClasifProgramaticoC> misClasifProgramaticoProyecto = new ObservableCollection<ClasifProgramaticoC>();
        private ObservableCollection<ActividadPoaC> misActividadPoaProyecto = new ObservableCollection<ActividadPoaC>();
        private ObservableCollection<EmpleadoC> misEmpleadosProyecto = new ObservableCollection<EmpleadoC>();
        private ObservableCollection<CuentaC> misCuentas = new ObservableCollection<CuentaC>();
        private ObservableCollection<PeriodoC> misPeriodos = new ObservableCollection<PeriodoC>();
        private ObservableCollection<AnioAplicaC> misPeriodosFuturos = new ObservableCollection<AnioAplicaC>();
        private ObservableCollection<ProyectoC> misProyectos= new ObservableCollection<ProyectoC>();
        private ObservableCollection<ProyectoC> misProyectosNuevos = new ObservableCollection<ProyectoC>();
        private ObservableCollection<AnioAplicaC> aniosAplica = new ObservableCollection<AnioAplicaC>();
        //--------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<AreaPresu> ocpresuArea = new ObservableCollection<AreaPresu>();
        ObservableCollection<DepartamentoClass> ocDepto = new ObservableCollection<DepartamentoClass>();
        ObservableCollection<GsaClass> octipoGs = new ObservableCollection<GsaClass>();
        ObservableCollection<ClasifFuncionalC> ocClasiFun = new ObservableCollection<ClasifFuncionalC>();
        ObservableCollection<TipoGastoClass> ocTipGasto= new ObservableCollection<TipoGastoClass>();
        ObservableCollection<Partida> listaPartida = new ObservableCollection<Partida>();
        string centroCostoC = "";
        string clasiPrograC = "";
        string compoActiviC = "";
        string clasiFunC = "";
        string fuenteFinaC = "";
        string tipoGastoC = "";
        string gastoC = "";
        //--------------------------------------------------------------------------------------------------------------------------
        public int idProyecto { get; set;}
        public int idEmpleado { set; get; }
        public int idDepto { set; get; }
        public int idPartida { get; set; }
        
     

        Db re = new Db();
        String[] mes = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre", "Cierre" };
        String nomUsua;
        public Proyectos() { 
        
        }
        public Proyectos(String user,int id,String nombre)
        {
            InitializeComponent();
            idEmpleado = 1;
            nomUsua = nombre;
            lblusuario.Content = nomUsua;
            MenuPrincipal temp = new MenuPrincipal(this, user,id,nombre);
            temp.crearMenu();
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            cargarProyecto();
    
            listaCta.Visibility = Visibility.Hidden;
        }
        
        /// /////////////////////////////////////////////////////////Metodos creados para llemar combos by BEST////////////////////////////////////////////////        
        public void llenaCmbArea()
        {
            ocpresuArea.Clear();
            var query = from x in re.Area
                        select x;

            foreach (var i in query)
            {
                ocpresuArea.Add(new AreaPresu { idArea = i.idArea, nomArea = i.nomArea, idJefe = i.idJefe.Value, clavePresupuestal = i.clavePresupuestal });

            }
            cmbAreaResp.ItemsSource = ocpresuArea;
            cmbAreaResp.SelectedValuePath = "idArea";
            cmbAreaResp.DisplayMemberPath = "nomArea";
        }

        public void llenaCmbDeptoRespon() {

            var query = from x in re.Departamento
                        select x;
            foreach(var i in query){
                ocDepto.Add(new DepartamentoClass {idDepto=i.idArea.Value,NombreDepto=i.NombreDepto,idjefe=i.idJefe.Value,idArea=i.idArea.Value,clavePresupuestal=i.clavePresupuestal });
            }
            comboRecursoProyecto.ItemsSource = ocDepto;
            comboRecursoProyecto.DisplayMemberPath = "NombreDepto";
            comboRecursoProyecto.SelectedValuePath = "idDepto";
        
        }

        public void llenaCmbFuenteFin()
        {
            var pe = from r in re.Recurso
                     where r.Vigente == true
                     select new { r.idRecurso, r.Nombre };

            misRecursosProyecto.Clear();
            foreach (var ele in pe)
            {
                misRecursosProyecto.Add(new RecursoC { idRecurso = ele.idRecurso, Nombre = ele.Nombre });
            }
            cmbFuentesFin.DisplayMemberPath = "Nombre";
            cmbFuentesFin.SelectedValuePath = "idRecurso";
            cmbFuentesFin.ItemsSource = misRecursosProyecto;

        }
        public void llenaCmbCompActi() {
            var pe = from r in re.ActividadPoa

                     select new { r.idActPoa, r.Nombre };

            misActividadPoaProyecto.Clear();
            foreach (var ele in pe)
            {
                misActividadPoaProyecto.Add(new ActividadPoaC { idActPoa = ele.idActPoa, Nombre = ele.Nombre });
            }

            cmbComActi.DisplayMemberPath = "Nombre";
            cmbComActi.SelectedValuePath = "idActPoa";
            cmbComActi.ItemsSource = misActividadPoaProyecto;
        //cmbComActi
        }
        public void llenaCmbGSA() { 

            var query=from x in re.GastoSocialAdmin
                      select x;
            foreach(var i in query){
                octipoGs.Add(new GsaClass {idGSA=i.idGSA,nombreGSA=i.nombreGSA,clavePresu=i.clavePresu,anioAplica=i.anioAplica.Value,fechaRegistro=Convert.ToString(i.fechaRegistro),empleado=Convert.ToString(i.idEmpleado),vigenteTab=Convert.ToString(i.vigente.Value)});
            
            }
            cmbGSA.ItemsSource = octipoGs;
            cmbGSA.DisplayMemberPath = "nombreGSA";
            cmbGSA.SelectedValuePath = "idGSA";
        }

        public void llenarClasifiFun()
        {
            var pe = from r in re.ClasificadorFuncional
                     where r.vigente == true
                     select new { r.idClasifFuncional, r.Nombre };

            
            foreach (var ele in pe)
            {
                ocClasiFun.Add(new ClasifFuncionalC { idClasifFuncional = ele.idClasifFuncional, Nombre = ele.Nombre });
            }
            comboClasifFuncionalProyecto.DisplayMemberPath = "Nombre";
            comboClasifFuncionalProyecto.SelectedValuePath = "idClasifFuncional";
            comboClasifFuncionalProyecto.ItemsSource = ocClasiFun;
        }

        public void  llenaTipoGasto(){

            var query = from x in re.TipoGasto
                        select x;
            foreach(var i in query){
                ocTipGasto.Add(new TipoGastoClass { idTG = i.idTG.Value, nombreTG = i.nombreTG, clavePresupuestal = i.clavePresupuestal, anioAplica = i.anioAplica.Value, fechaReg = Convert.ToString(i.fechaReg), Empleado =Convert.ToString( i.idEmpleado), vigenteTab =Convert.ToString(i.vigente) });
                cmbTipGasto.ItemsSource = ocTipGasto;
                cmbTipGasto.SelectedValuePath="idTG";
                cmbTipGasto.DisplayMemberPath = "nombreTG";
            }
        }
//_______________________________________________________________________________________________________________________________________________________________

//_________________________________________________________Metodo para obtener cuenta_____________________________________________________________________
        public void llenarCuenta() {

            var query= from x in re.Parametros
                       select x;
                foreach(var i in query){
                centroCostoC = i.centroCostos;
            }
                lblCuenta.Content = "CUENTA:  " + centroCostoC.Trim() + "-" + clasiPrograC.Trim() + "-" + compoActiviC.Trim() + "-" + clasiFunC.Trim() + "-" + fuenteFinaC.Trim() + "-" + tipoGastoC.Trim() + "-" + gastoC.Trim();
        
        }
//______________________________________________________________________________________________________________________________________________________________
        public void cargarRecurso()
        {
            var pe = from r in re.Recurso
                     where r.Vigente == true
                     select new { r.idRecurso, r.Nombre };

            misRecursosProyecto.Clear();
            foreach (var ele in pe)
            {
                misRecursosProyecto.Add(new RecursoC { idRecurso = ele.idRecurso, Nombre = ele.Nombre });
            }
            comboClasifFuncionalProyecto.DisplayMemberPath = "Nombre";
            comboClasifFuncionalProyecto.SelectedValuePath = "idRecurso";
            comboClasifFuncionalProyecto.ItemsSource = misRecursosProyecto;

        }

        public void cargarEmpleado()
        {
            var pe = from r in re.Empleado
                     where r.idCategoria == 1
                     select new { r.idEmpleado, r.Nombre };

            misEmpleadosProyecto.Clear();
            foreach (var ele in pe)
            {
                misEmpleadosProyecto.Add(new EmpleadoC { idEmpleado = ele.idEmpleado, Nombre = ele.Nombre});
            }
            comboResponsableProyecto.DisplayMemberPath = "Nombre";
            comboResponsableProyecto.SelectedValuePath = "idEmpleado";            
            comboResponsableProyecto.ItemsSource = misEmpleadosProyecto;

        }

        public void cargarClasifFuncional()
        {
            var pe = from r in re.ClasificadorFuncional

                     select new { r.idClasifFuncional, r.Nombre };

            misClasifFuncionalProyecto.Clear();
            foreach (var ele in pe)
            {
                misClasifFuncionalProyecto.Add(new ClasifFuncionalC { idClasifFuncional = ele.idClasifFuncional, Nombre = ele.Nombre });
            }
            comboClasifFuncionalProyecto.DisplayMemberPath = "Nombre";
            comboClasifFuncionalProyecto.SelectedValuePath = "idClasifFuncional";
            comboClasifFuncionalProyecto.ItemsSource = misClasifFuncionalProyecto;

        }

        public void cargarClasifProgramatico()
        {
            var pe = from r in re.ClasificadorProgramatico

                     select new { r.idClasificadorPro, r.Nombre };

            misClasifProgramaticoProyecto.Clear();
            foreach (var ele in pe)
            {
                misClasifProgramaticoProyecto.Add(new ClasifProgramaticoC { idClasifProgramatico = ele.idClasificadorPro, Nombre = ele.Nombre });
            }
            comboClasifProgramaticoProyecto.DisplayMemberPath = "Nombre";
            comboClasifProgramaticoProyecto.SelectedValuePath = "idClasifProgramatico";
            comboClasifProgramaticoProyecto.ItemsSource = misClasifProgramaticoProyecto;

        }
       

        //public void cargarCuentas()
        //{
        //    var pe = from r in re.CuentaEnc

        //             select new { r.IdCuenta, r.Nombre ,r.Cuenta};

        //    misCuentas.Clear();
        //    foreach (var ele in pe)
        //    {
        //        misCuentas.Add(new CuentaC { idCuenta = ele.IdCuenta, Nombre = ele.Nombre, Cuenta = ele.Cuenta});
        //    }

        //    cuentasCombo.DisplayMemberPath = "Cuenta";
        //    cuentasCombo.SelectedValuePath = "idCuenta";
        //    cuentasCombo.ItemsSource = misCuentas;
        //}

        //public void cargarPeriodos()
        //{
        //    var pe = from r in re.Periodo
        //             where r.mes !=13
        //             orderby r.mes
        //             select new { r.idPeriodo, r.mes, r.anio, r.status };
           

        //    misPeriodos.Clear();
        //    foreach (var ele in pe)
        //    {
        //        misPeriodos.Add(new PeriodoC { idPeriodo = ele.idPeriodo, anio = ele.anio.Value, mes = ele.mes.Value, status = ele.status.Value,  periodo = mes[ele.mes.Value - 1]});
        //    }

        //    periodoCombo.DisplayMemberPath = "periodo";
        //    periodoCombo.SelectedValuePath = "idPeriodo";
        //    periodoCombo.ItemsSource = misPeriodos;

        //}
        public int indexAnioAplica() {

            RecursoC rer = new RecursoC();
            int anioActual = DateTime.Today.Year;
            int indexA = 0;

            foreach (var a in rer.AnioList)
            {
                if (a.Equals(anioActual))
                {
                    break;
                }
                indexA++;
            }
            return indexA;
        }

        public void cargarAniosAplica(){
            llenarAnio a = new llenarAnio();
            a.llenarAnios(comboAnioProyecto);
            
            
          
        }
        public void cargarProyecto()
        {
            //Eder
            llenaCmbArea();
            llenaCmbDeptoRespon();
            llenaCmbFuenteFin();
            llenaCmbCompActi();
            llenaCmbGSA();
            llenarClasifiFun();
            llenaTipoGasto();
            
            //--------
            cargarAniosAplica();
            //cargarActividadPoa();
            //cargarClasifFuncional();
            //cargarRecurso();
            cargarEmpleado();
            cargarClasifProgramatico();
            //cargarCuentas();
            //cargarPeriodos();
            cargarPartidas();
            consultaProyecto();
            parti.IsEnabled = false;


        }

        //public void cargarProyectosNuevoAño() {
        // var anioList = (from p in re.Periodo
        //                 where p.status == 'A'
        //                 select new {p.idPeriodo} ).SingleOrDefault();

        // var pari = from p in re.PartidaProyecto
        //              where p.idPeriodo == anioList.idPeriodo
        //              select p;
        // misPartidasNuevas.Clear();

           
        //    foreach(var ele in pari){

        //        misPartidasNuevas.Add(new PartidaProyectoC { idCtaProy = ele.idCtaProy,idProyecto=ele.idProyecto.Value,  idCuenta = ele.idCuenta.Value, idPeriodo = ele.idPeriodo.Value, saldoInicial = ele.saldoInicial.Value, saldoFin = ele.saldoFin.Value });
        //    }

        //    partis.ItemsSource = misPartidasNuevas;
        //    Table<PartidaProyecto> par = re.GetTable<PartidaProyecto>();
        //    PartidaProyecto pa = new PartidaProyecto();

        //    foreach (var nu in misPartidasNuevas) 
        //    {

        //    double por = (double.Parse(Porcentaje.Value.ToString())/100)*nu.saldoInicial;
        //    double aumento = nu.saldoInicial + por;
        //    pa.idCuenta = nu.idCuenta;
        //    pa.idPeriodo = int.Parse(periodoSeleccion.SelectedValue.ToString());
        //    pa.idProyecto = nu.idProyecto;
        //    pa.saldoFin = aumento;
        //    pa.saldoInicial = aumento;

        //    }
        //    par.InsertOnSubmit(pa);
        //    par.Context.SubmitChanges();

            
            
        //      }

        //private void porcentaje_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        //{
        //   actualizarValor();
        //}

        //private void actualizarValor()

        //{

        //    double por = double.Parse(Porcentaje.Value.ToString());
        //    valorPorcentaje.Content = por+" %";
        //}
        public void cargarPartidas()
        {
            var pe = from r in re.PartidaProyecto
                     from c in re.CuentaEnc
                     where r.idCuenta==c.IdCuenta && r.idProyecto==idProyecto
                     select new { r.idCtaProy,r.idCuenta, r.idPeriodo, r.saldoFin, r.saldoInicial, c.Cuenta,c.Nombre,r.enero,r.febrero,r.marzo,r.abril,r.mayo,r.junio,r.julio,r.agosto,r.septiembre,r.octubre,r.noviembre,r.diciembre };

            misPartidas.Clear();
            foreach (var ele in pe)
            {
                misPartidas.Add(new PartidaProyectoC { idCtaProy = ele.idCtaProy, idCuenta = ele.idCuenta.Value, Cuenta = ele.Cuenta, Nombre = ele.Nombre, idPeriodo = ele.idPeriodo.Value, saldoInicial = ele.saldoInicial.Value, saldoFin = ele.saldoFin.Value, enero = ele.enero.Value, febrero = ele.febrero.Value, marzo = ele.marzo.Value, abril = ele.abril.Value, mayo = ele.mayo.Value, junio = ele.junio.Value, julio = ele.julio.Value, agosto = ele.agosto.Value, septiembre = ele.septiembre.Value, octubre = ele.octubre.Value, noviembre = ele.noviembre.Value, diciembre = ele.diciembre.Value });
            }

            tablaPartida.ItemsSource = misPartidas;

        }
        public void consultaProyecto() { 
         var pro=(from p in re.Proyecto
                 from e in re.Empleado
                 from d in re.Departamento
                 from cf in re.ClasificadorFuncional
                 from cp in re.ClasificadorProgramatico
                 from ap in re.ActividadPoa
                 from r in re.Recurso
                 from gsa in re.GastoSocialAdmin
                 from tg in re.TipoGasto
                 from area in re.Area
                 where p.idActPoa==ap.idActPoa
                       && p.idClasifFuncional==cf.idClasifFuncional
                       && p.idRecurso==r.idRecurso 
                       && p.idResponsable==e.idEmpleado
                       && p.idClasifProgramatico==cp.idClasificadorPro
                       && p.idDepto==d.idDepto
                       && p.idGSA==gsa.idGSA
                       && p.idTG==tg.idTG
                       && p.idArea==area.idArea
                       
                 select new {
                     p.idProyecto,
                     p.Nombre,
                     p.justificacion,
                     p.anioAplica,
                     e.idEmpleado,
                     d.idDepto,
                     cf.idClasifFuncional,
                     cp.idClasificadorPro,
                     ap.idActPoa,
                     r.idRecurso,
                     nres= e.Nombre,
                     d.NombreDepto,
                     ncf=cf.Nombre,
                     ncp= cp.Nombre,
                     nap=ap.Nombre,
                     nr=r.Nombre ,
                     gsa.nombreGSA,
                     tg.nombreTG,
                     area.idArea,
                     area.nomArea,p.clavePresu
                 
                 }).Distinct();
         misProyectos.Clear();
            foreach(var p in pro){

                misProyectos.Add(new ProyectoC{idProyecto=p.idProyecto,
                                                nombreP=p.Nombre,
                                                justificacion=p.justificacion,
                                                idResponsable=p.idEmpleado, 
                                                idDepto=p.idDepto,
                                                idActPoa=p.idActPoa,
                                                idClasifFuncional=p.idClasifFuncional,
                                                idClasifProgramatico=p.idClasificadorPro,
                                                idRecurso=p.idRecurso, 
                                                nombreCF=p.ncf,
                                                nombreCP=p.ncp,
                                                nombreD=p.NombreDepto,
                                                nombrePoa=p.nap,
                                                nombreR=p.nr,
                                                nombreRes=p.nres, 
                                                anioAplica=p.anioAplica.Value, nombreGSA=p.nombreGSA,nombreTG=p.nombreTG,idArea=p.idArea,nombreArea=p.nomArea,clavePresu=p.clavePresu
                                                    });
            }

            tablaProyectos.ItemsSource = misProyectos;
                   


        
        }
        private void tablaPartida_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            PartidaProyectoC pro = tablaPartida.SelectedItem as PartidaProyectoC;
          
            if (pro!= null) {

                String cuentaS = pro.Cuenta;
                int  periodoS = pro.Periodo;
                int indexC = 0;
                int indexP = 0;

                foreach (var es in misCuentas)
                {
                    if (es.Cuenta.Equals(cuentaS))
                    {
                        break;
                    }

                    indexC++;

                }

                foreach (var es in misPeriodos)
                {
                    if (es.anio.Equals(periodoS))
                    {
                        break;
                    }

                    indexP++;

                }

                //cuentasCombo.SelectedIndex = indexC;
                txtcuenta.Text = pro.Nombre;
                txtEnero.Text = pro.enero.ToString();
                txtFebrero.Text = pro.febrero.ToString();
                txtMarzo.Text = pro.marzo.ToString();
                txtAbril.Text = pro.abril.ToString();
                txtMayo.Text = pro.mayo.ToString();
                txtJunio.Text = pro.junio.ToString();
                txtJulio.Text = pro.julio.ToString();
                txtAgost.Text = pro.agosto.ToString();
                txtSept.Text = pro.septiembre.ToString();
                txtOct.Text = pro.octubre.ToString();
                txtNov.Text = pro.noviembre.ToString();
                txtDic.Text = pro.diciembre.ToString();
                nombre.Content = pro.Nombre;
                //periodoCombo.SelectedIndex = indexP; 
                //saldoFinPartida.Text = "" + pro.saldoFin;
                saldoInicialPartida.Text = "" + pro.saldoInicial;
                idPartida = pro.idCtaProy;
            }
            else
            {
                idPartida = 0;
            }           
            this.Background = Brushes.LightGray;
            Partidas.IsOpen = true; 
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
                Partidas.IsOpen = false;
                txtcuenta.Text = "";
                listaCta.Visibility = Visibility.Hidden;
                limpiarPartida();
        }

        //private void cuentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (cuentasCombo.SelectedIndex == -1)
        //    {
        //        cuentasCombo.Text = "";
        //    }
        //    else
        //    {
        //        int id = int.Parse(cuentasCombo.SelectedValue.ToString());

        //        var pe = from c in re.CuentaEnc
        //                 where id == c.IdCuenta
        //                 select new { c.Nombre };

        //        foreach (var el in pe)
        //        {
        //            nombrePartida.Text = el.Nombre;

        //        }
        //    }
        //}

        private void depto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboResponsableProyecto.SelectedIndex == -1)
            {

                comboResponsableProyecto.Text = "";
            }
            else
            {

                int id = int.Parse(comboResponsableProyecto.SelectedValue.ToString());

                var pe = from c in re.Departamento
                         where id == c.idDepto
                         select new { c.NombreDepto, c.idDepto };

                foreach (var el in pe)
                {
                    comboRecursoProyecto.Text = el.NombreDepto;
                    cmbAreaResp.Text = el.NombreDepto;
                    idDepto = el.idDepto;

                }
            }
            
        }

        private void grabarProyecto(object sender, RoutedEventArgs e)
        {
            misProyectos.Clear();
            Table<Proyecto> pro = re.GetTable<Proyecto>();
            Proyecto tb = new Proyecto();
            tb.idRecurso = int.Parse(cmbFuentesFin.SelectedValue.ToString());
            tb.idDepto = idDepto;
            tb.idResponsable=int.Parse(comboResponsableProyecto.SelectedValue.ToString());
            tb.Nombre = textNombreProyecto.Text;
            tb.idActPoa = int.Parse(cmbComActi.SelectedValue.ToString());
            tb.idClasifProgramatico = int.Parse(comboClasifProgramaticoProyecto.SelectedValue.ToString());
            tb.idClasifFuncional = int.Parse(comboClasifFuncionalProyecto.SelectedValue.ToString());
            tb.anioAplica = int.Parse(comboAnioProyecto.SelectedValue.ToString());
            tb.justificacion = textjustificacionProyecto.Text;
            tb.idGSA = int.Parse(cmbGSA.SelectedValue.ToString());
            tb.idTG = int.Parse(cmbTipGasto.SelectedValue.ToString());
            tb.idArea = int.Parse(cmbAreaResp.SelectedValue.ToString());
            tb.saldoInicial = 0;
            tb.saldoFinal = 0;
            tb.clavePresu = txtClave.Text;
            pro.InsertOnSubmit(tb);
            pro.Context.SubmitChanges();
            MessageBox.Show("El registro se agrego correctamente.");

            int sel = (from p in re.Proyecto
                       select p.idProyecto).Max();
            idProyecto = sel;

            parti.IsExpanded = true;
            parti.IsEnabled = true;
            consultaProyecto();
            limpiarProyecto();

        }

        public void modificar(object sender, RoutedEventArgs e)
        {
            Proyecto tb = (from p in re.Proyecto
                           where p.idProyecto == idProyecto
                           select p).SingleOrDefault();   
            tb.idRecurso = int.Parse(comboRecursoProyecto.SelectedValue.ToString());
            tb.idDepto = idDepto;
            tb.idResponsable = int.Parse(comboResponsableProyecto.SelectedValue.ToString());
            tb.Nombre = textNombreProyecto.Text;
            tb.idActPoa = int.Parse(cmbComActi.SelectedValue.ToString());
            tb.idClasifProgramatico = int.Parse(comboClasifProgramaticoProyecto.SelectedValue.ToString());
            tb.idClasifFuncional = int.Parse(comboClasifFuncionalProyecto.SelectedValue.ToString());
            tb.anioAplica = int.Parse(comboAnioProyecto.SelectedValue.ToString());
            tb.justificacion = textjustificacionProyecto.Text;

            re.SubmitChanges();
            limpiarProyecto();
            
                     
           
            MessageBox.Show("El registro se modifico correctamente.");
        
        }

        public void limpiarProyecto() {
            txtClave.Text = "";
            textNombreProyecto.Text = "";
            textjustificacionProyecto.Text = "";
            cmbAreaResp.SelectedIndex = -1;
            comboAnioProyecto.SelectedIndex = indexAnioAplica();
            comboClasifFuncionalProyecto.SelectedIndex = -1;
            comboClasifProgramaticoProyecto.SelectedIndex = -1;
            cmbComActi.SelectedIndex = -1;
            comboRecursoProyecto.SelectedIndex = -1;
            comboResponsableProyecto.SelectedIndex = -1;
            comboRecursoProyecto.SelectedIndex = -1;
            idProyecto = 0;
            idDepto = 0;
            parti.IsEnabled = false;
            parti.IsExpanded = false;
            lblCuenta.Content = "CUENTA:";
            comboAnioProyecto.SelectedIndex = -1;
            cmbFuentesFin.SelectedIndex = -1;
            cmbTipGasto.SelectedIndex = -1;
            cmbGSA.SelectedIndex = -1;
        }

        private void grabarPartida(object sender, RoutedEventArgs e)
        {
            if (idPartida == 0)
            {
                Table<PartidaProyecto> par = re.GetTable<PartidaProyecto>();
                PartidaProyecto pa = new PartidaProyecto();
                double saldo = double.Parse(txtEnero.Text) + double.Parse(txtFebrero.Text) + double.Parse(txtMarzo.Text) + double.Parse(txtAbril.Text) + double.Parse(txtMayo.Text) + double.Parse(txtJunio.Text) + double.Parse(txtJulio.Text) + double.Parse(txtAgost.Text) + double.Parse(txtSept.Text) + double.Parse(txtOct.Text) + double.Parse(txtNov.Text) + double.Parse(txtDic.Text);
                pa.idCuenta = int.Parse(listaCta.SelectedValue.ToString());
                pa.idPeriodo = 0;
                pa.idProyecto = idProyecto;
                pa.enero =double.Parse(txtEnero.Text);
                pa.febrero = double.Parse(txtFebrero.Text);
                pa.marzo = double.Parse(txtMarzo.Text);
                pa.abril = double.Parse(txtAbril.Text);
                pa.mayo = double.Parse(txtMayo.Text);
                pa.junio = double.Parse(txtJunio.Text);
                pa.julio = double.Parse(txtJulio.Text);
                pa.agosto = double.Parse(txtAgost.Text);
                pa.septiembre = double.Parse(txtSept.Text);
                pa.octubre = double.Parse(txtOct.Text);
                pa.noviembre = double.Parse(txtNov.Text);
                pa.diciembre = double.Parse(txtDic.Text);
                pa.saldoFin = double.Parse(saldo.ToString()); ;
                pa.saldoInicial = double.Parse(saldo.ToString());
                par.InsertOnSubmit(pa);
                par.Context.SubmitChanges();
                cargarPartidas();
                limpiarPartida();
            }
            else
            {
                PartidaProyecto pa = (from pp in re.PartidaProyecto
                                      where pp.idCtaProy == idPartida
                                      select pp).SingleOrDefault();
                pa.idCuenta = int.Parse(listaCta.SelectedValue.ToString());
                //pa.idPeriodo = int.Parse(periodoCombo.SelectedValue.ToString());
                pa.idProyecto = idProyecto;
                pa.saldoFin = double.Parse(saldoInicialPartida.Text);
                pa.saldoInicial = double.Parse(saldoInicialPartida.Text);
                re.SubmitChanges();
                Partidas.IsOpen = false;
                cargarPartidas();
                limpiarPartida();

            }




        }

        public void limpiarPartida() {

            txtcuenta.Text = "";
            txtEnero.Text = "";
            txtFebrero.Text="";
            txtMarzo.Text="";
            txtAbril.Text="";
            txtMayo.Text="";
            txtJunio.Text="";
            txtJulio.Text="";
            txtAgost.Text="";
            txtSept.Text="";
            txtOct.Text="";
            txtNov.Text="";
            txtDic.Text="";
           // cuentasCombo.SelectedIndex = -1;
            nombre.Content = "";
            //saldoFinPartida.Text = "";
            saldoInicialPartida.Text = "";
            txtEnero.Text = "";

            //periodoCombo.SelectedIndex = indexAnioAplica();
            

        
        }

        private void tablaProyectos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ProyectoC pro = tablaProyectos.SelectedItem as ProyectoC;
            if (pro != null)
            {

                String nombreRes = pro.nombreRes;
                String nombreCF = pro.nombreCF;
                String nombreCP = pro.nombreCP;
                String nombrePoa = pro.nombrePoa;
                String nombreR = pro.nombreR;
                String nombreFF = pro.nombreR;
                int anioApli = pro.anioAplica;
                //---------------------------
                //String nombregsa = pro.nombreGSA;
                //---------------------------

                int indexRe = 0;
                int indexCF = 0;
                int indexCP = 0;
                int indexPoa = 0;
                int indexR = 0;
                int indexA = 0;
                //-----------------------
                //int indexff = 0;
                int indexArea = 0;
                int indexTipG = 0;
                int indexgsa = 0;
                int indexDepto = 0;
                ////-----------------------
                foreach(var es in octipoGs){
                    if(es.nombreGSA.Equals(pro.nombreGSA)){
                        break;
                    }
                    indexgsa++;
                }

                foreach (var es in ocTipGasto)
                {
                    if (es.nombreTG.Equals(pro.nombreTG))
                    {
                        break;
                    }
                    indexTipG++;
                }
                foreach (var es in ocpresuArea)
                {
                    if (es.nomArea.Equals(pro.nombreArea))
                    {
                        break;
                    }
                    indexArea++;
                }

                foreach (var es in ocDepto)
                {
                    if (es.NombreDepto.Equals(pro.nombreD))
                    {
                        break;
                    }
                    indexDepto++;
                }

                //--------------------------------------------
                foreach (var es in misEmpleadosProyecto)
                {
                    if (es.Nombre.Equals(nombreRes))
                    {
                        break;
                    }
                    
                    indexRe++;

                }


                foreach (var es in ocClasiFun)
                {
                    if (es.Nombre.Equals(nombreCF))
                    {
                        break;
                    }
                    
                    indexCF++;

                }
                foreach (var es in misClasifProgramaticoProyecto)
                {
                    if (es.Nombre.Equals(nombreCP))
                    {
                        break;
                    }
                    
                    indexCP++;

                }

                foreach (var es in misActividadPoaProyecto)
                {
                    if (es.Nombre.Equals(nombrePoa))
                    {
                        break;
                    }
                    
                    indexPoa++;

                }
                foreach (var es in misRecursosProyecto)
                {
                    if (es.Nombre.Equals(nombreR))
                    {
                        break;
                    }
                    
                    indexR++;

                }

                foreach (var es in aniosAplica)
                {
                    if (es.anio.Equals(anioApli))
                    {
                        break;
                    }
                    
                    indexA++;
                    //MessageBox.Show("anio combo "+indexA);

                }
                textNombreProyecto.Text = pro.nombreP;
                //depto.Text = pro.nombreD;
                textjustificacionProyecto.Text = pro.justificacion;
                comboResponsableProyecto.SelectedIndex = indexRe;
                comboClasifFuncionalProyecto.SelectedIndex = indexCF;
                comboClasifProgramaticoProyecto.SelectedIndex = indexCP;
                comboRecursoProyecto.SelectedIndex = -1;
                //Eder---------------------------------------------------
                cmbFuentesFin.SelectedIndex = indexR;
                cmbGSA.SelectedIndex = indexgsa;
                cmbTipGasto.SelectedIndex = indexTipG;
                cmbAreaResp.SelectedIndex = indexArea;
                comboRecursoProyecto.SelectedIndex = indexDepto;
                //---------------------------------------------------------
                cmbComActi.SelectedIndex = indexPoa;
                comboAnioProyecto.Text =Convert.ToString(pro.anioAplica);
                idProyecto = pro.idProyecto;
                tabContr.SelectedIndex = 0;
                txtClave.Text = pro.clavePresu;
                //area.Text = pro.nombreD;
                cargarPartidas();
                parti.IsExpanded = true;
                parti.IsEnabled = true;

            }
            else {
                tabContr.SelectedIndex = 0;
            
            }
        }

        private void limpiarTodo(object sender, RoutedEventArgs e)
        {
            limpiarProyecto();
        }

        //private void Button_Click_2(object sender, RoutedEventArgs e)

        //{
        //    var anioFuturo = (from p in re.Periodo
        //                      where p.status == 'F'
        //                      select new { p.idPeriodo, p.anio });
           
        //    misPeriodosFuturos.Clear();
        //    foreach (var af in anioFuturo)
        //    {
        //        misPeriodosFuturos.Add(new AnioAplicaC { idPeriodo = af.idPeriodo, anio = af.anio.Value });

        //    }

        //    periodoSeleccion.ItemsSource = misPeriodosFuturos;
        //    periodoSeleccion.SelectedValuePath = "idPeriodo";
        //    periodoSeleccion.DisplayMemberPath = "anio";
        //    PeriodoCambio.IsOpen = true;
            
        //}

        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    cargarProyectosNuevoAño();
        //    PeriodoCambio.IsOpen = false;
        //}

        private void clasificadorFuncional(object sender, SelectionChangedEventArgs e)
        {
            if(comboClasifFuncionalProyecto.SelectedValue==null){
                return;
            }
            var clasifi = from cf in re.ClasificadorFuncional
                          where cf.idClasifFuncional == (int)comboClasifFuncionalProyecto.SelectedValue
                          select cf;
            foreach (var d in clasifi)
            {
                clasiFunC = d.Clave;
            }
            llenarCuenta();
        }

        private void fuentesFinanciamiento(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFuentesFin.SelectedValue == null)
            {
                return;
            }
            var fuentesFin = from ff in re.Recurso
                             where ff.idRecurso == (int)cmbFuentesFin.SelectedValue
                             select ff;
            foreach (var f in fuentesFin)
            {
                fuenteFinaC = f.ClavePresupuestal.ToString();//ERROR PORQ ES STRING A INT WTF!!
            }
            llenarCuenta();
        }

        private void gastoSocialAdmin(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGSA.SelectedValue == null)
            {
                return;
            }
            var gastoSoci = from gs in re.GastoSocialAdmin
                            where gs.idGSA == (int)cmbGSA.SelectedValue
                            select gs;
            foreach (var gsA in gastoSoci)
            {
                gastoC = gsA.clavePresu;
            }
            llenarCuenta();
        }

        private void compActiPOA(object sender, SelectionChangedEventArgs e)
        {
            if (cmbComActi.SelectedValue == null)
            {
                return;
            }
            var comAct = from poa in re.ActividadPoa
                         where poa.idActPoa == (int)cmbComActi.SelectedValue
                         select poa;
            foreach (var y in comAct)
            {
                compoActiviC = y.clavePresu;
            }
            llenarCuenta();
        }

        private void clasificadorProgra(object sender, SelectionChangedEventArgs e)
        {
            if (comboClasifProgramaticoProyecto.SelectedValue == null)
            {
                return;
            }
            var clasifiProgra = from clp in re.ClasificadorProgramatico
                                where clp.idClasificadorPro == (int)comboClasifProgramaticoProyecto.SelectedValue
                                select clp;
            foreach (var z in clasifiProgra)
            {
                clasiPrograC = z.Clave;
            }
            llenarCuenta();
        }

        private void tipoGasto(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipGasto.SelectedValue == null)
            {
                return;
            }
            var query = from x in re.TipoGasto
                        where x.idTG ==(int) cmbTipGasto.SelectedValue
                        select x;
            foreach(var i in query){
                tipoGastoC = i.clavePresupuestal;
            }
            llenarCuenta();            
        }

        private void cuenta_KeyUp(object sender, KeyEventArgs e)
        {
            listaCta.Visibility = Visibility.Visible;
            String partida = txtcuenta.Text.Trim();
            
            var ctas = from ele in re.CuentaEnc
                       where ele.Cuenta.Contains(partida) && ele.Hoja == 'A'
                       orderby ele.Cuenta
                       select ele;
            if (ctas.Count() == 0)
            {
                ctas = from ele in re.CuentaEnc
                       where ele.Nombre.Contains(partida) && ele.Hoja == 'A'
                       orderby ele.Nombre
                       select ele;
            }
            listaPartida.Clear();
            foreach (var el in ctas)
            {
                listaPartida.Add(new Partida { IdCta = el.IdCuenta, Nombre = el.Nombre, Cuenta = el.Cuenta, Padre = el.Padre.Value });

            }
            try
            {
                listaCta.ItemsSource = listaPartida;
                listaCta.DisplayMemberPath = "Nombre";
                listaCta.SelectedValuePath = "IdCta";
            }
            catch (Exception ex) { 
            
            }
        }

        private void listaCta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (listaCta.SelectedValue == null) return;
            int idCta = (int)listaCta.SelectedValue;
            Partida par = listaCta.SelectedItem as Partida;
            nombre.Content = par.Nombre;
            txtcuenta.Text = par.Cuenta;
            int padre = par.Padre;
            listaCta.Visibility = Visibility.Hidden;
        }
    }
}
