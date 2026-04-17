using Godot;
using System;
using System.Collections.Generic;


namespace StreamerBotApp
{
    /// <summary>
    /// Used to create and store basic JSON requests with no additional data
    /// </summary>
    public struct BasicRequest
    {
        public string request { get; set; }
        public string id { get; set; }

        /// <param name="requsetType">The Request Type as defined in the Streamer.Bot documentation</param>
        public BasicRequest(string requsetType)
        {
            request = requsetType;
            id = Guid.NewGuid().ToString(); ;
        }

        /// <summary>
        /// Returns a JSON formated version of the request.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{\"request\": \"{request}\", \"id\": \"{id}\"}}";
        }
    }


    /// <summary>
    /// The Root Node for the GetActions response from Streamer.Bot
    /// </summary>
    public struct GetActionsRoot
    {
        public List<ActionData> actions { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public string id { get; set; }
    }

    /// <summary>
    /// Data on the individual actions from Streamer.Bot
    /// </summary>
    public struct ActionData
    {
        public bool enabled { get; set; }
        public string group { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public int subaction_count { get; set; }
        public int trigger_count { get; set; }
    }

    /// <summary>
    /// Root of the DoAction Request to be sent to Streamer.Bot
    /// </summary>
    public struct DoActionRequestRoot
    {
        public string request { get; set; }
        public DoActionRequest action { get; set; }
        public string id { get; set; }
        /// <summary>
        /// <param name="actionData">The Action to be executed</param>
        public DoActionRequestRoot(ActionData actionData)
        {
            request = "DoAction";
            action = new(actionData.id);
            id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Returns a JSON formated version of the request.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{\"request\": \"{request}\", \"id\": \"{id}\", \"action\": {action.ToString()}}}";
        }
    }
    /// <summary>
    /// The Actual details of the action being requesting to be executed as part of a DoAction Request
    /// </summary>
    public struct DoActionRequest
    {
        public string id { get; set; }

        /// <param name="ID">String: Unique ID given to the action by Streamer.Bot</param>
        public DoActionRequest(string ID)
        {
            id = ID;
        }
        public override string ToString()
        {
            return $"{{\"id\": \"{id}\"}}";
        }
    }


    /// <summary>
    /// Root of the "Hello" object returned by Streamer.Bot on connection
    /// Used to validate connection and calculate authentication response
    /// </summary>
    public struct HelloRoot
    {
        public string timestamp { get; set; }
        public string session { get; set; }
        public string request { get; set; }
        public Info info { get; set; }
        public Auth authentication { get; set; }

    }
    /// <summary>
    /// The Info Section of the "Hello" object returned by Streamer.Bot on connection
    /// </summary>
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

    /// <summary>
    /// The Authentication section of the "Hello" object returned by Streamer.Bot on connection
    /// Only received if authentication is enabled on the server and used to establish an authenticated
    /// connection.
    /// </summary>
    public struct Auth
    {
        public string salt { get; set; }
        public string challenge { get; set; }
    }
}