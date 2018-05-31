using UnityEngine;
using System.IO.Ports;
using RedRunner.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace RedRunner
{
    public class ArduinoManager : MonoBehaviour
    {
        #region Constants
        private static readonly string API_URL = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true&returnFaceAttributes=emotion";
        private static readonly string API_KEY = "4c01340088d245a9a7832850a5313107";

        private const int BAUD_RATE = 9600;

        private readonly Dictionary<string, string> POST_HEADER = new Dictionary<string, string>()
        {
            {"Ocp-Apim-Subscription-Key", API_KEY},
            {"Content-Type", "application/octet-stream"}
        };
        #endregion

        private RedCharacter character;

        [SerializeField]
        private float cameraDelay;

        private WebCamTexture webCamTex;

        #region Routine
        private Coroutine controllerRoutine;
        private Coroutine cameraRoutine;
        #endregion

        private string controllerPortName = "COM1";
        private string cameraPortName = "COM2";

        private static ArduinoManager intance;
        public static ArduinoManager Instance { get { return Instance; } }

		private void Start()
		{
            webCamTex = new WebCamTexture();
            Renderer renderer = this.GetComponent<Renderer>();
            renderer.material.mainTexture = webCamTex;
            webCamTex.Play();

            character = (RedCharacter)(GameManager.Instance.MainCharacter);
		}

        public void OnGameStart(){
            if (controllerRoutine != null)
                StopCoroutine(controllerRoutine);
            if (cameraRoutine != null)
                StopCoroutine(cameraRoutine);

            controllerRoutine = StartCoroutine(ProcessController());
            cameraRoutine = StartCoroutine(ProcessCamera());
        }

        public void SetControllerPortName(string name){
            controllerPortName = name;
        }

        public void SetCameraPortName(string name){
            cameraPortName = name;
        }

        private IEnumerator ProcessController(){
            SerialPort stream = new SerialPort(controllerPortName, BAUD_RATE);
            stream.Open();

            while (true){
                if (stream.IsOpen == false)
                    continue;

                if (character == null)
                    continue;

                //TODO : character handle
                yield return null;
            }
        }

        private IEnumerator ProcessCamera(){
            SerialPort stream = new SerialPort(cameraPortName, BAUD_RATE);
            stream.Open();

            while (true){
                if (stream.IsOpen == false)
                    continue;
                
                //TODO : camera byteArr 넘겨주기 
                yield return StartCoroutine(ProcessFaceAPI());

                yield return new WaitForSeconds(cameraDelay);
            }
        }

        private IEnumerator ProcessFaceAPI(){
            while (true){
                Texture2D _TextureFromCamera = new Texture2D(GetComponent<Renderer>().material.mainTexture.width,
                GetComponent<Renderer>().material.mainTexture.height);
                _TextureFromCamera.SetPixels((GetComponent<Renderer>().material.mainTexture as WebCamTexture).GetPixels());
                _TextureFromCamera.Apply();
                byte[] bytes = _TextureFromCamera.EncodeToJPG();

                WWW www = new WWW(API_URL, bytes, POST_HEADER);
                yield return www;
                if (www.error != null){
                    Debug.Log(www.error);
                }
                else{
                    Debug.Log(www.text);
                    // TODO : Json 파싱
                    GameManager.Instance.HandleFaceAPIResult();
                }
            }
        }
	}
}
