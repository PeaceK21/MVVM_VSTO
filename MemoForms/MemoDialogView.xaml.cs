using MemoForms.DBAccess;
using MemoForms.Models;
using MemoForms.ViewModel;
using System;
using System.Windows;

namespace MemoForms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public bool SaveMemoOnLoad()
        {
            bool isMemoInserted = false;

            if (this.DataContext != null)
            {
                Random random = new Random();

                MemoDBAccess objDBAcess = new MemoDBAccess();
                MemoData obj = new MemoData
                {

                    File_id = ((MemoViewModel)this.DataContext).TxtBoxFile_id,

                    //Section_No = ((MemoViewModel)this.DataContext).SectionNumber,
                    Section_No = random.Next(1000).ToString(),

                    Original_Text = ((MemoViewModel)this.DataContext).OriginalSelectedText,
                    Paragraph_Text = ((MemoViewModel)this.DataContext).ParagraphText,
                    Content = ((MemoViewModel)this.DataContext).TxtBoxContent,
                    Memo = ((MemoViewModel)this.DataContext).TxtBoxMemoText
                };

                isMemoInserted = objDBAcess.InsertMemo(obj);

                // You can remove this message boxes and use it how you like.
                if (isMemoInserted == true)
                {
                    MessageBox.Show("Memo data inserted successfully.", "Insert Result", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show("Memo data could not be inserted.", "Insert Result", MessageBoxButton.OK);
                }

            }
         
            return isMemoInserted;
        }
    }
}