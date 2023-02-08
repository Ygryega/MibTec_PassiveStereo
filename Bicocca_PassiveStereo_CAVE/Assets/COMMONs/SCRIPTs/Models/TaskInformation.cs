using System.Xml.Serialization;

[XmlRoot("Task")]
public class TaskInformation
{
    //public int TaskIndex { get; set; }
    //public int SessionIndex { get; set; }
    //public int TaskOrder { get; set; }

    [XmlAttribute("id")]
    public int TaskId { get; set; }

    [XmlAttribute("name")]
    public string TaskName { get; set; }

    [XmlAttribute("isDone")]
    public bool IsDone { get; set; }

    [XmlElement("ProtocolId")]
    public int ProtocolId { get; set; }

    [XmlElement("SessionId")]
    public int SessionId { get; set; }

    [XmlElement("UnitySceneName")]
    public string UnitySceneName { get; set; }

    [XmlElement("Description")]
    public string Description { get; set; }

    [XmlElement("Level")]
    public int Level { get; set; }
    //[XmlElement("TaskOrder")]
    //public int TaskOrder { get; set; }

    [XmlElement("TaskIcon")]
    public string TaskIcon { get; set; }

    [XmlElement("IsSynchro")]
    public bool IsSynchro { get; set; }

    [XmlElement("LocalTimeout")]
    public int LocalTimeout { get; set; }  //(milliseconds)

    [XmlElement("GlobalTimeout")]
    public int GlobalTimeout { get; set; }  //(milliseconds)

    [XmlElement("ExecutionTime")]
    public int ExecutionTime { get; set; }  //(milliseconds)

    [XmlElement("HelpsHistory")]
    public string HelpsHistory { get; set; }

    [XmlElement("ActionsHistory")]
    public string ActionsHistory { get; set; }

    [XmlElement("ExecutionTimes")]
    public string ExecutionTimes { get; set; }

    [XmlElement("LatencyTimes")]
    public string LatencyTimes { get; set; }

    [XmlElement("HelpsNumber")]
    public int HelpsNumber { get; set; }     //Help Number

    [XmlElement("CorrectAnswersNumber")]
    public int CorrectAnswersNumber { get; set; }

    [XmlElement("WrongAnswersNumber")]
    public int WrongAnswersNumber { get; set; }    //Wrong Answers Number

    [XmlElement("UngivenAnswersNumber")]
    public int UngivenAnswersNumber { get; set; }    //Ungiven Answers Number

    [XmlElement("StoringDate")]
    public string StoringDate { get; set; }

    [XmlElement("StoringTime")]
    public string StoringTime { get; set; }

}

