using System;
using System.Data.SQLite;
using System.Diagnostics;
using Dapper;
namespace BoostDraftAlpha.CollFiltering.Controllers
{
  public  class SqLiteBaseRepository : IDisposable
    {
        //public static string DbFile
        //{
        //    get { return System.IO.Path.Combine(BoostDraftConfig.FolderPath, "BoostDefinitions.db"); }
        //}
        public static SQLiteConnection SimpleDbConnection()
        {
            SQLiteConnection sqlcon = new SQLiteConnection();
            try
            {
                // Replace this DbFile as a variable after attaching all the code
                sqlcon = new SQLiteConnection("Data Source=" + "DbFile");
                //Check weather connection is valid or not, only required for parallels/mac env? 
                sqlcon.Open();
                sqlcon.Close();
            }
            catch(Exception e)
            {
                var filename = "";
             //   UsageLogger.AddError(filename, e);
            }
            return sqlcon;
        }
        public static void CreateDatabase()
        {
            using (var cnn = SimpleDbConnection())
            {
                try
                {
                    cnn.Open();
                    cnn.Execute(@"create table IF NOT EXISTS DocumentFile
                      (
                         File_Id                            integer primary key AUTOINCREMENT,
                         Filename                           nvarchar(100) not null,
                         Path                               nvarchar(100) not null,
                         Key                                nvarchar(100),
                         Type                               nvarchar(100),
                         Lang                               nvarchar(100) not null,
                         Created_at                         datetime not null,
                         Updated_at                         datetime not null
                      )");
                    cnn.Execute(@"create table IF NOT EXISTS DefinedTerm
                      (
                         Term_Id                             integer primary key AUTOINCREMENT,
                         Term                                nvarchar(100) not null,
                         Created_at                          datetime not null,
                         Updated_at                          datetime not null
                      )");
                    cnn.Execute(@"create table IF NOT EXISTS FileandTerm
                      (
                         FileTermKey                         nvchar(100) primary key,
                         File_Id                             integer not null,
                         Term_Id                             integer not null,
                         Description                         nvarchar(500),
                         Updated_at                          datetime not null
                      )");
                    cnn.Execute(@"create table IF NOT EXISTS TermSimilarity
                      (
                         TermKey                             nvchar(100) primary key, 
                         TermA_Id                            integer not null,
                         TermB_Id                            integer not null,
                         Similarity                          real not null,
                         Updated_at                          datetime not null
                      )");

                    // Added query for Memo table
                    cnn.Execute(@"create table IF NOT EXISTS Memo
                      (
                         File_ID                             integer, 
                         Section_No                          nvarchar not null,
                         Paragraph_Text                      nvarchar not null,
                         Original_Text                       nvarchar not null,
                         Content                             nvarchar not null,
                         Memo                                nvarchar
                         PRIMARY KEY(""File_ID"")
                      )");
                }
                catch (Exception e)
                {
                    var filename = "";
                   // UsageLogger.AddError(filename, e);
                }
             }
        }
        public void Dispose()
        {
            SQLiteConnection.ClearAllPools();
        }
    }
}
