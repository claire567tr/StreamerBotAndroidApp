using Godot;
using System;
using System.Collections.Generic;

public struct BasicRequest
{
    public string request { get; set; }
    public string id { get; set; }

    public BasicRequest(string requsetType)
    {
        request = requsetType;
        id = Guid.NewGuid().ToString(); ;
    }

    public override string ToString()
    {
        return $"{{\"request\": \"{request}\", \"id\": \"{id}\"}}";
    }
}

public struct GetActionsRoot
{
    public List<ActionData> actions { get; set; }
    public int count { get; set; }
    public string status { get; set; }
    public string id { get; set; }
}


public struct ActionData
{
    public bool enabled { get; set; }
    public string group { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public int subaction_count { get; set; }
    public int trigger_count { get; set; }
}


public struct DoActionRequestRoot
{
    public string request { get; set; }
    public DoActionRequest action { get; set; }
    public string id { get; set; }

    public DoActionRequestRoot(ActionData actionData)
    {
        request = "DoAction";
        action = new(actionData.id);
        id = Guid.NewGuid().ToString();
    }

    public override string ToString()
    {
        return $"{{\"request\": \"{request}\", \"id\": \"{id}\", \"action\": {action.ToString()}}}";
    }
}

public struct DoActionRequest
{
    public string id { get; set; }
    public DoActionRequest(string ID)
    {
        id = ID;
    }
    public override string ToString()
    {
        return $"{{\"id\": \"{id}\"}}";
    }
}


public struct HelloRoot
{
    public string timestamp { get; set; }
    public string session { get; set; }
    public string request { get; set; }
    public Info info { get; set; }
    public Auth authentication { get; set; }

}

public struct Info
{
    public string instanceId { get; set; }
    public string name { get; set; }
    public string version { get; set; }
    public string os { get; set; }
    public string osVersion { get; set; }
    public string mode { get; set; }
    public bool darkMode { get; set; }
    public string source { get; set; }
}

public struct Auth
{
    public string salt { get; set; }
    public string challenge { get; set; }
}