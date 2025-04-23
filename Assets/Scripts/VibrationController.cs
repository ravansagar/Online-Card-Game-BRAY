//using UnityEngine;

//public class VibrationController : MonoBehaviour
//{
//#if UNITY_ANDROID && !UNITY_EDITOR
//    private AndroidJavaObject vibrationEffect;
//    private AndroidJavaObject vibrator;
//    void Start()
//    {
//        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
//        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
//    }

//    public void Vibrate(long milliseconds)
//    {
//        if (vibrator != null)
//        {
//            vibrator.Call("vibrate", milliseconds);
//        }
//    }
//#endif

//    public void NotifyPlayerTurn()
//    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        Vibrate(500); // Vibrates for 500 milliseconds
//#else
//        Handheld.Vibrate();
//#endif
//    }
//}
