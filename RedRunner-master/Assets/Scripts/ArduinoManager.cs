﻿using UnityEngine;
using System.IO.Ports;
using RedRunner.Characters;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using MiniJSON;
using System.IO;
using B83.Image.BMP;

namespace RedRunner
{
    public class ArduinoManager : MonoBehaviour
    {
        #region Constants
        private static readonly string API_URL = "https://eastasia.api.cognitive.microsoft.com/face/v1.0/detect?returnFaceId=true&returnFaceAttributes=emotion";
        private static readonly string API_KEY = "4c01340088d245a9a7832850a5313107";

        private static readonly string BMP_DIR = @"c:\out\";

        private const int BAUD_RATE = 9600;

        private readonly Dictionary<string, string> POST_HEADER = new Dictionary<string, string>()
        {
            {"Ocp-Apim-Subscription-Key", API_KEY},
            {"Content-Type", "application/octet-stream"}
        };

        private readonly string PHOTO_DIR = "";
        private const float cameraDelay = 2;
        #endregion

        private RedCharacter character;

        [SerializeField]
        private Image camRenderer;

        #region Routine
        private Coroutine controllerRoutine;
        private Coroutine cameraRoutine;
        #endregion

        private string controllerPortName = "COM5";
        private string cameraPortName = "COM2";

        private SerialPort com6Port;
        
        private static ArduinoManager instance;
        public static ArduinoManager Instance { get { return instance; } }

        private BMPLoader bmpLoader;

		private void Awake()
		{
            if (instance != null)
                Destroy(instance);
            
            instance = this;
		}

		private void Start()
		{
            bmpLoader = new BMPLoader();
            character = (RedCharacter)(GameManager.Instance.MainCharacter);
		}

		public void OnGameStart(){
            if (controllerRoutine != null)
                StopCoroutine(controllerRoutine);
            if (cameraRoutine != null)
                StopCoroutine(cameraRoutine);

            controllerRoutine = StartCoroutine(ProcessController());
            cameraRoutine = StartCoroutine(ProcessCamera());
            StartBGM();
        }

        public void SetControllerPortName(string name){
            controllerPortName = name;
        }

        public void SetCameraPortName(string name){
            cameraPortName = name;
        }

        public void StartBGM(){
            
        }

        public void PlayJumpSound(){
            
        }

        public void PlayDashSound(){
            
        }

        private IEnumerator ProcessController(){
            character.isArduinoSet = true;
            SerialPort stream = new SerialPort(controllerPortName, BAUD_RATE);
            stream.Open();

            //ms
            stream.ReadTimeout = 30;
            float stopDelay = 0.1f;
            float curDelay = 0;

            string debug = "";

            while (true){
                if (stream.IsOpen == false)
                {
                    Debug.Log("stream closed");
                    continue;
                }
                if (character == null)
                {
                    Debug.Log("character null");
                    continue;
                }

                try
                {
                    string value = stream.ReadExisting().Split(',')[0];

                    if (string.IsNullOrEmpty(value))
                    {
                        curDelay += Time.deltaTime;
                        if (curDelay > stopDelay)
                        {
                            //Debug.Log("stop : " + curDelay);
                            character.Move(0);
                        }
                    }
                    else
                    {
                        curDelay = 0;
                        Debug.Log("Reset");
                        switch (Int32.Parse(value))
                        {
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
                                character.Dash();
                                break;
                            default:
                                break;
                        }
                    }
                }catch(TimeoutException e){
                    Debug.Log("TimeOut Exception");
                }catch(Exception e){
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator ProcessCamera(){

            while (true){
                if (camRenderer == null)
                {
                    yield return new WaitForSeconds(cameraDelay);
                    continue;
                }

                var files = Directory.GetFiles(BMP_DIR);
                string faceDir = string.Empty;
                foreach(string file in files){
                    if (file.EndsWith(".bmp"))
                    {
                        faceDir = file;
                        break;
                    }
                }

                Debug.Log(faceDir);

                if (faceDir.Equals(string.Empty) || !File.Exists(faceDir))
                {
                    yield return new WaitForSeconds(cameraDelay);
                    continue;
                }

                yield return StartCoroutine(ProcessFaceAPI(bmpLoader.LoadBMP(faceDir).ToTexture2D()));

                foreach(string file in files){
                    if (File.Exists(file))
                        File.Delete(file);
                }
                yield return new WaitForSeconds(cameraDelay);
            }
        }

        private void SendCOM6(double value){
            if(com6Port == null){
                com6Port = new SerialPort("COM6", BAUD_RATE);
                com6Port.Open();
            }

            Debug.Log("gyyoon" + (value * 100).ToString("0"));
            com6Port.Write((value*100).ToString("0"));
        }

        private IEnumerator ProcessFaceAPI(Texture2D texture){

            camRenderer.material.mainTexture = texture;
            
            Texture2D _TextureFromCamera = new Texture2D(camRenderer.material.mainTexture.width,
                                                         camRenderer.material.mainTexture.height);
            _TextureFromCamera.SetPixels((camRenderer.material.mainTexture as Texture2D).GetPixels());
            _TextureFromCamera.Apply();
            byte[] bytes = _TextureFromCamera.EncodeToJPG();

            WWW www = new WWW(API_URL, bytes, POST_HEADER);
            yield return www;
            if (www.error != null){
                Debug.Log(www.error);
            }
            else{
                try
                {
                    var l = (List<object>)Json.Deserialize(www.text);
                    var fa = ((Dictionary<string, object>)l[0])["faceAttributes"];
                    var emotion = (Dictionary<string, object>)(((Dictionary<string, object>)fa)["emotion"]);

                    var happy = (double)(emotion["happiness"]);

                    /*
                    if(happy > 0.5)
                    { 
                        Debug.Log("HAPPY : " + happy);
                    }else
                        Debug.Log("NOT HAPPY : " + happy);
                    */

                    GameManager.Instance.HandleFaceAPIResult(happy);
                    SendCOM6(happy);
                }
                catch(Exception e){Debug.Log("Parsing FAIL");}
            }
        }
	}
}
