using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPCOpenAIController : MonoBehaviour
{
    public GameObject chatPanel;
    public TMP_Text dialogueText;
    public TMP_InputField inputField;
    public Button sendButton;

    public string NPCname;

    private OpenAIAPI api;
    private List<ChatMessage> chat;
    private StoryManager storyManager;

    public int maxExchanges = 5;
    private int RemainingExchanges;

    private string systemPrompt =
    "You are the NPC dialogue system for a comedy detective game. " +
    "The player can interrogate 6 officers about a missing doughnut. " +
    "You must ALWAYS respond **in character** as the officer being questioned. " +
    "Keep answers short, funny, and true to personality. " +
    "The conversation should not last more than 5 exchanges per officer. " +
    "Nobody should admit they are guilty. The player should try to deduce that from the conversations" +
    "if the conversation diverges too much, gently steer it back on track or end the conversation in character. ";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chatPanel.SetActive(true);
        api = new OpenAIAPI(Environment.GetEnvironmentVariable("OPENAI_API_KEY", EnvironmentVariableTarget.User));
     
        storyManager = FindFirstObjectByType<StoryManager>();
        RemainingExchanges = maxExchanges;

        NPCname = SceneManager.GetActiveScene().name.Replace("_scene", "");
        initializeConversation();
        sendButton.onClick.AddListener(() => GetResponse());
    }



    void initializeConversation()
    {
        string NPCprompt =  storyManager.GetPhasePrompt(NPCname);

        Debug.Log($"Initializing conversation with Officer {NPCname}:\n{systemPrompt + NPCprompt}");
        dialogueText.text = $"You are now questioning Officer {this.NPCname}.\n\nAsk your questions.";

        chat = new List<ChatMessage>()
        {
            new ChatMessage(
                ChatMessageRole.System,
                systemPrompt + NPCprompt)
        };
        
    }

    private async void GetResponse()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        sendButton.interactable = false;

        if (RemainingExchanges == 0)
        {
            inputField.interactable = false;
            sendButton.interactable = false;
            dialogueText.text += $"\n{NPCname}: I've answered enough. Leave me alone!";
            RemainingExchanges = maxExchanges;
            return;
        }

        // Add player's message
        ChatMessage userMessage = new ChatMessage(
            ChatMessageRole.User,
            $"Officer {NPCname}, the player asks: \"{inputField.text}\""
        );
        chat.Add(userMessage);

        storyManager.EvaluatePlayerMessage(inputField.text);

        // Update UI
        dialogueText.text = $"YOU: {inputField.text}\n\nOfficer {NPCname}: ...";

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

        string NPCReply = response.Choices[0].Message.Content;

        // Add NPC reply to chat
        chat.Add(new ChatMessage(ChatMessageRole.Assistant, NPCReply));

        // Update UI with final answer
        dialogueText.text = $"YOU: {playerQuestion}\n\n{NPCname}: {NPCReply}";

        RemainingExchanges--;

        sendButton.interactable = true;
    }


}
