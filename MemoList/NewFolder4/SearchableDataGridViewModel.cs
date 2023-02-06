using Apex.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using MemoForms.DBAccess;
using MemoForms.ViewModel;
using MemoForms.Models;
using MemoForms;

namespace MemoList.UI.ViewModels
{
    public class SearchableDataGridViewModel : ViewModelBase
    {
        private CollectionViewSource entities;
        public CollectionViewSource Entities
        {
            get { return entities; }
            set
            {
                entities = value;
                OnPropertyChanged();
            }
        }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged();
            }
        }

        private bool canUserFilter;
        public bool CanUserFilter
        {
            get { return canUserFilter; }
            set
            {
                canUserFilter = value;
                OnPropertyChanged();
            }
        }

        private bool filterIsEnabled;
        public bool FilterIsEnabled
        {
            get { return filterIsEnabled; }
            set
            {
                filterIsEnabled = value;
                OnPropertyChanged();
            }
        }

        public ICommand EmptyCollectionCommand { get; set; }
        public ICommand RowClickCommand { get; set; }

        public SearchableDataGridViewModel()
        {
            CanUserFilter = true;
            FilterIsEnabled = true;
            RowClickCommand = new Command(obj =>
            {
                var row = obj as System.Windows.Controls.DataGridRow;
                var item = row.Item as Entity;
                if (item.OnClickAction != null) item.OnClickAction();
                else item.OnClickAction = delegate () { OpenMemoDialog(item.Id, item.Content, item.MemoText); };
            }, true);

            EmptyCollectionCommand = new Command(EmptyCollection);

            Populate();
        }

        private void OpenMemoDialog(int id, string content, string memoText)
        {
            // Launches the WPF memo saving window
            var memoWindowApp = new MainWindow();
            memoWindowApp.DataContext = new MemoViewModel();
            ((MemoViewModel)memoWindowApp.DataContext).TxtBoxContent = content;
            ((MemoViewModel)memoWindowApp.DataContext).TxtBoxFile_id = id;
            ((MemoViewModel)memoWindowApp.DataContext).TxtBoxMemoText = memoText;
            memoWindowApp.Show();
        }

        private void Populate()
        {
            var entities = new ObservableCollection<Entity>();
            var DBAccess = new MemoDBAccess();

            try
            {
                List<MemoData> lstMemoData = DBAccess.SelectMemo();

                for (int i = 0; i < lstMemoData.Count; i++)
                {
                    entities.Add(new Entity
                    {
                        Id = lstMemoData[i].File_id,
                        Content = lstMemoData[i].Content,
                        MemoText = lstMemoData[i].Memo,
                        FileName = "XYZ",
                        Date = DateTime.Now.AddDays(i)
                    }); ;
                }
            }
            catch (Exception)
            {
                // Do any required exception handling
            }

            Entities = new CollectionViewSource();
            Entities.Source = entities;
        }

        private void EmptyCollection()
        {
            if (Entities.View.Cast<Entity>().Count() == 0) { }

            //                Populate();
            else
                Entities = new CollectionViewSource() { Source = new List<Entity>() };
        }

        public void SetEntities(List<Entity> entities)
        {
            Entities = new CollectionViewSource();
            Entities.Source = entities;
        }
    }

    public class Entity
    {
        public string FileName { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Where { get; set; }
        public DateTime Date { get; set; }
        public string MemoText { get; set; }

        public bool Selected { get; set; }
        public Action OnClickAction { get; set; } = null;
    }
}