using UnityEngine;
using System.IO.Ports;
using RedRunner.Characters;
using System;

namespace AssemblyCSharp.Assets.Scripts
{
    public class ArduinoManager : MonoBehaviour
    {
        [SerializeField]
        private RedCharacter character;

        private WebCamTexture webCam;

        SerialPort stream = new SerialPort("/dev/cu.usbmodem14411", 9600);

		private void Start()
		{
            stream.Open();
		}
        
		private void Update()
		{
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

            stream.Write("test");
		}
	}
}
