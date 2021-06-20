using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Chat_Manage : MonoBehaviour
{
   GameObject Player;
   GameObject Cat; 
   Text player_Talk;
   Text cat_Talk;
   Text input;

   public string[] playerScript = new string[]{"啊？我已经，很多年，没有见到过动物了！这是……猫？按照我的印象，这种动物已经在城市中灭绝了。但看着这么可爱，带回家应该也没问题吧？","可是，可是，这里，地下室很破败，也算不上什么家呢。"};

   public string[] catScript = new string[]{"太好了！！！真的很感谢喵！终于有家了喵！"};

   int playerChatPosi;
   int catChatPosi;
   int chaosRound;
   string prompt;

   public class chatMessage{
      public int playerId;
      public string text;
    }


   private void Start() {

      Player = GameObject.Find("Player");
      Cat = GameObject.Find("Cat");
      player_Talk = GameObject.Find("playerTalk").GetComponent<Text>();
      cat_Talk = GameObject.Find("catTalk").GetComponent<Text>();
      input = GameObject.Find("inputText").GetComponent<Text>();

      playerChatPosi = 0;
      catChatPosi = 0;
      chaosRound = 0;

      intialization();
   }
   private void Update() {

      if(Input.GetButtonDown("Submit")){
         nextConversation();
      }
   }

   void intialization(){
      generatePlayerTalk();
      Player.SetActive(true);
      Cat.SetActive(false);
      input.gameObject.transform.parent.gameObject.SetActive(false);
   }

   void nextConversation(){
      if(Player.active){
         Cat.SetActive(true);
         Player.SetActive(false);
         generateCatTalk();
      }else{
         Cat.SetActive(false);
         Player.SetActive(true);
         generatePlayerTalk();
      }
   }

   void generateCatTalk(){
      // TODO:call api  
      if(catChatPosi >= 2 && catChatPosi <=4){
         cat_Talk.text = "...";
         getResponseAPI();
      }else{
         cat_Talk.text = catScript[catChatPosi];
         catChatPosi = catChatPosi +1;
      }
      
      // Clear Input Field and hide the input
      input.transform.parent.gameObject.SetActive(false);
   }

   void generatePlayerTalk(){
      if(playerChatPosi >= 2 && playerChatPosi <=4){
         input.transform.parent.gameObject.SetActive(true);
         player_Talk.text = "...";
      }else{
         player_Talk.text = playerScript[playerChatPosi];
         playerChatPosi = playerChatPosi +1;
      }

   }

   public void sendText(){
      player_Talk.text = input.text;
      prompt = input.text;
      input.transform.parent.GetComponent<InputField>().Select();
      input.transform.parent.GetComponent<InputField>().text = "";

   }

   public void getResponseAPI(){
      StartCoroutine(Post("http://47.98.203.153/api/translator/conversation/",prompt));
   }

   IEnumerator Post(string url, string prompt)
    {
        var request = new UnityWebRequest(url, "POST");
        
        chatMessage sendJsonData = new chatMessage();
        sendJsonData.playerId= 1;
        sendJsonData.text = prompt;
        
        string bodyJsonString = JsonUtility.ToJson(sendJsonData);
        Debug.Log(bodyJsonString);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        string response = request.downloadHandler.text;
         Debug.Log("Status Code: " + request.downloadHandler.text);
        Debug.Log(response);
        Debug.Log(response.Split(':')[1]);

        string answer = response.Split(':')[1].Replace("}","").Replace("\"","");
        byte[] answerByte = Encoding.Unicode.GetBytes(answer);
        Debug.Log(answerByte);
        Encoding ascii = Encoding.UTF8;
        Encoding unicode = Encoding.Unicode;
         byte[] asciiBytes = Encoding.Convert(unicode, ascii, answerByte);

         char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
         ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
        string asciiString = new string(asciiChars);
        answer = Encoding.UTF8.GetString(answerByte);
        
        Debug.Log("Status Code: " + request.downloadHandler.text);
        Debug.Log("Answer: " + asciiString);

        // Store userID as the information
        cat_Talk.text = answer;
    }
}
