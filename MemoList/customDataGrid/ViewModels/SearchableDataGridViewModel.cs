using Apex.MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace BoostDraft.UI.ViewModels
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
            RowClickCommand = new Command(obj => {
                var row = obj as System.Windows.Controls.DataGridRow;
                var item = row.Item as Entity;
                if (item.OnClickAction != null) item.OnClickAction();
            }, true);

            EmptyCollectionCommand = new Command(EmptyCollection);

            Populate();
        }

        private void Populate()
        {
            var entities = new ObservableCollection<Entity>();

            for (int i = 0; i < 1000; i++)
            {
                entities.Add(new Entity
                {
                    Id = i,
                    Content = "Test content Test content Test content Test content Test content Test content Test content Test content " + i,
                    Date = DateTime.Now.AddDays(i)
                });
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Where { get; set; }
        public DateTime Date { get; set; }
        
        public bool Selected { get; set; }
        public Action OnClickAction { get; set; } = null;
    }
}