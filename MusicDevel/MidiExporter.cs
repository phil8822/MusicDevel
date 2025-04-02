using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDevel
{
    public class MidiExporter
    {
        public void CreateMidiFile(string fileName, DataTable musicTable)
        {
            int MidiFileType = 0;
            int BeatsPerMinute = 120;
            int TicksPerQuarterNote = 120;
            int NoteDuration = 4 * TicksPerQuarterNote / 4;
            long SpaceBetweenNotes = TicksPerQuarterNote;
            long absoluteTime = 0;
            int TrackNumber = 0;
            int ChannelNumber = 1;
            int patchNumber = 1; // temporary fixed value Acoutic Grand Piano
            int NoteVelocity = 100;

            var midiEvts = new MidiEventCollection(MidiFileType, TicksPerQuarterNote);

            // Add title meta event (fails in MuseScore, appears in MusicMasterWorks)
            var txEvent1 = new TextEvent("Sequence track name", MetaEventType.SequenceTrackName, absoluteTime);
            midiEvts.AddEvent(txEvent1, 0);

            var txEvent2 = new TextEvent("Copyright text string", MetaEventType.Copyright, absoluteTime);
            midiEvts.AddEvent(txEvent2, 0);

            var txEvent3 = new TextEvent("Program Name", MetaEventType.ProgramName, absoluteTime);
            midiEvts.AddEvent(txEvent3, 0);

            midiEvts.AddEvent(new TextEvent("Text Event - Note Stream", MetaEventType.TextEvent, absoluteTime), TrackNumber);
            
            midiEvts.AddEvent(new TempoEvent(MicrosecondsPerQuaterNote(BeatsPerMinute), absoluteTime), TrackNumber);
            midiEvts.AddEvent(new PatchChangeEvent(0, ChannelNumber, patchNumber), TrackNumber);


            for (int i = 0; i < musicTable.Rows.Count; i++)
            {
                DataRow row = musicTable.Rows[i];
                absoluteTime = Convert.ToInt32(row["AbsoluteTime"]);
                int midiValue = Convert.ToInt32(row["IntVal"]);
                int durationTicks = Convert.ToInt32(row["DurationTicks"]);

                midiEvts.AddEvent(new NoteOnEvent(absoluteTime, ChannelNumber, midiValue, NoteVelocity, durationTicks), TrackNumber);
                midiEvts.AddEvent(new NoteEvent(absoluteTime + durationTicks, ChannelNumber, MidiCommandCode.NoteOff, midiValue, 0), TrackNumber);
            }

            midiEvts.PrepareForExport();
            MidiFile.Export(fileName, midiEvts);
        }


        private static int MicrosecondsPerQuaterNote(int bpm)
        {
            return 60 * 1000 * 1000 / bpm;
        }

    }  // end of class
}
