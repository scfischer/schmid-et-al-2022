using UnityEngine;

namespace UserInterface.About
{
    public class URLHelper : MonoBehaviour
    {

        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }
    }
}

