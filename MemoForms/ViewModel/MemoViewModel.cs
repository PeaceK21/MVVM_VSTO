using Apex.MVVM;
using MemoForms.DBAccess;
using MemoForms.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace MemoForms.ViewModel
{
    public class MemoViewModel : INotifyPropertyChanged
    {
        private MemoData _memo;
        public int TxtBoxFile_id
        {
            get { return _memo.File_id; }
            set { _memo.File_id = value; OnPropertyChanged(); }
        }

        public string TxtBoxContent
        {
            get { return _memo.Content; }
            set { _memo.Content = value; OnPropertyChanged(); }
        }

        public string TxtBoxMemoText
        {
            get { return _memo.Memo; }
            set { _memo.Memo = value; OnPropertyChanged(); }
        }

        public string SectionNumber
        {
            get { return _memo.Section_No; }
            set { _memo.Section_No = value; OnPropertyChanged(); }
        }

        public string ParagraphText
        {
            get { return _memo.Paragraph_Text; }
            set { _memo.Paragraph_Text = value; OnPropertyChanged(); }
        }

        public string OriginalSelectedText
        {
            get { return _memo.Original_Text; }
            set { _memo.Original_Text = value; OnPropertyChanged(); }
        }

        public ICommand CloseCommand { get; set; }

        public ICommand SaveMemoCommand { get; set; }

        public MemoViewModel()
        {
            _memo = new MemoData();

            CloseCommand = new Command(obj => ((Window)obj).Close());
            SaveMemoCommand = new Command(SaveMemo);
        }

        private void SaveMemo(object windowObj)
        {
            MemoDBAccess objDBAcess = new MemoDBAccess();
            MemoData obj = new MemoData
            {
                File_id = this.TxtBoxFile_id,
                Section_No = this.SectionNumber,
                Original_Text = this.OriginalSelectedText,
                Paragraph_Text = this.ParagraphText,
                Content = this.TxtBoxContent,
                Memo = this.TxtBoxMemoText
            };

            bool isMemoInserted = objDBAcess.UpdateMemo(obj);

            if (isMemoInserted == true)
            {
                MessageBox.Show("Memo data updated successfully.", "Insert Result", MessageBoxButton.OK);
                CloseCommand.Execute(windowObj);
            }
            else
            {
                MessageBox.Show("Memo data cannot be updated successfully.", "Insert Result", MessageBoxButton.OK);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
