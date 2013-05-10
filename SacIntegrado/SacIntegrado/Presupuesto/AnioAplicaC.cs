using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SacIntegrado.Presupuesto
{
    class AnioAplicaC
    {
        public int anio {get;set; }
        public int idPeriodo {get; set; }

    }

    class llenarAnio {

        private ObservableCollection<AnioAplicaC> aniosAplica = new ObservableCollection<AnioAplicaC>();
        Db re = new Db();
        public void llenarAnios(ComboBox combo)
        {
        //    var anioList = (from p in re.Periodo
        //                    where p.status == 'A' || p.status == 'P'
        //                    select p).Distinct();



           foreach (var a in AnioList)
            {
                aniosAplica.Add(new AnioAplicaC { anio = a});
            }

           combo.ItemsSource = aniosAplica;
           combo.DisplayMemberPath = "anio";
           combo.SelectedValuePath = "anio";
           combo.SelectedIndex = indexAnioAplica();
        
        }

        public int indexAnioAplica()
        {
            var pa = from p in re.Parametros
                     select p.idPeriodo;
            int id = 0;

            foreach(var e in pa){
                id = e.Value;
            }
           
            int indexA = 0;

            foreach (var a in aniosAplica)
            {
                if (a.idPeriodo==id)
                {
                    break;
                }
                indexA++;
            }
            return indexA;
        }

        public List<int> AnioList
        {

            get
            {

                int anioC = DateTime.Today.Year;
                int anioS = DateTime.Today.Year + 1;
                int anioA = DateTime.Today.Year - 1;
                return new List<int> { anioA, anioC, anioS };
            }


        }
         
    
    }
}
