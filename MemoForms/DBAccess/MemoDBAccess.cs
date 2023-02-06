using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using Dapper;
using MemoForms.Models;
using MemoForms.ViewModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BoostDraftAlpha.CollFiltering.Controllers;
using System.Windows;
using System.Reflection;
using System.IO;

namespace MemoForms.DBAccess
{
    public class MemoDBAccess
    {
        // Update DB path to test with Test DB...

        // NOTE : You can use bootstrap config's DB path and replace this code.
        private string _DBPath = string.Empty;

        /*
        /// <summary>
        /// Can use this constructor to assign DB path dynamically
        /// </summary>
        /// <param name="sDBPath"> DB pass to be set </param>
        public MemoDBAccess(string sDBPath)
        {
            _DBPath = sDBPath;
        }
        */

        /// <summary>
        /// Constructor to check if DB exists or not
        /// </summary>
        public MemoDBAccess()
        {
            string currentWorkingDir = Directory.GetCurrentDirectory();
            string directory = Directory.GetParent(Directory.GetParent(Directory.GetParent(currentWorkingDir).FullName).FullName).FullName;
            _DBPath = Path.Combine(directory,"Database","MemoDB.db");
            string databaseSourcePath = string.Format("Data Source={0}", _DBPath);

            try
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var query = "SELECT name FROM sqlite_master WHERE type='table' AND name='MemoData';";
                    var tableExists = connection.ExecuteScalar(query);

                    // The query returns null when it cannot find the table
                    if (tableExists == null)
                    {
                        CreateMemoDataTable(databaseSourcePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            finally
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Create table if not already exists in the database
        /// </summary>
        /// <param name="sDatabaseSourcePath"> Database path </param>
        /// <returns>
        /// True : If table created successfully
        /// False: If any exception occurred while creating table
        /// </returns>
        private bool CreateMemoDataTable(string sDatabaseSourcePath)
        {
            string databaseSourcePath = string.Format("Data Source={0}", _DBPath);

            try
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    // Checking connection
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    // Query to create new MemoData table if it does not exists already
                    string sQuery = @"create table IF NOT EXISTS MemoData
                      (
                         File_ID                             integer primary key, 
                         Section_No                          nvarchar not null,
                         Paragraph_Text                      nvarchar not null,
                         Original_Text                       nvarchar not null,
                         Content                             nvarchar not null,
                         Memo                                nvarchar
                      )";

                    // Added query for Memo table
                    connection.Execute(sQuery);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return false;
            }
            finally
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public bool InsertMemo(MemoData objMemoModel)
        {
            if (ValidateMemoData(objMemoModel) == false)
            {
                return false;
            }

            string databaseSourcePath = string.Format("Data Source={0}", _DBPath);

            try
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var query = new StringBuilder();
                    query.Append("INSERT INTO MemoData (File_ID, Section_No, Paragraph_Text, Original_Text, Content, Memo) VALUES (@File_ID,@Section_No,@Paragraph_Text,@Original_Text,@Content,@Memo)");

                    int rowInserted = connection.Execute(query.ToString(),
                        new
                        {
                            File_ID = objMemoModel.File_id,
                            Section_No = objMemoModel.Section_No,
                            Paragraph_Text = objMemoModel.Paragraph_Text,
                            Original_Text = objMemoModel.Original_Text,
                            Content = objMemoModel.Content,
                            Memo = objMemoModel.Memo
                        });

                    //var command = new SQLiteCommand(query.ToString());
                    //command.Parameters.Add(new SQLiteParameter("@File_ID", objMemoModel.File_id));
                    //command.Parameters.Add(new SQLiteParameter("@Section_No", objMemoModel.SectionNumber));
                    //command.Parameters.Add(new SQLiteParameter("@Paragraph_Text", objMemoModel.Paragraphtext));
                    //command.Parameters.Add(new SQLiteParameter("@Original_Text", objMemoModel.OriginalSelectedText));
                    //command.Parameters.Add(new SQLiteParameter("@Content", objMemoModel.Content));
                    //command.Parameters.Add(new SQLiteParameter("@Memo", objMemoModel.MemoText));
                    //command.ExecuteNonQuery();

                    Debug.Write(rowInserted);

                    if (rowInserted == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return false;
            }
            finally
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public List<MemoData> SelectMemo()
        {
            string databaseSourcePath = string.Format("Data Source={0}", _DBPath);
            List<MemoData> lstReadMemo = new List<MemoData>();

            try
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var query = new StringBuilder();
                    query.Append("SELECT * FROM MemoData");

                    //SQLiteCommand command = connection.CreateCommand();
                    ////SQLiteCommand command = new SQLiteCommand(query.ToString());
                    //command.CommandText = query.ToString();
                    //command.CommandType = CommandType.Text;
                    //SQLiteDataReader sqlReader = command.ExecuteReader();

                    lstReadMemo = connection.Query<MemoData>(query.ToString()).ToList();

                    //while (sqlReader.Read())
                    //{
                    //    MemoViewModel objMemo = new MemoViewModel();
                    //    objMemo.TxtBoxFile_id = (int)sqlReader["File_ID"];
                    //    objMemo.SectionNumber = (string)sqlReader["Section_No"];
                    //    objMemo.ParagraphText = (string)sqlReader["Paragraph_Text"];
                    //    objMemo.OriginalSelectedText = (string)sqlReader["Original_Text"];
                    //    objMemo.TxtBoxContent= (string)sqlReader["Content"];
                    //    objMemo.TxtBoxMemoText = (string)sqlReader["Memo"];

                    //    lstReadMemo.Add(objMemo);
                    //}
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return null;
            }
            finally
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }

            return lstReadMemo;
        }

        public bool UpdateMemo(MemoData objMemoModel)
        {
            string databaseSourcePath = string.Format("Data Source={0}", _DBPath);

            try
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var selectQuery = new StringBuilder();
                    selectQuery.Append("SELECT * FROM MemoData WHERE File_id = " + objMemoModel.File_id);
                    MemoData objMemoData = connection.Query<MemoData>(selectQuery.ToString()).First();

                    if (string.IsNullOrEmpty(objMemoModel.Content) == true)
                    {
                        objMemoModel.Content = objMemoData.Content;
                    }
                    if (string.IsNullOrEmpty(objMemoModel.Memo) == true)
                    {
                        objMemoModel.Memo = objMemoData.Memo;
                    }
                    if (string.IsNullOrEmpty(objMemoModel.Original_Text) == true)
                    {
                        objMemoModel.Original_Text = objMemoData.Original_Text;
                    }
                    if (string.IsNullOrEmpty(objMemoModel.Paragraph_Text) == true)
                    {
                        objMemoModel.Paragraph_Text = objMemoData.Paragraph_Text;
                    }
                    if (string.IsNullOrEmpty(objMemoModel.Section_No) == true)
                    {
                        objMemoModel.Section_No = objMemoData.Section_No;
                    }

                    var query = new StringBuilder();
                    query.Append("UPDATE MemoData SET Section_No = @Section_No, Paragraph_Text = @Paragraph_Text, Original_Text = @Original_Text, Content = @Content, Memo = @Memo WHERE File_ID = @File_ID");

                    connection.Query<MemoData>(query.ToString(),
                       new
                       {
                           File_ID = objMemoModel.File_id,
                           Section_No = objMemoModel.Section_No,
                           Paragraph_Text = objMemoModel.Paragraph_Text,
                           Original_Text = objMemoModel.Original_Text,
                           Content = objMemoModel.Content,
                           Memo = objMemoModel.Memo
                       });

                    //var command = new SQLiteCommand(query.ToString());
                    //command.Parameters.Add(new SQLiteParameter("@File_ID", objMemoModel.TxtBoxFile_id));
                    //command.Parameters.Add(new SQLiteParameter("@Section_No", objMemoModel.SectionNumber));
                    //command.Parameters.Add(new SQLiteParameter("@Paragraph_Text", objMemoModel.ParagraphText));
                    //command.Parameters.Add(new SQLiteParameter("@Original_Text", objMemoModel.OriginalSelectedText));
                    //command.Parameters.Add(new SQLiteParameter("@Content", objMemoModel.TxtBoxContent));
                    //command.Parameters.Add(new SQLiteParameter("@Memo", objMemoModel.TxtBoxMemoText));
                    //command.ExecuteNonQuery();
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return false;
            }
            finally
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public void DeleteMemo(MemoData objMemo)
        {
            if (ValidateMemoData(objMemo) == false)
            {
                return;
            }

            string databaseSourcePath = string.Format("Data Source={0}", _DBPath);

            try
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    var query = new StringBuilder();
                    query.Append("DELETE MemoData WHERE File_ID = @File_ID");
                    connection.Query<MemoData>(query.ToString(), new { File_ID = objMemo.File_id });

                    //var command = new SQLiteCommand(query.ToString());
                    //command.Parameters.Add(new SQLiteParameter("@File_ID", objMemo.TxtBoxFile_id));
                    //command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            finally
            {
                using (var connection = new SQLiteConnection(databaseSourcePath))
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Function to validate MemoData before adding into DB
        /// </summary>
        /// <param name="objMemo"> Memo Data to be validated </param>
        /// <returns></returns>
        public bool ValidateMemoData(MemoData objMemo)
        {
            bool isValidMemo = true;

            if (string.IsNullOrEmpty(objMemo.Section_No) == true)
            {
                MessageBox.Show("Section No must not be null or empty.", "Invalid Memo Data", MessageBoxButton.OK);
                Debug.Write("Section number cannot be null.");
                isValidMemo = false;
            }
            else if (string.IsNullOrEmpty(objMemo.Paragraph_Text) == true)
            {
                MessageBox.Show("Paragraph text must not be null or empty.", "Invalid Memo Data", MessageBoxButton.OK);
                Debug.Write("Paragraph Text cannot be null.");
                isValidMemo = false;
            }
            else if (string.IsNullOrEmpty(objMemo.Original_Text) == true)
            {
                MessageBox.Show("Original Text must not be null or empty.", "Invalid Memo Data", MessageBoxButton.OK);
                Debug.Write("Original Text cannot be null.");
                isValidMemo = false;
            }
            else if (string.IsNullOrEmpty(objMemo.Content) == true)
            {
                MessageBox.Show("Contents text must not be null or empty.", "Invalid Memo Data", MessageBoxButton.OK);
                Debug.Write("Content cannot be null.");
                isValidMemo = false;
            }

            return isValidMemo;
        }
    }
}
