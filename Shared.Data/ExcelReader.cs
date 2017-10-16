using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Shared.Data
{
    // Class for obtaining information from a .xlsx file
    public class ExcelReader
    {
        #region Class Attributes

        // Path to the .xlsx file
        private string pathToFile;

        // Sheet Name to Excel Sheet Dictionary represented with string to matrices dictionary
        private Dictionary<string, object[,]> sheetsMatrices;

        // Sheet Name to Excel Sheet Dictionary representad with string to datatables dictionary
        private Dictionary<string, System.Data.DataTable> sheetsDataTables;

        #endregion

        // Constructor
        public ExcelReader(string path)
        {
            pathToFile = @path;
            Update();
        }

        #region Access and Modifiers

        public void SetPathToFile(string path)
        {
            pathToFile = @path;
            Update();
        }

        public Dictionary<string, object[,]> GetSheetsMatrices()
        {
            return sheetsMatrices;
        }

        public Dictionary<string, System.Data.DataTable> GetSheetsDataTables()
        {
            return sheetsDataTables;
        }

        #endregion

        #region Routines for fetching data

        // Update data in matrices y datatables
        public void Update()
        {
            FillSheetsMatrices();
            FillSheetsDataTables();
        }

        // Fills a dictionary of sheet names to excel sheets, represented in a string to matrices dictionary.
        private void FillSheetsMatrices()
        {
            try
            {
                // Start excel
                Application app = new Application();
                Workbook wbk = app.Workbooks.Open(@pathToFile);

                // Save each sheet in a string to object matrix dictionary...
                // Initialization
                int n = wbk.Sheets.Count;
                sheetsMatrices = (new Dictionary<string, object[,]>());

                // For each excel sheet...
                for (int sh = 0; sh + 1 <= n; sh++)
                {
                    // Save active range from the sheet in an equivalent object matrix, and add an entry to the dictionary, relating the name and the created matrix
                    Worksheet wsh = (Worksheet)wbk.Sheets[sh + 1];
                    Range activeRange = wsh.UsedRange;
                    sheetsMatrices.Add(wsh.Name, (object[,])activeRange.get_Value(XlRangeValueDataType.xlRangeValueDefault));
                }

                // Closing file and releasing memory
                wbk.Close(false, pathToFile, null);
                Marshal.ReleaseComObject(wbk);
            }
            catch (Exception ex)
            {
                // Shows error in console
                Console.WriteLine("Error when fetching excel sheets representations (ExcelReader.FillSheetsMatrices())");
                Debug.WriteLine(ex.ToString());
            }
        }

        // Fills a sheet name to excel sheets dictionary, represented with a string to datatables dictionary
        private void FillSheetsDataTables()
        {
            // Get sheets number
            int NumSheets = sheetsMatrices.Count;

            // Initialize dictionary of string to datatables
            sheetsDataTables = (new Dictionary<string, System.Data.DataTable>());

            // Foreach excel sheet...
            foreach (KeyValuePair<string, object[,]> entry in sheetsMatrices)
            {
                // Inicializar tabla y obtener numero de filas y columnas
                // Initialize datatable and get number of rows and columns from the sheetMatrix
                sheetsDataTables.Add(entry.Key, new System.Data.DataTable());
                int NumRow = entry.Value.GetLength(0);
                int NumCol = entry.Value.GetLength(1);

                // For each column...
                for (int j = 1; j <= NumCol; j++)
                {
                    // Add column
                    sheetsDataTables[entry.Key].Columns.Add((string)entry.Value[1, j], typeof(object));
                }

                // For each active data row in the table
                for (int i = 2; i <= NumRow; i++)
                {
                    // Add new row to the datatable
                    object[] datos = new object[NumCol];
                    for (int j = 1; j <= NumCol; j++)
                    {
                        datos[j - 1] = entry.Value[i, j];
                    }
                    sheetsDataTables[entry.Key].Rows.Add(datos);
                }
            }
        }

        #endregion
    }
}