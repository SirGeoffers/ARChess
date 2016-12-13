using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class NetworkHUDController : MonoBehaviour {

    [SerializeField]
    private string defaultIP;

    [SerializeField]
    private Text inputField;

    [SerializeField]
    private GameObject placeholderText;

    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private GameObject hud;

    void Start() {

        inputField.text = defaultIP;

    }

    void Update() {

        if (inputField.text.Length == 0) {
            placeholderText.SetActive(true);
        } else {
            placeholderText.SetActive(false);
        }

    }

    public void InputCharacter(string c) {
        inputField.text += c;
        placeholderText.SetActive(false);
    }

    public void DeleteCharacter() {
        inputField.text = inputField.text.Remove(inputField.text.Length - 1);
    }

    public void Clear() {
        inputField.text = "";
    }

    public void Host() {

        networkManager.StartHost();
        hud.SetActive(false);

    }

    public void Connect() {

        networkManager.networkAddress = inputField.text;
        networkManager.StartClient();
        hud.SetActive(false);

    }

}
