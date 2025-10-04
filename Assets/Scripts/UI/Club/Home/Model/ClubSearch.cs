
using Newtonsoft.Json;

public class ClubSearch
{

    public int clubId;
    public string avatar;
    [JsonProperty("title")]
    public string name;
    public string creator;
    public int level;
    public int memberCount;
    public int memberLimit;
    public string intro;

}
