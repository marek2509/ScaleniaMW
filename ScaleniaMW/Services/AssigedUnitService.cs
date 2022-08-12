using ScaleniaMW.Entities;
using ScaleniaMW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScaleniaMW.Services
{
    public class AssigedUnitService
    {

        private readonly MainDbContext _dbContext;
        public List<JednRejNDto> JednoskiRejstroweNoweDto = new List<JednRejNDto>();

        public AssigedUnitService(MainDbContext dbContext)
        {

                _dbContext = dbContext;
                var jednostkiNowe = _dbContext.Jednostki_rej_n?.OrderBy(j => j.IJR);
                var jednoskiStare = _dbContext.Jednostki_rej;
                var jednSN = _dbContext.Jednostki_sn;
                var dzialkiN = _dbContext.Dzialki_nowe;



                foreach (var jednostka in jednostkiNowe)
                {

                    var tmpJSN = jednSN.Where(x => x.ID_JEDNN == jednostka.ID_ID).Select(x => x.ID_JEDNS);
                    var tmpJstare = jednoskiStare.Where(x => tmpJSN.Contains(x.ID_ID)).OrderBy(x => x.Obreb.ID).ThenBy(x => x.IJR);
                    var tmpDz = dzialkiN.Where(x => x.RJDR == jednostka.ID_ID).OrderBy(x => x.Obreb.ID).ThenBy(x => x.SIDD);

                    JednoskiRejstroweNoweDto.Add(new JednRejNDto(jednostka, tmpJstare, tmpDz));
                }


            //foreach (var j in JednoskiRejstroweNoweDto)
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("Jednostka NKR: " + j.Ijr);

            //    foreach (var d in j?.Dzialki)
            //    {
            //        Console.Write(", " + d.Idd);
            //    }
            //    foreach (var jprzed in j?.JednostkiPrzed)
            //    {
            //        Console.Write(", " + jprzed.Ijr);
            //    }
            //}


        }

        public List<JednRejNDto> GetJednRejNDto()
        {
            return JednoskiRejstroweNoweDto;
        }

        public int lastSelectedIndexNkr = 0;
        public int lastSelectedIndexDz = 0;
        public int lastSelectedIndexIJR = 0;

        internal void FillUI(WindowPrzypiszRejGr window)
        {
            Console.WriteLine("Fill start");
            var nkrToList = JednoskiRejstroweNoweDto
                .Where(j => j.Dzialki.Any(dz => dz.RjdrPrzed == null) && j.JednostkiPrzed.Any())
                //.Where(j => j.Dzialki.Any(dz => dz.RjdrPrzed == null))
                .Select(x => x.Ijr).ToList();


            window.listBoxNkr.ItemsSource = nkrToList;
            Console.WriteLine("FillUI 1");
            //-------------- selected index nkr 
            var selIdxNkr = window.listBoxNkr.SelectedIndex;
            ControlIndexScope(ref lastSelectedIndexNkr, ref selIdxNkr, window.listBoxNkr);
            window.listBoxNkr.SelectedIndex = selIdxNkr;
            //-------------- selected index nkr 

            var selectedNkr = (int?)window.listBoxNkr?.SelectedValue;
            var wybranaJednostka = JednoskiRejstroweNoweDto.FirstOrDefault(x => x.Ijr == selectedNkr);
            var dzialkiToList = wybranaJednostka?.Dzialki
                .Where(dz => dz.RjdrPrzed == null).Select(x => x.ObrNr + "-" + x.Idd);

            window.listBoxDzialkiNowe.ItemsSource = dzialkiToList;
            window.listBoxNrRej.ItemsSource = wybranaJednostka?.JednostkiPrzed.Select(j => j.ObrNr + "-" + j.Ijr);

            Console.WriteLine("FillUI 4");

            Console.WriteLine(DataToLoadDb().Equals(DBNull.Value));
            for (int i = 0; i < DataToLoadDb().Count(); i++)
            {
                Console.WriteLine(i);
                var item = DataToLoadDb().ElementAt(i);
                Console.Write(item.idJednPrzed);
                Console.Write(", ");
                Console.Write(item.id_parcel);
                Console.Write(", ");
                Console.Write(item.IJR);
                Console.Write(", ");
                Console.Write(item.NKR);
                Console.Write(", ");
                Console.Write(item.Nrdz);
                Console.Write(", ");
                Console.WriteLine();
            }
            Console.WriteLine("fill 4,5");
            window.dgNiedopJednostki.ItemsSource = DataToLoadDb();
            Console.WriteLine("FillUI 5");

            var selIdxDz = window.listBoxDzialkiNowe.SelectedIndex;
            ControlIndexScope(ref lastSelectedIndexDz, ref selIdxDz, window.listBoxDzialkiNowe);
            window.listBoxDzialkiNowe.SelectedIndex = selIdxDz;

            var selIdxIjr = window.listBoxNrRej.SelectedIndex;
            ControlIndexScope(ref lastSelectedIndexIJR, ref selIdxIjr, window.listBoxNrRej);
            window.listBoxDzialkiNowe.SelectedIndex = selIdxDz;

            var numberParcelToAssigne = JednoskiRejstroweNoweDto.Sum(x => x.Dzialki.Where(d => d.RjdrPrzed is null).Count());
            window.labelAllParcelToAssige.Content = numberParcelToAssigne;
            Console.WriteLine("FillUI end");
        }

        void ControlIndexScope(ref int lastSelectedIndex, ref int selIdx, ListBox listbox)
        {
            if (selIdx < 0)
            {


                if (lastSelectedIndex >= 0 && lastSelectedIndex < listbox.Items.Count)
                {
                    selIdx = lastSelectedIndex;
                    return;
                }
                else if (lastSelectedIndex >= listbox.Items.Count)
                {
                    lastSelectedIndex--;
                    selIdx = lastSelectedIndex;
                    return;
                }

                selIdx = 0;
                lastSelectedIndex = selIdx;
                return;
            }

            if (lastSelectedIndex >= listbox.Items.Count)
            {
                lastSelectedIndex--;
                selIdx = lastSelectedIndex;
                return;
            }


            lastSelectedIndex = selIdx;

        }

        public IEnumerable<AssignedUnitInParcelToDatabase> DataToLoadDb()
        {
            //foreach (var item in JednoskiRejstroweNoweDto)
            //{
            //    Console.WriteLine(item.Id_id);
            //    Console.WriteLine(item.Ijr);
            //    Console.WriteLine(item.JednostkiPrzed);
            //    Console.WriteLine(item.ObrNr);
            //    Console.WriteLine(item.ObrNaz ?? "null");

            //    foreach (var d in item.Dzialki)
            //    {
            //    Console.WriteLine(d.Idd ?? "Null");
            //    Console.WriteLine(d.Id_Id);
            //    Console.WriteLine(d.KW ?? "Null");
            //    Console.WriteLine(d.ObrNaz ?? "Null");
            //    Console.WriteLine(d.ObrNr);
            //    Console.WriteLine(d.Pew);
            //    Console.WriteLine(d.Rjdr);
            //    Console.WriteLine(d.RjdrPrzed);
            //    Console.WriteLine(d.Sidd ?? "Null");
            //        Console.WriteLine("\ndz\n");
            //    }

            //    Console.WriteLine();
            //}.Dzialki.Any(dz => dz.RjdrPrzed == null)



            //foreach (var item in tempUnitExist)
            //{
            //    Console.WriteLine(item.Ijr);

            //    foreach (var dz in item.Dzialki)
            //    {
            //        Console.WriteLine(dz.ObrNr + " " + dz.Idd);
            //        Console.WriteLine(dz.Id_Id);
            //    }

            //    foreach (var j in item.JednostkiPrzed)
            //    {
            //        Console.WriteLine(j.ObrNr + " " + j.Ijr);
            //        Console.WriteLine(j.Id_id);
            //    }
            //}

            var toDataGrid = JednoskiRejstroweNoweDto.SelectMany(jn => jn.Dzialki, (jednN, dzialki) => new { jednN, dzialki })
                .Where(w => w.dzialki.RjdrPrzed != null )
                .Select(n => new AssignedUnitInParcelToDatabase
                {
                    NKR = n.jednN.Ijr,
                    Nrdz = n.dzialki.ObrNr + "-" + n.dzialki.Idd,
                    IJR = (n.jednN.JednostkiPrzed.FirstOrDefault(c => c.Id_id == n.dzialki.RjdrPrzed)?.ObrNr + "-" + n.jednN.JednostkiPrzed.FirstOrDefault(c => c.Id_id == n.dzialki.RjdrPrzed)?.Ijr) == "-" ? "Brak JR przed" 
                    : (n.jednN.JednostkiPrzed.FirstOrDefault(c => c.Id_id == n.dzialki.RjdrPrzed)?.ObrNr + "-" + n.jednN.JednostkiPrzed.FirstOrDefault(c => c.Id_id == n.dzialki.RjdrPrzed)?.Ijr),
                    idJednPrzed = n.jednN.JednostkiPrzed.FirstOrDefault(c => c.Id_id == n.dzialki.RjdrPrzed)?.Id_id ?? null,
                    id_parcel = n.dzialki.Id_Id,

                });


            //Console.WriteLine("last " +toDataGrid.LastOrDefault().NKR);
            //foreach (var item in toDataGrid)
            //{
            //    Console.WriteLine($"{item.idJednPrzed,-10} | {item.id_parcel, -10} | {item.IJR, -10} | {item.NKR, -10} | {item.Nrdz, -10}|");
            //}

            //Console.WriteLine("Koniec");
            return toDataGrid;
        }

        internal void AssignedSelected(WindowPrzypiszRejGr windowPrzypiszRejGr)
        {
            //FillUI(windowPrzypiszRejGr);

            var selectedNKR = windowPrzypiszRejGr.listBoxNkr.SelectedValue;
            if (selectedNKR == null) return;

            var selectedIjrPrzed = windowPrzypiszRejGr.listBoxNrRej.SelectedValue;
            if (selectedIjrPrzed == null) return;

            //var selectedParcel = windowPrzypiszRejGr.listBoxDzialkiNowe.SelectedValue.ToString();

            var nkrToEdit = JednoskiRejstroweNoweDto.FirstOrDefault(n => n.Ijr == (int)selectedNKR);
            var rjdrPrzed = nkrToEdit.JednostkiPrzed.FirstOrDefault(j => (j.ObrNr + "-" + j.Ijr) == selectedIjrPrzed.ToString()).Id_id;

            var multiSelectedParcel = windowPrzypiszRejGr.listBoxDzialkiNowe.SelectedItems;
            foreach (var parcel in multiSelectedParcel)
            {
                nkrToEdit.Dzialki.FirstOrDefault(d => (d.ObrNr + "-" + d.Idd) == parcel.ToString()).RjdrPrzed = rjdrPrzed;
            }




            lastSelectedIndexNkr = windowPrzypiszRejGr.listBoxNkr.SelectedIndex;
            lastSelectedIndexDz = windowPrzypiszRejGr.listBoxDzialkiNowe.SelectedIndex;
            lastSelectedIndexIJR = windowPrzypiszRejGr.listBoxNrRej.SelectedIndex;

            FillUI(windowPrzypiszRejGr);
        }

        internal void AutoAssignment()
        {
            var parcelWithOnlyOneIjrBefore = JednoskiRejstroweNoweDto.Where(nkr => nkr.JednostkiPrzed.Count == 1);

            foreach (var unit in parcelWithOnlyOneIjrBefore)
            {
                foreach (var parcel in unit.Dzialki)
                {
                    //var parcelDb = _dbContext.Dzialki_nowe.FirstOrDefault(x => x.ID_ID == parcel.Id_Id);

                    var idUnitBefore = unit.JednostkiPrzed.FirstOrDefault().Id_id;

                    parcel.RjdrPrzed = idUnitBefore;
                }
            }

        }


    }
}
