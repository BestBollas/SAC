using SacIntegrado.Presupuesto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
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
    /// <summary>
    /// Lógica de interacción para Recursos.xaml
    /// </summary>
    /// 

    public partial class Recursos : Page
    {

      
        public int idEmpleadoModificar = 0;
        public int idRecurso = 0;     
        public int idEmple = 0;
        llenarAnio a = new llenarAnio();
        Db con = new Db();
        private ObservableCollection<RecursoC> recursosTabla = new ObservableCollection<RecursoC>();
        private ObservableCollection<AnioAplicaC> aniosAplica = new ObservableCollection<AnioAplicaC>();

        public Recursos(String user, int id, String nombre)
        {
            InitializeComponent();
            MenuPrincipal temp = new MenuPrincipal(this, user, id, nombre);
            temp.crearMenu();
            idEmple = id;           
            menuPrincipal.Items.Clear();
            menuPrincipal.Items.Add(temp.MiMenu);
            ConsultaRecurso();
        }


        public void ConsultaRecurso()
        {

               var recursos = (from r in con.Recurso
               from  e in con.Empleado
               where e.idEmpleado==r.idEmpleado
               select new
               {
                   r.idRecurso,r.Nombre,r.Observaciones,r.SaldoFin,r.SaldoInicial,r.Vigente,r.FechaRegistro,r.AnioAplica,r.ClavePresupuestal,
                   e.idEmpleado,nombreEmpleado=e.Nombre
               });
               recursosTabla.Clear();
              foreach (var e in recursos)
              {
                  recursosTabla.Add(new RecursoC { idRecurso = e.idRecurso, ClavePresupuestal=e.ClavePresupuestal.ToString() ,Nombre = e.Nombre, FechaRegistro = e.FechaRegistro.Value, AnioAplica = e.AnioAplica.Value, idEmpleadó = e.idEmpleado, NombreEmpleado = e.nombreEmpleado, Vigente = e.Vigente.Value, SaldoInicial = e.SaldoInicial.Value, SaldoFinal = e.SaldoFin.Value, Observaciones = e.Observaciones });
              }
              tablaRecursos.ItemsSource = recursosTabla;
              cargarAnio();
            

        }

        public void eliminaRecurso(object sender, RoutedEventArgs e)
        {
            if (idRecurso != 0)
            {
                try
                {

                    if (MessageBox.Show("Seguro que deseas eliminar el registro", "Peligro", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {


                        RecursoC miRenglon = tablaRecursos.SelectedItem as RecursoC;

                        Recurso prueba = (from r in con.Recurso
                                          where r.idRecurso == miRenglon.idRecurso
                                          select r).Single();
                        con.Recurso.DeleteOnSubmit(prueba);
                        con.SubmitChanges();
                        tablaRecursos.Items.Refresh();
                        MessageBox.Show("El registro se elimino correctamente.");
                        ConsultaRecurso();
                        limpiarRecurso();
                    }
                }

                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                    return;
                }

            }
            else {

                MessageBox.Show("Seleciones un registro");
            }
            
        }

        private void nuevoRecurso(object sender, RoutedEventArgs e)
        {

            if (idRecurso == 0)
            {

                Table<Recurso> pu = con.GetTable<Recurso>();
                Recurso tb = new Recurso();

                tb.Nombre = nombre.Text;
                tb.ClavePresupuestal = clave.Text;
                tb.AnioAplica = int.Parse(anios.Text);
                tb.FechaRegistro = DateTime.Today;
                tb.idEmpleado = idEmple;
                tb.Observaciones = observaciones.Text;
                tb.SaldoFin = int.Parse(final.Text);
                tb.SaldoInicial = int.Parse(inicial.Text);
                tb.Vigente = vigente.IsChecked;


                pu.InsertOnSubmit(tb);
                pu.Context.SubmitChanges();
                limpiarRecurso();
                tablaRecursos.Items.Refresh();
                ConsultaRecurso();
                MessageBox.Show("El registro se agrego correctamente.");
            }
            else {

               
                int m = idRecurso;
                Recurso tb= (from p in con.Recurso
                               where p.idRecurso == m
                               select p).SingleOrDefault();

                tb.Nombre = nombre.Text;
                tb.ClavePresupuestal = clave.Text;
                tb.AnioAplica = int.Parse(anios.Text);
                tb.FechaRegistro = DateTime.Today;
                tb.idEmpleado = idEmpleadoModificar;
                tb.Observaciones = observaciones.Text;
                tb.SaldoFin = int.Parse(final.Text);
                tb.SaldoInicial = int.Parse(inicial.Text);
                tb.Vigente = vigente.IsChecked;
                con.SubmitChanges();
                ConsultaRecurso();
                limpiarRecurso();
                MessageBox.Show("El registro se modifico correctamente.");

            
            }
        }

       

        public void cargarAnio() {

           
            a.llenarAnios(anios);

            //RecursoC rer = new RecursoC();
            //Proyectos pro=new Proyectos();

            //foreach (var a in rer.AnioList)
            //{
            //    aniosAplica.Add(new AnioAplicaC { anioAplica = a });
            //}

            //anios.DisplayMemberPath = "anioAplica";
            //anios.SelectedValuePath = "anioAplica";
            //anios.SelectedIndex=pro.indexAnioAplica();
            //anios.ItemsSource = aniosAplica;
        
        }

       
        public void limpiarRecurso()
        {
            clave.Text = "";
            nombre.Text = "";
            a.llenarAnios(anios);
            vigente.IsChecked = false;
            observaciones.Text = "";
            inicial.Text = "";
            final.Text = "";
            idRecurso = 0;
            idEmpleadoModificar=0;

        }

        private void tablaRecursos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecursoC rec = tablaRecursos.SelectedItem as RecursoC;

            if (rec != null)
            {

                nombre.Text = rec.Nombre;
                clave.Text = rec.ClavePresupuestal + "";
                inicial.Text = rec.SaldoInicial + "";
                final.Text = rec.SaldoFinal + "";
                observaciones.Text = rec.Observaciones;
                int anio = rec.AnioAplica;
                int indexA = 0;

                foreach (var es in aniosAplica)
                {
                    if (es.anio.Equals(anio))
                    {
                        break;
                    }

                    indexA++;

                }

                anios.SelectedIndex = indexA;
                vigente.IsChecked = rec.Vigente;
                idEmpleadoModificar = rec.idEmpleadó;
                idRecurso = rec.idRecurso;

            }
            else {

                limpiarRecurso();

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            limpiarRecurso();
        }

    }
}
