using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDevel
{
    public static class DiagHelper
    {
        /// <summary>
        /// Dumps all rows of the provided DataTable to Debug output in the format:
        /// table: xxx
        /// col name1, col name2, etc
        /// value1, value2, etc
        /// </summary>
        /// <param name="dataTable">The DataTable to dump</param>

        public static void DumpTable(DataTable dataTable, string tableName = null)
        {
            try
            {
                Debug.Print("");
                Debug.Print(new string('-', 50));

                // Check if DataTable is null or empty
                if (dataTable == null)
                {
                    Debug.Print("Error: DataTable is null.");
                    return;
                }
                if (dataTable.Rows.Count == 0)
                {
                    string effectiveName = !string.IsNullOrEmpty(tableName) ? tableName :
                                          !string.IsNullOrEmpty(dataTable.TableName) ? dataTable.TableName :
                                          "UnnamedTable";
                    Debug.Print($"table: {effectiveName}");
                    Debug.Print("No rows found.");
                    return;
                }

                // Determine the effective table name
                string finalTableName = !string.IsNullOrEmpty(tableName) ? tableName :
                                       !string.IsNullOrEmpty(dataTable.TableName) ? dataTable.TableName :
                                       "UnnamedTable";
                Debug.Print($"table: {finalTableName}");

                // Print column names
                string[] columnNames = dataTable.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();
                Debug.Print(string.Join(", ", columnNames));

                // Print each row
                foreach (DataRow row in dataTable.Rows)
                {
                    string[] rowValues = row.ItemArray.Select(item => item?.ToString().Trim() ?? "NULL").ToArray();
                    Debug.Print(string.Join(", ", rowValues));
                }
            }
            catch (Exception ex)
            {
                Debug.Print($"Error dumping table: {ex.Message}");
            }
        }
    }
}