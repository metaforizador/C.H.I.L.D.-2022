using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

/// <summary>
/// Holds the question variables which child asks.
/// </summary>
public struct Question {
    [XmlElement("mood")]
    public Mood mood;

    [XmlElement("questionText")]
    public string questionText;

    [XmlArray("answers")]
    [XmlArrayItem("answer")]
    public Answer[] answers;
}

/// <summary>
/// Holds the answer variables which player chooses.
/// </summary>
public struct Answer {
    [XmlElement("answerType")]
    public WordsType answerType;

    [XmlElement("answerText")]
    public string answerText;
}

/// <summary>
/// Holds the replies which child gives to player's answers.
/// </summary>
public struct Reply {
    [XmlElement("replyType")]
    public WordsType replyType;

    [XmlElement("replyText")]
    public string replyText;
}

/// <summary>
/// Parses the xml file for dialogues.
/// </summary>
[XmlRoot("root"), XmlType("questions")]
public class XMLDialogueParser {

    [XmlArray("questions")]
    [XmlArrayItem("question")]
    public List<Question> questions = new List<Question>();

    [XmlArray("replies")]
    [XmlArrayItem("reply")]
    public List<Reply> replies = new List<Reply>();

    private static string path = Path.Combine(Application.streamingAssetsPath, "XMLFiles/Dialogues.xml");

    /// <summary>
    /// Loads the xml file from memory and parses it's content.
    /// </summary>
    /// <returns></returns>
    public static XMLDialogueParser Load() {
        try {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLDialogueParser));
            using (FileStream stream = new FileStream(path, FileMode.Open)) {
                return serializer.Deserialize(stream) as XMLDialogueParser;
            }
        } catch (Exception e) {
            UnityEngine.Debug.LogError("Exception loading config file: " + e);

            return null;
        }
    }

    /// <summary>
    /// Returns a random question which is filtered by the parameter value.
    /// </summary>
    /// <param name="mood">Mood for the question</param>
    /// <returns>Random question</returns>
    public static Question GetRandomQuestion(XMLDialogueParser holder, Mood mood) {
        // Create an array for the questions
        List<Question> questions = new List<Question>();

        // Add all questions to list which belong to this mood
        foreach (Question o in holder.questions) {
            if (o.mood.Equals(mood)) {
                questions.Add(o);
            }
        }

        // Throw error if no questions are found with provided parameter
        if (questions.Count == 0) {
            throw new Exception($"There are no questions defined for {mood}!");
        }

        // Retrieve already asked questions and loop through remaining ones
        Dictionary<Mood, List<string>> askedQuestions = CanvasMaster.Instance.askedQuestions;

        // If askedQuestions doesn't have current mood as key yet, create new key
        if (!askedQuestions.ContainsKey(mood)) {
            askedQuestions.Add(mood, new List<string>());
        }

        Question question;
        while (true) {
            // If count is 5, random returns values between 0 and 4
            question = questions[UnityEngine.Random.Range(0, questions.Count)];

            // If question has already been asked, randomize new one
            if (askedQuestions[mood].Contains(question.questionText)) {
                continue;
            }

            // Add question to asked questions
            askedQuestions[mood].Add(question.questionText);

            // If all of the questions have been asked from this area, clear list
            if (askedQuestions[mood].Count == questions.Count) {
                askedQuestions[mood].Clear();
            }

            break;
        }

        return question;
    }

    /// <summary>
    /// Returns a random reply which is filtered by the parameter type.
    /// </summary>
    /// <param name="type">WordsType of the reply</param>
    /// <returns>Random reply</returns>
    public static string GetRandomReply(XMLDialogueParser holder, WordsType type) {
        // Create an array for the replies
        List<Reply> replies = new List<Reply>();

        // Add all replies to list which belong to provided type
        foreach (Reply r in holder.replies) {
            if (r.replyType == type) {
                replies.Add(r);
            }
        }

        // Retrieve already given replies and loop through remaining ones
        Dictionary<WordsType, List<string>> givenReplies = CanvasMaster.Instance.givenReplies;

        // If givenReplies doesn't have current type as key yet, create new key
        if (!givenReplies.ContainsKey(type)) {
            givenReplies.Add(type, new List<string>());
        }

        Reply reply;
        while (true) {
            // If count is 5, random returns values between 0 and 4
            reply = replies[UnityEngine.Random.Range(0, replies.Count)];

            // If reply has already been given, randomize new one
            if (givenReplies[type].Contains(reply.replyText)) {
                continue;
            }

            // Add reply to given replies
            givenReplies[type].Add(reply.replyText);

            // If all of the replies have been given from this type, clear list
            if (givenReplies[type].Count == replies.Count) {
                givenReplies[type].Clear();
            }

            break;
        }

        return reply.replyText;
    }
}
