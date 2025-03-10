using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Google;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GoogleAuthentication : MonoBehaviour
{
    public string imageURL;
    public string userNameStr;
    public Sprite _profilePic;
    public Image _googleProfile;
    public Image _guestProfile;

    public TextMeshProUGUI userNameText;

    public bool guestloginbool;
    // public string guestId;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject menuPanel;

    // public GameObject loginPanel, profilePanel;
    private GoogleSignInConfiguration configuration;
    private string webClientId = "378472586729-aut826k6jbi0oklnrg97e2rigdm37vmq.apps.googleusercontent.com";
    public static GoogleAuthentication instance;


    /*void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }*/
    private void Start()
    {
        CheckLastLogin();
    }

    void CheckLastLogin()
    {
        if (PlayerPrefs.HasKey("IsGoogleLogin") && PlayerPrefs.GetInt("IsGoogleLogin") == 1)
        {
            // Google Login Detected
            LoadGoogleUserData();
        }
        else if (PlayerPrefs.HasKey("IsGuestLogin") && PlayerPrefs.GetInt("IsGuestLogin") == 1)
        {
            // Guest Login Detected
            LoadGuestData();
        }
        else
        {
            // No previous login detected, show login screen
            loginPanel.SetActive(true);
            menuPanel.SetActive(false);
        }
    }

    private void OnEnable()
    {

        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            UseGameSignIn = false,
            RequestEmail = true
        };
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;
        Debug.LogError("Calling SignIn");

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
            OnAuthenticationFinished, TaskScheduler.Default);

    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.LogError("Got unexpected exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Google Sign-In Canceled");
        }
        else
        {
            Debug.LogError("Welcome: " + task.Result.DisplayName + "!");
            imageURL = task.Result.ImageUrl?.ToString();

            StartCoroutine(LoadProfilePic(imageURL));

            // Save Google Login Data
            PlayerPrefs.SetInt("IsGoogleLogin", 1); // 1 = true
            PlayerPrefs.SetString("DisplayName", task.Result.DisplayName);
            PlayerPrefs.SetString("LoadProfilePic", imageURL);
            PlayerPrefs.Save();

            loginPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }

    IEnumerator LoadProfilePic(string imageUrl)
    {
        if (imageUrl != null)
        {

            WWW www = new WWW(imageUrl);
            yield return www;

            _profilePic = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            _googleProfile.sprite = _profilePic;
           // OnlyData.Instance.GSprite = _profilePic;
            //OnlyData.Instance.googleLoginbool = true;
            loginPanel.SetActive(false);
            menuPanel.SetActive(true);
            _googleProfile.gameObject.SetActive(true);
        }

    }

    void LoadGoogleUserData()
    {
        if (PlayerPrefs.HasKey("DisplayName"))
        {
            userNameStr = PlayerPrefs.GetString("DisplayName");
            userNameText.text = userNameStr;

            string savedImageUrl = PlayerPrefs.GetString("LoadProfilePic");
            if (!string.IsNullOrEmpty(savedImageUrl))
            {
                StartCoroutine(LoadProfilePic(savedImageUrl));
            }

            loginPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }

    public void OnSignOut()
    {
        // Delete all login data
        PlayerPrefs.DeleteKey("IsGoogleLogin");
        PlayerPrefs.DeleteKey("IsGuestLogin");
        PlayerPrefs.DeleteKey("DisplayName");
        PlayerPrefs.DeleteKey("LoadProfilePic");
        PlayerPrefs.DeleteKey("guestId");
        PlayerPrefs.Save();

        if (userNameText.text != null) userNameText.text = "";
        // Sign out from Google
        GoogleSignIn.DefaultInstance.SignOut();

        _guestProfile.gameObject.SetActive(false);
        _googleProfile.gameObject.SetActive(false);
        // Show login panel
        menuPanel.SetActive(false);
        loginPanel.SetActive(true);
    }

    public void GuestLogin()
    {
        string Guestid = GenerateGuestId();
        userNameText.text = Guestid;

        // Save guest login status
        PlayerPrefs.SetInt("IsGuestLogin", 1); // 1 = true
        PlayerPrefs.Save();

        loginPanel.SetActive(false);
        menuPanel.SetActive(true);
        _guestProfile.gameObject.SetActive(true);
    }

    private string GenerateGuestId()
    {
        // Check if a Guest ID exists
        string savedGuestId = PlayerPrefs.GetString("guestId", null);
        if (!string.IsNullOrEmpty(savedGuestId))
        {
            return savedGuestId; // Return the existing Guest ID
        }

        // Generate a new unique Guest ID
        string newGuestId = System.Guid.NewGuid().ToString();

        // Save the new Guest ID for future logins
        PlayerPrefs.SetString("guestId", newGuestId);
        PlayerPrefs.Save();

        return newGuestId;
    }

    string GenerateGuestID()
    {
        return System.Guid.NewGuid().ToString(); // Unique ID for each guest
    }

    void SaveGuestData(string guestId)
    {
        PlayerPrefs.SetString("GuestID", guestId);
        PlayerPrefs.Save();
    }

    void LoadGuestData()
    {
        if (PlayerPrefs.HasKey("guestId"))
        {
            string guestId = PlayerPrefs.GetString("guestId");
            userNameText.text = guestId;

            loginPanel.SetActive(false);
            menuPanel.SetActive(true);
        }
    }

}


