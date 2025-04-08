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

        #region Declarations
        // mostly temp values for testing, will be replaced by live data from SQL
        int MidiFileType = 0;
        int BeatsPerMinute = 120;
        int TicksPerQuarterNote = 120;
        long absoluteTime = 0;
        int TrackNumber = 0;
        int NoteVelocity = 100;

        int ChannelNumberMelody = 1;
        int patchNumberVibraphone = 12; // Vibraphone

        int ChannelNumberHarmony = 4;
        int patchNumberPiano = 1; // Acoutic Grand Piano
        #endregion


        public void CreateMidiFile(string fileName, DataTable musicTable)
        {
            // --------------------------- Melody only  --------------------------------------------------

            // Create a new MIDI file with one track
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
            midiEvts.AddEvent(new PatchChangeEvent(0, ChannelNumberMelody, patchNumberVibraphone), TrackNumber);

            for (int i = 0; i < musicTable.Rows.Count; i++)
            {
                DataRow row = musicTable.Rows[i];
                absoluteTime = Convert.ToInt32(row["AbsoluteTime"]);
                int midiValue = Convert.ToInt32(row["IntVal"]);
                int durationTicks = Convert.ToInt32(row["DurationTicks"]);

                midiEvts.AddEvent(new NoteOnEvent(absoluteTime, ChannelNumberMelody, midiValue, NoteVelocity, durationTicks), TrackNumber);
                midiEvts.AddEvent(new NoteEvent(absoluteTime + durationTicks, ChannelNumberMelody, MidiCommandCode.NoteOff, midiValue, 0), TrackNumber);
            }

            midiEvts.PrepareForExport();
            MidiFile.Export(fileName, midiEvts);


            // --------------------------- Melody and Harmony  --------------------------------------------------




        }


        private static int MicrosecondsPerQuaterNote(int bpm)
        {
            return 60 * 1000 * 1000 / bpm;
        }

    }  // end of class
}
