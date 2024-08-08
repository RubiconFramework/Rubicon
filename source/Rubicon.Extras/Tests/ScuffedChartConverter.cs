using Godot;
using Rubicon.Extras.Data;
using Rubicon.Extras.Utilities;

namespace Rubicon.Extras.Tests
{
    public partial class ScuffedChartConverter : Node
    {
        [Export] public string SaveFolder = "";

        [Export] public string FileName = "";
        
        [ExportGroup("Funkin"), Export(PropertyHint.File, "*.json")] public string FunkinChartPath = "";

        [ExportGroup("Stepmania"), Export(PropertyHint.File, "*.sm")]
        public string StepManiaChartPath = "";

        public override void _Ready()
        {
            base._Ready();

            if (!string.IsNullOrEmpty(FunkinChartPath))
            {
                string funkinJson = FileAccess.GetFileAsString(FunkinChartPath);
                SongTriplet triplet = ChartConverter.FromFunkin(funkinJson);
                
                FileAccess chartOutput = FileAccess.Open($"{SaveFolder}/{FileName}.json", FileAccess.ModeFlags.Write);
                chartOutput.StoreLine(triplet.Chart.Stringify());
                chartOutput.Close();
                
                FileAccess eventsOutput = FileAccess.Open($"{SaveFolder}/events.json", FileAccess.ModeFlags.Write);
                eventsOutput.StoreLine(triplet.Events.Stringify());
                eventsOutput.Close();

                ResourceSaver.Save(triplet.Metadata, $"{SaveFolder}/meta.tres");
            }
            
            if (!string.IsNullOrEmpty(StepManiaChartPath))
            {
                string smFile = FileAccess.GetFileAsString(StepManiaChartPath);
                SongTriplet triplet = ChartConverter.FromStepMania(smFile);
                
                FileAccess chartOutput = FileAccess.Open($"{SaveFolder}/{FileName}.json", FileAccess.ModeFlags.Write);
                chartOutput.StoreLine(triplet.Chart.Stringify());
                chartOutput.Close();

                ResourceSaver.Save(triplet.Metadata, $"{SaveFolder}/meta.tres");
            }
        }
    }
}