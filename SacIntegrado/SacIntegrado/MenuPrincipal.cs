using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using SacIntegrado.Adquisiciones;
using SacIntegrado.Tesoreria;
using SacIntegrado.Presupuesto;
using SacIntegrado.OficiosComision;




namespace SacIntegrado
{
    public class opMenu
    {
        public int id{get; set;}
        public String nombre{ get; set;}
        public String variable{get; set;}
        public int papa {get; set;}
    }
    public class MenuPrincipal
    {
        static Db con = new Db();
        Page pagina;
        String usuario;
        int id;
        String nombre;
        public static ObservableCollection<opMenu> datosMenu=new ObservableCollection<opMenu>();
        Menu miMenu = new Menu();

        public Menu MiMenu
        {
            get
            {
                return miMenu;
            }
            set { miMenu = value; }
        }

        public MenuPrincipal(Page pagina,String usuario,int id,String nombre)
        {
            this.pagina = pagina;
            this.usuario=usuario;
            this.id = id;
            this.nombre = nombre;
        }

        public static void llenaDatos(int empleado)
        {
            var menu = from um in con.UsuarioMenu
                       from m in con.MenuTabla
                       where um.idMenu == m.idMenu && um.idEmpleado == empleado
                       orderby m.Papa
                       select m;
            foreach (var ele in menu)
            {
                datosMenu.Add(new opMenu{id=ele.idMenu,nombre=ele.Nombre,variable=ele.variable,papa=ele.Papa.Value});
            }
        }

        public void crearMenu()
        {
            var menu = from um in datosMenu
                       orderby um.papa
                       select um;
            int pos = 0;
            foreach (var ele in menu)
            {
                MenuItem opMenu = new MenuItem();
                opMenu.Header = ele.nombre.Trim();
                opMenu.Name = ele.variable.Trim();
                if (ele.papa == 0)
                {
                    miMenu.Items.Add(opMenu);
                    opMenu.Click += new RoutedEventHandler(opMenu_Click);
                    pos++;
                }
                agregaHijos(opMenu, ele.id);
            }
        }
        

        public void agregaHijos(MenuItem opMenu, int papa)
        {
            var menu = from m in datosMenu
                       where m.papa == papa
                       select m;
            foreach (var ele in menu)
            {
                MenuItem nvoMenu = new MenuItem();
                nvoMenu.Header = ele.nombre.Trim();
                nvoMenu.Name = ele.variable.Trim();
                nvoMenu.Click += new RoutedEventHandler(opMenu_Click);
                opMenu.Items.Add(nvoMenu);
                agregaHijos(nvoMenu, ele.id);
            }
        }

        public void opMenu_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = e.Source as FrameworkElement;
            switch (fe.Name)
            {
                case "mRequiNueva":
                    ProcesoRequi rq = new ProcesoRequi(usuario,id,nombre);
                    pagina.NavigationService.Navigate(rq);
                    break;
                case "mPoliza":
                    Poliza poliza = new Poliza(usuario,id,nombre);
                    pagina.NavigationService.Navigate(poliza);
                    break;
                case "mCuentas":
                    Conta_Presu cp = new Conta_Presu(usuario,id,nombre);
                    pagina.NavigationService.Navigate(cp);
                    break;
                case "mAutorizaRequi":
                    Autorizacion autoriza = new Autorizacion(usuario,id,nombre);
                    pagina.NavigationService.Navigate(autoriza);
                    break;
                case "mUsuarios":
                    Agregar usu = new Agregar(usuario, id, nombre);
                    pagina.NavigationService.Navigate(usu);
                    break;
                case "mProyectoPresup":
                    Proyectos proy = new Proyectos(usuario, id, nombre);
                    pagina.NavigationService.Navigate(proy);
                    break;
                case "mRecursos":
                    Recursos rec = new Recursos(usuario, id, nombre);
                    pagina.NavigationService.Navigate(rec);
                    break;
                case "mClasificador":
                    ClaFun cla = new ClaFun(usuario,id,nombre);
                    pagina.NavigationService.Navigate(cla);
                    break;
                case "mFuncional":
                    ClasifiProgra pro = new ClasifiProgra(usuario,id,nombre);
                    pagina.NavigationService.Navigate(pro);
                    break;
                 case "mVoBoGmOc":
                    VoBoJC vobojc = new VoBoJC(usuario, id, nombre);
                    pagina.NavigationService.Navigate(vobojc);
                    break;
                    //EDICIÓN FOZZIE
                 case "mCotiza":
                    cotizacion cot = new cotizacion(usuario, id, nombre);
                    pagina.NavigationService.Navigate(cot);
                    break;
                 case "mVoBoComite":
                    VoBoEnvCo voboenvco = new VoBoEnvCo(usuario, id, nombre);
                    pagina.NavigationService.Navigate(voboenvco);
                    break;
                 case "mVoBoPresup":
                    VoBoPresu vobopresu = new VoBoPresu(usuario, id, nombre);
                    pagina.NavigationService.Navigate(vobopresu);
                    break;
                    //EDICION FOZZ 1 ENERO
                 case "mNvoRecibo":
                    Ventanilla vtl = new Ventanilla(usuario, id, nombre);
                    pagina.NavigationService.Navigate(vtl);
                    break;
                 case "mCatClientes":
                    CatalogoCliente ccl = new CatalogoCliente(usuario, id, nombre);
                    pagina.NavigationService.Navigate(ccl);
                    break;
                 case "mCatServicios":
                    CatalogoServicios csr = new CatalogoServicios(usuario, id, nombre);
                    pagina.NavigationService.Navigate(csr);
                    break;
                 case "mVentanillaConsulta":
                    ConsultaTes vcn = new ConsultaTes(usuario, id, nombre);
                    pagina.NavigationService.Navigate(vcn);
                    break;
                 case "mBalanza":
                    ReporteBal repBal = new ReporteBal(usuario, id, nombre);
                    pagina.NavigationService.Navigate(repBal);
                    break;
                case "mppresupuesto":
                    pre_presupuesto ppres = new pre_presupuesto(usuario, id, nombre);
                    pagina.NavigationService.Navigate(ppres);
                    break;
                    ////EDER-------------------------------------------------------
                case "maltaArea":
                altaClavePresuArea altclave = new altaClavePresuArea(usuario, id, nombre);
                pagina.NavigationService.Navigate(altclave);
                break;
                case "mcompoActivi":
                    altaCompoActivi altCompAct = new altaCompoActivi(usuario, id, nombre);
                    pagina.NavigationService.Navigate(altCompAct);
                break;
                case"mtipoGasto":
                    altaTipoGasto tipoGas = new altaTipoGasto(usuario, id, nombre);
                    pagina.NavigationService.Navigate(tipoGas);
                break;
                case "mfuentesFina":
                    FuentesFina fuentesFina = new FuentesFina(usuario, id, nombre);
                    pagina.NavigationService.Navigate(fuentesFina);
                break;
                case"mgastoSocial":
                    gastoSocialAdminClass gsa = new gastoSocialAdminClass(usuario, id, nombre);
                    pagina.NavigationService.Navigate(gsa);
                break;
                case"mPoa":
                    ActiviPOA poa = new ActiviPOA(usuario, id, nombre);
                    pagina.NavigationService.Navigate(poa);
                break;

                case "mSolicitudOficio":
                oficiosdeComision ofi1 = new oficiosdeComision(usuario, id, nombre,1);
                pagina.NavigationService.Navigate(ofi1);
                break;
                case"mVoBoPresu":
                AsignaPresupOficioComi asiPreOfi = new AsignaPresupOficioComi(usuario, id, nombre);
                pagina.NavigationService.Navigate(asiPreOfi);
                break;
                case "mVoBoJefeInme":
                oficiosdeComision ofi3 = new oficiosdeComision(usuario, id, nombre,3);
                pagina.NavigationService.Navigate(ofi3);
                break;
                case "mVoBoJefeArea":
                oficiosdeComision ofi4 = new oficiosdeComision(usuario, id, nombre,4);
                pagina.NavigationService.Navigate(ofi4);
                break;
                case "mVoBoRespProy":
                oficiosdeComision ofi5 = new oficiosdeComision(usuario, id, nombre,5);
                pagina.NavigationService.Navigate(ofi5);
                break;
            }
            e.Handled = true;
        }
        
    }
}
