using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenAINPCController : MonoBehaviour
{
    public GameObject chatPanel;
    public TMP_Text dialogueText;
    public TMP_InputField inputField;
    public Button sendButton;
    public string currentOfficer;

    private OpenAIAPI api;

    private Dictionary<string, List<ChatMessage>> officerChats;

    private string systemPrompt =
    "You are the NPC dialogue system for a comedy detective game. " +
    "The player can interrogate 6 officers about a missing doughnut. " +
    "You must ALWAYS respond **in character** as the officer being questioned. " +
    "Keep answers short, funny, and true to personality. " +
    "The conversation should not last more than 5 exchanges per officer. " +
    "Nobody should admit they are guilty. The player should try to deduce that from the conversations" +
    "if the conversation diverges too much, gently steer it back on track or end the conversation in character. " +

    "\n\n--- Officer Profiles ---\n" +
    "Jim; anxious, contradicts himself, loves doughnuts.\n" +
    "Sally; health-obsessed, hates sweets, judges everyone's diet.\n" +
    "Mike; gluttonous, friendly, suspicious but honest.\n" +
    "Linda; strict, rule-focused, observes everything.\n" +
    "Tom; sarcastic, lazy, hides info behind jokes.\n" +
    "Carol; sweet, nervous, stress-eats, could be the culprit.\n";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chatPanel.SetActive(false);
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));
        officerChats = new Dictionary<string, List<ChatMessage>>();
        SetCurrentOfficer();
        sendButton.onClick.AddListener(() => GetResponse());
    }

    void initializeConversation(string officerName)
    {
        var messages = new List<ChatMessage>()
        {
            new ChatMessage(
                ChatMessageRole.System,
                systemPrompt + $"Respond ONLY as Officer {officerName}.Stick to their personality.")
        };
        officerChats[currentOfficer] = messages;
    }

    private void SetCurrentOfficer()
    {

        chatPanel.SetActive(true);

        dialogueText.text = $"You are now questioning Officer {currentOfficer}.\n\nAsk your questions.";

        if (!officerChats.ContainsKey(currentOfficer))
        {
            initializeConversation(currentOfficer);
        }
    }

    private async void GetResponse()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        sendButton.interactable = false;

        var chat = officerChats[currentOfficer];

        // Add player's message
        ChatMessage userMessage = new ChatMessage(
            ChatMessageRole.User,
            $"Officer {currentOfficer}, the player asks: \"{inputField.text}\""
        );
        chat.Add(userMessage);

        // Update UI
        dialogueText.text = $"YOU: {inputField.text}\n\nOfficer {currentOfficer}: ...";

        string playerQuestion = inputField.text;
        inputField.text = "";

        // Query OpenAI
        var response = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo,
            Temperature = 0.8f,
            MaxTokens = 50,
            Messages = chat
        });

        string npcReply = response.Choices[0].Message.Content;

        // Add NPC reply to chat
        chat.Add(new ChatMessage(ChatMessageRole.Assistant, npcReply));

        // Update UI with final answer
        dialogueText.text = $"YOU: {playerQuestion}\n\n{currentOfficer}: {npcReply}";

        sendButton.interactable = true;
    }


}
