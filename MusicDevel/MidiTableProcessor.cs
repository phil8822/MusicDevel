using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MusicDevel
{
    public static class MidiCombiner
    {
        /// <summary>
        /// Adds ChannelNumber column to Melody (1) and Harmony (2), then combines into AllEvents sorted by AbsoluteTime.
        /// </summary>
        /// <param name="melody">DataTable containing Melody MIDI data</param>
        /// <param name="harmony">DataTable containing Harmony MIDI data</param>
        /// <returns>Combined DataTable named "AllEvents" sorted by AbsoluteTime</returns>
        public static DataTable CombineMidiTables(DataTable melody, DataTable harmony)
        {
            try
            {
                // Validate inputs
                if (melody == null || harmony == null)
                    throw new ArgumentNullException("Melody or Harmony DataTable is null.");
                if (!melody.Columns.Contains("AbsoluteTime") || !harmony.Columns.Contains("AbsoluteTime"))
                    throw new ArgumentException("Both tables must have an AbsoluteTime column.");
                if (!AreColumnsIdentical(melody, harmony))
                    throw new ArgumentException("Melody and Harmony must have identical column structures.");

                // A. Add ChannelNumber as the first column
                AddChannelNumberColumn(melody, 1);
                AddChannelNumberColumn(harmony, 2);

                // B. Combine into AllEvents and sort by AbsoluteTime
                DataTable allEvents = MergeAndSortTables(melody, harmony);
                allEvents.TableName = "AllEvents";

                return allEvents;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing MIDI tables: {ex.Message}");
                throw; // Re-throw for caller to handle if needed
            }
        }

        /// <summary>
        /// Adds ChannelNumber column as the first column with the specified value.
        /// </summary>
        private static void AddChannelNumberColumn(DataTable table, int channelValue)
        {
            // Create new ChannelNumber column
            DataColumn channelColumn = new DataColumn("ChannelNumber", typeof(int))
            {
                DefaultValue = channelValue // Set default value for all rows
            };

            // Insert as first column (index 0)
            table.Columns.Add(channelColumn);
            table.Columns["ChannelNumber"].SetOrdinal(0);

            // Ensure existing rows reflect the default value
            foreach (DataRow row in table.Rows)
            {
                if (row.IsNull("ChannelNumber"))
                    row["ChannelNumber"] = channelValue;
            }
        }

        /// <summary>
        /// Merges two tables and sorts by AbsoluteTime.
        /// </summary>
        private static DataTable MergeAndSortTables(DataTable table1, DataTable table2)
        {
            // Create a new DataTable with the same structure
            DataTable mergedTable = table1.Clone(); // Copies schema, not data

            // Import rows from both tables
            foreach (DataRow row in table1.Rows)
                mergedTable.ImportRow(row);
            foreach (DataRow row in table2.Rows)
                mergedTable.ImportRow(row);

            // Sort by AbsoluteTime
            DataView dv = mergedTable.DefaultView;
            dv.Sort = "AbsoluteTime ASC"; // Assumes ascending order; use DESC if needed
            return dv.ToTable(); // Converts sorted view back to DataTable
        }

        /// <summary>
        /// Checks if two DataTables have identical column structures (names and types).
        /// </summary>
        private static bool AreColumnsIdentical(DataTable table1, DataTable table2)
        {
            if (table1.Columns.Count != table2.Columns.Count)
                return false;

            var columns1 = table1.Columns.Cast<DataColumn>().Select(c => (c.ColumnName, c.DataType)).OrderBy(c => c.ColumnName);
            var columns2 = table2.Columns.Cast<DataColumn>().Select(c => (c.ColumnName, c.DataType)).OrderBy(c => c.ColumnName);

            return columns1.SequenceEqual(columns2);
        }
    }
}