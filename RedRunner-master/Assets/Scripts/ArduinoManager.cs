using UnityEngine;
using System.IO.Ports;
using RedRunner.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace AssemblyCSharp.Assets.Scripts
{
    public class FaceAPIMapper{
        public string faceId;
        public Dictionary<string, int> faceRect;
        public Dictionary<string, Dictionary<string, float>> attribute;
    }

    public class ArduinoManager : MonoBehaviour
    {
        private static readonly string url = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true&returnFaceAttributes=emotion";
        private static readonly string key = "4c01340088d245a9a7832850a5313107";

        [SerializeField]
        private RedCharacter character;

        private WebCamTexture webCamTex;

        //SerialPort stream = new SerialPort("/dev/cu.usbmodem14411", 9600);

		private void Start()
		{
            //stream.Open();
            webCamTex = new WebCamTexture();
            Renderer renderer = this.GetComponent<Renderer>();
            renderer.material.mainTexture = webCamTex;
            webCamTex.Play();
		}
        
		private void Update()
		{
            if (Input.GetKeyDown(KeyCode.A))
            {
                StartCoroutine(ProcessFaceAPI());
            }

            /*
            if (!stream.IsOpen)
                return;

            string value = stream.ReadExisting();
            Debug.Log(value);
            
            switch(Int32.Parse(value)){
                case 0:
                    character.Move(-1);
                    break;
                case 1:
                    character.Move(1);
                    break;
                case 2:
                    character.Jump();
                    break;
                case 3:
                    break;
            }

            stream.Write("test");*/
		}

        private IEnumerator ProcessFaceAPI(){
            yield return new WaitForEndOfFrame();
            Texture2D _TextureFromCamera = new Texture2D(GetComponent<Renderer>().material.mainTexture.width,
            GetComponent<Renderer>().material.mainTexture.height);
            _TextureFromCamera.SetPixels((GetComponent<Renderer>().material.mainTexture as WebCamTexture).GetPixels());
            _TextureFromCamera.Apply();
            byte[] bytes = _TextureFromCamera.EncodeToJPG();

            Dictionary<string, string> postHeader = new Dictionary<string, string>();
            postHeader.Add("Ocp-Apim-Subscription-Key", key);
            postHeader.Add("Content-Type", "application/octet-stream");

            WWW www = new WWW(url, bytes, postHeader);
            yield return www;
            if(www.error != null){
                Debug.Log(www.error);
            }else {
                Debug.Log(www.text);
            }
        }
	}
}
