namespace Buzzer;

public class BuzzData
{
    public string HostConnectionID = "";
    public List<(string Name, long Time)> Buzzes { get; set; } = new();

    public void Add(string name, long time)
    {
        Buzzes.Add((name, time));
        Buzzes.Sort((pair1, pair2) => pair1.Time.CompareTo(pair2.Time));
    }

    public string Serialize()
    {
        return HostConnectionID + Buzzes.Select(pair => pair.Name + " @ " + pair.Time.ToString()).Aggregate("", (output, input) => output += "|||" + input);
    }

    public static BuzzData Deserialize(string encoded) 
    {
        BuzzData data = new BuzzData();
        data.HostConnectionID = encoded.Split("|||")[0];
        if (encoded.Split("|||").Length > 1)
        {
            var buzzes = encoded.Split("|||");
            for (int i = 1; i < buzzes.Length; i++)
            {
                var split = buzzes[i].Split(" @ ");
                data.Add(split[0], long.Parse(split[1]));
            }
        }
        return data;
    }

    public static BuzzData LoadOrCreate()
    {
        if (File.Exists("data.txt"))
        {
            return Deserialize(File.ReadAllText("data.txt"));
        }
        else
        {
            var output = new BuzzData();
            output.Save();
            return output;
        }
    }

    public void Save()
    {
        File.WriteAllText("data.txt", Serialize());
    }
}
