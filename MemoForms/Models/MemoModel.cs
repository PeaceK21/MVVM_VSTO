using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MemoForms.Models
{
    /// <summary>
    /// Model class to store memo data
    /// </summary>
    public class MemoData : INotifyPropertyChanged
    {
        private int _File_id;

        /// <summary>
        /// Property to store id of file
        /// </summary>
        public int File_id
        {
            get { return _File_id; }
            set { _File_id = value; OnPropertyChanged("File_id"); }
        }

        private string _Section_No;

        /// <summary>
        /// Property to store section number of document
        /// </summary>
        public string Section_No
        {
            get { return _Section_No; }
            set { _Section_No = value; OnPropertyChanged("Section_No"); }
        }

        private string _Paragraph_Text;

        /// <summary>
        /// Property to store text of paragraph from which content is selected
        /// </summary>
        public string Paragraph_Text
        {
            get { return _Paragraph_Text; }
            set { _Paragraph_Text = value; OnPropertyChanged("Paragraph_Text"); }
        }

        private string _OriginalSelectedText;

        /// <summary>
        /// The original text from document which is selected as memo content
        /// </summary>
        public string Original_Text
        {
            get { return _OriginalSelectedText; }
            set { _OriginalSelectedText = value; OnPropertyChanged("Original_Text"); }
        }

        private string _Content;

        /// <summary>
        /// Initially it is selected from the word document, but could be modified to store as memo content
        /// </summary>
        public string Content
        {
            get { return _Content; }
            set { _Content = value; OnPropertyChanged("Content"); }
        }

        private string _MemoText;

        /// <summary>
        /// Short description(Memo) about content selected and stored from the document 
        /// </summary>
        public string Memo
        {
            get { return _MemoText; }
            set { _MemoText = value; OnPropertyChanged("Memo"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
