using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Facebook.Unity;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class FacebookManager : MonoBehaviour
{
    public string Name;
    public TextMeshProUGUI fbUserNameText;

    public static FacebookManager instance;

    //public Texture fbProfilepicTexture;
    public Sprite fbprofilepic;
    public Image fbProfile;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject menuPanel;

    #region Initialize

    private void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                    FB.ActivateApp();
                else
                    print("Couldn't initialize");
            },
            isGameShown =>
            {
                if (!isGameShown)
                    Time.timeScale = 0;
                else
                    Time.timeScale = 1;
            });
        }
        else
            FB.ActivateApp();
        instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("FBUserName"))
        {
            LoadFacebookUserData(); // Load Facebook user data
            loginPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }

    private void OnEnable()
    {
        /*if (OnlyData.Instance.FBLoginbool == true)
        {
            mainMenubackbtn.onClick.AddListener(LogOut);
        }*/
    }

    void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("Facebook is Login!");
            string s = "client token" + FB.ClientToken + "User Id" + AccessToken.CurrentAccessToken.UserId + "token string" + AccessToken.CurrentAccessToken.TokenString;
            //OnlyData.Instance.FBLoginbool = true;
        }
        else
        {
            Debug.Log("Facebook is not Logged in!");
        }
        DealWithFbMenus(FB.IsLoggedIn);
    }

    void onHidenUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    void DealWithFbMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        }
        else
        {
            print("Not logged in");
        }
    }
    void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
            Name = "" + result.ResultDictionary["first_name"];
            Debug.Log("FB user name"+ Name);
            fbUserNameText.text = Name;
            PlayerPrefs.SetString("FBUserName", Name); // Save username
            PlayerPrefs.Save();
            Debug.Log("" + Name);
        }
        else
        {
            Debug.Log(result.Error);
        }
    }
    void DisplayProfilePic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            Debug.Log("Profile Pic");
           /* var fbProfilepicTexture = result.Texture;
            fbprofilepic. = fbProfilepicTexture;*/

            string profilePicURL = "https://graph.facebook.com/" + AccessToken.CurrentAccessToken.UserId + "/picture?type=large";
            PlayerPrefs.SetString("FBProfilePicURL", profilePicURL);
            PlayerPrefs.Save();
            StartCoroutine(LoadFBProfilePic(profilePicURL));
            loginPanel.SetActive(false);
            menuPanel.SetActive(true);
            fbProfile.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    IEnumerator LoadFBProfilePic(string imageUrl)
    {
        if (!string.IsNullOrEmpty(imageUrl))
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load Facebook profile picture: " + request.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                fbprofilepic = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                fbProfile.sprite = fbprofilepic;
                /*fbProfilepicTexture = texture;
                fbprofilepic.texture = fbProfilepicTexture; // Set image to UI*/
            }
        }
    }

    void LoadFacebookUserData()
    {
        if (PlayerPrefs.HasKey("FBUserName"))
        {
            Name = PlayerPrefs.GetString("FBUserName");
            fbUserNameText.text = Name;  // Set username in UI

            string savedImageUrl = PlayerPrefs.GetString("FBProfilePicURL");
            if (!string.IsNullOrEmpty(savedImageUrl))
            {
                StartCoroutine(LoadFBProfilePic(savedImageUrl)); // Load saved profile picture
            }
        }
    }

    #endregion
    //login
    public void Facebook_LogIn()
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        FB.LogInWithReadPermissions(permissions, AuthCallBack);
    }

    void AuthCallBack(IResult result)
    {
        if (FB.IsLoggedIn)
        {
            SetInit();
            var aToken = AccessToken.CurrentAccessToken;
            //UserSessionManager.Instance.LoginType = LoginType.Facebook;
            //UserSessionManager.Instance.UserId = aToken.UserId;
            foreach (string perm in aToken.Permissions)
            {
                print(perm);
            }
        }
        else
        {
            Debug.Log("Failed to log in");
            loginPanel.SetActive(true);
            menuPanel.SetActive(false);
        }
    }

    public void Facebook_LogOut()
    {
        LogOut();
    }

    private void LogOut()
    {
        // Delete stored Facebook login data
        PlayerPrefs.DeleteKey("FBUserName");
        PlayerPrefs.DeleteKey("FBProfilePicURL");
        PlayerPrefs.Save();
        if (fbUserNameText.text != null) fbUserNameText.text = "";
        Debug.Log("Facebook Sign Out and Data Deleted");

        // Log out from Facebook
        FB.LogOut();

        // Reset UI
        fbProfile.gameObject.SetActive(false);
        menuPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    #region other

    public void FacebookSharefeed()
    {
        string url = "https:developers.facebook.com/docs/unity/reference/current/FB.ShareLink";
        FB.ShareLink(
            new Uri(url),
            "Checkout COCO 3D channel",
            "I just watched " + "22" + " times of this channel",
            null,
            ShareCallback);
    }

    private static void ShareCallback(IShareResult result)
    {
        Debug.Log("ShareCallback");
        SpentCoins(2, "sharelink");
        if (result.Error != null)
        {
            Debug.LogError(result.Error);
            return;
        }
        Debug.Log(result.RawResult);
    }
    public static void SpentCoins(int coins, string item)
    {
        var param = new Dictionary<string, object>();
        param[AppEventParameterName.ContentID] = item;
        FB.LogAppEvent(AppEventName.SpentCredits, (float)coins, param);
    }

    /*public void GetFriendsPlayingThisGame()
    {
        string query = "/me/friends";
        FB.API(query, HttpMethod.GET, result =>
        {
            Debug.Log("the raw" + result.RawResult);
            var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
            var friendsList = (List<object>)dictionary["data"];

            foreach (var dict in friendsList)
            {
                GameObject go = Instantiate(friendstxtprefab);
                go.GetComponent<Text>().text = ((Dictionary<string, object>)dict)["name"].ToString();
                go.transform.SetParent(GetFriendsPos.transform, false);
                FriendsText[1].text += ((Dictionary<string, object>)dict)["name"];
            }
        });
    }*/

    //public void GetFriendsPlayingThisGame()
    //{
    //    string query = "/me/friends";
    //    FB.API(query, HttpMethod.GET, result =>
    //    {
    //        var dictionary = (Dictionary<string, object>)Facebook.MiniJSON.Json.Deserialize(result.RawResult);
    //        var friendsList = (List<object>)dictionary["data"];
    //        FriendsText.text = string.Empty;
    //        Debug.Log("FBTEST4" + friendsList.Count);
    //        foreach (var dict in friendsList)
    //        {
    //            FriendsText.text += ((Dictionary<string, object>)dict)["name"];
    //        }
    //    });
    //}
    #endregion

}